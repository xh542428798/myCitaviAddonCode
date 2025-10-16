using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DuplicateComparing;  // 添加这行
using System.Threading.Tasks;
using System.Windows.Forms;
using Infragistics.Win.UltraWinSchedule;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Shell;
using SwissAcademic.Controls;



namespace DuplicateComparingAddon
{
    public partial class DuplicateComparing
        :
        CitaviAddOn<MainForm>
    {
        public override void OnHostingFormLoaded(MainForm mainForm)
        {
            mainForm.GetMainCommandbarManager().GetReferenceEditorCommandbar(MainFormReferenceEditorCommandbarId.Menu).GetCommandbarMenu(MainFormReferenceEditorCommandbarMenuId.References).InsertCommandbarButton(14, "Merge duplicates", "Merge the selected references");
            //button.Shortcut = (System.Windows.Forms.Shortcut)(System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.W);
            mainForm.GetReferenceEditorNavigationCommandbarManager().GetCommandbar(MainFormReferenceEditorNavigationCommandbarId.Toolbar).GetCommandbarMenu(MainFormReferenceEditorNavigationCommandbarMenuId.Tools).AddCommandbarButton("Merge duplicates", "Merge the selected references");
        }

        public override void OnBeforePerformingCommand(MainForm mainForm, BeforePerformingCommandEventArgs e)
        {
            SwissAcademic.Citavi.Project activeProject = Program.ActiveProjectShell.Project;

            switch (e.Key)
            {
                case "Merge duplicates":
                    {
                        e.Handled = true;
                        List<Reference> references = mainForm.GetSelectedReferences();

                        if (references.Count != 2)
                        {
                            MessageBox.Show("Currently this script only supports merging two references. Please select two and try again.");
                        }
                        else
                        {
                            if (references[0].ReferenceType != references[1].ReferenceType)
                            {
                                MessageBox.Show("Currently this script only supports merging two references of the same type. Please convert and try again.");
                            }
                            else
                            {
                                // 确定哪个是更早的引用（保留更早的作为基础）
                                if (DateTime.Compare(references[0].CreatedOn, references[1].CreatedOn) > 0)
                                {
                                    Reference temp = references[0];
                                    references[0] = references[1];
                                    references[1] = temp;
                                }

                                // 使用新的综合比较窗体
                                using (var dialog = new ComprehensiveMergeForm(mainForm, references[0], references[1]))
                                {
                                    if (dialog.ShowDialog() == DialogResult.OK)
                                    {
                                        // 合并完成后，删除第二个引用
                                        Program.ActiveProjectShell.Project.References.Remove(references[1]);
                                        MessageBox.Show("References merged successfully!");
                                    }
                                }
                            }
                        }
                    }
                    break;

            }

            base.OnBeforePerformingCommand(mainForm, e);
        }

    }
}

