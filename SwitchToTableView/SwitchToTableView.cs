
using System;
using System.Collections.Generic;

using System.Windows.Forms;

using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Shell;
using SwissAcademic.Controls;


namespace SwitchActiveWindows
{
    public class SwitchToTableView
                        :
        CitaviAddOn<MainForm>
    {
        public override void OnHostingFormLoaded(MainForm mainForm)
        {
            var button = mainForm.GetMainCommandbarManager().GetReferenceEditorCommandbar(MainFormReferenceEditorCommandbarId.Menu).GetCommandbarMenu(MainFormReferenceEditorCommandbarMenuId.Window).InsertCommandbarButton(4, "Switch to table view", "Switch active window to table view");
            button.Text = "Switch to table view";
            button.Shortcut = (System.Windows.Forms.Shortcut)(System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.W);
        }

        public override void OnBeforePerformingCommand(MainForm mainForm, BeforePerformingCommandEventArgs e)
        {
            switch (e.Key)
            {

                case "Switch to table view":
                    {
                        e.Handled = true;
                        string nameToCheck = "ReferenceGridForm";
                        List<string> formNameList = new List<string>();
                        for (int i = 0; i < Application.OpenForms.Count; i++)
                        {
                            Form myform = Application.OpenForms[i];
                            if (!myform.Visible) continue;
                            formNameList.Add(myform.Name);
                            if (myform.Name  == nameToCheck)
                            {
                                myform.Activate();
                            }
                            //form.Activate();
                            //MessageBox.Show(myform.Name);
                        }

                        bool isInList = formNameList.Contains(nameToCheck);
                        if (! isInList)
                        { // 名字不在列表中
                            MessageBox.Show("Table view did not open");
                        }
                    }
                    break;

            }

            base.OnBeforePerformingCommand(mainForm, e);
        }
    }

}
