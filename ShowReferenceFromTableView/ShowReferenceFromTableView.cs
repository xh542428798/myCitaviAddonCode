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
        //public override AddOnHostingForm HostingForm
        //{
        //    get { return AddOnHostingForm.ReferenceGridForm; }
        //}


        public override void OnHostingFormLoaded(System.Windows.Forms.Form hostingForm)
        {
            // CommandbarMenu commandbarMenu = referenceGridForm.GetCommandbar(ReferenceGridFormCommandbarId.Toolbar).InsertCommandbarMenu(4,"ShowReference", "ShowReference", CommandbarItemStyle.ImageAndText, image: SwissAcademic.Citavi.Shell.Properties.Resources.Filter);
            ReferenceGridForm referenceGridForm = (ReferenceGridForm)hostingForm;
            referenceGridForm.GetCommandbar(ReferenceGridFormCommandbarId.Toolbar).AddCommandbarButton("ShowReferenceFromTableView", "ShowReferenceFromTableView", CommandbarItemStyle.ImageOnly,image: SwissAcademic.Citavi.Shell.Properties.Resources.Filter);

            //referenceGridForm.GetCommandbar(ReferenceGridFormCommandbarId.Toolbar).AddCommandbarButton("ShowReferenceFromTableView", "ShowReferenceFromTableView", CommandbarItemStyle.ImageAndText, image: SwissAcademic.Citavi.Shell.Properties.Resources.Filter);
            // mycommandbarButton.HasSeparator = true;

            base.OnHostingFormLoaded(hostingForm);
        }

        protected override void OnBeforePerformingCommand(SwissAcademic.Controls.BeforePerformingCommandEventArgs e)
        {
            switch (e.Key)
            {
                case "ShowReferenceFromTableView":
                    {
                        e.Handled = true;
                        System.Windows.Forms.MessageBox.Show("Finished"); //break;
                    }
                    break;
            }

            base.OnBeforePerformingCommand(e);
        }
    }
}
