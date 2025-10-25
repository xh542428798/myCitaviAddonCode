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
                if (viewer != null)
                {
                    viewer.DocumentChanged += Viewer_DocumentChanged;
                    viewer.DocumentClosing += Viewer_DocumentClosing;
                    // --- 新增：订阅选中文本变化事件 ---
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
                    // --- 新增：取消订阅 ---
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
                bitmapImage.UriSource = new Uri("/PDFThumbnailAddon;component/Resources/thumbs.png", UriKind.RelativeOrAbsolute);
                bitmapImage.EndInit();

                var translationControl = new TranslationControl(mainForm);


                var tabPage = new TabItem
                {
                    Tag = "TranslationAddon", // 改个Tag以区分
                    Header = new Image { Height = 16, Source = bitmapImage },
                    Content = translationControl
                };

                // --- 新增：设置TabItem的默认宽度 ---
                // 你可以根据需要调整这个值，比如350或400
                //tabPage.Width = 330;
                tabControl.Items.Add(tabPage);

                //translationControl.Refresh();
            }
        }

        #endregion

        #region EventHandlers
        // 在 Addon.cs 的 EventHandlers 区域

        // 修改后的方法，注意参数从 SelectionChangedEventArgs e 变成了 EventArgs e
        private void Viewer_SelectionChanged(object sender, EventArgs e)
        {
            // 这个事件会在PDF中的选择发生变化时触发
            // 我们在这里找到翻译控件并更新它
            var pdfViewer = sender as PdfViewControl;
            if (pdfViewer?.GetSideBar() is System.Windows.Controls.TabControl tabControl)
            {
                var translationTabItem = tabControl.Items.Cast<TabItem>()
                    .FirstOrDefault(item => item.Tag != null && item.Tag.ToString().Equals("TranslationAddon", StringComparison.OrdinalIgnoreCase));

                if (translationTabItem?.Content is TranslationControl translationControl)
                {
                    // 只有选择真正变化时，才调用更新
                    translationControl.OnSelectionChanged();
                }
            }
        }

        // 优化点 1 & 2：在这里添加按钮，确保每个新文档都会尝试添加
        private void Viewer_DocumentChanged(object sender, EventArgs e)
        {
            // --- 书签按钮逻辑 (保持不变) ---
            if (sender is PdfViewControl pdfViewer)
            {
                AddExpandCollapseButtonToBookmarksTab(pdfViewer);
            }

            // --- 翻译插件逻辑 ---
            if (sender is PdfViewControl viewer && viewer.GetSideBar() is System.Windows.Controls.TabControl tabControl)
            {
                var translationTabItem = tabControl.Items.Cast<TabItem>()
                    .FirstOrDefault(item => item.Tag != null && item.Tag.ToString().Equals("TranslationAddon", StringComparison.OrdinalIgnoreCase));

                if (translationTabItem != null && translationTabItem.Content is TranslationControl translationControl)
                {
                    // 文档切换时，清空内容并尝试更新
                    translationControl.ClearContent();
                    translationControl.OnSelectionChanged(); // <--- 添加这一行
                }
            }
        }


        private void Viewer_DocumentClosing(object sender, DocumentClosingArgs args)
        {
            // --- 翻译插件的核心逻辑 ---
            // 当文档关闭时，清空翻译界面的内容
            if (sender is PdfViewControl viewer && viewer.GetSideBar() is System.Windows.Controls.TabControl tabControl)
            {
                // 寻找我们自己的翻译插件Tab
                var translationTabItem = tabControl.Items.Cast<TabItem>()
                    .FirstOrDefault(item => item.Tag != null && item.Tag.ToString().Equals("TranslationAddon", StringComparison.OrdinalIgnoreCase));

                // 如果找到了翻译插件，并且它的内容是我们的TranslationControl
                if (translationTabItem != null && translationTabItem.Content is TranslationControl translationControl)
                {
                    // 调用ClearContent方法清空内容
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
