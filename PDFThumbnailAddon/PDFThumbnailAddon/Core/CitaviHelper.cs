using SwissAcademic.Citavi.Controls.Wpf;
using SwissAcademic.Citavi.Shell;
using SwissAcademic.Citavi.Shell.Controls.Preview;
using System;
using System.Reflection;
using System.Windows.Controls;

namespace PDFThumbnail
{
    public static class CitaviHelper
    {
        static readonly BindingFlags Flags = BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic;

        internal static PdfViewControl GetPdfViewControl(this PreviewControl previewControl)
        {
            return previewControl?
                    .GetType()
                    .GetProperty("PdfViewControl", Flags, null, typeof(PdfViewControl), new Type[0], null)?
                    .GetValue(previewControl, Flags, null, null, null) as PdfViewControl;
        }

        internal static TabControl GetSideBar(this PdfViewControl pdfViewControl)
        {
            if (pdfViewControl == null) return null;

            return WPFHelper.FindChild<TabControl>(pdfViewControl);
        }

        internal static pdftron.PDF.PDFViewWPF GetViewer(this MainForm mainForm)
        {
            return
               mainForm
               .PreviewControl?
               .GetPdfViewControl()?
               .GetType()
               .GetProperty("Viewer", Flags)?
               .GetValue(mainForm.PreviewControl.GetPdfViewControl()) as pdftron.PDF.PDFViewWPF;
        }
    }
}
