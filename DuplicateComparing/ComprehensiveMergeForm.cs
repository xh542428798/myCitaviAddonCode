using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DiffPlex.DiffBuilder.Model;
using DiffPlex.DiffBuilder;
using DiffPlex;
using SwissAcademic.Citavi;
using SwissAcademic.Controls;
using System.Linq;

namespace DuplicateComparing
{
    public partial class ComprehensiveMergeForm : FormBase
    {
        private Dictionary<string, MergeControl> mergeControls = new Dictionary<string, MergeControl>();
        private Reference leftRef;
        private Reference rightRef;

        public ComprehensiveMergeForm(Form owner, Reference leftRef, Reference rightRef) : base(owner)
        {
            this.leftRef = leftRef;
            this.rightRef = rightRef;
            InitializeComponent();
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
                Font = new Font("Microsoft YaHei UI", 11, FontStyle.Regular)
            };
            scrollPanel.Controls.Add(contentPanel);

            // 重要的放前面
            // === 文本类字段 ===
            AddFieldComparison(contentPanel, "Title", leftRef.Title, rightRef.Title);
            AddFieldComparison(contentPanel, "Abstract", leftRef.Abstract.Text, rightRef.Abstract.Text);

            AddFieldComparison(contentPanel, "Subtitle", leftRef.Subtitle, rightRef.Subtitle);
            AddFieldComparison(contentPanel, "ShortTitle", leftRef.ShortTitle, rightRef.ShortTitle);
            AddFieldComparison(contentPanel, "TranslatedTitle", leftRef.TranslatedTitle, rightRef.TranslatedTitle);
            // === 特殊字段 ===
            AddFieldComparison(contentPanel, "TableOfContents", leftRef.TableOfContents.Text, rightRef.TableOfContents.Text);
            AddFieldComparison(contentPanel, "Evaluation", leftRef.Evaluation.Text, rightRef.Evaluation.Text);

            AddSpecialFieldComparison(contentPanel, "Periodical", leftRef.Periodical.ToString(), rightRef.Periodical.ToString());
            // === 分类字段（人员） ===
            AddCategoryFieldComparison(contentPanel, "Authors", GetCategoryStr(leftRef, "Author"), GetCategoryStr(rightRef, "Author"));
            AddCategoryFieldComparison(contentPanel, "Editors", GetCategoryStr(leftRef, "Editor"), GetCategoryStr(rightRef, "Editor"));
            AddCategoryFieldComparison(contentPanel, "Collaborators", GetCategoryStr(leftRef, "Collaborator"), GetCategoryStr(rightRef, "Collaborator"));
            AddCategoryFieldComparison(contentPanel, "Organizations", GetCategoryStr(leftRef, "Organization"), GetCategoryStr(rightRef, "Organization"));
            AddCategoryFieldComparison(contentPanel, "OthersInvolved", GetCategoryStr(leftRef, "OthersInvolved"), GetCategoryStr(rightRef, "OthersInvolved"));
            AddCategoryFieldComparison(contentPanel, "Publishers", GetCategoryStr(leftRef, "Publishers"), GetCategoryStr(rightRef, "Publishers"));

            // === 分类字段（其他） ===
            AddCategoryFieldComparison(contentPanel, "Keywords", GetCategoryStr(leftRef, "Keyword"), GetCategoryStr(rightRef, "Keyword"));
            AddCategoryFieldComparison(contentPanel, "Groups", GetCategoryStr(leftRef, "Group"), GetCategoryStr(rightRef, "Group"));
            AddCategoryFieldComparison(contentPanel, "Categories", GetCategoryStr(leftRef, "Category"), GetCategoryStr(rightRef, "Category"));
            AddCategoryFieldComparison(contentPanel, "Locations", GetCategoryStr(leftRef, "Location"), GetCategoryStr(rightRef, "Location"));

            // === 自定义字段 ===
            for (int i = 1; i <= 9; i++)
            {
                string fieldName = "CustomField" + i;
                string leftValue = GetFieldValue(leftRef, fieldName);
                string rightValue = GetFieldValue(rightRef, fieldName);
                AddFieldComparison(contentPanel, fieldName, leftValue, rightValue);
            }
            AddFieldComparison(contentPanel, "Year", leftRef.Year, rightRef.Year);
            // === 标识符字段 ===
            AddFieldComparison(contentPanel, "DOI", leftRef.Doi, rightRef.Doi);
            AddFieldComparison(contentPanel, "ISBN", leftRef.Isbn.ToString(), rightRef.Isbn.ToString());
            AddFieldComparison(contentPanel, "PubMedId", leftRef.PubMedId, rightRef.PubMedId);

