
using System.Collections.Generic;

using System.Windows.Forms;

using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Shell;
using SwissAcademic.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace SwitchToMainWindow
{
    public class SwitchToMainWindow
                    :
    CitaviAddOn<ReferenceGridForm>
    {
        public override void OnHostingFormLoaded(ReferenceGridForm gridForm)
        {
            //var viewMenu = gridForm.GetCommandbar(ReferenceGridFormCommandbarId.Menu).GetCommandbarMenu(ReferenceGridFormCommandbarMenuId.Window);

            //var button_grid = viewMenu.AddCommandbarButton("Switch to main view", "Switch active window to main view", CommandbarItemStyle.ImageOnly, image: SwissAcademic.Citavi.Shell.Properties.Resources.BackButtonIcon);
            var button_grid = gridForm.GetCommandbar(ReferenceGridFormCommandbarId.Toolbar).AddCommandbarButton("Switch to main view", "Switch active window to main view", CommandbarItemStyle.ImageOnly, image: SwissAcademic.Citavi.Shell.Properties.Resources.BackButtonIcon);
            //button_grid.Text = "Switch to table view";
            button_grid.Shortcut = (System.Windows.Forms.Shortcut)(System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.W);
            button_grid.HasSeparator = true;
        }

        public override void OnBeforePerformingCommand(ReferenceGridForm gridForm, BeforePerformingCommandEventArgs e)
        {
            switch (e.Key)
            {

                case "Switch to main view":
                    {
                        e.Handled = true;
                        string nameToCheck = "MainForm";
                        // List<string> formNameList = new List<string>();
                        for (int i = 0; i < Application.OpenForms.Count; i++)
                        {
                            Form myform = Application.OpenForms[i];
                            if (!myform.Visible) continue;
                            // formNameList.Add(myform.Name);
                            if (myform.Name == nameToCheck)
                            {
                                myform.Activate();
                            }
                            //form.Activate();
                            //MessageBox.Show(myform.Name);
                        }

                    }
                    break;

            }

            base.OnBeforePerformingCommand(gridForm, e);
        }
    }
}
