using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SwissAcademic.Citavi;
using SwissAcademic.Controls;
using AutoWindowsSize;


namespace DuplicateComparing
{
    
    public partial class ComparingForm : FormBase
    {
        //public string leftRef;
        //public string rightRef;
        AutoAdaptWindowsSize AutoSize;
        // 返回结果属性
        public string DialogResult { get; private set; }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (AutoSize != null) // 一定加这个判断，电脑缩放布局不是100%的时候，会报错
            {
                AutoSize.FormSizeChanged();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AutoSize = new AutoAdaptWindowsSize(this); 
        }

        public override void Localize()
        {
            base.Localize();

        }

        protected override void OnApplicationIdle()
        {
            base.OnApplicationIdle();

        }


        public ComparingForm(Form owner, string leftRef, string rightRef) : base(owner)
        {
            InitializeComponent();
            Owner = owner;
            // 假设panelLeft是窗体中的Panel控件
            this.panelLeft.Text = leftRef;
            this.panelRight.Text = rightRef;
            //this.leftRef = leftRef;
            //this.rightRef = rightRef;
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
    }
}
