using SwissAcademic.Citavi.Shell;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AutoRef
{
    internal static class Extensions
    {
        // Fields

        static readonly BindingFlags fieldBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        static readonly BindingFlags privateMethodBindingFlags = BindingFlags.InvokeMethod | BindingFlags.NonPublic;
        static readonly BindingFlags staticEventBindingFlags = fieldBindingFlags | BindingFlags.Static;
        static readonly BindingFlags staticFieldBindingFlags = BindingFlags.NonPublic | BindingFlags.Static;

        static readonly string regex_pattern = "\\/\\/[ ]*autoref[ ]*\"(.+)\"";

        // Methods

        static string GetMacroDirectory(this MacroEditorForm macroEditorForm)
        {
            var fileName = macroEditorForm.GetType().GetField("_fileName", fieldBindingFlags)?.GetValue(macroEditorForm).ToString();
            if (string.IsNullOrEmpty(fileName) || !System.IO.File.Exists(fileName)) return null;
            return System.IO.Path.GetDirectoryName(fileName);
        }

        public static IEnumerable<string> GetReferencedAssemblies(this MacroEditorForm macroEditorForm)
        {
            var macroEditor = macroEditorForm.GetType().GetField("macroEditor", fieldBindingFlags)?.GetValue(macroEditorForm);
            var compilerParameters = macroEditor?.GetType().GetField("_compilerParameters", fieldBindingFlags)?.GetValue(macroEditor) as CompilerParameters;
            return compilerParameters?.ReferencedAssemblies.Cast<string>().ToList();
        }

        public static void AddAutoRefComments(this MacroEditorForm macroEditorForm, IEnumerable<string> assemblies)
        {
            var stringBuilder = new StringBuilder();
            foreach (var assembly in assemblies)
            {
                stringBuilder.AppendLine($"// autoref \"{ConvertToCommentPath(assembly, macroEditorForm)}\"\r");
            }
            stringBuilder.AppendLine(macroEditorForm.MacroCode);
            macroEditorForm.MacroCode = stringBuilder.ToString().Trim();
        }

        public static void AddReferences(this MacroEditorForm macroEditorForm, IEnumerable<string> references)
        {
            if (references.Count() == 0) return;

            var macroEditor = macroEditorForm.GetType().GetField("macroEditor", fieldBindingFlags)?.GetValue(macroEditorForm);
            var compilerParameters = macroEditor?.GetType().GetField("_compilerParameters", fieldBindingFlags)?.GetValue(macroEditor) as CompilerParameters;


            compilerParameters?.ReferencedAssemblies.AddRange(references.ToArray());
            macroEditor?.GetType().GetMethod("ResetDotNetProjectResolver", privateMethodBindingFlags)?.Invoke(macroEditor, new object[] { });
            macroEditor?.GetType().GetMethod("RemoveRestrictedAssembliesFromCompilerParameters", privateMethodBindingFlags)?.Invoke(macroEditor, new object[] { });
        }

        public static void RemoveAutoRefComments(this MacroEditorForm macroEditorForm)
        {
            var codeLines = macroEditorForm.MacroCode.Split(new char[] { '\n' }).ToList();
            codeLines.RemoveAll(line => Regex.IsMatch(line, regex_pattern));
            macroEditorForm.MacroCode = string.Join("\n", codeLines).Trim();
        }

        public static IEnumerable<string> ParseAutoRefComments(this MacroEditorForm macroEditorForm)
        {
            return Regex.Matches(macroEditorForm.MacroCode, regex_pattern)
                        .Cast<Match>()
                        .Where(match => match.Groups.Count >= 2)
                        .Select(match => ConvertToAssemblyPath(macroEditorForm, match.Groups[1].Value))
                        .Where(path => !string.IsNullOrEmpty(path))
                        .Distinct(StringEqualityComparer.OrdinalIgnoreCase)
                        .ToList();
        }

        static string ConvertToAssemblyPath(MacroEditorForm macroEditorForm, string match)
        {
            if (string.IsNullOrEmpty(match)) return null;
            if (System.IO.Path.IsPathRooted(match) && System.IO.File.Exists(match)) return match;
            if (!string.IsNullOrEmpty(macroEditorForm.GetMacroDirectory()) && System.IO.Path.Combine(macroEditorForm.GetMacroDirectory(), match) is string macroPath && System.IO.File.Exists(macroPath)) return macroPath;
            if (System.IO.Path.Combine(Application.StartupPath, match) is string applicationPath && System.IO.File.Exists(applicationPath)) return applicationPath;
            return null;
        }

        static string ConvertToCommentPath(this string path, MacroEditorForm macroEditorForm)
        {
            if (!System.IO.Path.IsPathRooted(path) || !System.IO.File.Exists(path)) return path;
            if (path.StartsWith(macroEditorForm.GetMacroDirectory(), StringComparison.OrdinalIgnoreCase)) return System.IO.Path.GetFileName(path);
            if (path.StartsWith(Application.StartupPath, StringComparison.OrdinalIgnoreCase)) return System.IO.Path.GetFileName(path);
            return path;
        }
    }
}