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

namespace SortKnowledge
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
            var commandbarButtonKnowledgeItemsSortInCategory = knowledgeOrganizerKnowledgeItemsContextMenu.AddCommandbarButton("SortKnowledgeItemsInSelection2", "Sort selected knowledge items in current category");

            // Knowledge Organizer Category Pop-Up Menu

            var knowledgeOrganizerCategoriesColumnContextMenu = CommandbarMenu.Create(mainForm.GetKnowledgeOrganizerCategoriesCommandbarManager().ToolbarsManager.Tools["KnowledgeOrganizerCategoriesColumnContextMenu"] as PopupMenuTool);

            var commandbarButtonSortKnowledgeItemsInCategory = knowledgeOrganizerCategoriesColumnContextMenu.AddCommandbarButton("SortKnowledgeItemsInCategory2", "Sort all knowledge items in this category by reference and position");

            // Fin

            base.OnHostingFormLoaded(hostingForm);
        }

        protected override void OnBeforePerformingCommand(SwissAcademic.Controls.BeforePerformingCommandEventArgs e)
        {
            switch (e.Key)
            {
                #region KnowledgeOrganizerKnowledgeItemsContextMenu
                case "SortKnowledgeItemsInSelection2":
                    {
                        e.Handled = true;

                        var mainForm = (MainForm)e.Form;
                        if (mainForm.KnowledgeOrganizerFilterSet.Filters.Count() != 1 || mainForm.KnowledgeOrganizerFilterSet.Filters[0].Name == "Knowledge items without categories")
                        {
                            MessageBox.Show("You must select one category.");
                            return;
                        }

                        KnowledgeItemInSelectionSorter.SortSelectedKnowledgeItems(mainForm);
                        // 因为有SortKnowledgeItemsInCategory，所有添加subheading暂时不要了
                        // 先排序,然后添加subheading	
                        //var category = mainForm.GetSelectedKnowledgeOrganizerCategory();
                        //var knowledgeItems = category.KnowledgeItems.ToList();
                        //CreateSubheadings(knowledgeItems, category, false);
                    }
                    break;

                case "SortKnowledgeItemsInCategory2":
                    {
                        e.Handled = true;

                        var mainForm = (MainForm)e.Form;
                        if (mainForm.KnowledgeOrganizerFilterSet.Filters.Count() != 1 || mainForm.KnowledgeOrganizerFilterSet.Filters[0].Name == "Knowledge items without categories")
                        {
                            MessageBox.Show("You must select one category.");
                            return;
                        }

                        KnowledgeItemInCategorySorter.SortKnowledgeItemsInCategorySorter(mainForm);
                    }
                    break;
                    #endregion
            }
            base.OnBeforePerformingCommand(e);
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