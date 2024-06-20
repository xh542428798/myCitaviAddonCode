using Infragistics.Win.UltraWinToolbars;
using SwissAcademic.Controls;

namespace SwissAcademic.Citavi.Shell
{
    public class MacroEditorFormCommandbarManager : CommandbarManager
    {
        // Constructors

        protected internal MacroEditorFormCommandbarManager(UltraToolbarsManager toolbarsManager) : base(toolbarsManager)
        {
        }

        // Methods

        public MacroEditorFormCommandbar GetCommandbar(MacroEditorFormCommandbarId macroEditorFormCommandbarId)
        {
            switch (macroEditorFormCommandbarId)
            {
                case MacroEditorFormCommandbarId.Menu:
                    return new MacroEditorFormMenuCommandbar(ToolbarsManager.Toolbars["menu"]);
                case MacroEditorFormCommandbarId.Toolbar:
                    return new MacroEditorFormCommandbar(ToolbarsManager.Toolbars["standard"]);
                default:
                    return null;
            }
        }

        public MacroEditorFormMenuCommandbar GetMainMenuCommandbar() => GetCommandbar(MacroEditorFormCommandbarId.Menu) as MacroEditorFormMenuCommandbar;
    }
}
