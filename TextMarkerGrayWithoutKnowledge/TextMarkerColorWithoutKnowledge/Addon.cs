using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Shell;
using SwissAcademic.Citavi.Shell.Controls.Preview;
using SwissAcademic.Citavi.Controls.Wpf;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Pdf;
using SwissAcademic.Pdf.Analysis;
using System;
using System.Reflection;
using System.Linq;
using SwissAcademic.Controls;
using System.Windows.Forms;

namespace TextMarkerColorWithoutKnowledge
{
    public class TextMarkerColorWithoutKnowledge : CitaviAddOn<MainForm>
    {
        private const string RedHighlightButtonKey = "Highlight in gray";

        #region Methods

        public override void OnHostingFormLoaded(MainForm mainForm)
        {
            var toolbar = mainForm.GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar);
            var redButton = toolbar.InsertCommandbarButton(1,RedHighlightButtonKey, "用灰色高亮但不保存Knowledge", CommandbarItemStyle.ImageOnly, icon.gray);
            //redButton.HasSeparator = true;
            redButton.Shortcut = (Shortcut)(Keys.B);

            base.OnHostingFormLoaded(mainForm);
        }

        public override void OnBeforePerformingCommand(MainForm mainForm, BeforePerformingCommandEventArgs e)
        {
            if (e.Key == RedHighlightButtonKey)
            {
                var previewControl = mainForm.PreviewControl;
                if (previewControl.ActivePreviewType == PreviewType.Pdf)
                {
                    var pdfViewControl = previewControl.GetPdfViewControl();
                    if (pdfViewControl != null && pdfViewControl.GetSelectedContentType() == ContentType.Text)
                    {
                        CreateRedHighlight(pdfViewControl, mainForm);
                        e.Handled = true;
                    }
                }
            }

            base.OnBeforePerformingCommand(mainForm, e);
        }

        private void CreateRedHighlight(PdfViewControl pdfViewControl, MainForm mainForm)
        {
            try
            {
                // 1. 获取基本对象
                var project = Program.ActiveProjectShell.Project;
                var location = pdfViewControl.Location;

                if (location == null)
                {
                    throw new Exception("无法获取Location");
                }

                var reference = location.Reference;
                if (reference == null)
                {
                    throw new Exception("无法获取Reference");
                }

                // 2. 获取选中的文本内容
                var textContent = pdfViewControl.GetSelectedContentFromType(
                    pdfViewControl.GetSelectedContentType(), -1, false, true) as TextContent;

                if (textContent == null)
                {
                    throw new Exception("无法获取选中的文本内容，请先选择要标记的文本");
                }

                // 3. 使用Citavi内部方法创建高亮
                var annotation = CreateHighlightUsingCitaviMethod(pdfViewControl, textContent);

                if (annotation == null)
                {
                    throw new Exception("无法创建高亮注释");
                }


                // 8. 跳转到新创建的Annotation
                //pdfViewControl.GoToAnnotation(annotation);
            }
            catch (Exception ex)
            {
                MessageBox.Show("创建红色标记失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine("CreateRedHighlight错误: " + ex);
            }
        }

        private Annotation CreateHighlightUsingCitaviMethod(PdfViewControl pdfViewControl, TextContent textContent)
        {
            try
            {
                var method = pdfViewControl.GetType().GetMethod(
                    "OnAnnotationCreating",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new[] { typeof(Content), typeof(short), typeof(bool) },
                    null);

                if (method != null)
                {
                    // 直接调用，只需要改这个数字来测试不同颜色
                    method.Invoke(pdfViewControl, new object[] { textContent, (short)2, true });
                    return FindNewlyCreatedAnnotation(pdfViewControl);
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("CreateHighlightUsingCitaviMethod错误: " + ex);
                return null;
            }
        }


        private Annotation FindNewlyCreatedAnnotation(PdfViewControl pdfViewControl)
        {
            try
            {
                var location = pdfViewControl.Location;
                if (location == null) return null;

                // 获取所有注释，找到最新创建的
                var annotations = location.Annotations.ToList();
                if (annotations.Count == 0) return null;

                // 返回最后一个注释（假设是最新创建的）
                return annotations.LastOrDefault();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("FindNewlyCreatedAnnotation错误: " + ex);
                return null;
            }
        }

        public override void OnApplicationIdle(MainForm mainForm)
        {
            var previewControl = mainForm.PreviewControl;
            var redButton = mainForm.GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar)
                .GetCommandbarButton(RedHighlightButtonKey);

            if (redButton != null)
            {
                bool shouldShow = previewControl.ActivePreviewType == PreviewType.Pdf &&
                                previewControl.GetPdfViewControl()?.GetSelectedContentType() == ContentType.Text;

                redButton.Visible = shouldShow;
            }

            base.OnApplicationIdle(mainForm);
        }

        #endregion
    }
}
