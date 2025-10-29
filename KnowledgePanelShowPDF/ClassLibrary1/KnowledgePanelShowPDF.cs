
using System.Collections.Generic;
//using System.Linq;

using System.Windows.Forms;

using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Shell;
//using SwissAcademic.Citavi.Shell.Controls.SmartRepeaters;
using SwissAcademic.Controls;

using Infragistics.Win.UltraWinToolbars;

namespace KnowledgePanelShowPDF
{
    class KnowledgePanelShowPDFAddon
                :
        CitaviAddOn
    {
        public override AddOnHostingForm HostingForm
        {
            get { return AddOnHostingForm.MainForm; }
        }

        protected override void OnHostingFormLoaded(Form hostingForm)
        {
            MainForm mainForm = (MainForm)hostingForm;


            // Knowledge Item Pop-Up Menu

            var knowledgeOrganizerKnowledgeItemsContextMenu = CommandbarMenu.Create(mainForm.GetKnowledgeOrganizerKnowledgeItemsCommandbarManager().ToolbarsManager.Tools["KnowledgeOrganizerKnowledgeItemsContextMenu"] as PopupMenuTool);
            knowledgeOrganizerKnowledgeItemsContextMenu.AddCommandbarButton("ShowPDFinRightPanel", "在当前视图的右边栏预览PDF");
            mainForm.GetKnowledgeOrganizerKnowledgeItemsCommandbarManager().GetCommandbar(MainFormKnowledgeOrganizerKnowledgeItemsCommandbarId.Toolbar).AddCommandbarButton("ShowPDFinRightPanel1", "在当前视图的右边栏预览PDF", CommandbarItemStyle.ImageOnly, SwissAcademic.Citavi.Shell.Properties.Resources.ArrowGreenRight);
            // Fin

            base.OnHostingFormLoaded(hostingForm);
        }

        protected override void OnBeforePerformingCommand(SwissAcademic.Controls.BeforePerformingCommandEventArgs e)
        {
            switch (e.Key)
            {
                case "ShowPDFinRightPanel":
                    {
                        e.Handled = true;
                        var mainForm = (MainForm)e.Form;
                        startShowPDFinRightPanel(mainForm);

                    }
                    break;
                case "ShowPDFinRightPanel1":
                    {
                        e.Handled = true;
                        var mainForm = (MainForm)e.Form;
                        startShowPDFinRightPanel(mainForm);

                    }
                    break;
            }
            base.OnBeforePerformingCommand(e);
        }
        public async void startShowPDFinRightPanel(MainForm mainForm)
        {
            // 获取项目中所有的知识条目，用于后续查找。
            List<KnowledgeItem> foundKnowledgeItems = mainForm.GetSelectedKnowledgeItems();
            // 检查是否是一个知识条目。
            if (foundKnowledgeItems.Count == 1)
            {
                KnowledgeItem targetKnowledgeItem = foundKnowledgeItems[0];
                mainForm.ActiveKnowledgeItem = targetKnowledgeItem;

                // 3. 核心：通过遍历控件树来找到 QuotationSmartRepeater，这个应该没用
                //SmartRepeater<KnowledgeItem> KnowledgeItemSmartRepeater = Program.ActiveProjectShell.PrimaryMainForm.Controls.Find("SmartRepeater", true).FirstOrDefault() as SmartRepeater<KnowledgeItem>;
                //QuotationSmartRepeater quotationSmartRepeaterAsQuotationSmartRepeater =
                //Program.ActiveProjectShell.PrimaryMainForm.Controls.Find("knowledgeItemPreviewSmartRepeater", true).FirstOrDefault() as QuotationSmartRepeater;

                //if (quotationSmartRepeaterAsQuotationSmartRepeater != null)
                //{
                //    // 调用方法进行高亮
                //    quotationSmartRepeaterAsQuotationSmartRepeater.SelectAndActivate(targetKnowledgeItem, true);
                //}

                // --- 5. 执行PDF跳转 ---
                // 检查目标知识条目是否确实关联了一个有效的地址（通常是PDF文件）。
                if (targetKnowledgeItem.Address != null)
                {
                    // 调用核心方法，在PDF预览中异步跳转到知识条目对应的位置。
                    await Program.ActiveProjectShell.PrimaryMainForm.PreviewControl.ShowPdfLinkAsync(mainForm, targetKnowledgeItem);

                }
                else
                {
                    // 如果知识条目没有关联地址（例如，它是一个纯文本的摘要），则提示用户。
                    MessageBox.Show("知识条目没有关联的PDF文件。", "无法跳转", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("请选择一个Knowledge", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}