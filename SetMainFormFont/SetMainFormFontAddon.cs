using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Shell;
using SwissAcademic.Controls;
using System.Drawing;

namespace SetMainFormFont
{
    public class SetMainFormFontAddon
        :
        CitaviAddOn<MainForm>
    {
        public override void OnHostingFormLoaded(MainForm mainForm)
        {
            mainForm.GetMainCommandbarManager().GetReferenceEditorCommandbar(MainFormReferenceEditorCommandbarId.Toolbar).InsertCommandbarButton(8,"SetMainFormFontTo13", "Set Font of MainForm to 13px", CommandbarItemStyle.ImageOnly, image: SwissAcademic.Citavi.Shell.Properties.Resources.FitHeight);

            base.OnHostingFormLoaded(mainForm);
        }

        public override void OnBeforePerformingCommand(MainForm mainForm, BeforePerformingCommandEventArgs e)
        {
            switch (e.Key)
            {
                case "SetMainFormFontTo13":
                    {
                        e.Handled = true;
                        Font font = new Font(mainForm.Font.FontFamily, 13); // 在此处指定所需的字体名称和字体大小
                        mainForm.Font = font;
                    }
                    break;
            }

            base.OnBeforePerformingCommand(mainForm, e);
        }
    }
}