            // === 标题相关字段 ===
            AddFieldComparison(contentPanel, "Additions", leftRef.Additions, rightRef.Additions);
            AddFieldComparison(contentPanel, "ParallelTitle", leftRef.ParallelTitle, rightRef.ParallelTitle);
            AddFieldComparison(contentPanel, "UniformTitle", leftRef.UniformTitle, rightRef.UniformTitle);
            AddFieldComparison(contentPanel, "TitleSupplement", leftRef.TitleSupplement, rightRef.TitleSupplement);
            AddFieldComparison(contentPanel, "TitleInOtherLanguages", leftRef.TitleInOtherLanguages, rightRef.TitleInOtherLanguages);

            // === 出版信息字段 ===
            AddFieldComparison(contentPanel, "PlaceOfPublication", leftRef.PlaceOfPublication, rightRef.PlaceOfPublication);
            AddFieldComparison(contentPanel, "Date", leftRef.Date, rightRef.Date);
            AddFieldComparison(contentPanel, "Date2", leftRef.Date2, rightRef.Date2);
            AddFieldComparison(contentPanel, "Edition", leftRef.Edition, rightRef.Edition);
            AddFieldComparison(contentPanel, "Volume", leftRef.Volume, rightRef.Volume);
            AddFieldComparison(contentPanel, "Number", leftRef.Number, rightRef.Number);
            AddFieldComparison(contentPanel, "NumberOfVolumes", leftRef.NumberOfVolumes, rightRef.NumberOfVolumes);

            // === 其他信息字段 ===
            AddFieldComparison(contentPanel, "Language", leftRef.Language, rightRef.Language);
            AddFieldComparison(contentPanel, "Notes", leftRef.Notes, rightRef.Notes);
            AddFieldComparison(contentPanel, "OnlineAddress", leftRef.OnlineAddress, rightRef.OnlineAddress);
            AddFieldComparison(contentPanel, "OriginalCheckedBy", leftRef.OriginalCheckedBy, rightRef.OriginalCheckedBy);
            AddFieldComparison(contentPanel, "OriginalPublication", leftRef.OriginalPublication, rightRef.OriginalPublication);
            AddFieldComparison(contentPanel, "Price", leftRef.Price, rightRef.Price);
            AddFieldComparison(contentPanel, "SourceOfBibliographicInformation", leftRef.SourceOfBibliographicInformation, rightRef.SourceOfBibliographicInformation);
            AddFieldComparison(contentPanel, "StorageMedium", leftRef.StorageMedium, rightRef.StorageMedium);
            AddFieldComparison(contentPanel, "TextLinks", leftRef.TextLinks, rightRef.TextLinks);

            // === 特定字段 ===
            for (int i = 1; i <= 7; i++)
            {
                string fieldName = "SpecificField" + i;
                string leftValue = GetFieldValue(leftRef, fieldName);
                string rightValue = GetFieldValue(rightRef, fieldName);
                AddFieldComparison(contentPanel, fieldName, leftValue, rightValue);
            }

            // === 特殊处理字段 ===
            AddSpecialFieldComparison(contentPanel, "SeriesTitle", leftRef.SeriesTitle?.ToString() ?? "", rightRef.SeriesTitle?.ToString() ?? "");

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


