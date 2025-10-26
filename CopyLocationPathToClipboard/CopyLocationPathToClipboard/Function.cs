// ToolboxReference.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Shell;

namespace CopyLocationPathToClipboard
{
    class Function
    {
        /// <summary>
        /// 获取用户选中的所有电子附件，并将其本地文件路径复制到剪贴板。
        /// </summary>
        public static void CopyLocationClipboard()
        {
            // 获取用户在附件列表中选中的所有电子附件
            List<Location> locations = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedElectronicLocations();
            List<String> output = new List<String>();

            // 遍历每一个选中的附件
            foreach (Location location in locations)
            {
                // 如果不是第一个文件，先添加一个换行符
                if (output.Count > 0) output.Add("\n");

                // 将文件路径用双引号括起来，然后添加到输出列表
                output.Add("\"");
                output.Add(location.Address.Resolve().LocalPath);
                output.Add("\"");
            }

            // 将所有路径合并成一个字符串，并设置到剪贴板
            Clipboard.SetText(String.Join("", output));
        }
    }
}
