using SwissAcademic.Citavi.Controls.Wpf;
using SwissAcademic.Citavi.Shell;
using System;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Linq;
using System.Windows;

namespace PDFBookmarkExpandCollapse
{
    public class Addon : CitaviAddOn<MainForm>
    {
        #region Methods

        public override void OnHostingFormLoaded(MainForm mainForm)
        {
            // 插件加载时，开始观察PDF预览控件的事件
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
                    // 只订阅文档相关的事件，不再需要选中文本事件
                    viewer.DocumentChanged += Viewer_DocumentChanged;
                    viewer.DocumentClosing += Viewer_DocumentClosing;
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
                }
            }
        }

        #endregion

        #region EventHandlers

        // 当PDF文档加载或切换时触发
        private void Viewer_DocumentChanged(object sender, EventArgs e)
        {
            if (sender is PdfViewControl pdfViewer)
            {
                // 每次换文档时，都尝试添加一下书签按钮
                AddExpandCollapseButtonToBookmarksTab(pdfViewer);
            }
        }

        // 当PDF文档关闭时触发
        private void Viewer_DocumentClosing(object sender, DocumentClosingArgs args)
        {
            // 如果需要，可以在这里添加文档关闭时的清理逻辑
            // 目前对于书签按钮来说，不需要特殊处理
        }

        // 当Citavi主窗口关闭时触发
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sender is MainForm mainForm)
            {
                // 取消所有事件订阅，防止内存泄漏
                Observe(mainForm, false);
            }
        }

        #endregion

        #region Bookmarks Button Logic

        /// <summary>
        /// 在PDF预览的书签页面添加一个全部展开/收起的按钮
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

            // 检查按钮是否已经存在，避免重复添加
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

                ExpandOrCollapseAllItems(bookmarkTree, shouldExpand);

                button.Content = shouldExpand ? "▲ 全部收起" : "▼ 全部展开";
            };

            // --- 尝试在顶部添加 ---
            bool added = false;

            // 方法2：如果方法1失败，尝试通过已知按钮找父容器
            if (!added)
            {
                var knownButton = WPFHelper.FindChild<System.Windows.Controls.Button>(bookmarkSidebar);
                if (knownButton?.Parent is System.Windows.Controls.Panel parentPanel)
                {
                    parentPanel.Children.Insert(0, expandCollapseButton);
                    added = true;
                }
            }

            // 方法3：如果都失败，回退到底部
            //if (!added)
            //{
            //    rootGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            //    Grid.SetRow(expandCollapseButton, rootGrid.RowDefinitions.Count - 1);
            //    rootGrid.Children.Add(expandCollapseButton);
            //}
        }

        /// <summary>
        /// 递归展开或收起TreeView的所有项目
        /// </summary>
        private void ExpandOrCollapseAllItems(System.Windows.Controls.ItemsControl itemsControl, bool expand)
        {
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
