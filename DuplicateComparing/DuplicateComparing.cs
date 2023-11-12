using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DuplicateComparing;
using Infragistics.Win.UltraWinSchedule;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Shell;
using SwissAcademic.Controls;

namespace DuplicateComparingAddon
{
    public class DuplicateComparing
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
                                string originalTitle = references[0].Title;

                                // CreatedOn, check which one is older and then take that CreatedOn and CreatedBy
                                if (DateTime.Compare(references[0].CreatedOn, references[1].CreatedOn) > 0)
                                {
                                    // second reference is older,将referecne1改为更新的,0为更老更重要
                                    // CreatedOn is write-protected. We therefore switch the references...
                                    Reference newer = references[0];
                                    references[0] = references[1];
                                    references[1] = newer;
                                }

                                // Title
                                string strLeft = references[0].Title.Trim();
                                string strRight = references[1].Title.Trim();
                                // do not compare ignore case, otherwise we might lose capitalization information; in that case we rely on manual edits after the merge
                                if (String.Compare(strLeft, strRight, false) == 0)
                                {
                                    using (var dialog = new ComparingForm(mainForm, strLeft,strRight))
                                    {
                                        if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                                        {
                                            string result = dialog.DialogResult; // 获取对话框返回的结果
                                            if(result== "left")
                                            {
                                                references[0].Title = references[0].Title;
                                            }
                                            else if (result == "right")
                                            {
                                                references[0].Title = references[1].Title;
                                            }
                                            else if(result == "combine")
                                            {
                                                references[0].Title = references[0].Title + " // " + references[1].Title;
                                            }
                                            //if(string.IsNullOrEmpty(result))
                                            //{
                                            //    MessageBox.Show(result);

                                            //}

                                        }
                                        ;
                                    };
                                }


                                // ReferenceCategory
                                Reference reference = references[0];
                                reference.Categories.FindName
                                string strLeft = references[0].Categories.ToList();
                                references[0].Categories.AddRange(references[1].Categories);

                            }
                        }


                   
                    }
                    break;

            }

            base.OnBeforePerformingCommand(mainForm, e);
        }

    }
}

