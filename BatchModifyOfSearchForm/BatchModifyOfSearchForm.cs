using SwissAcademic.Citavi.Shell;
using SwissAcademic.Citavi;
using SwissAcademic.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatchModifyOfSearchFormAddon
{
    public class BatchModifyOfSearchForm
                        :
        CitaviAddOn<MainForm>
    {
        public override void OnHostingFormLoaded(MainForm mainForm)
        {
            mainForm.GetMainCommandbarManager().GetReferenceEditorCommandbar(MainFormReferenceEditorCommandbarId.Toolbar).InsertCommandbarButton(7, "SetFilter", "Set Filter for selected references of SearchForm in MainForm", CommandbarItemStyle.ImageOnly, image: SwissAcademic.Citavi.Shell.Properties.Resources.FilterByMore);
            mainForm.GetMainCommandbarManager().GetReferenceEditorCommandbar(MainFormReferenceEditorCommandbarId.Toolbar).InsertCommandbarButton(8, "AssignCategories", "Assign Categories for selected references of SearchForm", CommandbarItemStyle.ImageOnly, image: SwissAcademic.Citavi.Shell.Properties.Resources.CategoryAdd);
            mainForm.GetMainCommandbarManager().GetReferenceEditorCommandbar(MainFormReferenceEditorCommandbarId.Toolbar).InsertCommandbarButton(9, "RemoveCategories", "Remove Categories for selected references of SearchForm", CommandbarItemStyle.ImageOnly, image: SwissAcademic.Citavi.Shell.Properties.Resources.CategoryDelete);
            mainForm.GetMainCommandbarManager().GetReferenceEditorCommandbar(MainFormReferenceEditorCommandbarId.Toolbar).InsertCommandbarButton(10, "ApplyGroups", "Apply Groups for selected references of SearchForm", CommandbarItemStyle.ImageOnly, image: SwissAcademic.Citavi.Shell.Properties.Resources.GroupAdd);
            mainForm.GetMainCommandbarManager().GetReferenceEditorCommandbar(MainFormReferenceEditorCommandbarId.Toolbar).InsertCommandbarButton(11, "RemoveGroups", "Remove Groups for selected references of SearchForm", CommandbarItemStyle.ImageOnly, image: SwissAcademic.Citavi.Shell.Properties.Resources.GroupDelete);
            base.OnHostingFormLoaded(mainForm);
        }

        public override void OnBeforePerformingCommand(MainForm mainForm, BeforePerformingCommandEventArgs e)
        {
            
            switch (e.Key)
            {
                case "SetFilter":
                    {
                        e.Handled = true;
                        // 判断SearchForm是否已经打开
                        string Statu = myCheckStatus("SetFilter");
                        if (Statu == "OK")
                        {
                            // 名字在列表中
                            SearchForm searchForm = Program.ActiveProjectShell.SearchForm;
                            // 获取类型信息
                            Type type = typeof(SearchForm);
                            // 获取私有方法信息
                            MethodInfo methodInfo = type.GetMethod("GetSelectedReferences", BindingFlags.NonPublic | BindingFlags.Instance);
                            // 调用私有方法
                            List<Reference> references = (List<Reference>)methodInfo.Invoke(searchForm, null);

                            ReferenceFilter referenceFilter = new ReferenceFilter(references, "Search Selected Result", false);
                            mainForm.ReferenceEditorFilterSet.Filters.ReplaceBy(new List<ReferenceFilter> { referenceFilter });
                            mainForm.Activate();
                        }
                    }
                    break;
                case "AssignCategories":
                    {
                        e.Handled = true;
                        // 判断SearchForm是否已经打开
                        string Statu = myCheckStatus("BatchAssignCategories");
                    }
                    break;
                case "RemoveCategories":
                    {
                        e.Handled = true;
                        string Statu = myCheckStatus("BatchRemoveCategories");
                    }
                    break;
                case "ApplyGroups":
                    {
                        e.Handled = true;
                        string Statu = myCheckStatus("BatchApplyGroups");
                    }
                    break;
                case "RemoveGroups":
                    {
                        e.Handled = true;
                        string Statu = myCheckStatus("BatchRemoveGroups");
                    }
                    break;
            }

            base.OnBeforePerformingCommand(mainForm, e);
        }

        public static string myCheckStatus(string button_name)
        {
            List<string> formNameList = new List<string>();
            for (int i = 0; i < Application.OpenForms.Count; i++)
            {
                Form myform = Application.OpenForms[i];
                if (!myform.Visible) continue;
                formNameList.Add(myform.Name);
            }
            // 判断一个名字是否在字符串列表中
            string nameToCheck = "SearchForm";
            bool isInList = formNameList.Contains(nameToCheck);
            if (!isInList)
            {
                // 名字不在列表中
                MessageBox.Show("SearchForm did not open");
                return "NoSearchForm";
            }
            else
            {
                // 名字在列表中
                SearchForm searchForm = Program.ActiveProjectShell.SearchForm;
                // 获取类型信息
                Type type = typeof(SearchForm);
                // 获取私有方法信息
                MethodInfo methodInfo = type.GetMethod("GetSelectedReferences", BindingFlags.NonPublic | BindingFlags.Instance);
                // 调用私有方法
                List<Reference> references = (List<Reference>)methodInfo.Invoke(searchForm, null);
                if (references.Count == 0)
                {
                    MessageBox.Show("No reference selected in searchForm");
                    return "NoReference";
                }
                else
                {
                    if (button_name == "SetFilter")
                    {
                        return "OK";
                    }
                    else
                    {
                        searchForm.PerformCommand(button_name);
                        return "OK";
                    }  
                }
            }


        }
    }
}
