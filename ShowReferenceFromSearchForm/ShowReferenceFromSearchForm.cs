using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Shell;
using SwissAcademic.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;

namespace ShowReferenceFromSearchFormAddon
{
    public class ShowReferenceFromSearchForm
                        :
        CitaviAddOn<MainForm>
    {
        public override void OnHostingFormLoaded(MainForm mainForm)
        {
            
            mainForm.GetMainCommandbarManager().GetReferenceEditorCommandbar(MainFormReferenceEditorCommandbarId.Toolbar).InsertCommandbarButton(7,"ShowReferenceFromSearchForm", "Show active reference of SearchForm in MainForm", CommandbarItemStyle.ImageOnly, image: SwissAcademic.Citavi.Shell.Properties.Resources.Filter);
            base.OnHostingFormLoaded(mainForm);
        }

        public override void OnBeforePerformingCommand(MainForm mainForm, BeforePerformingCommandEventArgs e)
        {
            switch (e.Key)
            {
                case "ShowReferenceFromSearchForm":
                    {
                        e.Handled = true;
                        // 判断SearchForm是否已经打开
                        // 创建一个字符串列表，保存所有 Form 的名字
                        List<string> formNameList = new List<string>();
                        for (int i = 0; i < Application.OpenForms.Count; i++)
                        {
                            Form myform = Application.OpenForms[i];
                            if (!myform.Visible) continue;
                            formNameList.Add(myform.Name);
                            //form.Activate();
                            //MessageBox.Show(myform.Name);
                        }
                        // 判断一个名字是否在字符串列表中
                        string nameToCheck = "SearchForm";
                        bool isInList = formNameList.Contains(nameToCheck);
                        if (isInList)
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
                            }
                            else
                            {
                                mainForm.ActiveReference = references[0]; //只会将第1个文献作为Active Reference显示 
                                mainForm.Activate();
                            }
                        }
                        else
                        {
                            // 名字不在列表中
                            MessageBox.Show("SearchForm did not open");
                        }
                    }
                    break;
            }

            base.OnBeforePerformingCommand(mainForm, e);
        }
    }
}
