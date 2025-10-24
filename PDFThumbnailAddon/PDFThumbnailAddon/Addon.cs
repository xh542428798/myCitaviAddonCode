using SwissAcademic.Citavi.Controls.Wpf;
using SwissAcademic.Citavi.Shell;
using System;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Windows;

namespace PDFThumbnail
{
    public class Addon : CitaviAddOn<MainForm>
    {
        #region Methods

        public override void OnHostingFormLoaded(MainForm mainForm)
        {
            AddTabPageToSideBar(mainForm);
            // 注意：不再在这里调用 AddExpandCollapseButtonToBookmarksTab
            Observe(mainForm, true);

            base.OnHostingFormLoaded(mainForm);
        }

        void Observe(MainForm mainForm, bool observe)
        {
            if (observe)
            {
                mainForm.FormClosed += MainForm_FormClosed;

                var viewer = mainForm.PreviewControl.GetPdfViewControl();
                viewer.DocumentChanged += Viewer_DocumentChanged; // 这是关键
                viewer.DocumentClosing += Viewer_DocumentClosing;
            }
            else
            {
                mainForm.FormClosed -= MainForm_FormClosed;

                var viewer = mainForm.PreviewControl.GetPdfViewControl();
                viewer.DocumentChanged -= Viewer_DocumentChanged;
                viewer.DocumentClosing -= Viewer_DocumentClosing;
            }
        }

        void AddTabPageToSideBar(MainForm mainForm)
        {
            if (mainForm.PreviewControl.GetPdfViewControl().GetSideBar() is System.Windows.Controls.TabControl tabControl)
            {

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri("/PDFThumbnailAddon;component/Resources/thumbs.png", UriKind.RelativeOrAbsolute);
                bitmapImage.EndInit();

                var thumbnailControl = new ThumbnailsControl(mainForm.GetViewer());

                var tabPage = new TabItem
                {
                    Tag = "Addon",
                    Header = new Image { Height = 16, Source = bitmapImage },
                    Content = thumbnailControl
                };

                tabControl.Items.Add(tabPage);

                thumbnailControl.Refresh();
            }
        }

        #endregion

        #region EventHandlers

        // 优化点 1 & 2：在这里添加按钮，确保每个新文档都会尝试添加
        private void Viewer_DocumentChanged(object sender, EventArgs e)
        {
            if (sender is PdfViewControl viewer && viewer.GetSideBar() is System.Windows.Controls.TabControl tabControl)
            {
                var tabitem = tabControl.Items.Cast<TabItem>().FirstOrDefault(item => item.Tag != null && item.Tag.ToString().Equals("Addon", StringComparison.OrdinalIgnoreCase));
                if (tabitem != null && tabitem.Content is ThumbnailsControl thumbnailsControl)
                    thumbnailsControl.Refresh();
            }

            // --- 新增的核心逻辑 ---
            // 每当文档变化时，都尝试为书签页添加按钮
            // 这确保了按钮在需要时总能被正确添加
            if (sender is PdfViewControl pdfViewer)
            {
                AddExpandCollapseButtonToBookmarksTab(pdfViewer);
            }
        }

        private void Viewer_DocumentClosing(object sender, DocumentClosingArgs args)
        {
            if (sender is PdfViewControl viewer && viewer.GetSideBar() is System.Windows.Controls.TabControl tabControl)
            {
                var tabitem = tabControl.Items.Cast<TabItem>().FirstOrDefault(item => item.Tag != null && item.Tag.ToString().Equals("Addon", StringComparison.OrdinalIgnoreCase));
                if (tabitem != null && tabitem.Content is ThumbnailsControl thumbnailsControl)
                    thumbnailsControl.Clear();
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

        #region Bookmarks Button Logic

        /// <summary>
        /// 在PDF预览的目录页面添加一个全部展开/收起的按钮
        /// </summary>
        /// <param name="pdfViewer">PdfViewControl实例</param>
        public void AddExpandCollapseButtonToBookmarksTab(PdfViewControl pdfViewer)
        {
            var sideBarTabControl = pdfViewer?.GetSideBar();
            if (sideBarTabControl == null) return;

            var bookmarksTabItem = sideBarTabControl.Items.Cast<TabItem>()
                .FirstOrDefault(item => item.Content is BookmarkSidebar);

            if (bookmarksTabItem == null) return;

            var bookmarkSidebar = bookmarksTabItem.Content as BookmarkSidebar;
            var rootGrid = WPFHelper.FindChild<Grid>(bookmarkSidebar);
            if (rootGrid == null) return;

            // 检查是否已经添加过按钮，避免重复添加
            if (rootGrid.Children.OfType<System.Windows.Controls.Button>().Any(b => (string)b.Tag == "ExpandCollapseButton"))
            {
                return; // 按钮已存在
            }

            var expandCollapseButton = new System.Windows.Controls.Button
            {
                Content = "▼ 全部展开",
                Tag = "ExpandCollapseButton",
                Margin = new Thickness(5),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
            };

            expandCollapseButton.Click += (sender, e) =>
            {
                var bookmarkTree = WPFHelper.FindChild<System.Windows.Controls.TreeView>(bookmarkSidebar);
                if (bookmarkTree == null) return;

                var button = sender as System.Windows.Controls.Button;
                bool shouldExpand = button.Content.ToString().StartsWith("▼");

                // 调用优化后的展开/收起方法
                ExpandOrCollapseAllItems(bookmarkTree, shouldExpand);

                button.Content = shouldExpand ? "▲ 全部收起" : "▼ 全部展开";
            };

            rootGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Grid.SetRow(expandCollapseButton, rootGrid.RowDefinitions.Count - 1);
            rootGrid.Children.Add(expandCollapseButton);
        }

        /// <summary>
        /// 递归展开或收起TreeView的所有项目 (优化版)
        /// </summary>
        private void ExpandOrCollapseAllItems(System.Windows.Controls.ItemsControl itemsControl, bool expand)
        {
            // 优化点 3：在获取容器前，强制更新布局
            itemsControl.UpdateLayout();

            foreach (var item in itemsControl.Items)
            {
                if (itemsControl.ItemContainerGenerator.ContainerFromItem(item) is System.Windows.Controls.TreeViewItem treeViewItem)
                {
                    treeViewItem.IsExpanded = expand;
                    if (treeViewItem.Items.Count > 0)
                    {
                        ExpandOrCollapseAllItems(treeViewItem, expand);
                    }
                }
            }
        }

        #endregion
    }
}
