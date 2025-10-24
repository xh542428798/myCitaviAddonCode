using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Shell;
using SwissAcademic.Citavi.Shell.Controls.Preview;
using SwissAcademic.Controls;
using SwissAcademic.Pdf.Analysis;
using System;
using System.Linq;
using System.Windows.Forms;

namespace CopyTextOfSelectedAnnotation
{
    public class Addon : CitaviAddOn<MainForm>
    {
        #region Constants

        const string Keys_Button_CopyTextOfSelectedAnnotation = "CopyTextOfSelectedAnnotation.Button.CopyTextOfSelectedAnnotation";
        // 新增的Obsidian链接按钮
        const string Keys_Button_CopyObsidianLink = "CopyTextOfSelectedAnnotation.Button.CopyObsidianLink";
        #endregion

        #region Methods

        public override void OnHostingFormLoaded(MainForm mainForm)
        {
            var button = mainForm
                          .GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar)
                          .GetCommandbarMenu(MainFormPreviewCommandbarMenuId.Tools)
                          .AddCommandbarButton(Keys_Button_CopyTextOfSelectedAnnotation, "CopyTextOfSelectedAnnotation");
            button.Shortcut = (Shortcut)(Keys.Shift | Keys.C);
            button.Visible = true;

            // 新增的“复制Obsidian链接”按钮
            var copyLinkButton = mainForm
                          .GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar)
                          .GetCommandbarMenu(MainFormPreviewCommandbarMenuId.Tools)
                          .AddCommandbarButton(Keys_Button_CopyObsidianLink, "CopyFormattedLink"); 
            copyLinkButton.Shortcut = (Shortcut)(Keys.Shift| Keys.Control | Keys.O); // 使用 Shift+Ctrl+O 作为快捷键
            copyLinkButton.Visible = true;

            base.OnHostingFormLoaded(mainForm);
        }
        public override void OnBeforePerformingCommand(MainForm mainForm, BeforePerformingCommandEventArgs e)
        {
            if (e.Key.Equals(Keys_Button_CopyTextOfSelectedAnnotation, StringComparison.OrdinalIgnoreCase))
            {
                ExtractTextFromEntity(mainForm, mainForm.PreviewControl.GetSelectedCitaviEntity());
                e.Handled = true;
            }
            // 新增功能：复制Obsidian链接
            if (e.Key.Equals(Keys_Button_CopyObsidianLink, StringComparison.OrdinalIgnoreCase))
            {
                GenerateAndCopyObsidianLink(mainForm);
                e.Handled = true;
            }
            base.OnBeforePerformingCommand(mainForm, e);
        }

        public override void OnApplicationIdle(MainForm mainForm)
        {
            var button = mainForm
                         .GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar)
                         .GetCommandbarMenu(MainFormPreviewCommandbarMenuId.Tools)
                         .GetCommandbarButton(Keys_Button_CopyTextOfSelectedAnnotation);
            if (button != null)
            {
                button.Tool.SharedProps.Enabled = mainForm.PreviewControl.ActivePreviewType == PreviewType.Pdf
                                                 && mainForm.PreviewControl.GetSelectedCitaviEntity().IsSupportedCitaviEntity();
            }

            // --- 添加处理新的“复制Obsidian链接”按钮 ---
            var obsidianButton = mainForm
                                 .GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar)
                                 .GetCommandbarMenu(MainFormPreviewCommandbarMenuId.Tools)
                                 .GetCommandbarButton(Keys_Button_CopyObsidianLink);
            if (obsidianButton != null)
            {
                obsidianButton.Tool.SharedProps.Enabled = mainForm.PreviewControl.ActivePreviewType == PreviewType.Pdf
                                                         && mainForm.PreviewControl.GetSelectedKnowledgeItems().Any();
            }

            base.OnApplicationIdle(mainForm);
        }

        void ExtractTextFromEntity(MainForm mainForm, ICitaviEntity citaviEntity)
        {
            if (citaviEntity.EntityLinks.FirstOrDefault(link => link.Indication.Equals("PdfKnowledgeItem", StringComparison.OrdinalIgnoreCase))?.Target is Annotation annotation)
            {
                var documentParser = new DocumentParser(mainForm.PreviewControl.GetPdfViewControl()?.Document)
                {
                    ParseType = ParseType.Text,
                    DetectParagraphAlignment = true,
                    ExtractIdentifier = false
                };

                var text = documentParser
                           .Run(annotation.Quads.Where(q => !q.IsContainer).ToList())?
                           .GetDocumentText()?
                           .ContentAsPlainText;
                if (!string.IsNullOrEmpty(text)) Clipboard.SetText(text);
            }
        }

        #region 新增功能：复制Obsidian链接
        void GenerateAndCopyObsidianLink(MainForm mainForm)
        {
            var knowledges = mainForm.PreviewControl.GetSelectedKnowledgeItems();

            if (!knowledges.Any())
            {
                MessageBox.Show("请先在PDF中选中一个知识条目高亮。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var knowledge = knowledges.First();

            // --- 核心逻辑 ---
            string coreStatement = knowledge.CoreStatement;
            string pageRange = knowledge.PageRange.ToString();
            string knowledgeId = knowledge.Id.ToString();
            string pdfPath = GetPdfPathFromKnowledgeItem(knowledge);

            if (string.IsNullOrEmpty(pdfPath))
            {
                MessageBox.Show("无法找到关联的PDF文件路径。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string obsidianPath = pdfPath.Replace('\\', '/');
            string linkText = string.Format("{0}/p.{1}", coreStatement, pageRange);
            string finalLink = string.Format("[{0}](file:///{1})KnowID：{2}", linkText, obsidianPath, knowledgeId);

            Clipboard.SetText(finalLink);
            //MessageBox.Show("Obsidian链接已复制到剪贴板！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 核心辅助函数：从一个知识条目对象中，解析出其关联的PDF文件的本地路径。
        /// </summary>
        public static string GetPdfPathFromKnowledgeItem(KnowledgeItem knowledgeItem)
        {
            if (knowledgeItem == null) return null;

            EntityLink pdfLink = knowledgeItem.Project.EntityLinks
                .FirstOrDefault(el => el.Source == knowledgeItem && el.Indication == "PdfKnowledgeItem");

            if (pdfLink?.Target is SwissAcademic.Citavi.Annotation pdfAnnotation)
            {
                var location = pdfAnnotation.Location;
                var address = location?.Address;
                Uri pdfUri = address?.Resolve();

                if (pdfUri != null && pdfUri.IsFile)
                {
                    return pdfUri.LocalPath;
                }
            }
            return null;
        }

        #endregion

        #endregion
    }
}