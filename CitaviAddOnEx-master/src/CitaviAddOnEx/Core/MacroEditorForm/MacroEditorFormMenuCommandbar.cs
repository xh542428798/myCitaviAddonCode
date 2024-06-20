using Infragistics.Win.UltraWinToolbars;
using SwissAcademic.Controls;

namespace SwissAcademic.Citavi.Shell
{
    public class MacroEditorFormMenuCommandbar : MacroEditorFormCommandbar
    {
        // Constructors

        internal MacroEditorFormMenuCommandbar(UltraToolbar ultraToolbar) : base(ultraToolbar) { }

        // Methods

        public CommandbarMenu GetCommandbarMenu(MacroEditorFormCommandbarMenuId macroEditorFormCommandbarMenuId)
        {
            switch (macroEditorFormCommandbarMenuId)
            {
                case MacroEditorFormCommandbarMenuId.File:
                    return CommandbarMenu.Create(Toolbar.Tools["FileMenu"] as PopupMenuTool);

                case MacroEditorFormCommandbarMenuId.Edit:
                    return CommandbarMenu.Create(Toolbar.Tools["EditMenu"] as PopupMenuTool);

                case MacroEditorFormCommandbarMenuId.Build:
                    return CommandbarMenu.Create(Toolbar.Tools["BuildMenu"] as PopupMenuTool);

                case MacroEditorFormCommandbarMenuId.Tools:
                    return CommandbarMenu.Create(Toolbar.Tools["ToolsMenu"] as PopupMenuTool);

                case MacroEditorFormCommandbarMenuId.Help:
                    return CommandbarMenu.Create(Toolbar.Tools["HelpMenu"] as PopupMenuTool);

                default:
                    return null;
            }
        }
    }
}
