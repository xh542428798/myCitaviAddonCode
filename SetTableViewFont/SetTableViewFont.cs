using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Shell;
using System.Drawing;
using SwissAcademic.Controls;


namespace SetTableViewFontAddon
{
    public class SetTableViewFont
                    :
    CitaviAddOn<ReferenceGridForm>
    {
        public override void OnHostingFormLoaded(ReferenceGridForm gridForm)
        {
            gridForm.GetCommandbar(ReferenceGridFormCommandbarId.Toolbar).AddCommandbarButton("SetFontTo16", "Set Font of TableView to 16px", CommandbarItemStyle.ImageOnly, image: SwissAcademic.Citavi.Shell.Properties.Resources.FitHeight);
        }

        public override void OnBeforePerformingCommand(ReferenceGridForm gridForm, BeforePerformingCommandEventArgs e)
        {
            switch (e.Key)
            {

                case "SetFontTo16":
                    {
                        e.Handled = true;
                        Font font = new Font(gridForm.Font.FontFamily, 16); // 在此处指定所需的字体名称和字体大小
                        gridForm.Font = font;

                    }
                    break;

            }

            base.OnBeforePerformingCommand(gridForm, e);
        }
    }
}
