
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Shell;
using SwissAcademic.Controls;
using System.Collections.Generic;
using System.Drawing;

namespace DoiPdfAddon
{
  public class Addon : CitaviAddOn<MainForm>
  {
    private static string batchDownloadCommandKey = "ToolBar.BatchDownload.Doi";
    private static string toolBarMenuKey = "ToolBar.DownloadPdf";
    private static string locationMenuKey = "Location.DownloadPdf";
    private static string menuName = "Download pdf";

    public override void OnHostingFormLoaded(MainForm mainForm)
    {
      base.OnHostingFormLoaded(mainForm);
      CommandbarMenu commandbarMenu = ((Commandbar) mainForm.GetReferenceEditorElectronicLocationsCommandbarManager().GetCommandbar((MainFormReferenceEditorElectronicLocationsCommandbarId) 0)).GetCommandbarMenu(Addon.locationMenuKey) ?? ((Commandbar) mainForm.GetReferenceEditorElectronicLocationsCommandbarManager().GetCommandbar((MainFormReferenceEditorElectronicLocationsCommandbarId) 0)).InsertCommandbarMenu(3, Addon.locationMenuKey, Addon.menuName, (CommandbarItemStyle) 1, (Image) IconResources.download);
      ((CommandbarItem) commandbarMenu).HasSeparator = true;
      commandbarMenu.AddCommandbarButton(DoiPdfDownloader.CommandKey, DoiPdfDownloader.CommandName, (CommandbarItemStyle) 1, (Image) IconResources.doi);
      (((Commandbar) mainForm.GetMainCommandbarManager().GetReferenceEditorCommandbar((MainFormReferenceEditorCommandbarId) 1)).GetCommandbarMenu(Addon.toolBarMenuKey) ?? ((Commandbar) mainForm.GetMainCommandbarManager().GetReferenceEditorCommandbar((MainFormReferenceEditorCommandbarId) 1)).InsertCommandbarMenu(11, Addon.toolBarMenuKey, Addon.menuName, (CommandbarItemStyle) 1, (Image) IconResources.download)).AddCommandbarButton(Addon.batchDownloadCommandKey, DoiPdfDownloader.CommandName, (CommandbarItemStyle) 1, (Image) IconResources.doi);
    }

    public override void OnBeforePerformingCommand(MainForm form, BeforePerformingCommandEventArgs e)
    {
      Project project = Program.ActiveProjectShell.Project;
      MainForm primaryMainForm = Program.ActiveProjectShell.PrimaryMainForm;
      Reference activeReference = primaryMainForm.ActiveReference;
      List<Reference> selectedReferences = primaryMainForm.GetSelectedReferences();
      if (e.Key == DoiPdfDownloader.CommandKey)
      {
        e.Handled = true;
        DoiPdfDownloader.run(primaryMainForm, activeReference);
      }
      else if (e.Key == Addon.batchDownloadCommandKey)
      {
        e.Handled = true;
        foreach (Reference reference in selectedReferences)
          DoiPdfDownloader.run(primaryMainForm, reference, true);
      }
      base.OnBeforePerformingCommand(primaryMainForm, e);
    }
  }
}
