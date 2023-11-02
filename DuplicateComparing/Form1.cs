using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DuplicateComparing
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            // 创建 Panel 控件并设置其属性
            //Panel panel1 = new Panel();
            //panel1.Location = new System.Drawing.Point(10, 10);
            //panel1.Size = new System.Drawing.Size(300, 200);
            //panel1.BorderStyle = BorderStyle.FixedSingle;
            //panel1.AutoScroll = true;

            //// 将 Panel 控件添加到窗体中
            ////this.Controls.Add(panel1);

            //// 创建一个新的 VScrollBar 控件并设置其属性
            //VScrollBar vScrollBar1 = new VScrollBar();
            //vScrollBar1.Dock = DockStyle.Right;
            //vScrollBar1.Scroll += new ScrollEventHandler(panel1_Scroll);
            //panel1.Controls.Add(vScrollBar1);

            //// 添加 Scroll 事件处理程序以实现滚动效果
            //void panel1_Scroll(object sender, ScrollEventArgs e)
            //{
            //    panel1.VerticalScroll.Value = e.NewValue;
            //}

            // 设置每个标签的高度和间距
            int labelHeight = 20;
            int labelSpacing = 5;

            // 循环添加 10 个标签
            for (int i = 0; i < 50; i++)
            {
                // 创建一个新的 Label 控件并设置其属性
                Label label = new Label();
                label.Text = "这是标签 " + (i + 1).ToString();  // 设置标签文本
                label.Location = new System.Drawing.Point(10, 10 + i * (labelHeight + labelSpacing));  // 设置标签位置
                label.Size = new System.Drawing.Size(200, labelHeight);

                // 将 Label 控件添加到 Panel 控件中
                panel1.Controls.Add(label);
            }

            labelHeight = 20;
            labelSpacing = 5;
            // 循环添加 10 个标签
            for (int i = 0; i < 50; i++)
            {
                // 创建一个新的 Label 控件并设置其属性
                Label label = new Label();
                label.Text = "这是标签 " + (i + 1).ToString();  // 设置标签文本
                label.Location = new System.Drawing.Point(10, 10 + i * (labelHeight + labelSpacing));  // 设置标签位置
                label.Size = new System.Drawing.Size(200, labelHeight);

                // 将 Label 控件添加到 Panel 控件中
                panel2.Controls.Add(label);
            }

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
