using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Shell;
using SwissAcademic.Controls;

namespace ShowReferenceFromTableViewAddon
{
    public class ShowReferenceFromTableView
                :
        CitaviAddOn<ReferenceGridForm>
    {
        public override void OnHostingFormLoaded(ReferenceGridForm referenceGridForm)
        {

            referenceGridForm.GetCommandbar(ReferenceGridFormCommandbarId.Toolbar).AddCommandbarButton("ShowReferenceFromTableView", "ShowReferenceFromTableView", CommandbarItemStyle.ImageOnly, image: SwissAcademic.Citavi.Shell.Properties.Resources.Filter);

            //referenceGridForm.GetCommandbar(ReferenceGridFormCommandbarId.Toolbar).AddCommandbarButton("ShowReferenceFromTableView", "ShowReferenceFromTableView", CommandbarItemStyle.ImageAndText, image: SwissAcademic.Citavi.Shell.Properties.Resources.Filter);
            // mycommandbarButton.HasSeparator = true;

            base.OnHostingFormLoaded(referenceGridForm);
        }

        public override void OnBeforePerformingCommand(ReferenceGridForm referenceGridForm,BeforePerformingCommandEventArgs e)
        {
            switch (e.Key)
            {
                case "ShowReferenceFromTableView":
                    {
                        e.Handled = true;
                        // System.Windows.Forms.MessageBox.Show("Finished"); 

                        //Get the active ("primary") MainForm
                        MainForm mainForm = Program.ActiveProjectShell.PrimaryMainForm;
                        //if this macro should affect just filtered rows in the active MainForm, choose:
                        List<Reference> references = referenceGridForm.GetSelectedReferences().ToList();
                        mainForm.ActiveReference = references[0]; //只会将第1个文献作为Active Reference显示 
                        mainForm.Activate();
                    }
                    break;
            }

            base.OnBeforePerformingCommand(referenceGridForm,e);
        }
    }
}
