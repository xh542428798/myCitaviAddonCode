using SwissAcademic.Citavi.Shell;
using SwissAcademic.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoRef
{
    public class Addon : CitaviAddOnEx<MacroEditorForm>
    {
        // Fields

        IEnumerable<string> _defaultAssemblies;

        // Methods

        public override void OnHostingFormLoaded(MacroEditorForm macroEditorForm)
        {
            _defaultAssemblies = _defaultAssemblies ?? macroEditorForm.GetReferencedAssemblies();
        }

        public override void OnBeforePerformingCommand(MacroEditorForm macroEditorForm, BeforePerformingCommandEventArgs e)
        {
            if (e.Key.Equals("Save", StringComparison.OrdinalIgnoreCase) || e.Key.Equals("SaveAs", StringComparison.OrdinalIgnoreCase))
            {
                var assemblies = macroEditorForm.GetReferencedAssemblies().Except(_defaultAssemblies, StringEqualityComparer.OrdinalIgnoreCase).ToList();

                macroEditorForm.AddAutoRefComments(assemblies);
            }
        }

        public override void OnAfterPerformingCommand(MacroEditorForm macroEditorForm, AfterPerformingCommandEventArgs e)
        {
            switch (e.Key)
            {
                case "Save":
                case "SaveAs":
                    {
                        macroEditorForm.RemoveAutoRefComments();
                    }
                    break;
                case "Open":
                    {
                        var references = macroEditorForm.ParseAutoRefComments();
                        macroEditorForm.AddReferences(references);
                        macroEditorForm.RemoveAutoRefComments();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}