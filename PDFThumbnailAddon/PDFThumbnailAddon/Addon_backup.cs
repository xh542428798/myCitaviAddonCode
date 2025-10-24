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
            // 调用新方法来添加按钮
            AddExpandCollapseButtonToBookmarksTab(mainForm);
            Observe(mainForm, true);

            base.OnHostingFormLoaded(mainForm);
        }

        void Observe(MainForm mainForm, bool observe)
        {
            if (observe)
            {
                mainForm.FormClosed += MainForm_FormClosed;
                
                var viewer = mainForm.PreviewControl.GetPdfViewControl();
                viewer.DocumentChanged += Viewer_DocumentChanged;
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
                    Tag="Addon",
                    Header = new Image { Height = 16, Source = bitmapImage },
                    Content = thumbnailControl
                };

                tabControl.Items.Add(tabPage);

                thumbnailControl.Refresh();
            }
        }

        #endregion

        #region EventHandlers

        private void Viewer_DocumentChanged(object sender, EventArgs e)
        {
            if (sender is PdfViewControl viewer && viewer.GetSideBar() is System.Windows.Controls.TabControl tabControl)
            {
                var tabitem = tabControl.Items.Cast<TabItem>().FirstOrDefault(item => item.Tag != null && item.Tag.ToString().Equals("Addon", StringComparison.OrdinalIgnoreCase));
                if (tabitem != null && tabitem.Content is ThumbnailsControl thumbnailsControl)
                    thumbnailsControl.Refresh();
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
        /// <summary>
        /// 在PDF预览的目录页面添加一个全部展开/收起的按钮
        /// </summary>
        /// <param name="mainForm">Citavi主窗体实例</param>
        public void AddExpandCollapseButtonToBookmarksTab(MainForm mainForm)
        {
            // 1. 找到侧边栏的 TabControl
            var sideBarTabControl = mainForm.PreviewControl.GetPdfViewControl()?.GetSideBar();
            if (sideBarTabControl == null) return;

            // 2. 查找内容为 BookmarkSidebar 的 TabItem (即目录页面)
            var bookmarksTabItem = sideBarTabControl.Items.Cast<TabItem>()
                .FirstOrDefault(item => item.Content is BookmarkSidebar);

            if (bookmarksTabItem == null) return;

            var bookmarkSidebar = bookmarksTabItem.Content as BookmarkSidebar;

            // 3. 找到 BookmarkSidebar 内部的根 Grid (名为 "Root")
            var rootGrid = WPFHelper.FindChild<Grid>(bookmarkSidebar);
            if (rootGrid == null) return;

            // 检查是否已经添加过按钮，避免重复添加
            // 推荐的修改
            if (rootGrid.Children.OfType<System.Windows.Controls.Button>().Any(b => (string)b.Tag == "ExpandCollapseButton"))
            {
                return; // 按钮已存在
            }

            // 4. 创建我们的按钮 (使用完全限定名 System.Windows.Controls.Button)
            var expandCollapseButton = new System.Windows.Controls.Button
            {
                Content = "▼ 全部展开", // 初始状态
                Tag = "ExpandCollapseButton", // 现在编译器知道 Tag 是来自 FrameworkElement
                Margin = new Thickness(5),
                // 使用完全限定名 System.Windows.HorizontalAlignment
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
            };

            // 5. 为按钮添加点击事件
            expandCollapseButton.Click += (sender, e) =>
            {
                // 找到 BookmarkSidebar 内部的 TreeView (名为 "BookmarkTree")
                var bookmarkTree = WPFHelper.FindChild<System.Windows.Controls.TreeView>(bookmarkSidebar);
                if (bookmarkTree == null) return;

                var button = sender as System.Windows.Controls.Button;
                bool shouldExpand = button.Content.ToString().StartsWith("▼");

                // 递归遍历 TreeView 的所有项目
                ExpandOrCollapseAllItems(bookmarkTree, shouldExpand);

                // 更新按钮文本
                button.Content = shouldExpand ? "▲ 全部收起" : "▼ 全部展开";
            };

            // 6. 将按钮添加到 Root Grid 中
            rootGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Grid.SetRow(expandCollapseButton, rootGrid.RowDefinitions.Count - 1);
            rootGrid.Children.Add(expandCollapseButton);
        }

        /// <summary>
        /// 递归展开或收起TreeView的所有项目
        /// </summary>
        private void ExpandOrCollapseAllItems(System.Windows.Controls.ItemsControl itemsControl, bool expand)
        {
            foreach (var item in itemsControl.Items)
            {
                if (itemsControl.ItemContainerGenerator.ContainerFromItem(item) is System.Windows.Controls.TreeViewItem treeViewItem)
                {
                    treeViewItem.IsExpanded = expand;
                    // 如果该项还有子项，递归调用
                    if (treeViewItem.Items.Count > 0)
                    {
                        ExpandOrCollapseAllItems(treeViewItem, expand);
                    }
                }
            }
        }
    }
}