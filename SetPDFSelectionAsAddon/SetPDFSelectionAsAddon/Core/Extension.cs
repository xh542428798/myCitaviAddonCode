using SwissAcademic.Citavi.Controls.Wpf;
using SwissAcademic.Citavi.Shell.Controls.Preview;
using System.Reflection;

namespace SetPDFSelectionAs
{
    public static class Extension
    {
        public static PdfViewControl GetPdfViewer(this PreviewControl previewControl)
        {
           return previewControl.GetType().GetProperty("PdfViewControl", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(previewControl) as PdfViewControl;
        }

        public static string GetSelectionAsText(this PreviewControl previewControl)
        {
            if (previewControl.GetPdfViewer()?.GetSelectedContentFromType(ContentType.Text) is TextContent textcontent) return textcontent.Text;
            return null;
        }

        public static void ClearSelection(this PreviewControl previewControl)
        {
            previewControl.GetPdfViewer()?.ClearSelection();
        }
    }
}
