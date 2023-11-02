using DuplicateComparing;
using System;
using System.Windows.Forms;

namespace YourNamespace
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());  // 替换为你的主窗体类名
        }
    }
}