using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Shell;
using SwissAcademic.Controls;


namespace SearchInActiveGroupAddon
{
    public class SearchInActiveGroup
                :
        CitaviAddOn<MainForm>
    {
        public override void OnHostingFormLoaded(MainForm mainForm)
        {
            //这里是设置在Reference的搜索框旁边
            //mainForm.GetReferenceEditorNavigationCommandbarManager().GetCommandbar(MainFormReferenceEditorNavigationCommandbarId.Toolbar).AddCommandbarButton("SearchInActiveCategory", "Search active category in advanced search", CommandbarItemStyle.ImageOnly, SwissAcademic.Citavi.Shell.Properties.Resources.SearchKnowledge);
            //mainForm.GetReferenceEditorNavigationCommandbarManager().GetCommandbar(MainFormReferenceEditorNavigationCommandbarId.Toolbar).AddCommandbarButton("SearchInActiveGroup", "Search active group in advanced search", CommandbarItemStyle.ImageOnly, SwissAcademic.Citavi.Shell.Properties.Resources.Search);

            // 这里是放在开关侧边栏的那里
            mainForm.GetReferenceEditorFilterCommandbarManager().GetCommandbar(MainFormReferenceEditorFilterCommandbarId.Toolbar).AddCommandbarButton("SearchInActiveCategory", "Search active category in advanced search", CommandbarItemStyle.ImageOnly, SwissAcademic.Citavi.Shell.Properties.Resources.SearchKnowledge);
            mainForm.GetReferenceEditorFilterCommandbarManager().GetCommandbar(MainFormReferenceEditorFilterCommandbarId.Toolbar).AddCommandbarButton("SearchInActiveGroup", "Search active group in advanced search", CommandbarItemStyle.ImageOnly, SwissAcademic.Citavi.Shell.Properties.Resources.Search);

            base.OnHostingFormLoaded(mainForm);
        }

        public override void OnBeforePerformingCommand(MainForm mainForm, BeforePerformingCommandEventArgs e)
        {
            switch (e.Key)
            {

                case "SearchInActiveCategory":
                    {
                        e.Handled = true;
                        Category my_Category = mainForm.GetSelectedReferenceEditorCategory();
                        Program.ActiveProjectShell.ShowSearchForm(mainForm, SearchFormWorkspace.Extended);
                        if (my_Category != null)
                        {
                            // 在这里修改自己的搜索策略  字段缩写:搜索词
                            string search_string = String.Format("rc:\"{0}\" AND t1:请输入", my_Category.FullName);
                            SearchForm mySearchForm = Program.ActiveProjectShell.SearchForm;
                            mySearchForm.SetQuery(search_string);
                        }
                        //MessageBox.Show(my_group.Name);

                    }
                    break;
                case "SearchInActiveGroup":
                    {
                        e.Handled = true;
                        // System.Windows.Forms.MessageBox.Show("Finished"); 
                        //Get the active ("primary") MainForm
                        Group my_group = mainForm.GetSelectedReferenceEditorGroup();
                        Program.ActiveProjectShell.ShowSearchForm(mainForm, SearchFormWorkspace.Extended);
                        //MessageBox.Show(my_group.Name);
                        if (my_group != null)
                        {
                            // 在这里修改自己的搜索策略  字段缩写:搜索词
                            string search_string = String.Format("rg:\"{0}\" AND t1:请输入", my_group.FullName);
                            SearchForm mySearchForm = Program.ActiveProjectShell.SearchForm;
                            mySearchForm.SetQuery(search_string);
                        }
                    }
                    break;
            }

            base.OnBeforePerformingCommand(mainForm, e);
        }
    }
}
