using SwissAcademic.Citavi.Controls.Wpf;
using SwissAcademic.Citavi.Shell;
using System;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Windows;

namespace PDFThumbnailTranslation
{
    public class Addon : CitaviAddOn<MainForm>
    {
        #region Methods

        public override void OnHostingFormLoaded(MainForm mainForm)
        {
            // 插件加载时，添加翻译选项卡并开始观察事件
            AddTabPageToSideBar(mainForm);
            Observe(mainForm, true);

            base.OnHostingFormLoaded(mainForm);
        }

        void Observe(MainForm mainForm, bool observe)
        {
            if (observe)
            {
                mainForm.FormClosed += MainForm_FormClosed;

                var viewer = mainForm.PreviewControl.GetPdfViewControl();
                if (viewer != null)
                {
                    // 只订阅翻译功能需要的事件
                    viewer.DocumentChanged += Viewer_DocumentChanged;
                    viewer.DocumentClosing += Viewer_DocumentClosing;
                    viewer.SelectionChanged += Viewer_SelectionChanged;
                }
            }
            else
            {
                mainForm.FormClosed -= MainForm_FormClosed;

                var viewer = mainForm.PreviewControl.GetPdfViewControl();
                if (viewer != null)
                {
                    viewer.DocumentChanged -= Viewer_DocumentChanged;
                    viewer.DocumentClosing -= Viewer_DocumentClosing;
                    viewer.SelectionChanged -= Viewer_SelectionChanged;
                }
            }
        }

        void AddTabPageToSideBar(MainForm mainForm)
        {
            if (mainForm.PreviewControl.GetPdfViewControl().GetSideBar() is System.Windows.Controls.TabControl tabControl)
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri("/PDFThumbnailTranslation;component/Resources/TranslationIcon.png", UriKind.RelativeOrAbsolute);
                bitmapImage.EndInit();

                var translationControl = new TranslationControl(mainForm);

                var tabPage = new TabItem
                {
                    Tag = "TranslationAddon",
                    Header = new Image { Height = 16, Source = bitmapImage },
                    Content = translationControl
                };

                tabControl.Items.Add(tabPage);
            }
        }

        #endregion

        #region EventHandlers

        private void Viewer_SelectionChanged(object sender, EventArgs e)
        {
            var pdfViewer = sender as PdfViewControl;
            if (pdfViewer?.GetSideBar() is System.Windows.Controls.TabControl tabControl)
            {
                var translationTabItem = tabControl.Items.Cast<TabItem>()
                    .FirstOrDefault(item => item.Tag != null && item.Tag.ToString().Equals("TranslationAddon", StringComparison.OrdinalIgnoreCase));

                if (translationTabItem?.Content is TranslationControl translationControl)
                {
                    // 只有在UI上的复选框被勾选时，才执行翻译逻辑
                    if (translationControl.AutoTranslateCheckBox.IsChecked == true)
                    {
                        translationControl.OnSelectionChanged();
                    }
                }
            }
        }

        private void Viewer_DocumentChanged(object sender, EventArgs e)
        {
            // 当PDF文档切换时，清空翻译界面的内容
            if (sender is PdfViewControl viewer && viewer.GetSideBar() is System.Windows.Controls.TabControl tabControl)
            {
                var translationTabItem = tabControl.Items.Cast<TabItem>()
                    .FirstOrDefault(item => item.Tag != null && item.Tag.ToString().Equals("TranslationAddon", StringComparison.OrdinalIgnoreCase));

                if (translationTabItem != null && translationTabItem.Content is TranslationControl translationControl)
                {
                    translationControl.ClearContent();
                }
            }
        }

        private void Viewer_DocumentClosing(object sender, DocumentClosingArgs args)
        {
            // 当PDF文档关闭时，清空翻译界面的内容
            if (sender is PdfViewControl viewer && viewer.GetSideBar() is System.Windows.Controls.TabControl tabControl)
            {
                var translationTabItem = tabControl.Items.Cast<TabItem>()
                    .FirstOrDefault(item => item.Tag != null && item.Tag.ToString().Equals("TranslationAddon", StringComparison.OrdinalIgnoreCase));

                if (translationTabItem != null && translationTabItem.Content is TranslationControl translationControl)
                {
                    translationControl.ClearContent();
                }
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sender is MainForm mainForm)
            {
                Observe(mainForm, false);
            }
        }

        #endregion
    }
}
