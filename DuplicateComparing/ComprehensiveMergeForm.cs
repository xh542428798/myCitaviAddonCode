using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SwissAcademic.Citavi;
using SwissAcademic.Controls;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

// 添加这行来解决命名冲突
using WinFormsLabel = System.Windows.Forms.Label;

namespace DuplicateComparing
{
    public partial class ComprehensiveMergeForm : FormBase
    {
        private Dictionary<string, MergeControl> mergeControls = new Dictionary<string, MergeControl>();
        private Reference leftRef;
        private Reference rightRef;

        // 添加这两行
        private CancellationTokenSource _cancellationTokenSource;
        private static int _instanceCount = 0;
        private readonly int _instanceId;

        // 添加字段数据缓存
        private List<(string name, string left, string right, bool isSpecial, bool isCategory)> allFields = new List<(string, string, string, bool, bool)>();

        public ComprehensiveMergeForm(Form owner, Reference leftRef, Reference rightRef) : base(owner)
        {    // 添加实例ID跟踪
            _instanceId = Interlocked.Increment(ref _instanceCount);
            System.Diagnostics.Debug.WriteLine($"Creating ComprehensiveMergeForm instance {_instanceId}");

            this.leftRef = leftRef;
            this.rightRef = rightRef;

            // 创建取消令牌
            _cancellationTokenSource = new CancellationTokenSource();

            // 显示加载状态
            this.Cursor = Cursors.WaitCursor;

            InitializeComponent();

            // 恢复光标
            this.Cursor = Cursors.Default;

            // 窗体加载完成后才开始差异计算
            this.Shown += ComprehensiveMergeForm_Shown;

            // 添加窗体关闭事件
            this.FormClosing += ComprehensiveMergeForm_FormClosing;
        }

        private void ComprehensiveMergeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Closing ComprehensiveMergeForm instance {_instanceId}");

            // 取消所有异步操作
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        private void ComprehensiveMergeForm_Shown(object sender, EventArgs e)
        {
            // 确保窗体完全显示后再开始异步加载
            var contentPanel = this.Controls.Find("contentPanel", true).FirstOrDefault() as TableLayoutPanel;
            if (contentPanel != null)
            {

                LoadDifferentFieldsAsync(contentPanel);
            }
        }

