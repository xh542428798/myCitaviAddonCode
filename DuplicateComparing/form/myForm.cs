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
    public partial class myForm : FormBase
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


        public myForm(Form owner, string leftRef, string rightRef, string titleText, bool Comparing = true) : base(owner)
        {
            InitializeComponent();
            Owner = owner;
            this.label1.Text = titleText;
            // 比较单词数组
            if (Comparing)
            {
                this.panelLeft.AppendText(titleText);
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

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
