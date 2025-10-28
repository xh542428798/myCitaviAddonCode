using SetPDFSelectionAs.Properties;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Shell;
using SwissAcademic.Controls;

namespace SetPDFSelectionAs
{
    public class SetPDFSelectionAs : CitaviAddOn<MainForm>
    {
        #region Constants

        const string Keys_Button_Title = "SetPDFSelectionAsAddon.Button_Title";
        const string Keys_Button_Author_FirstName_LastName = "SetPDFSelectionAsAddon.Button_Author_FirstName_LastName";
        const string Keys_Button_Author_LastName_FirstName = "SetPDFSelectionAsAddon.Button_Author_LastName_FirstName";
        // 【新增】为中国姓名按钮添加常量
        const string Keys_Button_Chinese_Name = "SetPDFSelectionAsAddon.Button_Chinese_Name";
        #endregion

        #region Methods

        public override void OnHostingFormLoaded(MainForm mainForm)
        {
            var button = mainForm.GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar).GetCommandbarMenu(MainFormPreviewCommandbarMenuId.More).InsertCommandbarButton(3, Keys_Button_Title, SetPDFSelectionAsAddonResources.Button_Title);
            button.HasSeparator = true;

            mainForm.GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar).GetCommandbarMenu(MainFormPreviewCommandbarMenuId.More).InsertCommandbarButton(4, Keys_Button_Author_FirstName_LastName, SetPDFSelectionAsAddonResources.Button_Author_FirstName_LastName);
            mainForm.GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar).GetCommandbarMenu(MainFormPreviewCommandbarMenuId.More).InsertCommandbarButton(5, Keys_Button_Author_LastName_FirstName, SetPDFSelectionAsAddonResources.Button_Author_LastName_FirstName);
            // 【新增】添加中国姓名按钮
            mainForm.GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar).GetCommandbarMenu(MainFormPreviewCommandbarMenuId.More).InsertCommandbarButton(6, Keys_Button_Chinese_Name, SetPDFSelectionAsAddonResources.Button_Chinese_Name);

            base.OnHostingFormLoaded(mainForm);
        }

        public override void OnLocalizing(MainForm mainForm)
        {
            var button = mainForm.GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar).GetCommandbarMenu(MainFormPreviewCommandbarMenuId.More).GetCommandbarButton(Keys_Button_Title);
            if (button != null) button.Text = SetPDFSelectionAsAddonResources.Button_Title;

            button = mainForm.GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar).GetCommandbarMenu(MainFormPreviewCommandbarMenuId.More).GetCommandbarButton(Keys_Button_Author_FirstName_LastName);
            if (button != null) button.Text = SetPDFSelectionAsAddonResources.Button_Author_FirstName_LastName;

            button = mainForm.GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar).GetCommandbarMenu(MainFormPreviewCommandbarMenuId.More).GetCommandbarButton(Keys_Button_Author_LastName_FirstName);
            if (button != null) button.Text = SetPDFSelectionAsAddonResources.Button_Author_LastName_FirstName;
            // 【新增】本地化中国姓名按钮
            button = mainForm.GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar).GetCommandbarMenu(MainFormPreviewCommandbarMenuId.More).GetCommandbarButton(Keys_Button_Chinese_Name);
            if (button != null) button.Text = SetPDFSelectionAsAddonResources.Button_Chinese_Name;

            base.OnLocalizing(mainForm);
        }

        public override void OnBeforePerformingCommand(MainForm mainForm, BeforePerformingCommandEventArgs e)
        {
            
            var reference = mainForm.PreviewControl.ActiveLocation?.Reference;

            if (reference != null)
            {
                e.Handled = true;
                switch (e.Key)
                {
                    case (Keys_Button_Title):
                        {
                            var selectionAsText = mainForm.PreviewControl.GetSelectionAsText();
                            reference.Title = selectionAsText;
                            mainForm.PreviewControl.ClearSelection();
                            break;
                        }

                    case (Keys_Button_Author_FirstName_LastName):
                        {
                            var selectionAsText = mainForm.PreviewControl.GetSelectionAsText();
                            var persons = PersonTextParser.ParseRawTextInFirstNameLastNameSequence(reference.Project, selectionAsText);
                            reference.Authors.AddRange(persons);
                            mainForm.PreviewControl.ClearSelection();
                            break;
                        }
                    case (Keys_Button_Author_LastName_FirstName):
                        {
                            var selectionAsText = mainForm.PreviewControl.GetSelectionAsText();
                            var person = PersonTextParser.ParseSinglePersonName(selectionAsText, PersonTextParserSettings.CarriageReturnOrSemicolon_LastNameFirstnameWithComma, reference.Project);
                            reference.Authors.Add(person);
                            mainForm.PreviewControl.ClearSelection();
                            break;
                        }
                    // 【新增】处理中国姓名的逻辑
                    case (Keys_Button_Chinese_Name):
                        {
                            var selectionAsText = mainForm.PreviewControl.GetSelectionAsText();

                            // 1. 清理字符串：去掉前后和中间的所有空格
                            string cleanedName = System.Text.RegularExpressions.Regex.Replace(selectionAsText, @"\s", "");

                            if (!string.IsNullOrEmpty(cleanedName))
                            {
                                string lastName = cleanedName.Substring(0, 1); // 第一个字是姓
                                string firstName = cleanedName.Substring(1);   // 后面的都是名

                                // 创建一个新的 Person 对象
                                var person = new Person(reference.Project);
                                person.LastName = lastName;
                                person.FirstName = firstName;

                                reference.Authors.Add(person);
                            }

                            mainForm.PreviewControl.ClearSelection();
                            break;
                        }
                    default:
                        {
                            e.Handled = false;
                            break;
                        }
                }
            }

            base.OnBeforePerformingCommand(mainForm, e);
        }

        public override void OnApplicationIdle(MainForm mainForm)
        {
            if (mainForm.PreviewControl.ActivePreviewType == SwissAcademic.Citavi.Shell.Controls.Preview.PreviewType.Pdf)
            {
                var pdfViewer = mainForm.PreviewControl.GetPdfViewer();
                var isTextSelected = pdfViewer.GetSelectedContentType() == SwissAcademic.Citavi.Controls.Wpf.ContentType.Text;

                var button = mainForm.GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar).GetCommandbarMenu(MainFormPreviewCommandbarMenuId.More).GetCommandbarButton(Keys_Button_Title);
                if (button != null) button.Visible = isTextSelected;

                button = mainForm.GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar).GetCommandbarMenu(MainFormPreviewCommandbarMenuId.More).GetCommandbarButton(Keys_Button_Author_FirstName_LastName);
                if (button != null) button.Visible = isTextSelected;

                button = mainForm.GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar).GetCommandbarMenu(MainFormPreviewCommandbarMenuId.More).GetCommandbarButton(Keys_Button_Author_LastName_FirstName);
                if (button != null) button.Visible = isTextSelected;
                // 【新增】控制中国姓名按钮的可见性
                button = mainForm.GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar).GetCommandbarMenu(MainFormPreviewCommandbarMenuId.More).GetCommandbarButton(Keys_Button_Chinese_Name);
                if (button != null) button.Visible = isTextSelected;
            }
            else
            {
                var button = mainForm.GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar).GetCommandbarMenu(MainFormPreviewCommandbarMenuId.More).GetCommandbarButton(Keys_Button_Title);
                if (button != null) button.Visible = false;

                button = mainForm.GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar).GetCommandbarMenu(MainFormPreviewCommandbarMenuId.More).GetCommandbarButton(Keys_Button_Author_FirstName_LastName);
                if (button != null) button.Visible = false;

                button = mainForm.GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar).GetCommandbarMenu(MainFormPreviewCommandbarMenuId.More).GetCommandbarButton(Keys_Button_Author_LastName_FirstName);
                if (button != null) button.Visible = false;
                // 【新增】在没有PDF预览时隐藏按钮
                button = mainForm.GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar).GetCommandbarMenu(MainFormPreviewCommandbarMenuId.More).GetCommandbarButton(Keys_Button_Chinese_Name);
                if (button != null) button.Visible = false;
            }

            base.OnApplicationIdle(mainForm);
        }


        #endregion
    }
}