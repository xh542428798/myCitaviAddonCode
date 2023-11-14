using System;
using System.Windows.Forms;
using SwissAcademic.Citavi;
using SwissAcademic.Controls;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using DiffPlex.WindowsForms.Controls;
using System.Drawing;
using System.Collections.Generic;

namespace DuplicateComparing
{
    public partial class ComparingForm : FormBase
    {
        //public string leftRef;
        //public string rightRef;
        // 返回结果属性
        public string DialogResult { get; private set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        public override void Localize()
        {
            base.Localize();

        }

        protected override void OnApplicationIdle()
        {
            base.OnApplicationIdle();

        }


        public ComparingForm(Form owner, string leftRef, string rightRef, bool doi =false) : base(owner)
        {
            InitializeComponent();
            Owner = owner;

            // 比较单词数组
            if(!doi)
            {
                CompareStrings(leftRef, rightRef);
            }
            else
            {
                // 假设panelLeft是窗体中的Panel控件
                this.panelLeft.Text = leftRef;
                this.panelRight.Text = rightRef;
            }



            // 取消 TextBox 的文本选中状态
            panelLeft.SelectionStart = 0;
            panelLeft.SelectionLength = 0;
            panelRight.SelectionStart = 0;
            panelRight.SelectionLength = 0;
            //// 设置窗体的默认尺寸为宽度为800，高度为600
            //this.Size = new Size(900, 400);
            // 获取屏幕的工作区域（不包括任务栏）
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            int formHeight = this.Height;
            // 调整窗体位置和大小
            this.Location = new Point(this.Location.X, screenHeight / 3 - formHeight / 2);

        }
            private void CompareStrings(string leftRef, string rightRef)
        {
            SideBySideDiffBuilder diffBuilder = new SideBySideDiffBuilder(new Differ());
            SideBySideDiffModel diffModel = diffBuilder.BuildDiffModel(leftRef, rightRef);

            this.panelLeft.Clear();
            this.panelRight.Clear();
            List<int> delelteInfo = new List<int>();
            foreach (var line in diffModel.OldText.Lines)
            {
                for (int i = 0; i < line.SubPieces.Count; i++)
                {
                    var change = line.SubPieces[i];
                //}
                //foreach (var change in line.SubPieces)
                //{
                    
                    // if (string.IsNullOrEmpty(change?.Text)) continue;
                    if (change.Type == ChangeType.Imaginary)
                    {
                        continue;
                    }
                    else if(change.Type == ChangeType.Deleted)
                    {
                        if (change.Text == " ")
                        {
                            this.panelLeft.SelectionBackColor = Color.Transparent;
                            this.panelLeft.AppendText(" ");
                            delelteInfo.Add(i);
                        }
                        else
                        {
                            this.panelLeft.SelectionBackColor = Color.Transparent;
                            this.panelLeft.SelectionColor = Color.Red;
                            this.panelLeft.AppendText(change.Text);
                        }
                            
                        
                    }
                    else if (change.Type == ChangeType.Inserted)
                    {
                        this.panelLeft.SelectionBackColor = Color.Transparent;
                        this.panelLeft.SelectionColor = Color.Green;
                        this.panelLeft.AppendText(change.Text);
                    }
                    else if (change.Type == ChangeType.Modified)
                    {
                        this.panelLeft.SelectionBackColor = Color.Transparent;
                        this.panelLeft.SelectionColor = Color.Yellow;
                        this.panelLeft.AppendText(change.Text);
                    }
                    else
                    {
                        this.panelLeft.SelectionBackColor = Color.Transparent;
                        this.panelLeft.SelectionColor = Color.Black;
                        this.panelLeft.AppendText(change.Text);
                    }
                }

                this.panelLeft.AppendText(" ");
            }

            foreach (var line in diffModel.NewText.Lines)
            {
                for (int i = 0; i < line.SubPieces.Count; i++)
                {
                    var change = line.SubPieces[i];
                    if (delelteInfo.Contains(i))
                    {
                        this.panelRight.SelectionBackColor = Color.Red;
                        this.panelRight.AppendText(" ");
                    }
                    //}
                    // if (string.IsNullOrEmpty(change?.Text)) continue;
                    if (change.Type == ChangeType.Imaginary)
                    {
                        continue;
                    }
                    else if (change.Type == ChangeType.Deleted)
                    {
                        if (change.Text == " ")
                        {
                            continue;
                            //this.panelRight.SelectionBackColor = Color.Red;
                            //this.panelRight.AppendText(" ");
                        }
                        else
                        {
                            this.panelRight.SelectionBackColor = Color.Transparent;
                            this.panelRight.SelectionColor = Color.Red;
                            this.panelRight.AppendText(change.Text);
                        }
                    }
                    else if (change.Type == ChangeType.Inserted)
                    {
                        this.panelRight.SelectionBackColor = Color.Transparent;
                        this.panelRight.SelectionColor = Color.Green;
                        this.panelRight.AppendText(change.Text);
                    }
                    else if (change.Type == ChangeType.Modified)
                    {
                        this.panelRight.SelectionBackColor = Color.Transparent;
                        this.panelRight.SelectionColor = Color.Yellow;
                        this.panelRight.AppendText(change.Text);
                    }
                    else
                    {
                        this.panelRight.SelectionBackColor = Color.Transparent;
                        this.panelRight.SelectionColor = Color.Black;
                        this.panelRight.AppendText(change.Text);
                    }
                }

                this.panelRight.AppendText(" ");
            }
        }

        private void UsingleftButton_Click(object sender, EventArgs e)
        {
            DialogResult = "left";
            this.Close();
        }

        private void UsingrightButton_Click(object sender, EventArgs e)
        {
            DialogResult = "right";
            this.Close();
        }

        private void CombineButton_Click(object sender, EventArgs e)
        {
            DialogResult = "combine";
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = "cancel";
            this.Close();
        }
    }
}
