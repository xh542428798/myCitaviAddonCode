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
            
            mainForm.GetMainCommandbarManager().GetReferenceEditorCommandbar(MainFormReferenceEditorCommandbarId.Toolbar).InsertCommandbarButton(11,"ShowReferenceFromSearchForm", "Show active reference of SearchForm in MainForm", CommandbarItemStyle.ImageOnly, image: SwissAcademic.Citavi.Shell.Properties.Resources.Filter);
            base.OnHostingFormLoaded(mainForm);
        }

        public override void OnBeforePerformingCommand(MainForm mainForm, BeforePerformingCommandEventArgs e)
        {
            switch (e.Key)
            {
                case "ShowReferenceFromSearchForm":
                    {
                        e.Handled = true;

                        //Get the active ("primary") MainForm
                        SearchForm searchForm = Program.ActiveProjectShell.SearchForm;
                        // 获取类型信息
                        Type type = typeof(SearchForm);
                        // 获取私有方法信息
                        MethodInfo methodInfo = type.GetMethod("GetSelectedReferences", BindingFlags.NonPublic | BindingFlags.Instance);
                        // 调用私有方法
                        List<Reference> references = (List<Reference>)methodInfo.Invoke(searchForm, null);
                        if (references.Count == 0 )
                        {
                            MessageBox.Show("No reference selected in searchForm");
                        }
                        else
                        {
                            // MessageBox.Show(references[0].Title);
                            mainForm.ActiveReference = references[0]; //只会将第1个文献作为Active Reference显示 
                            mainForm.Activate();
                        }

                    }
                    break;
            }

            base.OnBeforePerformingCommand(mainForm, e);
        }
    }
}
