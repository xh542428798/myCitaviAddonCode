using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Shell;
using SwissAcademic.Controls;

namespace ShortTitleFilterDemoAddon 
{
    public class ShortTitleFilterDemo
        :
        CitaviAddOn
    {
        public override AddOnHostingForm HostingForm
        {
            get { return AddOnHostingForm.MainForm; }
        }

        protected override void OnHostingFormLoaded(System.Windows.Forms.Form hostingForm)
        {
            if (Reference.ShortTitleFilter == null)
            {
                Reference.ShortTitleFilter = new Filter();
            }
            MainForm mainForm = (MainForm)hostingForm;
            mainForm.GetMainCommandbarManager().GetReferenceEditorCommandbar(MainFormReferenceEditorCommandbarId.Menu).GetCommandbarMenu(MainFormReferenceEditorCommandbarMenuId.Tools).AddCommandbarButton("GenerateShortTitle", "Re-build all short titles");


            mainForm.GetMainCommandbarManager().GetReferenceEditorCommandbar(MainFormReferenceEditorCommandbarId.Menu).GetCommandbarMenu(MainFormReferenceEditorCommandbarMenuId.Tools).AddCommandbarButton("GenerateShortTitle", "Re-build all short titles");


            base.OnHostingFormLoaded(hostingForm);



        }

        protected override void OnBeforePerformingCommand(SwissAcademic.Controls.BeforePerformingCommandEventArgs e)
        {
            switch (e.Key)
            {
                case "GenerateShortTitle":
                    {
                        e.Handled = true;

                        var filter = new Filter();
                        bool handled;
                        List<Reference> references = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedReferences();
                        foreach (Reference reference in references)
                        {
                            reference.ShortTitle = filter.GetFilterResult(reference, out handled);
                            reference.ShortTitleUpdateType = UpdateType.Manual;
                        }

                        System.Windows.Forms.MessageBox.Show("Finished");
                    }
                    break;
            }
            base.OnBeforePerformingCommand(e);
        }
    }
}