        // 添加安全的UI更新方法
        private void SafeInvoke(Action action)
        {
            if (this.IsHandleCreated && _cancellationTokenSource?.Token.IsCancellationRequested != true)
            {
                if (this.InvokeRequired)
                {
                    try
                    {
                        this.Invoke(action);
                    }
                    catch (ObjectDisposedException)
                    {
                        // 窗体已被释放，忽略
                    }
                    catch (InvalidOperationException)
                    {
                        // 窗体句柄未创建或已销毁，忽略
                    }
                }
                else
                {
                    try
                    {
                        action();
                    }
                    catch (ObjectDisposedException)
                    {
                        // 窗体已被释放，忽略
                    }
                }
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // 设置窗体基本属性 - 完全可调整
            this.Text = "Merge References - Comprehensive Comparison";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimumSize = new Size(800, 600);

            // 设置窗体默认字体
            this.Font = new Font("Microsoft YaHei UI", 11, FontStyle.Regular);

            // 创建主容器 - 使用TableLayoutPanel实现更好的布局控制
            var mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1,
                Padding = new Padding(5)
            };

            // 设置行样式：上部可伸缩，底部固定
            mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100));
            mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70));
            this.Controls.Add(mainTableLayoutPanel);

            // 创建滚动容器
            var scrollPanel = new System.Windows.Forms.Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Font = new Font("Microsoft YaHei UI", 11, FontStyle.Regular)
            };
            mainTableLayoutPanel.Controls.Add(scrollPanel, 0, 0);

            // 创建内容面板 - 自适应
            var contentPanel = new System.Windows.Forms.TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                Font = new Font("Microsoft YaHei UI", 11, FontStyle.Regular),
                Name = "contentPanel"  // 添加Name以便后续查找
            };
            scrollPanel.Controls.Add(contentPanel);

            // 先收集所有字段数据，不立即创建UI
            CollectAllFields();

            // 异步加载有差异的字段
            LoadDifferentFieldsAsync(contentPanel);

            // 添加底部按钮面板 - 关键修复：添加到正确的容器
            var buttonPanel = new System.Windows.Forms.FlowLayoutPanel
            {
                Dock = DockStyle.Fill,  // 改为Fill，填充整个底部区域
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(10),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            var cancelButton = new System.Windows.Forms.Button
            {
                Text = "Cancel",
                Width = 100,
                Height = 40,
                AutoSize = false
            };
            cancelButton.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            var okButton = new System.Windows.Forms.Button
            {
                Text = "Apply All Changes",
                Width = 150,
                Height = 40,
                BackColor = Color.LightGreen,
                AutoSize = false
            };
            okButton.Click += (s, e) => ApplyAllChanges();

            buttonPanel.Controls.AddRange(new Control[] { cancelButton, okButton });

            // 关键修复：添加到mainTableLayoutPanel的底部行
            mainTableLayoutPanel.Controls.Add(buttonPanel, 0, 1);

            this.ResumeLayout(false);
        }

        // 收集所有字段数据
        private void CollectAllFields()
        {
            // === 文本类字段 ===
            allFields.Add(("Title", leftRef.Title, rightRef.Title, false, false));
            allFields.Add(("Abstract", leftRef.Abstract.Text, rightRef.Abstract.Text, false, false));
            allFields.Add(("Subtitle", leftRef.Subtitle, rightRef.Subtitle, false, false));
            allFields.Add(("ShortTitle", leftRef.ShortTitle, rightRef.ShortTitle, false, false));
            allFields.Add(("TranslatedTitle", leftRef.TranslatedTitle, rightRef.TranslatedTitle, false, false));

            // === 特殊字段 ===
            allFields.Add(("TableOfContents", leftRef.TableOfContents.Text, rightRef.TableOfContents.Text, false, false));
            allFields.Add(("Evaluation", leftRef.Evaluation.Text, rightRef.Evaluation.Text, false, false));
            allFields.Add(("Periodical", leftRef.Periodical.ToString(), rightRef.Periodical.ToString(), true, false));

            // === 分类字段（人员） ===
            allFields.Add(("Authors", GetCategoryStr(leftRef, "Author"), GetCategoryStr(rightRef, "Author"), false, true));
            allFields.Add(("Editors", GetCategoryStr(leftRef, "Editor"), GetCategoryStr(rightRef, "Editor"), false, true));
            allFields.Add(("Collaborators", GetCategoryStr(leftRef, "Collaborator"), GetCategoryStr(rightRef, "Collaborator"), false, true));
            allFields.Add(("Organizations", GetCategoryStr(leftRef, "Organization"), GetCategoryStr(rightRef, "Organization"), false, true));
            allFields.Add(("OthersInvolved", GetCategoryStr(leftRef, "OthersInvolved"), GetCategoryStr(rightRef, "OthersInvolved"), false, true));
            allFields.Add(("Publishers", GetCategoryStr(leftRef, "Publishers"), GetCategoryStr(rightRef, "Publishers"), false, true));

            // === 分类字段（其他） ===
            allFields.Add(("Keywords", GetCategoryStr(leftRef, "Keyword"), GetCategoryStr(rightRef, "Keyword"), false, true));
            allFields.Add(("Groups", GetCategoryStr(leftRef, "Group"), GetCategoryStr(rightRef, "Group"), false, true));
            allFields.Add(("Categories", GetCategoryStr(leftRef, "Category"), GetCategoryStr(rightRef, "Category"), false, true));
            allFields.Add(("Locations", GetCategoryStr(leftRef, "Location"), GetCategoryStr(rightRef, "Location"), false, true));

            // === 自定义字段 ===
            for (int i = 1; i <= 9; i++)
            {
                string fieldName = "CustomField" + i;
                string leftValue = GetFieldValue(leftRef, fieldName);
                string rightValue = GetFieldValue(rightRef, fieldName);
                allFields.Add((fieldName, leftValue, rightValue, false, false));
            }

            allFields.Add(("Year", leftRef.Year, rightRef.Year, false, false));

            // === 标识符字段 ===
            allFields.Add(("DOI", leftRef.Doi, rightRef.Doi, false, false));
            allFields.Add(("ISBN", leftRef.Isbn.ToString(), rightRef.Isbn.ToString(), false, false));
            allFields.Add(("PubMedId", leftRef.PubMedId, rightRef.PubMedId, false, false));

            // === 标题相关字段 ===
            allFields.Add(("Additions", leftRef.Additions, rightRef.Additions, false, false));
            allFields.Add(("ParallelTitle", leftRef.ParallelTitle, rightRef.ParallelTitle, false, false));
            allFields.Add(("UniformTitle", leftRef.UniformTitle, rightRef.UniformTitle, false, false));
            allFields.Add(("TitleSupplement", leftRef.TitleSupplement, rightRef.TitleSupplement, false, false));
            allFields.Add(("TitleInOtherLanguages", leftRef.TitleInOtherLanguages, rightRef.TitleInOtherLanguages, false, false));

            // === 出版信息字段 ===
            allFields.Add(("PlaceOfPublication", leftRef.PlaceOfPublication, rightRef.PlaceOfPublication, false, false));
            allFields.Add(("Date", leftRef.Date, rightRef.Date, false, false));
            allFields.Add(("Date2", leftRef.Date2, rightRef.Date2, false, false));
            allFields.Add(("Edition", leftRef.Edition, rightRef.Edition, false, false));
            allFields.Add(("Volume", leftRef.Volume, rightRef.Volume, false, false));
            allFields.Add(("Number", leftRef.Number, rightRef.Number, false, false));
            allFields.Add(("NumberOfVolumes", leftRef.NumberOfVolumes, rightRef.NumberOfVolumes, false, false));

            // === 其他信息字段 ===
            allFields.Add(("Language", leftRef.Language, rightRef.Language, false, false));
            allFields.Add(("Notes", leftRef.Notes, rightRef.Notes, false, false));
            allFields.Add(("OnlineAddress", leftRef.OnlineAddress, rightRef.OnlineAddress, false, false));
            allFields.Add(("OriginalCheckedBy", leftRef.OriginalCheckedBy, rightRef.OriginalCheckedBy, false, false));
            allFields.Add(("OriginalPublication", leftRef.OriginalPublication, rightRef.OriginalPublication, false, false));
            allFields.Add(("Price", leftRef.Price, rightRef.Price, false, false));
            allFields.Add(("SourceOfBibliographicInformation", leftRef.SourceOfBibliographicInformation, rightRef.SourceOfBibliographicInformation, false, false));
            allFields.Add(("StorageMedium", leftRef.StorageMedium, rightRef.StorageMedium, false, false));
            allFields.Add(("TextLinks", leftRef.TextLinks, rightRef.TextLinks, false, false));

            // === 特定字段 ===
            for (int i = 1; i <= 7; i++)
            {
                string fieldName = "SpecificField" + i;
                string leftValue = GetFieldValue(leftRef, fieldName);
                string rightValue = GetFieldValue(rightRef, fieldName);
                allFields.Add((fieldName, leftValue, rightValue, false, false));
            }

            // === 特殊处理字段 ===
            allFields.Add(("SeriesTitle", leftRef.SeriesTitle?.ToString() ?? "", rightRef.SeriesTitle?.ToString() ?? "", true, false));
        }

        // 异步加载有差异的字段，修改 LoadDifferentFieldsAsync 方法
        private async void LoadDifferentFieldsAsync(TableLayoutPanel contentPanel)
        {
            try
            {
                // 在后台线程中筛选有差异的字段
                var differentFields = await Task.Run(() =>
                {
                    var result = new List<(string name, string left, string right, bool isSpecial, bool isCategory)>();

                    foreach (var field in allFields)
                    {
                        // 检查是否已取消
                        if (_cancellationTokenSource.Token.IsCancellationRequested)
                            return null;

                        // 改进的差异比较逻辑
                        bool leftEmpty = string.IsNullOrEmpty(field.left);
                        bool rightEmpty = string.IsNullOrEmpty(field.right);

                        // 如果两边都为空，跳过
                        if (leftEmpty && rightEmpty)
                            continue;

                        // 如果两边内容不同（包括一边为空的情况）
                        if (!string.Equals(field.left ?? "", field.right ?? "", StringComparison.OrdinalIgnoreCase))
                        {
                            result.Add(field);
                        }
                    }

                    return result;
                }, _cancellationTokenSource.Token);

                // 检查是否已取消
                if (_cancellationTokenSource.Token.IsCancellationRequested || differentFields == null)
                    return;

                // 如果没有差异，显示提示
                if (differentFields.Count == 0)
                {
                    var noDiffLabel = new WinFormsLabel
                    {
                        Text = "No differences found between the selected references.",
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Microsoft YaHei UI", 14, FontStyle.Bold),
                        ForeColor = Color.Green
                    };
                    contentPanel.Controls.Add(noDiffLabel);
                    return;
                }

                // 先添加差异统计标签
                var summaryLabel = new WinFormsLabel
                {
                    Text = $"Found {differentFields.Count} fields with differences",
                    Dock = DockStyle.Top,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Microsoft YaHei UI", 10, FontStyle.Bold),
                    ForeColor = Color.DarkBlue,
                    Padding = new Padding(0, 8, 0, 8),
                    AutoSize = true,
                    Margin = new Padding(10, 5, 10, 5)
                };
                contentPanel.Controls.Add(summaryLabel);

                // 一次性添加所有字段
                foreach (var field in differentFields)
                {
                    // 检查是否已取消
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                        return;

                    if (field.isSpecial)
                    {
                        AddSpecialFieldComparison(contentPanel, field.name, field.left, field.right);
                    }
                    else if (field.isCategory)
                    {
                        AddCategoryFieldComparison(contentPanel, field.name, field.left, field.right);
                    }
                    else
                    {
                        AddFieldComparison(contentPanel, field.name, field.left, field.right);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // 操作被取消，正常返回
                return;
            }
            catch (Exception ex)
            {
                // 记录错误但不显示给用户（因为窗体可能已关闭）
                System.Diagnostics.Debug.WriteLine($"Error in LoadDifferentFieldsAsync: {ex.Message}");
            }
        }


        private void AddFieldComparison(System.Windows.Forms.TableLayoutPanel parent, string fieldName, string leftValue, string rightValue)
        {
            // 检查是否已经添加过这个字段
            if (mergeControls.ContainsKey(fieldName))
            {
                return; // 已存在，跳过
            }

            // 根据字段名设置初始高度
            int initialHeight = GetInitialFieldHeight(fieldName);

            var fieldPanel = new System.Windows.Forms.TableLayoutPanel
            {
                RowCount = 2,
                ColumnCount = 3,
                Dock = DockStyle.Top,
                Height = initialHeight,
                Padding = new Padding(2),
                AutoSize = false,  // 改为false，不允许自动调整大小
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            fieldPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30));
            fieldPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100));

            // 列宽比例：46% + 8% + 46%
            fieldPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 47));
            fieldPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6));
            fieldPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 47));

            // 字段标题 - 字体12
            var titleLabel = new WinFormsLabel
            {
                Text = fieldName,
                Font = new Font("Microsoft YaHei UI", 12, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Red  // 有差异的字段标记为红色
            };
            fieldPanel.Controls.Add(titleLabel, 0, 0);
            fieldPanel.SetColumnSpan(titleLabel, 3);

            // 左侧内容 - 字体12
            var leftTextBox = new System.Windows.Forms.RichTextBox
            {
                Text = leftValue ?? "",
                Dock = DockStyle.Fill,
                Font = new Font("Microsoft YaHei UI", 12, FontStyle.Regular),
                ScrollBars = RichTextBoxScrollBars.Vertical,
                Margin = new Padding(2),
                MinimumSize = new Size(40, 40),
                ReadOnly = true,  // 设为只读
                BackColor = Color.White  // 保持白色背景
            };
            fieldPanel.Controls.Add(leftTextBox, 0, 1);

            // 右侧内容 - 字体12
            var rightTextBox = new System.Windows.Forms.RichTextBox
            {
                Text = rightValue ?? "",
                Dock = DockStyle.Fill,
                Font = new Font("Microsoft YaHei UI", 12, FontStyle.Regular),
                ScrollBars = RichTextBoxScrollBars.Vertical,
                Margin = new Padding(2),
                MinimumSize = new Size(40, 40),
                ReadOnly = true,  // 设为只读
                BackColor = Color.White  // 保持白色背景
            };
            fieldPanel.Controls.Add(rightTextBox, 2, 1);

            // 中间选择按钮 - 使用自动调整大小
            var buttonPanel = new System.Windows.Forms.FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(2),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            var leftButton = new System.Windows.Forms.RadioButton
            {
                Text = "Left",
                Checked = true,
                AutoSize = true,  // 启用自动调整大小
                Font = new Font("Microsoft YaHei UI", 9, FontStyle.Bold),  // 使用较小字体
                Margin = new Padding(0, 1, 0, 1),
                TextAlign = ContentAlignment.MiddleCenter
            };
            var rightButton = new System.Windows.Forms.RadioButton
            {
                Text = "Right",
                AutoSize = true,  // 启用自动调整大小
                Font = new Font("Microsoft YaHei UI", 9, FontStyle.Bold),  // 使用较小字体
                Margin = new Padding(0, 1, 0, 1),
                TextAlign = ContentAlignment.MiddleCenter
            };
            var combineButton = new System.Windows.Forms.RadioButton
            {
                Text = "Both",
                AutoSize = true,  // 启用自动调整大小
                Font = new Font("Microsoft YaHei UI", 9, FontStyle.Bold),  // 使用较小字体
                Margin = new Padding(0, 1, 0, 1),
                TextAlign = ContentAlignment.MiddleCenter
            };

            buttonPanel.Controls.AddRange(new Control[] { leftButton, rightButton, combineButton });
            fieldPanel.Controls.Add(buttonPanel, 1, 1);

            // 添加调整大小的功能（仅对特定字段）
            if (IsResizableField(fieldName))
            {
                AddResizeCapability(fieldPanel, titleLabel);
            }

            // 延迟差异计算 - 在后台线程中执行
            Task.Run(() =>
            {
                // 检查是否已取消
                if (_cancellationTokenSource?.Token.IsCancellationRequested != true)
                {
                    CompareStrings(leftTextBox, rightTextBox, leftValue, rightValue);
                }
            }, _cancellationTokenSource.Token);

            mergeControls[fieldName] = new MergeControl
            {
                LeftControl = leftTextBox,
                RightControl = rightTextBox,
                LeftButton = leftButton,
                RightButton = rightButton,
                CombineButton = combineButton
            };

            parent.Controls.Add(fieldPanel);
            parent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        }

        // 获取字段的初始高度
        private int GetInitialFieldHeight(string fieldName)
        {
            // 需要拉长的字段列表
            string[] expandableFields = { "Abstract", "TableOfContents", "Evaluation" };

            // 如果是需要拉长的字段，返回较大的初始高度
            if (expandableFields.Contains(fieldName))
            {
                return 200; // 初始高度200像素
            }

            // 其他字段返回默认高度
            return 120;
        }

        // 判断字段是否可调整大小
        private bool IsResizableField(string fieldName)
        {
            string[] resizableFields = { "Abstract", "TableOfContents", "Evaluation" };
            return resizableFields.Contains(fieldName);
        }

        // 添加调整大小的功能
        private void AddResizeCapability(TableLayoutPanel fieldPanel, WinFormsLabel titleLabel)
        {
            // 在字段右下角添加一个小的调整角标
            var resizeCorner = new Panel
            {
                Size = new Size(16, 16),
                BackColor = Color.Transparent,
                Cursor = Cursors.SizeNWSE,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };

            // 绘制调整图标
            resizeCorner.Paint += (sender, e) =>
            {
                // 绘制一个小的三角形调整图标
                using (var brush = new SolidBrush(Color.LightGray))
                {
                    var points = new Point[]
                    {
                new Point(12, 4),
                new Point(12, 12),
                new Point(4, 12)
                    };
                    e.Graphics.FillPolygon(brush, points);
                }

                // 绘制两条线
                using (var pen = new Pen(Color.Gray, 1))
                {
                    e.Graphics.DrawLine(pen, 8, 12, 12, 12);
                    e.Graphics.DrawLine(pen, 12, 8, 12, 12);
                }
            };

            // 添加到字段面板
            fieldPanel.Controls.Add(resizeCorner);

            // 添加调整功能
            bool isResizing = false;
            int startY = 0;
            int startHeight = 0;

            resizeCorner.MouseDown += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    isResizing = true;
                    startY = Control.MousePosition.Y;
                    startHeight = fieldPanel.Height;
                    resizeCorner.Capture = true;
                }
            };

            resizeCorner.MouseMove += (sender, e) =>
            {
                if (isResizing)
                {
                    int newHeight = startHeight + (Control.MousePosition.Y - startY);
                    newHeight = Math.Max(100, Math.Min(500, newHeight));
                    fieldPanel.Height = newHeight;
                }
            };

            resizeCorner.MouseUp += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    isResizing = false;
                    resizeCorner.Capture = false;
                }
            };
        }
        private void AddCategoryFieldComparison(System.Windows.Forms.TableLayoutPanel parent, string fieldName, string leftValue, string rightValue)
        {
            // 对于分类字段，使用相同的方法但标记为特殊类型
            AddFieldComparison(parent, fieldName, leftValue, rightValue);
            if (mergeControls.ContainsKey(fieldName))
            {
                mergeControls[fieldName].IsCategoryField = true;
            }
        }

        private void AddSpecialFieldComparison(System.Windows.Forms.TableLayoutPanel parent, string fieldName, string leftValue, string rightValue)
        {
            // 对于特殊字段，禁用合并选项
            AddFieldComparison(parent, fieldName, leftValue, rightValue);
            if (mergeControls.ContainsKey(fieldName))
            {
                mergeControls[fieldName].CombineButton.Enabled = false;
                mergeControls[fieldName].IsSpecialField = true;
            }
        }

        // 替换 CompareStrings 方法
        private void CompareStrings(System.Windows.Forms.RichTextBox leftBox, System.Windows.Forms.RichTextBox rightBox, string leftText, string rightText)
        {
            // 检查是否已取消
            if (_cancellationTokenSource?.Token.IsCancellationRequested == true)
                return;
            try
            {
                leftText = leftText ?? "";
                rightText = rightText ?? "";

                // 对于超长文本，使用分段处理
                if (leftText.Length > 3000 || rightText.Length > 3000)
                {
                    CompareInChunks(leftBox, rightBox, leftText, rightText);
                    return;
                }

                // 对于中等长度文本，使用优化的字符级比较
                if (leftText.Length > 500 || rightText.Length > 500)
                {
                    PerformOptimizedCharDiff(leftBox, rightBox, leftText, rightText);
                    return;
                }

                // 短文本使用精细比较
                PerformDetailedCharDiff(leftBox, rightBox, leftText, rightText);
            }
            catch (Exception ex)
            {
                SafeInvoke(() =>
                {
                    if (!leftBox.IsDisposed && !rightBox.IsDisposed)
                    {
                        leftBox.Text = leftText ?? "";
                        rightBox.Text = rightText ?? "";
                        leftBox.BackColor = Color.MistyRose;
                        rightBox.BackColor = Color.MistyRose;
                    }
                });
            }
        }

        // 分段处理长文本
        private void CompareInChunks(System.Windows.Forms.RichTextBox leftBox, System.Windows.Forms.RichTextBox rightBox, string leftText, string rightText)
        {
            SafeInvoke(() =>
            {
                if (!leftBox.IsDisposed && !rightBox.IsDisposed)
                {
                    leftBox.Clear();
                    rightBox.Clear();

                    // 按段落分割
                    var leftParagraphs = SplitIntoParagraphs(leftText);
                    var rightParagraphs = rightText.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.None);

                    int maxParagraphs = Math.Max(leftParagraphs.Length, rightParagraphs.Length);

                    for (int i = 0; i < maxParagraphs; i++)
                    {
                        string leftPara = i < leftParagraphs.Length ? leftParagraphs[i] : "";
                        string rightPara = i < rightParagraphs.Length ? rightParagraphs[i] : "";

                        if (string.IsNullOrEmpty(leftPara) && !string.IsNullOrEmpty(rightPara))
                        {
                            // 插入段落
                            rightBox.SelectionColor = Color.Green;
                            rightBox.SelectionBackColor = Color.LightGreen;
                            rightBox.AppendText(rightPara);
                            rightBox.SelectionColor = Color.Black;
                            rightBox.SelectionBackColor = Color.Transparent;
                        }
                        else if (!string.IsNullOrEmpty(leftPara) && string.IsNullOrEmpty(rightPara))
                        {
                            // 删除段落
                            leftBox.SelectionColor = Color.Red;
                            leftBox.SelectionBackColor = Color.LightYellow;
                            leftBox.AppendText(leftPara);
                            leftBox.SelectionColor = Color.Black;
                            leftBox.SelectionBackColor = Color.Transparent;
                        }
                        else if (leftPara == rightPara)
                        {
                            // 相同段落
                            leftBox.AppendText(leftPara);
                            rightBox.AppendText(rightPara);
                        }
                        else
                        {
                            // 不同段落，进行字符级比较
                            PerformOptimizedCharDiffInPlace(leftBox, rightBox, leftPara, rightPara);
                        }

                        if (i < maxParagraphs - 1)
                        {
                            leftBox.AppendText("\r\n\r\n");
                            rightBox.AppendText("\r\n\r\n");
                        }
                    }
                }
            });
        }

        // 智能分割段落（保留句子完整性）
        private string[] SplitIntoParagraphs(string text)
        {
            // 先按双换行分割
            var paragraphs = text.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.None);

            // 如果段落太长，按句子分割
            var result = new List<string>();
            foreach (var para in paragraphs)
            {
                if (para.Length > 1000)
                {
                    // 按句子分割（中英文句号）
                    var sentences = System.Text.RegularExpressions.Regex.Split(para, @"(?<=[.。！？!?])\s+");
                    foreach (var sentence in sentences)
                    {
                        if (!string.IsNullOrWhiteSpace(sentence))
                        {
                            result.Add(sentence.Trim());
                        }
                    }
                }
                else
                {
                    result.Add(para);
                }
            }

            return result.ToArray();
        }

        // 优化的字符级比较
        private void PerformOptimizedCharDiff(System.Windows.Forms.RichTextBox leftBox, System.Windows.Forms.RichTextBox rightBox, string leftText, string rightText)
        {
            SafeInvoke(() =>
            {
                if (!leftBox.IsDisposed && !rightBox.IsDisposed)
                {
                    leftBox.Clear();
                    rightBox.Clear();
                    PerformOptimizedCharDiffInPlace(leftBox, rightBox, leftText, rightText);
                }
            });
        }

        // 在指定文本框中执行优化的字符级比较
        private void PerformOptimizedCharDiffInPlace(System.Windows.Forms.RichTextBox leftBox, System.Windows.Forms.RichTextBox rightBox, string leftText, string rightText)
        {
            // 使用改进的Myers差异算法
            var diffs = ComputeMyersDiff(leftText, rightText);

            foreach (var diff in diffs)
            {
                switch (diff.Type)
                {
                    case DiffType.Equal:
                        leftBox.AppendText(diff.Text);
                        rightBox.AppendText(diff.Text);
                        break;
                    case DiffType.Delete:
                        leftBox.SelectionColor = Color.Red;
                        leftBox.SelectionBackColor = Color.LightYellow;
                        leftBox.AppendText(diff.Text);
                        leftBox.SelectionColor = Color.Black;
                        leftBox.SelectionBackColor = Color.Transparent;
                        break;
                    case DiffType.Insert:
                        rightBox.SelectionColor = Color.Green;
                        rightBox.SelectionBackColor = Color.LightGreen;
                        rightBox.AppendText(diff.Text);
                        rightBox.SelectionColor = Color.Black;
                        rightBox.SelectionBackColor = Color.Transparent;
                        break;
                }
            }
        }

        // 精细的字符级比较（用于短文本）
        private void PerformDetailedCharDiff(System.Windows.Forms.RichTextBox leftBox, System.Windows.Forms.RichTextBox rightBox, string leftText, string rightText)
        {
            SafeInvoke(() =>
            {
                if (!leftBox.IsDisposed && !rightBox.IsDisposed)
                {
                    leftBox.Clear();
                    rightBox.Clear();

                    // 使用更精细的算法
                    var diffs = ComputeDetailedDiff(leftText, rightText);

                    foreach (var diff in diffs)
                    {
                        switch (diff.Type)
                        {
                            case DiffType.Equal:
                                leftBox.AppendText(diff.Text);
                                rightBox.AppendText(diff.Text);
                                break;
                            case DiffType.Delete:
                                leftBox.SelectionColor = Color.Red;
                                leftBox.SelectionBackColor = Color.LightYellow;
                                leftBox.AppendText(diff.Text);
                                leftBox.SelectionColor = Color.Black;
                                leftBox.SelectionBackColor = Color.Transparent;
                                break;
                            case DiffType.Insert:
                                rightBox.SelectionColor = Color.Green;
                                rightBox.SelectionBackColor = Color.LightGreen;
                                rightBox.AppendText(diff.Text);
                                rightBox.SelectionColor = Color.Black;
                                rightBox.SelectionBackColor = Color.Transparent;
                                break;
                            case DiffType.Modify:
                                // 修改：左侧删除，右侧插入
                                leftBox.SelectionColor = Color.DarkOrange;
                                leftBox.SelectionBackColor = Color.LightYellow;
                                leftBox.AppendText(diff.LeftText);
                                leftBox.SelectionColor = Color.Black;
                                leftBox.SelectionBackColor = Color.Transparent;

                                rightBox.SelectionColor = Color.DarkOrange;
                                rightBox.SelectionBackColor = Color.LightGreen;
                                rightBox.AppendText(diff.RightText);
                                rightBox.SelectionColor = Color.Black;
                                rightBox.SelectionBackColor = Color.Transparent;
                                break;
                        }
                    }
                }
            });
        }

        // 简化的Myers差异算法实现
        private List<TextDiff> ComputeMyersDiff(string left, string right)
        {
            var diffs = new List<TextDiff>();
            int leftIndex = 0, rightIndex = 0;

            // 找到公共前缀
            while (leftIndex < left.Length && rightIndex < right.Length && left[leftIndex] == right[rightIndex])
            {
                leftIndex++;
                rightIndex++;
            }

            if (leftIndex > 0)
            {
                diffs.Add(new TextDiff { Type = DiffType.Equal, Text = left.Substring(0, leftIndex) });
            }

            // 找到公共后缀
            int leftEnd = left.Length - 1;
            int rightEnd = right.Length - 1;
            while (leftEnd >= leftIndex && rightEnd >= rightIndex && left[leftEnd] == right[rightEnd])
            {
                leftEnd--;
                rightEnd--;
            }

            // 处理中间的差异部分
            if (leftIndex <= leftEnd || rightIndex <= rightEnd)
            {
                string leftMiddle = left.Substring(leftIndex, leftEnd - leftIndex + 1);
                string rightMiddle = right.Substring(rightIndex, rightEnd - rightIndex + 1);

                if (string.IsNullOrEmpty(leftMiddle))
                {
                    diffs.Add(new TextDiff { Type = DiffType.Insert, Text = rightMiddle });
                }
                else if (string.IsNullOrEmpty(rightMiddle))
                {
                    diffs.Add(new TextDiff { Type = DiffType.Delete, Text = leftMiddle });
                }
                else
                {
                    // 简单处理：将整个中间部分标记为修改
                    diffs.Add(new TextDiff { Type = DiffType.Delete, Text = leftMiddle });
                    diffs.Add(new TextDiff { Type = DiffType.Insert, Text = rightMiddle });
                }
            }

            // 添加公共后缀
            if (leftEnd < left.Length - 1)
            {
                diffs.Add(new TextDiff { Type = DiffType.Equal, Text = left.Substring(leftEnd + 1) });
            }

            return diffs;
        }

        // 精细差异计算（用于短文本）
        private List<DetailedDiff> ComputeDetailedDiff(string left, string right)
        {
            var diffs = new List<DetailedDiff>();
            int i = 0, j = 0;

            while (i < left.Length || j < right.Length)
            {
                if (i < left.Length && j < right.Length && left[i] == right[j])
                {
                    // 相同字符
                    int start = i;
                    while (i < left.Length && j < right.Length && left[i] == right[j])
                    {
                        i++;
                        j++;
                    }
                    diffs.Add(new DetailedDiff
                    {
                        Type = DiffType.Equal,
                        Text = left.Substring(start, i - start)
                    });
                }
                else
                {
                    // 找到下一个相同的位置
                    int nextMatch = FindNextMatch(left, right, i, j);

                    if (nextMatch != -1)
                    {
                        // 处理删除和插入
                        if (i < nextMatch)
                        {
                            diffs.Add(new DetailedDiff
                            {
                                Type = DiffType.Delete,
                                Text = left.Substring(i, nextMatch - i)
                            });
                        }
                        if (j < nextMatch)
                        {
                            diffs.Add(new DetailedDiff
                            {
                                Type = DiffType.Insert,
                                Text = right.Substring(j, nextMatch - j)
                            });
                        }
                        i = nextMatch;
                        j = nextMatch;
                    }
                    else
                    {
                        // 剩余部分
                        if (i < left.Length)
                        {
                            diffs.Add(new DetailedDiff
                            {
                                Type = DiffType.Delete,
                                Text = left.Substring(i)
                            });
                        }
                        if (j < right.Length)
                        {
                            diffs.Add(new DetailedDiff
                            {
                                Type = DiffType.Insert,
                                Text = right.Substring(j)
                            });
                        }
                        break;
                    }
                }
            }

            return diffs;
        }

        // 查找下一个匹配的位置
        private int FindNextMatch(string left, string right, int leftStart, int rightStart)
        {
            int maxSearch = Math.Min(50, Math.Min(left.Length - leftStart, right.Length - rightStart));

            for (int k = 1; k <= maxSearch; k++)
            {
                // 在左侧搜索
                for (int i = leftStart; i < Math.Min(leftStart + k, left.Length); i++)
                {
                    for (int j = rightStart; j < Math.Min(rightStart + k, right.Length); j++)
                    {
                        if (left[i] == right[j])
                        {
                            return Math.Max(i, j);
                        }
                    }
                }
            }

            return -1;
        }

        // 差异类型
        public enum DiffType
        {
            Equal,
            Delete,
            Insert,
            Modify
        }

        // 简单差异结构
        public class TextDiff
        {
            public DiffType Type { get; set; }
            public string Text { get; set; }
        }

        // 详细差异结构
        public class DetailedDiff
        {
            public DiffType Type { get; set; }
            public string Text { get; set; }
            public string LeftText { get; set; }
            public string RightText { get; set; }
        }

        private void ApplyAllChanges()
        {
            try
            {
                // 应用文本字段
                ApplyTextField("Title", (value) => leftRef.Title = value);
                ApplyTextField("Abstract", (value) => leftRef.Abstract.Text = value);
                ApplyTextField("Additions", (value) => leftRef.Additions = value);
                // 应用特殊字段
                ApplyTextField("TableOfContents", (value) => leftRef.TableOfContents.Text = value);
                ApplyTextField("Evaluation", (value) => leftRef.Evaluation.Text = value);
                // 应用标题相关字段
                ApplyTextField("Subtitle", (value) => leftRef.Subtitle = value);
                ApplyTextField("ShortTitle", (value) => leftRef.ShortTitle = value);
                ApplyTextField("TranslatedTitle", (value) => leftRef.TranslatedTitle = value);
                // 应用出版信息字段
                ApplyTextField("Date", (value) => leftRef.Date = value);
                ApplyTextField("Year", (value) => leftRef.Year = value);
                // 应用自定义字段
                for (int i = 1; i <= 9; i++)
                {
                    string fieldName = "CustomField" + i;
                    ApplyCustomField(fieldName, i);
                }
                // 应用标识符字段
                ApplyTextField("DOI", (value) => leftRef.Doi = value);
                ApplyTextField("ISBN", (value) => leftRef.Isbn = value);
                ApplyTextField("PubMedId", (value) => leftRef.PubMedId = value);

                //不重要的
                // 应用标题相关字段
                ApplyTextField("ParallelTitle", (value) => leftRef.ParallelTitle = value);
                ApplyTextField("UniformTitle", (value) => leftRef.UniformTitle = value);
                ApplyTextField("TitleSupplement", (value) => leftRef.TitleSupplement = value);
                ApplyTextField("TitleInOtherLanguages", (value) => leftRef.TitleInOtherLanguages = value);

                // 应用出版信息字段
                ApplyTextField("PlaceOfPublication", (value) => leftRef.PlaceOfPublication = value);
                ApplyTextField("Date2", (value) => leftRef.Date2 = value);
                ApplyTextField("Edition", (value) => leftRef.Edition = value);
                ApplyTextField("Volume", (value) => leftRef.Volume = value);
                ApplyTextField("Number", (value) => leftRef.Number = value);
                ApplyTextField("NumberOfVolumes", (value) => leftRef.NumberOfVolumes = value);

                // 应用分类字段
                ApplyCategoryField("Authors", () => leftRef.Authors, () => rightRef.Authors);
                ApplyCategoryField("Editors", () => leftRef.Editors, () => rightRef.Editors);
                ApplyCategoryField("Collaborators", () => leftRef.Collaborators, () => rightRef.Collaborators);
                ApplyCategoryField("Organizations", () => leftRef.Organizations, () => rightRef.Organizations);
                ApplyCategoryField("OthersInvolved", () => leftRef.OthersInvolved, () => rightRef.OthersInvolved);
                ApplyCategoryField("Publishers", () => leftRef.Publishers, () => rightRef.Publishers);
                ApplyCategoryField("Keywords", () => leftRef.Keywords, () => rightRef.Keywords);
                ApplyCategoryField("Groups", () => leftRef.Groups, () => rightRef.Groups);
                ApplyCategoryField("Categories", () => leftRef.Categories, () => rightRef.Categories);
                ApplyCategoryField("Locations", () => leftRef.Locations, () => rightRef.Locations);

                // 应用特殊处理字段
                ApplySpecialField("Periodical", (value) => leftRef.Periodical = rightRef.Periodical);
                ApplySpecialField("SeriesTitle", (value) => leftRef.SeriesTitle = rightRef.SeriesTitle);

                // 应用其他信息字段
                ApplyTextField("Language", (value) => leftRef.Language = value);
                ApplyTextField("Notes", (value) => leftRef.Notes = value);
                ApplyTextField("OnlineAddress", (value) => leftRef.OnlineAddress = value);
                ApplyTextField("OriginalCheckedBy", (value) => leftRef.OriginalCheckedBy = value);
                ApplyTextField("OriginalPublication", (value) => leftRef.OriginalPublication = value);
                ApplyTextField("Price", (value) => leftRef.Price = value);
                ApplyTextField("SourceOfBibliographicInformation", (value) => leftRef.SourceOfBibliographicInformation = value);
                ApplyTextField("StorageMedium", (value) => leftRef.StorageMedium = value);
                ApplyTextField("TextLinks", (value) => leftRef.TextLinks = value);

                // 应用特定字段
                for (int i = 1; i <= 7; i++)
                {
                    string fieldName = "SpecificField" + i;
                    ApplySpecificField(fieldName, i);
                }

                // 应用其他原始逻辑
                ApplyOriginalLogic();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying changes: {ex.Message}");
            }
        }

        private void ApplyTextField(string fieldName, Action<string> applyAction)
        {
            if (mergeControls.ContainsKey(fieldName))
            {
                var control = mergeControls[fieldName];
                if (control.LeftButton.Checked)
                    applyAction(control.LeftControl.Text);
                else if (control.RightButton.Checked)
                    applyAction(control.RightControl.Text);
                else if (!string.IsNullOrEmpty(control.LeftControl.Text) && !string.IsNullOrEmpty(control.RightControl.Text))
                    applyAction(control.LeftControl.Text + " // " + control.RightControl.Text);
                else if (!string.IsNullOrEmpty(control.LeftControl.Text))
                    applyAction(control.LeftControl.Text);
                else if (!string.IsNullOrEmpty(control.RightControl.Text))
                    applyAction(control.RightControl.Text);
            }
        }

        private void ApplyCustomField(string fieldName, int fieldIndex)
        {
            if (mergeControls.ContainsKey(fieldName))
            {
                var control = mergeControls[fieldName];
                string value = "";
                if (control.LeftButton.Checked)
                    value = control.LeftControl.Text;
                else if (control.RightButton.Checked)
                    value = control.RightControl.Text;
                else if (!string.IsNullOrEmpty(control.LeftControl.Text) && !string.IsNullOrEmpty(control.RightControl.Text))
                    value = control.LeftControl.Text + " // " + control.RightControl.Text;
                else if (!string.IsNullOrEmpty(control.LeftControl.Text))
                    value = control.LeftControl.Text;
                else if (!string.IsNullOrEmpty(control.RightControl.Text))
                    value = control.RightControl.Text;

                SetFieldValue(leftRef, fieldName, value);
            }
        }

        private void ApplySpecificField(string fieldName, int fieldIndex)
        {
            if (mergeControls.ContainsKey(fieldName))
            {
                var control = mergeControls[fieldName];
                string value = "";
                if (control.LeftButton.Checked)
                    value = control.LeftControl.Text;
                else if (control.RightButton.Checked)
                    value = control.RightControl.Text;
                else if (!string.IsNullOrEmpty(control.LeftControl.Text) && !string.IsNullOrEmpty(control.RightControl.Text))
                    value = control.LeftControl.Text + " // " + control.RightControl.Text;
                else if (!string.IsNullOrEmpty(control.LeftControl.Text))
                    value = control.LeftControl.Text;
                else if (!string.IsNullOrEmpty(control.RightControl.Text))
                    value = control.RightControl.Text;

                SetFieldValue(leftRef, fieldName, value);
            }
        }

        private void ApplyCategoryField<T>(string fieldName, Func<IEnumerable<T>> getLeftCollection, Func<IEnumerable<T>> getRightCollection) where T : class
        {
            if (mergeControls.ContainsKey(fieldName))
            {
                var control = mergeControls[fieldName];
                if (control.RightButton.Checked)
                {
                    var leftCollection = getLeftCollection() as IList<T>;
                    if (leftCollection != null)
                    {
                        leftCollection.Clear();
                        foreach (var item in getRightCollection())
                        {
                            leftCollection.Add(item);
                        }
                    }
                }
                else if (control.CombineButton.Checked)
                {
                    var leftCollection = getLeftCollection() as IList<T>;
                    if (leftCollection != null)
                    {
                        foreach (var item in getRightCollection())
                        {
                            if (!leftCollection.Contains(item))
                            {
                                leftCollection.Add(item);
                            }
                        }
                    }
                }
            }
        }

        // 添加到类的底部，在现有方法之后
        public static string GetCategoryStr(Reference reference, string cateName)
        {
            string result = "";

            try
            {
                switch (cateName)
                {
                    case "Keyword":
                        List<Keyword> refKeywords = reference.Keywords?.ToList() ?? new List<Keyword>();
                        List<string> nameString4 = new List<string>();

                        foreach (Keyword keyword in refKeywords)
                        {
                            if (keyword != null)
                            {
                                // 尝试多个属性
                                string keywordText = "";
                                if (!string.IsNullOrEmpty(keyword.FullName))
                                    keywordText = keyword.FullName;

                                if (!string.IsNullOrEmpty(keywordText))
                                    nameString4.Add(keywordText);
                            }
                        }

                        // 如果没有关键词，返回空字符串而不是 "[No Keywords]"
                        result = GetStringFromArray(nameString4);
                        break;

                    // ... 其他 case 保持不变，但确保空值处理 ...
                    case "Group":
                        List<Group> groupCategories = reference.Groups?.ToList() ?? new List<Group>();
                        List<string> nameString = new List<string>();
                        foreach (Group mygroup in groupCategories)
                        {
                            if (mygroup != null && !string.IsNullOrEmpty(mygroup.FullName))
                                nameString.Add(mygroup.FullName);
                        }
                        result = GetStringFromArray(nameString);
                        break;
                    case "Category":
                        List<Category> categoryRefCategories = reference.Categories?.ToList() ?? new List<Category>();
                        List<string> nameString2 = new List<string>();
                        foreach (Category category in categoryRefCategories)
                        {
                            if (category != null && !string.IsNullOrEmpty(category.FullName))
                                nameString2.Add(category.FullName);
                        }
                        result = GetStringFromArray(nameString2);
                        break;
                    case "Location":
                        List<Location> refLocation = reference.Locations?.ToList() ?? new List<Location>();
                        List<string> nameString3 = new List<string>();
                        foreach (Location location in refLocation)
                        {
                            if (location != null && !string.IsNullOrEmpty(location.FullName))
                                nameString3.Add(location.FullName);
                        }
                        result = GetStringFromArray(nameString3);
                        break;
                    case "Author":
                        List<Person> refPersons = reference.Authors?.ToList() ?? new List<Person>();
                        List<string> nameString5 = new List<string>();
                        foreach (Person person in refPersons)
                        {
                            if (person != null && !string.IsNullOrEmpty(person.FullName))
                                nameString5.Add(person.FullName);
                        }
                        result = GetStringFromArray(nameString5);
                        break;
                    case "Collaborator":
                        refPersons = reference.Collaborators?.ToList() ?? new List<Person>();
                        nameString5 = new List<string>();
                        foreach (Person person in refPersons)
                        {
                            if (person != null && !string.IsNullOrEmpty(person.FullName))
                                nameString5.Add(person.FullName);
                        }
                        result = GetStringFromArray(nameString5);
                        break;
                    case "Editor":
                        refPersons = reference.Editors?.ToList() ?? new List<Person>();
                        nameString5 = new List<string>();
                        foreach (Person person in refPersons)
                        {
                            if (person != null && !string.IsNullOrEmpty(person.FullName))
                                nameString5.Add(person.FullName);
                        }
                        result = GetStringFromArray(nameString5);
                        break;
                    case "Organization":
                        refPersons = reference.Organizations?.ToList() ?? new List<Person>();
                        nameString5 = new List<string>();
                        foreach (Person person in refPersons)
                        {
                            if (person != null && !string.IsNullOrEmpty(person.FullName))
                                nameString5.Add(person.FullName);
                        }
                        result = GetStringFromArray(nameString5);
                        break;
                    case "OthersInvolved":
                        refPersons = reference.OthersInvolved?.ToList() ?? new List<Person>();
                        nameString5 = new List<string>();
                        foreach (Person person in refPersons)
                        {
                            if (person != null && !string.IsNullOrEmpty(person.FullName))
                                nameString5.Add(person.FullName);
                        }
                        result = GetStringFromArray(nameString5);
                        break;
                    case "Publishers":
                        List<Publisher> refPublishers = reference.Publishers?.ToList() ?? new List<Publisher>();
                        nameString5 = new List<string>();
                        foreach (Publisher publisher in refPublishers)
                        {
                            if (publisher != null && !string.IsNullOrEmpty(publisher.FullName))
                                nameString5.Add(publisher.FullName);
                        }
                        result = GetStringFromArray(nameString5);
                        break;
                }
            }
            catch (Exception ex)
            {
                result = "";  // 出错时返回空字符串
            }

            return result;
        }


        public static string GetStringFromArray(List<string> nameString)
        {
            if (nameString == null || nameString.Count == 0)
                return "";  // 返回空字符串而不是 ""

            string[] strings = nameString.ToArray();
            Array.Sort(strings);
            return string.Join("\r\n", strings);
        }

        private void ApplySpecialField(string fieldName, Action<string> applyAction)
        {
            if (mergeControls.ContainsKey(fieldName))
            {
                var control = mergeControls[fieldName];
                if (control.RightButton.Checked)
                {
                    applyAction(control.RightControl.Text);
                }
            }
        }

        private void ApplyOriginalLogic()
        {
            // 应用原始代码中的特殊逻辑
            // AccessDate
            if (leftRef.AccessDate.Length < rightRef.AccessDate.Length)
            {
                leftRef.AccessDate = rightRef.AccessDate;
            }

            // CitationKey
            if ((leftRef.CitationKeyUpdateType == UpdateType.Automatic) && (rightRef.CitationKeyUpdateType == UpdateType.Manual))
            {
                leftRef.CitationKey = rightRef.CitationKey;
                leftRef.CitationKeyUpdateType = rightRef.CitationKeyUpdateType;
            }

            // CoverPath
            if (leftRef.CoverPath.LinkedResourceType == LinkedResourceType.Empty)
            {
                leftRef.CoverPath = rightRef.CoverPath;
            }

            // Rating
            leftRef.Rating = (short)Math.Floor((decimal)((leftRef.Rating + rightRef.Rating) / 2));

            // HasLabel1 and HasLabel2
            if (rightRef.HasLabel1)
            {
                leftRef.HasLabel1 = rightRef.HasLabel1;
            }
            if (rightRef.HasLabel2)
            {
                leftRef.HasLabel2 = rightRef.HasLabel2;
            }

            // Quotations
            if (leftRef.Quotations.Count != 0 || rightRef.Quotations.Count != 0)
            {
                leftRef.Quotations.AddRange(rightRef.Quotations);
            }

            // ReferenceCollaborator
            foreach (Person collaborator in rightRef.Collaborators)
            {
                if (!leftRef.Collaborators.Contains(collaborator))
                {
                    leftRef.Collaborators.Add(collaborator);
                }
            }

            // ReferenceEditor
            foreach (Person editor in rightRef.Editors)
            {
                if (!leftRef.Editors.Contains(editor))
                {
                    leftRef.Editors.Add(editor);
                }
            }

            // change crossreferences
            foreach (EntityLink entityLink in rightRef.EntityLinks)
            {
                if (entityLink.Source == rightRef)
                {
                    entityLink.Source = leftRef;
                }
                else if (entityLink.Target == rightRef)
                {
                    entityLink.Target = leftRef;
                }
            }
        }

        // === 辅助方法 ===

        private string GetFieldValue(Reference reference, string fieldName)
        {
            try
            {
                var property = reference.GetType().GetProperty(fieldName);
                if (property == null) return "";

                var value = property.GetValue(reference);
                if (value == null) return "";

                // 尝试获取Text属性（用于RichText等类型）
                var textProperty = value.GetType().GetProperty("Text");
                if (textProperty != null)
                {
                    return textProperty.GetValue(value)?.ToString() ?? "";
                }

                return value.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private void SetFieldValue(Reference reference, string fieldName, string value)
        {
            try
            {
                var property = reference.GetType().GetProperty(fieldName);
                if (property == null) return;

                var currentValue = property.GetValue(reference);
                if (currentValue != null)
                {
                    var textProperty = currentValue.GetType().GetProperty("Text");
                    if (textProperty != null)
                    {
                        textProperty.SetValue(currentValue, value);
                        return;
                    }
                }

                // 特殊处理ISBN
                if (fieldName == "Isbn")
                {
                    try
                    {
                        property.SetValue(reference, value);
                    }
                    catch
                    {
                        // 忽略错误
                    }
                    return;
                }

                property.SetValue(reference, value);
            }
            catch (Exception ex)
            {
                // 忽略错误
            }
        }

        private string GetAuthorsString(Reference reference)
        {
            var authors = new List<string>();
            foreach (var author in reference.Authors)
            {
                string name = "";
                if (!string.IsNullOrEmpty(author.FullName))
                    name = author.FullName;
                else if (!string.IsNullOrEmpty(author.LastName))
                    name = author.LastName + (string.IsNullOrEmpty(author.FirstName) ? "" : ", " + author.FirstName);
                else if (!string.IsNullOrEmpty(author.FirstName))
                    name = author.FirstName;
                else
                    name = "[Unknown Author]";

                authors.Add(name);
            }
            return string.Join(Environment.NewLine, authors);
        }

        private string GetEditorsString(Reference reference)
        {
            var editors = new List<string>();
            foreach (var editor in reference.Editors)
            {
                string name = "";
                if (!string.IsNullOrEmpty(editor.FullName))
                    name = editor.FullName;
                else if (!string.IsNullOrEmpty(editor.LastName))
                    name = editor.LastName + (string.IsNullOrEmpty(editor.FirstName) ? "" : ", " + editor.FirstName);
                else if (!string.IsNullOrEmpty(editor.FirstName))
                    name = editor.FirstName;
                else
                    name = "[Unknown Editor]";

                editors.Add(name);
            }
            return string.Join(Environment.NewLine, editors);
        }

        private string GetCollaboratorsString(Reference reference)
        {
            var collaborators = new List<string>();
            foreach (var collaborator in reference.Collaborators)
            {
                string name = "";
                if (!string.IsNullOrEmpty(collaborator.FullName))
                    name = collaborator.FullName;
                else if (!string.IsNullOrEmpty(collaborator.LastName))
                    name = collaborator.LastName + (string.IsNullOrEmpty(collaborator.FirstName) ? "" : ", " + collaborator.FirstName);
                else if (!string.IsNullOrEmpty(collaborator.FirstName))
                    name = collaborator.FirstName;
                else
                    name = "[Unknown Collaborator]";

                collaborators.Add(name);
            }
            return string.Join(Environment.NewLine, collaborators);
        }

        private string GetOrganizationsString(Reference reference)
        {
            var organizations = new List<string>();
            foreach (var organization in reference.Organizations)
            {
                string name = "";
                if (!string.IsNullOrEmpty(organization.FullName))
                    name = organization.FullName;
                else if (!string.IsNullOrEmpty(organization.LastName))
                    name = organization.LastName + (string.IsNullOrEmpty(organization.FirstName) ? "" : ", " + organization.FirstName);
                else if (!string.IsNullOrEmpty(organization.FirstName))
                    name = organization.FirstName;
                else
                    name = "[Unknown Organization]";

                organizations.Add(name);
            }
            return string.Join(Environment.NewLine, organizations);
        }

        private string GetOthersInvolvedString(Reference reference)
        {
            var others = new List<string>();
            foreach (var other in reference.OthersInvolved)
            {
                string name = "";
                if (!string.IsNullOrEmpty(other.FullName))
                    name = other.FullName;
                else if (!string.IsNullOrEmpty(other.LastName))
                    name = other.LastName + (string.IsNullOrEmpty(other.FirstName) ? "" : ", " + other.FirstName);
                else if (!string.IsNullOrEmpty(other.FirstName))
                    name = other.FirstName;
                else
                    name = "[Unknown Person]";

                others.Add(name);
            }
            return string.Join(Environment.NewLine, others);
        }

        private string GetPublishersString(Reference reference)
        {
            var publishers = new List<string>();
            foreach (var publisher in reference.Publishers)
            {
                string name = "";
                if (!string.IsNullOrEmpty(publisher.FullName))
                    name = publisher.FullName;
                else
                    name = "[Unknown Publisher]";

                publishers.Add(name);
            }
            return string.Join(Environment.NewLine, publishers);
        }

        private string GetKeywordsString(Reference reference)
        {
            var keywords = new List<string>();
            foreach (var keyword in reference.Keywords)
            {
                if (!string.IsNullOrEmpty(keyword.FullName))
                    keywords.Add(keyword.FullName);
            }
            return string.Join(Environment.NewLine, keywords);
        }

        private string GetGroupsString(Reference reference)
        {
            var groups = new List<string>();
            foreach (var group in reference.Groups)
            {
                if (!string.IsNullOrEmpty(group.FullName))
                    groups.Add(group.FullName);
            }
            return string.Join(Environment.NewLine, groups);
        }

        private string GetCategoriesString(Reference reference)
        {
            var categories = new List<string>();
            foreach (var category in reference.Categories)
            {
                if (!string.IsNullOrEmpty(category.FullName))
                    categories.Add(category.FullName);
            }
            return string.Join(Environment.NewLine, categories);
        }

        private string GetLocationsString(Reference reference)
        {
            var locations = new List<string>();
            foreach (var location in reference.Locations)
            {
                if (!string.IsNullOrEmpty(location.FullName))
                    locations.Add(location.FullName);
            }
            return string.Join(Environment.NewLine, locations);
        }
    }

    public class MergeControl
    {
        public System.Windows.Forms.RichTextBox LeftControl { get; set; }
        public System.Windows.Forms.RichTextBox RightControl { get; set; }
        public System.Windows.Forms.RadioButton LeftButton { get; set; }
        public System.Windows.Forms.RadioButton RightButton { get; set; }
        public System.Windows.Forms.RadioButton CombineButton { get; set; }
        public bool IsCategoryField { get; set; }
        public bool IsSpecialField { get; set; }
    }
}