        private void AddFieldComparison(System.Windows.Forms.TableLayoutPanel parent, string fieldName, string leftValue, string rightValue)
        {
            var fieldPanel = new System.Windows.Forms.TableLayoutPanel
            {
                RowCount = 2,
                ColumnCount = 3,
                Dock = DockStyle.Top,
                Height = 140,
                Padding = new Padding(5),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            fieldPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30));
            fieldPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100));

            // 大幅减少中间列宽度，增加文本框宽度
            fieldPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 47));  // 从40增到47
            fieldPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6));   // 从20减到6
            fieldPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 47));  // 从40增到47

            // 字段标题 - 字体12
            var titleLabel = new System.Windows.Forms.Label
            {
                Text = fieldName,
                Font = new Font("Microsoft YaHei UI", 12, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.DarkBlue
            };
            fieldPanel.Controls.Add(titleLabel, 0, 0);
            fieldPanel.SetColumnSpan(titleLabel, 3);

            // 左侧内容 - 字体12，移除最大宽度限制
            var leftTextBox = new System.Windows.Forms.RichTextBox
            {
                Text = leftValue ?? "",
                Dock = DockStyle.Fill,
                Font = new Font("Microsoft YaHei UI", 12, FontStyle.Regular),
                ScrollBars = RichTextBoxScrollBars.Vertical,
                Margin = new Padding(3),
                MinimumSize = new Size(40, 40)
                // 移除 MaximumSize，让文本框随窗体调整
            };
            fieldPanel.Controls.Add(leftTextBox, 0, 1);

            // 右侧内容 - 字体12，移除最大宽度限制
            var rightTextBox = new System.Windows.Forms.RichTextBox
            {
                Text = rightValue ?? "",
                Dock = DockStyle.Fill,
                Font = new Font("Microsoft YaHei UI", 12, FontStyle.Regular),
                ScrollBars = RichTextBoxScrollBars.Vertical,
                Margin = new Padding(3),
                MinimumSize = new Size(40, 40)
                // 移除 MaximumSize，让文本框随窗体调整
            };
            fieldPanel.Controls.Add(rightTextBox, 2, 1);

            // 中间选择按钮 - 垂直排列，完整单词但紧凑
            var buttonPanel = new System.Windows.Forms.FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(1),  // 最小内边距
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            var leftButton = new System.Windows.Forms.RadioButton
            {
                Text = "Left",
                Checked = true,
                Width = 45,  // 紧凑宽度
                Height = 20,
                Font = new Font("Microsoft YaHei UI", 9, FontStyle.Bold),  // 稍小字体
                Margin = new Padding(0, 0, 0, 0),  // 无边距
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter
            };
            var rightButton = new System.Windows.Forms.RadioButton
            {
                Text = "Right",
                Width = 50,  // 紧凑宽度
                Height = 20,
                Font = new Font("Microsoft YaHei UI", 9, FontStyle.Bold),  // 稍小字体
                Margin = new Padding(0, 0, 0, 0),  // 无边距
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter
            };
            var combineButton = new System.Windows.Forms.RadioButton
            {
                Text = "Both",
                Width = 45,  // 紧凑宽度
                Height = 20,
                Font = new Font("Microsoft YaHei UI", 9, FontStyle.Bold),  // 稍小字体
                Margin = new Padding(0, 0, 0, 0),  // 无边距
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter
            };


            buttonPanel.Controls.AddRange(new Control[] { leftButton, rightButton, combineButton });
            fieldPanel.Controls.Add(buttonPanel, 1, 1);

            // 如果内容不同，高亮显示差异
            if (!string.IsNullOrEmpty(leftValue) && !string.IsNullOrEmpty(rightValue) && leftValue != rightValue)
            {
                CompareStrings(leftTextBox, rightTextBox, leftValue, rightValue);
                titleLabel.ForeColor = Color.Red;
            }

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

        private void CompareStrings(System.Windows.Forms.RichTextBox leftBox, System.Windows.Forms.RichTextBox rightBox, string leftText, string rightText)
        {
            try
            {
                // 确保 text 不为 null
                leftText = leftText ?? "";
                rightText = rightText ?? "";

                SideBySideDiffBuilder diffBuilder = new SideBySideDiffBuilder(new Differ());
                SideBySideDiffModel diffModel = diffBuilder.BuildDiffModel(leftText, rightText);

                leftBox.Clear();
                rightBox.Clear();

                // 处理左侧文本
                foreach (var line in diffModel.OldText.Lines)
                {
                    if (line?.SubPieces == null) continue;

                    foreach (var change in line.SubPieces)
                    {
                        if (change == null) continue;

                        string textToAppend = change.Text ?? "";

                        switch (change.Type)
                        {
                            case ChangeType.Deleted:
                                leftBox.SelectionColor = Color.Red;
                                leftBox.SelectionBackColor = Color.LightYellow;
                                break;
                            case ChangeType.Inserted:
                                leftBox.SelectionColor = Color.Green;
                                leftBox.SelectionBackColor = Color.LightGreen;
                                break;
                            case ChangeType.Modified:
                                leftBox.SelectionColor = Color.DarkOrange;
                                leftBox.SelectionBackColor = Color.LightYellow;
                                break;
                            case ChangeType.Unchanged:
                            default:
                                leftBox.SelectionColor = Color.Black;
                                leftBox.SelectionBackColor = Color.Transparent;
                                break;
                        }

                        if (!string.IsNullOrEmpty(textToAppend))
                        {
                            leftBox.AppendText(textToAppend);
                        }
                    }
                    leftBox.AppendText(Environment.NewLine);
                }

                // 处理右侧文本
                foreach (var line in diffModel.NewText.Lines)
                {
                    if (line?.SubPieces == null) continue;

                    foreach (var change in line.SubPieces)
                    {
                        if (change == null) continue;

                        string textToAppend = change.Text ?? "";

                        switch (change.Type)
                        {
                            case ChangeType.Deleted:
                                rightBox.SelectionColor = Color.Red;
                                rightBox.SelectionBackColor = Color.LightYellow;
                                break;
                            case ChangeType.Inserted:
                                rightBox.SelectionColor = Color.Green;
                                rightBox.SelectionBackColor = Color.LightGreen;
                                break;
                            case ChangeType.Modified:
                                rightBox.SelectionColor = Color.DarkOrange;
                                rightBox.SelectionBackColor = Color.LightYellow;
                                break;
                            case ChangeType.Unchanged:
                            default:
                                rightBox.SelectionColor = Color.Black;
                                rightBox.SelectionBackColor = Color.Transparent;
                                break;
                        }

                        if (!string.IsNullOrEmpty(textToAppend))
                        {
                            rightBox.AppendText(textToAppend);
                        }
                    }
                    rightBox.AppendText(Environment.NewLine);
                }

                // 重置选择状态
                leftBox.SelectionStart = 0;
                leftBox.SelectionLength = 0;
                rightBox.SelectionStart = 0;
                rightBox.SelectionLength = 0;
            }
            catch (Exception ex)
            {
                // 如果比较出错，显示原始文本并用红色背景标记
                leftBox.Text = leftText ?? "";
                rightBox.Text = rightText ?? "";
                leftBox.BackColor = Color.MistyRose;
                rightBox.BackColor = Color.MistyRose;
            }
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

            switch (cateName)
            {
                case "Group":
                    List<Group> groupCategories = reference.Groups.ToList();
                    List<string> nameString = new List<string>();
                    foreach (Group mygroup in groupCategories)
                    {
                        if (mygroup != null && !string.IsNullOrEmpty(mygroup.FullName))
                            nameString.Add(mygroup.FullName);
                    }
                    result = GetStringFromArray(nameString);
                    break;
                case "Category":
                    List<Category> categoryRefCategories = reference.Categories.ToList();
                    List<string> nameString2 = new List<string>();
                    foreach (Category category in categoryRefCategories)
                    {
                        if (category != null && !string.IsNullOrEmpty(category.FullName))
                            nameString2.Add(category.FullName);
                    }
                    result = GetStringFromArray(nameString2);
                    break;
                case "Location":
                    List<Location> refLocation = reference.Locations.ToList();
                    List<string> nameString3 = new List<string>();
                    foreach (Location location in refLocation)
                    {
                        if (location != null && !string.IsNullOrEmpty(location.FullName))
                            nameString3.Add(location.FullName);
                    }
                    result = GetStringFromArray(nameString3);
                    break;
                case "Keyword":
                    List<Keyword> refKeywords = reference.Keywords.ToList();
                    List<string> nameString4 = new List<string>();
                    foreach (Keyword keyword in refKeywords)
                    {
                        if (keyword != null && !string.IsNullOrEmpty(keyword.FullName))
                            nameString4.Add(keyword.FullName);
                    }
                    result = GetStringFromArray(nameString4);
                    break;
                case "Author":
                    List<Person> refPersons = reference.Authors.ToList();
                    List<string> nameString5 = new List<string>();
                    foreach (Person person in refPersons)
                    {
                        if (person != null && !string.IsNullOrEmpty(person.FullName))
                            nameString5.Add(person.FullName);
                    }
                    result = GetStringFromArray(nameString5);
                    break;
                case "Collaborator":
                    refPersons = reference.Collaborators.ToList();
                    nameString5 = new List<string>();
                    foreach (Person person in refPersons)
                    {
                        if (person != null && !string.IsNullOrEmpty(person.FullName))
                            nameString5.Add(person.FullName);
                    }
                    result = GetStringFromArray(nameString5);
                    break;
                case "Editor":
                    refPersons = reference.Editors.ToList();
                    nameString5 = new List<string>();
                    foreach (Person person in refPersons)
                    {
                        if (person != null && !string.IsNullOrEmpty(person.FullName))
                            nameString5.Add(person.FullName);
                    }
                    result = GetStringFromArray(nameString5);
                    break;
                case "Organization":
                    refPersons = reference.Organizations.ToList();
                    nameString5 = new List<string>();
                    foreach (Person person in refPersons)
                    {
                        if (person != null && !string.IsNullOrEmpty(person.FullName))
                            nameString5.Add(person.FullName);
                    }
                    result = GetStringFromArray(nameString5);
                    break;
                case "OthersInvolved":
                    refPersons = reference.OthersInvolved.ToList();
                    nameString5 = new List<string>();
                    foreach (Person person in refPersons)
                    {
                        if (person != null && !string.IsNullOrEmpty(person.FullName))
                            nameString5.Add(person.FullName);
                    }
                    result = GetStringFromArray(nameString5);
                    break;
                case "Publishers":
                    List<Publisher> refPublishers = reference.Publishers.ToList();
                    nameString5 = new List<string>();
                    foreach (Publisher publisher in refPublishers)
                    {
                        if (publisher != null && !string.IsNullOrEmpty(publisher.FullName))
                            nameString5.Add(publisher.FullName);
                    }
                    result = GetStringFromArray(nameString5);
                    break;
            }
            return result;
        }

        public static string GetStringFromArray(List<string> nameString)
        {
            if (nameString == null || nameString.Count == 0) return "";

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
