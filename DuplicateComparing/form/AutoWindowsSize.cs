using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Drawing.Drawing2D;
//using System.Threading.Tasks;


namespace AutoWindowsSize
{
    class AutoAdaptWindowsSize                       //窗口缩放对象
    {
        double formOriginalWidth;//窗体高度原始宽度
        double formOriginalHeight;//窗体原始
        double scaleX;//水平缩放比例
        double scaleY;//垂直缩放比例
        Dictionary<string, string> ControlsInfo = new Dictionary<string, string>();//控件中心Left,Top,控件Width,控件Height,控件字体Size

        private Form _form;
        Panel Win_Panel1 = new Panel();
        public AutoAdaptWindowsSize(Form form)
        {
            _form = form;

            //代码生成一个容器panel1，添加至窗体
            _form.Controls.Add(Win_Panel1);
            Win_Panel1.BorderStyle = BorderStyle.None;    //容器border样式
            Win_Panel1.Dock = DockStyle.Fill;             //设置填充，下面添加控件至容器完成后，容器会填充窗口
            Win_Panel1.BackColor = Color.Transparent;    // 这里默认的背景颜色是form的背景颜色，如果form页面时图片，需要将这里的颜色设置成透明，否则会被覆盖。
            //将窗体所有控件添加至panel1
            while (_form.Controls[0].Name.Trim() != "")
            {
                foreach (Control item in _form.Controls)
                {

                    if (item.Name.Trim() != "" && item.Name.Trim() != Win_Panel1.Name.Trim())
                    {
                        Win_Panel1.Controls.Add(item);
                    }
                }
            }

            //保存窗体和控件初始大小
            InitControlsInfo(Win_Panel1);
        }

        public void InitControlsInfo(Control ctrlContainer)
        {
            if (ctrlContainer.Parent == _form)//获取窗体的高度和宽度
            {
                formOriginalWidth = Convert.ToDouble(ctrlContainer.Width);
                formOriginalHeight = Convert.ToDouble(ctrlContainer.Height);
            }
            foreach (Control item in ctrlContainer.Controls)
            {
                if (item.Name.Trim() != "")
                {
                    //添加信息：键值：控件名，内容：据左边距离，距顶部距离，控件宽度，控件高度，控件字体。
                    ControlsInfo.Add(item.Name, (item.Left + item.Width / 2) + "," + (item.Top + item.Height / 2) + "," + item.Width + "," + item.Height + "," + item.Font.Size);
                }
                if ((item as UserControl) == null && item.Controls.Count > 0)
                {
                    InitControlsInfo(item);
                }
            }

        }
        public void FormSizeChanged()
        {
            try
            {
                if (ControlsInfo.Count > 0)//如果字典中有数据，即窗体改变
                {
                    ControlsZoomScale(Win_Panel1);//表示pannel控件
                    ControlsChange(Win_Panel1);
                }
            }
            catch { }
        }
        private void ControlsZoomScale(Control ctrlContainer)
        {
            scaleX = (Convert.ToDouble(ctrlContainer.Width) / formOriginalWidth);
            scaleY = (Convert.ToDouble(ctrlContainer.Height) / formOriginalHeight);
        }

        // 改变控件大小

        private void ControlsChange(Control ctrlContainer)
        {
            double[] pos = new double[5];//pos数组保存当前控件中心Left,Top,控件Width,控件Height,控件字体Size
            foreach (Control item in ctrlContainer.Controls)//遍历控件
            {
                if (item.Name.Trim() != "")//如果控件名不是空，则执行
                {
                    if ((item as UserControl) == null && item.Controls.Count > 0)//如果不是自定义控件
                    {
                        ControlsChange(item);//循环执行
                    }
                    string[] strs = ControlsInfo[item.Name].Split(',');//从字典中查出的数据，以‘，’分割成字符串组

                    for (int i = 0; i < 5; i++)
                    {
                        pos[i] = Convert.ToDouble(strs[i]);//添加到临时数组
                    }
                    double itemWidth = pos[2] * scaleX;     //计算控件宽度，double类型
                    double itemHeight = pos[3] * scaleY;    //计算控件高度
                    item.Left = Convert.ToInt32(pos[0] * scaleX - itemWidth / 2);//计算控件距离左边距离
                    item.Top = Convert.ToInt32(pos[1] * scaleY - itemHeight / 2);//计算控件距离顶部距离
                    item.Width = Convert.ToInt32(itemWidth);//控件宽度，int类型
                    item.Height = Convert.ToInt32(itemHeight);//控件高度
                    if (float.Parse((pos[4] * Math.Min(scaleX, scaleY)).ToString()) != 0)         //缩放字体大小不能为0
                    { item.Font = new Font(item.Font.Name, float.Parse((pos[4] * Math.Min(scaleX, scaleY)).ToString())); }  //字体
                }
            }
        }
    }
}
