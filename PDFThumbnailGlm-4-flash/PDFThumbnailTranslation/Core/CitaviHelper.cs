using SwissAcademic.Citavi.Controls.Wpf;
using SwissAcademic.Citavi.Shell;
using SwissAcademic.Citavi.Shell.Controls.Preview;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PDFThumbnailTranslation
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

        // 在 CitaviHelper.cs 中添加这个新方法

        /// <summary>
        /// 获取PDF预览控件中当前选中的文本
        /// </summary>
        /// <param name="previewControl">Citavi的预览控件</param>
        /// <returns>选中的文本，如果没有选中文本则返回null</returns>
        internal static string GetSelectedTextFromPdf(this PreviewControl previewControl)
        {
            // 首先获取内部的PDF查看器
            var pdfViewer = previewControl.GetPdfViewControl();
            if (pdfViewer == null) return null;

            // 使用官方API获取选中的文本内容
            if (pdfViewer.GetSelectedContentFromType(SwissAcademic.Citavi.Controls.Wpf.ContentType.Text) is SwissAcademic.Citavi.Controls.Wpf.TextContent textContent)
            {
                return textContent.Text;
            }

            return null;
        }

    }

}
