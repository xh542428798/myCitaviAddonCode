using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Shell;
using SwissAcademic.Citavi.Shell.Controls.Preview;
using SwissAcademic.Citavi.Controls.Wpf;
using SwissAcademic.Citavi.Shell.Controls.SmartRepeaters;
using SwissAcademic.Controls;
using SwissAcademic.Pdf;

using Infragistics.Win.UltraWinToolbars;

namespace QuotationsToolbox
{
    class MenuQuotations
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

            mainForm.ActiveWorkspaceChanged += PrimaryMainForm_ActiveWorkspaceChanged;

            // Knowledge Item Pop-Up Menu

            var knowledgeOrganizerKnowledgeItemsContextMenu = CommandbarMenu.Create(mainForm.GetKnowledgeOrganizerKnowledgeItemsCommandbarManager().ToolbarsManager.Tools["KnowledgeOrganizerKnowledgeItemsContextMenu"] as PopupMenuTool);
            var commandbarButtonKnowledgeItemsSortInCategory = knowledgeOrganizerKnowledgeItemsContextMenu.AddCommandbarButton("SortKnowledgeItemsInSelectionAndCreateSubheadings", "Sort selected knowledge items in current category and creating subheadings by reference and position");
            // Fin

            base.OnHostingFormLoaded(hostingForm);
        }

        protected override void OnBeforePerformingCommand(SwissAcademic.Controls.BeforePerformingCommandEventArgs e)
        {
            switch (e.Key)
            {
                #region KnowledgeOrganizerKnowledgeItemsContextMenu
                case "SortKnowledgeItemsInSelectionAndCreateSubheadings":
                    {
                        e.Handled = true;

                        var mainForm = (MainForm)e.Form;
                        if (mainForm.KnowledgeOrganizerFilterSet.Filters.Count() != 1 || mainForm.KnowledgeOrganizerFilterSet.Filters[0].Name == "Knowledge items without categories")
                        {
                            MessageBox.Show("You must select one category.");
                            return;
                        }

                        KnowledgeItemInSelectionSorter.SortSelectedKnowledgeItems(mainForm);
                        //先排序,然后添加subheading	
                        var category = mainForm.GetSelectedKnowledgeOrganizerCategory();
                        var knowledgeItems = category.KnowledgeItems.ToList();
                        CreateSubheadings(knowledgeItems, category, false);
                    }
                    break;
                    #endregion
            }
            base.OnBeforePerformingCommand(e);
        }

        static void CreateSubheadings(List<KnowledgeItem> knowledgeItems, Category category, bool overwriteSubheadings)
        {
            var mainForm = Program.ActiveProjectShell.PrimaryMainForm;
            var projectShell = Program.ActiveProjectShell;
            var project = projectShell.Project;

            var categoryKnowledgeItems = category.KnowledgeItems;
            var subheadings = knowledgeItems.Where(item => item.KnowledgeItemType == KnowledgeItemType.Subheading).ToList();

            Reference currentReference = null;
            Reference previousReference = null;

            int nextInsertionIndex = -1;

            if (subheadings.Any())
            {
                if (!overwriteSubheadings)
                {
                    DialogResult result = MessageBox.Show("The filtered list of knowledge items in the category \"" + category.Name + "\" already contains sub-headings.\r\n\r\nIf you continue, these sub-headings will be removed first.\r\n\r\nContinue?", "Citavi", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No) return;
                }

                foreach (var subheading in subheadings)
                {
                    project.Thoughts.Remove(subheading);
                }

                projectShell.SaveAsync(mainForm);
            }

            foreach (var knowledgeItem in knowledgeItems)
            {
                if (knowledgeItem.KnowledgeItemType == KnowledgeItemType.Subheading)
                {
                    knowledgeItem.Categories.Remove(category);
                    continue;
                }

                if (knowledgeItem.Reference != null) currentReference = knowledgeItem.Reference;

                string headingText = "No short title available";
                if (currentReference != null)
                {
                    // 获取作者、时间、Title
                    Person author = currentReference.Authors[0];
                    string year = currentReference.Year;
                    string IF = currentReference.CustomField1;
                    string Qpart = currentReference.CustomField2;
                    // 获取Title并提取前10个单词
                    string originalTitle = currentReference.Title;
                    string[] words = originalTitle.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string result;
                    if (words.Length >= 10)
                    {// 取前10个单词，首字母大写，其余小写
                        result = string.Join(" ", words.Take(10).Select(word => word.First().ToString().ToUpper() + word.Substring(1).ToLower()));
                    }
                    else
                    {// 所有单词，首字母大写，其余小写
                        result = string.Join(" ", words.Select(word => word.First().ToString().ToUpper() + word.Substring(1).ToLower()));
                    }
                    string citationkey = author.LastName.ToString() + year + "_" + result + "_" + IF + Qpart;

                    headingText = citationkey; //currentReference.CitationKey;
                }
                else if (knowledgeItem.QuotationType == QuotationType.None)
                {
                    headingText = "Thoughts";
                }

                nextInsertionIndex = category.KnowledgeItems.IndexOf(knowledgeItem);
                category.KnowledgeItems.AddNextItemAtIndex = nextInsertionIndex;
                currentReference = knowledgeItem.Reference;

                if (nextInsertionIndex == 0)
                {
                    var subheading = new KnowledgeItem(project, KnowledgeItemType.Subheading) { CoreStatement = headingText };
                    subheading.Categories.Add(category);

                    project.Thoughts.Add(subheading);
                    projectShell.SaveAsync(mainForm);
                    previousReference = currentReference;
                    continue;
                }

                if (nextInsertionIndex > 0 && (currentReference != null && currentReference != previousReference))
                {
                    var subheading = new KnowledgeItem(project, KnowledgeItemType.Subheading) { CoreStatement = headingText };
                    subheading.Categories.Add(category);

                    project.Thoughts.Add(subheading);
                    projectShell.SaveAsync(mainForm);
                }

                previousReference = currentReference;
            }
        }

        void PrimaryMainForm_ActiveWorkspaceChanged(object o, EventArgs a)
        {
            if (Program.ActiveProjectShell.PrimaryMainForm.ActiveWorkspace == MainFormWorkspace.KnowledgeOrganizer)
            {
                SmartRepeater<KnowledgeItem> KnowledgeItemSmartRepeater = (SmartRepeater<KnowledgeItem>)Program.ActiveProjectShell.PrimaryMainForm.Controls.Find("SmartRepeater", true).FirstOrDefault();

                QuotationSmartRepeater quotationSmartRepeaterAsQuotationSmartRepeater = Program.ActiveProjectShell.PrimaryMainForm.Controls.Find("knowledgeItemPreviewSmartRepeater", true).FirstOrDefault() as QuotationSmartRepeater;

                KnowledgeItemSmartRepeater.ActiveListItemChanged += KnowledgeItemPreviewSmartRepeater_ActiveListItemChanged;
            }
            else if (Program.ActiveProjectShell.PrimaryMainForm.ActiveWorkspace == MainFormWorkspace.ReferenceEditor)
            {
                QuotationSmartRepeater quotationSmartRepeaterAsQuotationSmartRepeater = Program.ActiveProjectShell.PrimaryMainForm.Controls.Find("quotationSmartRepeater", true).FirstOrDefault() as QuotationSmartRepeater;

                quotationSmartRepeaterAsQuotationSmartRepeater.ActiveListItemChanged += QuotationSmartRepeater_ActiveListItemChanged;
            }
        }

        void QuotationSmartRepeater_ActiveListItemChanged(object o, EventArgs a)
        {
            if (Program.ActiveProjectShell.PrimaryMainForm.GetSelectedQuotations().Count == 0) return;

            KnowledgeItem activeQuotation = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedQuotations().FirstOrDefault();
            if (activeQuotation.EntityLinks == null) return;
            if (activeQuotation.EntityLinks.Where(e => e.Indication == EntityLink.PdfKnowledgeItemIndication).Count() == 0) return;

            Annotation annotation = activeQuotation.EntityLinks.Where(e => e.Indication == EntityLink.PdfKnowledgeItemIndication).FirstOrDefault().Target as Annotation;

            PreviewControl previewControl = PreviewMethods.GetPreviewControl();
            if (previewControl == null) return;

            SwissAcademic.Citavi.Controls.Wpf.PdfViewControl pdfViewControl = previewControl.GetPdfViewControl();
            if (pdfViewControl == null) return;

            pdfViewControl.GoToAnnotation(annotation);

        }

        void KnowledgeItemPreviewSmartRepeater_ActiveListItemChanged(object o, EventArgs a)
        {
            if (Program.ActiveProjectShell.PrimaryMainForm.GetSelectedKnowledgeItems().Count == 0) return;

            KnowledgeItem activeQuotation = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedKnowledgeItems().FirstOrDefault();
            if (activeQuotation.EntityLinks == null) return;
            if (activeQuotation.EntityLinks.Where(e => e.Indication == EntityLink.PdfKnowledgeItemIndication).Count() == 0) return;

            Annotation annotation = activeQuotation.EntityLinks.Where(e => e.Indication == EntityLink.PdfKnowledgeItemIndication).FirstOrDefault().Target as Annotation;

            PreviewControl previewControl = PreviewMethods.GetPreviewControl();
            if (previewControl == null) return;

            PdfViewControl pdfViewControl = previewControl.GetPdfViewControl();
            if (pdfViewControl == null) return;

            Document document = pdfViewControl.Document;
            if (document == null) return;

            if (previewControl.ActiveLocation != annotation.Location)
            {
                Program.ActiveProjectShell.ShowPreviewFullScreenForm(annotation.Location, previewControl, null);
            }
            pdfViewControl.GoToAnnotation(annotation);

            Program.ActiveProjectShell.PrimaryMainForm.Activate();
        }
    }
}