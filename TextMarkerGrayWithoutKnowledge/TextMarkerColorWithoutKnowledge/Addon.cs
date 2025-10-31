using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Shell;
using SwissAcademic.Citavi.Shell.Controls.Preview;
using SwissAcademic.Citavi.Controls.Wpf;
using SwissAcademic.Citavi.Metadata;
using SwissAcademic.Pdf;
using SwissAcademic.Pdf.Analysis;
using System;
using System.Reflection;
using System.Linq;
using SwissAcademic.Controls;
using System.Windows.Forms;
using System.Drawing;

namespace TextMarkerColorWithoutKnowledge
{
    public class TextMarkerColorWithoutKnowledge : CitaviAddOn<MainForm>
    {
        private const string GrayHighlightButtonKey = "Highlight in gray";
        private const string UnderlineButtonKey = "Underline Text";
        // 新增：存储当前下划线颜色，默认为红色
        private Color _currentUnderlineColor = Color.Red;
        #region Methods

        public override void OnHostingFormLoaded(MainForm mainForm)
        {
            var toolbar = mainForm.GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar);
            var redButton = toolbar.InsertCommandbarButton(1,GrayHighlightButtonKey, "用灰色高亮并创建隐藏的Knowledge", CommandbarItemStyle.ImageOnly, icon.gray);
            //redButton.HasSeparator = true;
            redButton.Shortcut = (Shortcut)(Keys.B);

            // 1. 下划线按钮（使用动态生成的红色图像）
            Image initialUnderlineImage = GenerateUnderlineImage(_currentUnderlineColor); // _currentUnderlineColor 默认是红色
            var underlineButton = toolbar.InsertCommandbarButton(2, UnderlineButtonKey, "添加下划线但不创建Knowledge", CommandbarItemStyle.ImageOnly, initialUnderlineImage);
            underlineButton.Shortcut = (Shortcut)(Keys.U);

            // 2. 颜色选择按钮（一个小的调色板图标）

            var colorPickerButton = toolbar.InsertCommandbarButton(3, "PickUnderlineColor", "选择下划线颜色", CommandbarItemStyle.ImageOnly, icon.palette); // 你需要一个调色板图标


            base.OnHostingFormLoaded(mainForm);
        }

        public override void OnBeforePerformingCommand(MainForm mainForm, BeforePerformingCommandEventArgs e)
        {
            if (e.Key == GrayHighlightButtonKey)
            {
                var previewControl = mainForm.PreviewControl;
                if (previewControl.ActivePreviewType == PreviewType.Pdf)
                {
                    var pdfViewControl = previewControl.GetPdfViewControl();
                    if (pdfViewControl != null && pdfViewControl.GetSelectedContentType() == ContentType.Text)
                    {
                        CreateRedHighlight(pdfViewControl, mainForm);
                        e.Handled = true;
                    }
                }
            }
            else if (e.Key == UnderlineButtonKey)
            {
                var previewControl = mainForm.PreviewControl;
                if (previewControl.ActivePreviewType == PreviewType.Pdf)
                {
                    var pdfViewControl = previewControl.GetPdfViewControl();
                    if (pdfViewControl != null && pdfViewControl.GetSelectedContentType() == ContentType.Text)
                    {
                        // 左键直接使用当前颜色创建下划线
                        CreateUnderline(pdfViewControl, mainForm, _currentUnderlineColor);
                        e.Handled = true;
                    }
                }
            }
            else if (e.Key == "PickUnderlineColor") // 处理颜色选择按钮
            {
                // 点击后弹出颜色选择框
                Color newColor = GetColorFromDialog(_currentUnderlineColor);
                if (newColor != Color.Empty)
                {
                    _currentUnderlineColor = newColor;

                    // 核心：生成新图像并更新按钮
                    Image newImage = GenerateUnderlineImage(_currentUnderlineColor);
                    var underlineButton = mainForm.GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar).GetCommandbarButton(UnderlineButtonKey);
                    if (underlineButton != null)
                    {
                        underlineButton.Image = newImage; // 现在类型匹配了！
                    }

                    //MessageBox.Show(string.Format("下划线颜色已更新为: {0}", newColor.Name), "颜色已更改", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                e.Handled = true;
            }


            base.OnBeforePerformingCommand(mainForm, e);
        }

        /// <summary>
        /// 根据指定的颜色动态生成一个下划线图像
        /// </summary>
        /// <param name="color">下划线的颜色</param>
        /// <returns>生成的图像</returns>
        private Image GenerateUnderlineImage(Color color)
        {
            // 1. 定义图像尺寸 (16x16 是标准工具栏图标大小)
            int width = 16;
            int height = 16;
            Bitmap bitmap = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // 2. 设置高质量绘制
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent); // 透明背景

                // 3. 绘制一个代表文本的字母 "A"
                // --- 修改在这里 ---
                // 使用 new FontFamily("Arial") 和完全限定的 System.Drawing.FontStyle.Regular
                using (Font font = new Font(new FontFamily("Arial"), 10, System.Drawing.FontStyle.Regular))
                using (SolidBrush textBrush = new SolidBrush(Color.Black)) // 字母用黑色
                {
                    g.DrawString("A", font, textBrush, 1, 0);
                }

