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

            // Reference Editor Reference Menu

            var referencesMenu = mainForm.GetMainCommandbarManager().GetReferenceEditorCommandbar(MainFormReferenceEditorCommandbarId.Menu).GetCommandbarMenu(MainFormReferenceEditorCommandbarMenuId.References);

            var commandbarButtonMoveAttachment = referencesMenu.AddCommandbarButton("MoveAttachment", "移动所选参考文献的本地附件到不同文件夹");
            commandbarButtonMoveAttachment.HasSeparator = true;

            var commandbarButtonExportAnnotations = referencesMenu.AddCommandbarButton("ExportAnnotations", "将所选参考文献中的quotations导出为PDF高亮");
            commandbarButtonExportAnnotations.HasSeparator = true;
            var commandbarButtonExportBookmarks = referencesMenu.AddCommandbarButton("ExportBookmarks", "将所选参考文献中的quick references导出为PDF书签");

            // Preview Tool Menu

            var previewCommandbarMenuTools = (mainForm.GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar).GetCommandbarMenu(MainFormPreviewCommandbarMenuId.Tools));

            var annotationsImportCommandbarMenu = previewCommandbarMenuTools.AddCommandbarMenu("AnnotationsImportCommandbarMenu", "导入注释…", CommandbarItemStyle.Default);

            annotationsImportCommandbarMenu.HasSeparator = true;

            annotationsImportCommandbarMenu.AddCommandbarButton("ImportDirectQuotations", "将活动文档中注释当作direct quotations导入");
            annotationsImportCommandbarMenu.AddCommandbarButton("ImportIndirectQuotations", "将活动文档中注释当作indirect quotations导入");
            annotationsImportCommandbarMenu.AddCommandbarButton("ImportComments", "将活动文档中注释当作comments导入");
            annotationsImportCommandbarMenu.AddCommandbarButton("ImportQuickReferences", "将活动文档中注释当作quick references导入");
            annotationsImportCommandbarMenu.AddCommandbarButton("ImportSummaries", "将活动文档中注释当作summaries导入");

            var commandBarButtonMergeAnnotations = previewCommandbarMenuTools.AddCommandbarButton("MergeAnnotations", "合并注释", CommandbarItemStyle.Default);
            commandBarButtonMergeAnnotations.HasSeparator = true;

            var commandBarButtonRedrawAnnotations = previewCommandbarMenuTools.AddCommandbarButton("SimplifyAnnotations", "重绘注释", CommandbarItemStyle.Default);

            // Quotations Pop-Up Menu

            var referenceEditorQuotationsContextMenu = CommandbarMenu.Create(mainForm.GetReferenceEditorQuotationsCommandbarManager().ToolbarsManager.Tools["ReferenceEditorQuotationsContextMenu"] as PopupMenuTool);

            var positionContextMenu = referenceEditorQuotationsContextMenu.AddCommandbarMenu("PositionContextMenu", "Quotation位置", CommandbarItemStyle.Default);
            positionContextMenu.HasSeparator = true;

            var commandbarButtonKnowledgeItemsSortInReference = positionContextMenu.AddCommandbarButton("KnowledgeItemsSortInReference", "按PDF中的位置对所选quotations排序");
            commandbarButtonKnowledgeItemsSortInReference.Shortcut = Shortcut.CtrlK;
            var commandbarButtonPageAssignFromPositionInPDF = positionContextMenu.AddCommandbarButton("PageAssignFromPositionInPDF", "根据PDF中的位置为所选quote分配页码范围");
            commandbarButtonPageAssignFromPositionInPDF.HasSeparator = true;
            positionContextMenu.AddCommandbarButton("PageAssignFromPreviousQuotation", "从上一个quote分配页码范围和编号类型给所选quote");
            var commandbarButtonShowQuotationAndSetPageRangeManually = positionContextMenu.AddCommandbarButton("ShowQuotationAndSetPageRangeManually", "在PDF中显示所选quote后手动分配页码范围");
            commandbarButtonShowQuotationAndSetPageRangeManually.Shortcut = Shortcut.CtrlDel;

            var commentsContextMenu = referenceEditorQuotationsContextMenu.AddCommandbarMenu("CommentsContextMenu", "Comments", CommandbarItemStyle.Default);

            var commandbarButtonCommentAnnotation = commentsContextMenu.AddCommandbarButton("CommentAnnotation", "将Comments链接到相关quote的相同PDF文本");
            commentsContextMenu.AddCommandbarButton("LinkQuotations", "链接所选quote和comment");
            var commandbarButtonCreateCommentOnQuotation = commentsContextMenu.AddCommandbarButton("CreateCommentOnQuotation", "为所选quote创建comment");
            commandbarButtonCreateCommentOnQuotation.Shortcut = Shortcut.CtrlShiftK;
            var commandbarButtonSelectLinkedKnowledgeItem = commentsContextMenu.AddCommandbarButton("SelectLinkedKnowledgeItem", "跳转到相关quote或comment");
            commandbarButtonSelectLinkedKnowledgeItem.Shortcut = Shortcut.AltRightArrow;

            var quickReferencesContextMenu = referenceEditorQuotationsContextMenu.AddCommandbarMenu("QuickReferencesContextMenu", "Quick References", CommandbarItemStyle.Default);
            quickReferencesContextMenu.AddCommandbarButton("QuickReferenceTitleCase", "将所选Quick References的core statement改为标题格式");
            var commandbarButtonConvertDirectQuoteToRedHighlight = quickReferencesContextMenu.AddCommandbarButton("ConvertDirectQuoteToRedHighlight", "将所选quotations转换为quick references");


            var commandbarButtonCleanQuotationsText = referenceEditorQuotationsContextMenu.AddCommandbarButton("CleanQuotationsText", "清理所选quotations的文本");
            commandbarButtonCleanQuotationsText.Shortcut = Shortcut.ShiftDel;
            var commandbarButtonQuotationsMerge = referenceEditorQuotationsContextMenu.AddCommandbarButton("QuotationsMerge", "合并所选quotations");
            var commandbarButtonCreateSummaryOnQuotations = referenceEditorQuotationsContextMenu.AddCommandbarButton("CreateSummaryOnQuotations", "创建所选quotes的summary");

            // Reference Editor Attachment Pop-Up Menu

            var referenceEditorUriLocationsContextMenu = CommandbarMenu.Create(mainForm.GetReferenceEditorElectronicLocationsCommandbarManager().ToolbarsManager.Tools["ReferenceEditorUriLocationsContextMenu"] as PopupMenuTool);

            // Knowledge Item Pop-Up Menu

            var knowledgeOrganizerKnowledgeItemsContextMenu = CommandbarMenu.Create(mainForm.GetKnowledgeOrganizerKnowledgeItemsCommandbarManager().ToolbarsManager.Tools["KnowledgeOrganizerKnowledgeItemsContextMenu"] as PopupMenuTool);

            var commandBarButtonOpenKnowledgeItemAttachment = knowledgeOrganizerKnowledgeItemsContextMenu.AddCommandbarButton("OpenKnowledgeItemAttachment", "在新窗口中打开附件");
            commandBarButtonOpenKnowledgeItemAttachment.HasSeparator = true;
            knowledgeOrganizerKnowledgeItemsContextMenu.AddCommandbarButton("SelectLinkedKnowledgeItem", "跳转到链接的知识项");
            var commandbarButtonKnowledgeItemsSortInCategory = knowledgeOrganizerKnowledgeItemsContextMenu.AddCommandbarButton("SortKnowledgeItemsInSelection", "在当前category中按参考文献和位置对所选知识项排序");

            // Knowledge Organizer Category Pop-Up Menu

            var knowledgeOrganizerCategoriesColumnContextMenu = CommandbarMenu.Create(mainForm.GetKnowledgeOrganizerCategoriesCommandbarManager().ToolbarsManager.Tools["KnowledgeOrganizerCategoriesColumnContextMenu"] as PopupMenuTool);


            var commandbarButtonSortKnowledgeItemsInCategory = knowledgeOrganizerCategoriesColumnContextMenu.AddCommandbarButton("SortKnowledgeItemsInCategory", "在此category中按参考文献和位置对所有知识项排序");
            commandbarButtonSortKnowledgeItemsInCategory.HasSeparator = true;

            // Fin

            base.OnHostingFormLoaded(hostingForm);
        }

        protected override void OnBeforePerformingCommand(SwissAcademic.Controls.BeforePerformingCommandEventArgs e)
        {
            switch (e.Key)
            {
                #region Annotation-based menus
                case "ImportComments":
                    {
                        e.Handled = true;
                        Reference reference = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedReferences().FirstOrDefault();
                        AnnotationsImporter.AnnotationsImport(QuotationType.Comment);
                    }
                    break;
                case "ImportDirectQuotations":
                    {
                        e.Handled = true;
                        Reference reference = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedReferences().FirstOrDefault();
                        AnnotationsImporter.AnnotationsImport(QuotationType.DirectQuotation);
                    }
                    break;
                case "ImportIndirectQuotations":
                    {
                        e.Handled = true;
                        Reference reference = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedReferences().FirstOrDefault();
                        AnnotationsImporter.AnnotationsImport(QuotationType.IndirectQuotation);
                    }
                    break;
                case "ImportQuickReferences":
                    {
                        e.Handled = true;
                        Reference reference = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedReferences().FirstOrDefault();
                        AnnotationsImporter.AnnotationsImport(QuotationType.QuickReference);
                    }
                    break;
                case "ImportSummaries":
                    {
                        e.Handled = true;
                        Reference reference = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedReferences().FirstOrDefault();
                        AnnotationsImporter.AnnotationsImport(QuotationType.Summary);
                    }
                    break;

                case "MergeAnnotations":
                    {
                        e.Handled = true;
                        AnnotationsAndQuotationsMerger.MergeAnnotations();
                    }
                    break;
                case "SimplifyAnnotations":
                    {
                        e.Handled = true;
                        AnnotationSimplifier.SimplifyAnnotations();
                    }
                    break;
                #endregion            
                #region Reference-based commands
                case "ExportAnnotations":
                    {
                        e.Handled = true;
                        List<Reference> references = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedReferences().ToList();
                        AnnotationsExporter.ExportAnnotations(references);
                    }
                    break;
                case "ExportBookmarks":
                    {
                        e.Handled = true;
                        List<Reference> references = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedReferences().ToList();
                        QuickReferenceBookmarkExporter.ExportBookmarks(references);
                    }
                    break;
                case "MoveAttachment":
                    {
                        e.Handled = true;
                        LocationMover.MoveAttachment();
                    }
                    break;
                #endregion
                #region Quotations Pop-Up Menu
                case "CleanQuotationsText":
                    {
                        e.Handled = true;
                        List<KnowledgeItem> quotations = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedQuotations().ToList();
                        QuotationTextCleaner.CleanQuotationsText(quotations);
                    }
                    break;
                case "ConvertDirectQuoteToRedHighlight":
                    {
                        e.Handled = true;
                        List<KnowledgeItem> quotations = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedQuotations().ToList();
                        QuotationTypeConverter.ConvertDirectQuoteToRedHighlight(quotations);
                    }
                    break;
                case "CreateCommentOnQuotation":
                    {
                        e.Handled = true;
                        List<KnowledgeItem> quotations = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedQuotations().ToList();
                        CommentCreator.CreateCommentOnQuotation(quotations);
                    }
                    break;
                case "CreateSummaryOnQuotations":
                    {
                        e.Handled = true;
                        List<KnowledgeItem> quotations = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedQuotations().ToList();
                        SummaryCreator.CreatesummaryOnQuotations(quotations);
                    }
                    break;
                case "KnowledgeItemsSortInReference":
                    {
                        e.Handled = true;
                        List<KnowledgeItem> quotations = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedQuotations().ToList();
                        QuotationsSorter.SortQuotations(quotations);
                    }
                    break;
                case "CommentAnnotation":
                    {
                        e.Handled = true;
                        List<KnowledgeItem> quotations = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedQuotations().ToList();
                        CommentAnnotationCreator.CreateCommentAnnotation(quotations);
                    }
                    break;
                case "LinkQuotations":
                    {
                        e.Handled = true;
                        List<KnowledgeItem> quotations = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedQuotations().ToList();
                        QuotationLinker.LinkQuotations(quotations);
                    }
                    break;
                case "PageAssignFromPositionInPDF":
                    {

                        e.Handled = true;
                        List<KnowledgeItem> quotations = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedQuotations().ToList();
                        PageRangeFromPositionInPDFAssigner.AssignPageRangeFromPositionInPDF(quotations);
                    }
                    break;
                case "PageAssignFromPreviousQuotation":
                    {
                        e.Handled = true;
                        List<KnowledgeItem> quotations = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedQuotations().ToList();
                        PageRangeFromPrecedingQuotationAssigner.AssignPageRangeFromPrecedingQuotation(quotations);
                    }
                    break;
                case "QuotationsMerge":
                    {
                        e.Handled = true;
                        List<KnowledgeItem> quotations = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedQuotations().ToList();
                        AnnotationsAndQuotationsMerger.MergeQuotations(quotations);
                    }
                    break;
                case "QuickReferenceTitleCase":
                    {
                        e.Handled = true;
                        List<KnowledgeItem> quotations = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedQuotations().ToList();
                        QuickReferenceTitleCaser.TitleCaseQuickReference(quotations);
                    }
                    break;
                case "ShowQuotationAndSetPageRangeManually":
                    {
                        e.Handled = true;
                        List<KnowledgeItem> quotations = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedQuotations().ToList();
                        PageRangeManualAssigner.AssignPageRangeManuallyAfterShowingAnnotation();
                    }
                    break;
                #endregion
                #region ReferenceEditorUriLocationsPopupMenu

                #endregion
                #region KnowledgeOrganizerKnowledgeItemsContextMenu
                case "OpenKnowledgeItemAttachment":
                    {
                        e.Handled = true;

                        MainForm mainForm = Program.ActiveProjectShell.PrimaryMainForm;

                        if (mainForm.ActiveWorkspace == MainFormWorkspace.KnowledgeOrganizer)
                        {
                            KnowledgeItem knowledgeItem = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedKnowledgeItems().FirstOrDefault();
                            if (knowledgeItem.EntityLinks.FirstOrDefault() == null) break;
                            Annotation annotation = knowledgeItem.EntityLinks.FirstOrDefault().Target as Annotation;
                            if (annotation == null) break;
                            Location location = annotation.Location;

                            SwissAcademic.Citavi.Shell.Controls.Preview.PreviewControl previewControl = Program.ActiveProjectShell.PrimaryMainForm.PreviewControl;

                            Program.ActiveProjectShell.ShowPreviewFullScreenForm(location, previewControl, null);

                        }
                    }
                    break;
                case "SelectLinkedKnowledgeItem":
                    {
                        e.Handled = true;

                        MainForm mainForm = Program.ActiveProjectShell.PrimaryMainForm;

                        KnowledgeItem target;
                        KnowledgeItem knowledgeItem;

                        if (mainForm.ActiveWorkspace == MainFormWorkspace.ReferenceEditor)
                        {
                            knowledgeItem = mainForm.GetSelectedQuotations().FirstOrDefault();
                        }
                        else if (mainForm.ActiveWorkspace == MainFormWorkspace.KnowledgeOrganizer)
                        {
                            knowledgeItem = mainForm.ActiveKnowledgeItem;
                        }
                        else
                        {
                            return;
                        }

                        if (knowledgeItem == null) return;

                        if (knowledgeItem.EntityLinks.Where(el => el.Indication == EntityLink.CommentOnQuotationIndication).Count() == 0) return;

                        if (knowledgeItem.QuotationType == QuotationType.Comment)
                        {

                            target = knowledgeItem.EntityLinks.ToList().Where(n => n != null && n.Indication == EntityLink.CommentOnQuotationIndication && n.Target as KnowledgeItem != null).ToList().FirstOrDefault().Target as KnowledgeItem;

                        }
                        else
                        {
                            target = knowledgeItem.EntityLinks.ToList().Where(n => n != null && n.Indication == EntityLink.CommentOnQuotationIndication && n.Target as KnowledgeItem != null).ToList().FirstOrDefault().Source as KnowledgeItem;
                        }

                        if (target == null) return;

                        if (mainForm.ActiveWorkspace == MainFormWorkspace.ReferenceEditor)
                        {
                            Control quotationSmartRepeater = Program.ActiveProjectShell.PrimaryMainForm.Controls.Find("quotationSmartRepeater", true).FirstOrDefault();
                            SwissAcademic.Citavi.Shell.Controls.SmartRepeaters.QuotationSmartRepeater quotationSmartRepeaterAsQuotationSmartRepeater = quotationSmartRepeater as SwissAcademic.Citavi.Shell.Controls.SmartRepeaters.QuotationSmartRepeater;

                            Reference reference = target.Reference;
                            if (reference == null) return;

                            List<KnowledgeItem> quotations = reference.Quotations.ToList();

                            int index = quotations.FindIndex(q => q == target);

                            quotationSmartRepeaterAsQuotationSmartRepeater.SelectAndActivate(quotations[index]);
                        }
                        else if (mainForm.ActiveWorkspace == MainFormWorkspace.KnowledgeOrganizer)
                        {
                            mainForm.ActiveKnowledgeItem = target;
                        }
                        else
                        {
                            return;
                        }



                        return;
                    }
                case "SortKnowledgeItemsInSelection":
                    {
                        e.Handled = true;

                        var mainForm = (MainForm)e.Form;
                        if (mainForm.KnowledgeOrganizerFilterSet.Filters.Count() != 1 || mainForm.KnowledgeOrganizerFilterSet.Filters[0].Name == "Knowledge items without categories")
                        {
                            MessageBox.Show("您必须选择一个类别。");
                            return;
                        }

                        KnowledgeItemInSelectionSorter.SortSelectedKnowledgeItems(mainForm);
                    }
                    break;

                case "SortKnowledgeItemsInCategory":
                    {
                        e.Handled = true;

                        var mainForm = (MainForm)e.Form;
                        if (mainForm.KnowledgeOrganizerFilterSet.Filters.Count() != 1 || mainForm.KnowledgeOrganizerFilterSet.Filters[0].Name == "Knowledge items without categories")
                        {
                            MessageBox.Show("您必须选择一个类别。");
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
                SmartRepeater<KnowledgeItem> KnowledgeItemSmartRepeater = Program.ActiveProjectShell.PrimaryMainForm.Controls.Find("SmartRepeater", true).FirstOrDefault() as SmartRepeater<KnowledgeItem>;
                QuotationSmartRepeater quotationSmartRepeaterAsQuotationSmartRepeater = Program.ActiveProjectShell.PrimaryMainForm.Controls.Find("knowledgeItemPreviewSmartRepeater", true).FirstOrDefault() as QuotationSmartRepeater;

                // 【关键修复】在使用控件前，检查它们是否为 null
                if (KnowledgeItemSmartRepeater != null)
                {
                    KnowledgeItemSmartRepeater.ActiveListItemChanged += KnowledgeItemPreviewSmartRepeater_ActiveListItemChanged;
                }

                if (quotationSmartRepeaterAsQuotationSmartRepeater != null)
                {
                    // 注意：这里你可能不需要订阅这个事件，但为了安全起见也检查一下
                    // quotationSmartRepeaterAsQuotationSmartRepeater.ActiveListItemChanged += ...;
                }
            }
            else if (Program.ActiveProjectShell.PrimaryMainForm.ActiveWorkspace == MainFormWorkspace.ReferenceEditor)
            {
                QuotationSmartRepeater quotationSmartRepeaterAsQuotationSmartRepeater = Program.ActiveProjectShell.PrimaryMainForm.Controls.Find("quotationSmartRepeater", true).FirstOrDefault() as QuotationSmartRepeater;

                // 【关键修复】同样地，检查控件是否为 null
                if (quotationSmartRepeaterAsQuotationSmartRepeater != null)
                {
                    quotationSmartRepeaterAsQuotationSmartRepeater.ActiveListItemChanged += QuotationSmartRepeater_ActiveListItemChanged;
                }
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
            // 【关键修复】增加工作区检查，只在Knowledge界面生效
            if (Program.ActiveProjectShell.PrimaryMainForm.ActiveWorkspace != MainFormWorkspace.KnowledgeOrganizer)
            {
                return; // 如果不是Knowledge界面，直接退出，什么都不做
            }
            if (Program.ActiveProjectShell.PrimaryMainForm.GetSelectedKnowledgeItems().Count == 0) return;

            // 【关键修复】在这里增加一个对 activeQuotation 本身的空值检查
            var activeQuotations = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedKnowledgeItems();
            if (activeQuotations == null || activeQuotations.Count == 0) return;
            KnowledgeItem activeQuotation = activeQuotations.FirstOrDefault();
            // 现在可以安全地使用 activeQuotation 了，因为我们已经确定集合不为空
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
