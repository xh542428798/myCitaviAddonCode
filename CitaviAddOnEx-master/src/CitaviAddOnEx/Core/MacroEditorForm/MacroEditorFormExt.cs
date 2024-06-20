namespace SwissAcademic.Citavi.Shell
{
    public static class MacroEditorFormExt
    {
        public static MacroEditorFormCommandbarManager GetCommandbarManager(this MacroEditorForm macroEditorForm) => new MacroEditorFormCommandbarManager(macroEditorForm.GetToolbarsManager());
    }
}