                // 4. 在字母下方绘制下划线
                using (Pen underlinePen = new Pen(color, 2)) // 使用传入的颜色，线宽为2
                {
                    // 下划线位置大概在字母 "A" 的下方
                    g.DrawLine(underlinePen, 1, 12, 13, 12);
                }
            }

            // 5. 直接返回 Bitmap 对象 (Bitmap 继承自 Image)
            return bitmap;
        }



        private Color GetColorFromDialog(Color initialColor)
        {
            try
            {
                using (ColorDialog colorDialog = new ColorDialog())
                {
                    colorDialog.AllowFullOpen = true;
                    colorDialog.AnyColor = true;
                    colorDialog.SolidColorOnly = true;
                    colorDialog.Color = initialColor; // 使用传入的颜色作为初始颜色

                    if (colorDialog.ShowDialog() == DialogResult.OK)
                    {
                        return colorDialog.Color;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("调用色板失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine("GetColorFromDialog错误: " + ex);
            }

            return Color.Empty;
        }

        private void CreateUnderline(PdfViewControl pdfViewControl, MainForm mainForm, Color underlineColor)
        {
            try
            {
                // 1. 获取基本对象
                var project = Program.ActiveProjectShell.Project;
                var location = pdfViewControl.Location;

                if (location == null)
                {
                    throw new Exception("无法获取Location");
                }

                var reference = location.Reference;
                if (reference == null)
                {
                    throw new Exception("无法获取Reference");
                }

                // 2. 获取选中的文本内容
                var textContent = pdfViewControl.GetSelectedContentFromType(
                    pdfViewControl.GetSelectedContentType(), -1, false, true) as TextContent;

                if (textContent == null)
                {
                    throw new Exception("无法获取选中的文本内容，请先选择要标记的文本");
                }

                // 3. 创建下划线注释（使用你提供的方法）
                Annotation annotation = new Annotation(location);
                annotation.Quads = textContent.Annotation.Quads;
                annotation.OriginalColor = underlineColor;
                location.Annotations.Add(annotation);
            }
            catch (Exception ex)
            {
                MessageBox.Show("创建下划线失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine("CreateUnderline错误: " + ex);
            }
        }

        private void CreateRedHighlight(PdfViewControl pdfViewControl, MainForm mainForm)
        {
            try
            {
                // 1. 获取基本对象
                var project = Program.ActiveProjectShell.Project;
                var location = pdfViewControl.Location;

                if (location == null)
                {
                    throw new Exception("无法获取Location");
                }

                var reference = location.Reference;
                if (reference == null)
                {
                    throw new Exception("无法获取Reference");
                }

                // 2. 获取选中的文本内容
                var textContent = pdfViewControl.GetSelectedContentFromType(
                    pdfViewControl.GetSelectedContentType(), -1, false, true) as TextContent;

                if (textContent == null)
                {
                    throw new Exception("无法获取选中的文本内容，请先选择要标记的文本");
                }

                // 3. 使用Citavi内部方法创建高亮
                var annotation = CreateHighlightUsingCitaviMethod(pdfViewControl, textContent);

                if (annotation == null)
                {
                    throw new Exception("无法创建高亮注释");
                }


                // 8. 跳转到新创建的Annotation
                //pdfViewControl.GoToAnnotation(annotation);
            }
            catch (Exception ex)
            {
                MessageBox.Show("创建红色标记失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Diagnostics.Debug.WriteLine("CreateRedHighlight错误: " + ex);
            }
        }

        private Annotation CreateHighlightUsingCitaviMethod(PdfViewControl pdfViewControl, TextContent textContent)
        {
            try
            {
                var method = pdfViewControl.GetType().GetMethod(
                    "OnAnnotationCreating",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new[] { typeof(Content), typeof(short), typeof(bool) },
                    null);

                if (method != null)
                {
                    // 直接调用，只需要改这个数字来测试不同颜色
                    method.Invoke(pdfViewControl, new object[] { textContent, (short)2, true });
                    return FindNewlyCreatedAnnotation(pdfViewControl);
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("CreateHighlightUsingCitaviMethod错误: " + ex);
                return null;
            }
        }


        private Annotation FindNewlyCreatedAnnotation(PdfViewControl pdfViewControl)
        {
            try
            {
                var location = pdfViewControl.Location;
                if (location == null) return null;

                // 获取所有注释，找到最新创建的
                var annotations = location.Annotations.ToList();
                if (annotations.Count == 0) return null;

                // 返回最后一个注释（假设是最新创建的）
                return annotations.LastOrDefault();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("FindNewlyCreatedAnnotation错误: " + ex);
                return null;
            }
        }

        public override void OnApplicationIdle(MainForm mainForm)
        {
            var previewControl = mainForm.PreviewControl;
            var toolbar = mainForm.GetPreviewCommandbar(MainFormPreviewCommandbarId.Toolbar);

            bool shouldShow = previewControl.ActivePreviewType == PreviewType.Pdf &&
                            previewControl.GetPdfViewControl()?.GetSelectedContentType() == ContentType.Text;

            // 控制所有相关按钮的可见性
            toolbar.GetCommandbarButton(GrayHighlightButtonKey).Visible = shouldShow;
            toolbar.GetCommandbarButton(UnderlineButtonKey).Visible = shouldShow;
            toolbar.GetCommandbarButton("PickUnderlineColor").Visible = shouldShow;

            base.OnApplicationIdle(mainForm);
        }

        #endregion
    }
}
