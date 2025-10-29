using SwissAcademic.Citavi.Controls.Wpf;
using SwissAcademic.Citavi.Shell.Controls.Preview;
using System.Collections.Generic;
using System.Reflection;

using SwissAcademic.Citavi;
using System.Linq;



namespace TextMarkerColorWithoutKnowledge
{
    public static class Extension
    {
        public static PdfViewControl GetPdfViewer(this PreviewControl previewControl)
        {
           return previewControl.GetType().GetProperty("PdfViewControl", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(previewControl) as PdfViewControl;
        }
        public static PdfViewControl GetPdfViewControl(this PreviewControl previewControl)
        {
            return previewControl?
                   .GetType()?
                   .GetField
                    (
                       "_pdfViewControl",
                       System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic
                    )?
                   .GetValue(previewControl) as PdfViewControl;
        }
        public static IEnumerable<Annotation> GetSelectedAnnotations(this PdfViewControl pdfViewControl)
        {
            return pdfViewControl?
                   .Tool
                   .GetSelectedHighlights()?
                   .Select(adornmentCanvas => adornmentCanvas.Annotation)
                   .Where(annotation => annotation != null)
                   .Distinct()
                   .ToList();
        }
        static List<AdornmentCanvas> GetSelectedHighlights(this Tool tool)
        {
            return tool?
                  .GetType()?
                  .GetField
                   (
                       "SelectedAdornmentContainers",
                       System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic
                   )?
                  .GetValue(tool) as List<AdornmentCanvas>;
        }
    }
}
