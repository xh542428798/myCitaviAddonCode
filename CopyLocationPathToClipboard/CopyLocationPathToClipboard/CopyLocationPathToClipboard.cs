// ReferencesToolboxUI.cs

using System.Windows.Forms;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Shell;
using SwissAcademic.Controls;
using Infragistics.Win.UltraWinToolbars;

namespace CopyLocationPathToClipboard
{
    // 插件的主类，继承自 CitaviAddOn
    class CopyLocationPathToClipboard : CitaviAddOn
    {
        // 指定插件宿主在 Citavi 的主窗体
        public override AddOnHostingForm HostingForm
        {
            get { return AddOnHostingForm.MainForm; }
        }

        // 当宿主窗体加载时执行此方法
        protected override void OnHostingFormLoaded(System.Windows.Forms.Form hostingForm)
        {
            MainForm mainForm = (MainForm)hostingForm;

            // --- 核心代码：在附件的右键菜单中添加一个按钮 ---
            // 1. 找到附件列表的右键上下文菜单
            var referenceEditorUriLocationsContextMenu = CommandbarMenu.Create(mainForm.GetReferenceEditorElectronicLocationsCommandbarManager().ToolbarsManager.Tools["ReferenceEditorUriLocationsContextMenu"] as PopupMenuTool);


            // 2. 在这个菜单中添加一个新的按钮
            //    "CopyLocationPathToClipboard" 是按钮的唯一标识符
            //    ReferencesToolboxLocalizations.CopyLocationPathToClipboard 是按钮显示的文本
            var commandBarButtonCopyLocationPathToClipboard = referenceEditorUriLocationsContextMenu.AddCommandbarButton("CopyLocationPathToClipboard", CopyLocationPathToClipboardLocalizations.CopyLocationPathToClipboard, CommandbarItemStyle.ImageAndText, SwissAcademic.Citavi.Shell.Properties.Resources.Copy);
            // --- 位置2：在“所有附件”视图的右键菜单中添加按钮 (这是你出错的部分，已修正) ---
            var commandBarButtonCopyLocationPathToClipboard2 = mainForm.GetReferenceEditorLocationsCommandbarManager().GetCommandbar(MainFormReferenceEditorLocationsCommandbarId.Toolbar).AddCommandbarButton("CopyLocationPathToClipboard", CopyLocationPathToClipboardLocalizations.CopyLocationPathToClipboard, CommandbarItemStyle.ImageOnly, SwissAcademic.Citavi.Shell.Properties.Resources.Copy);
            CommandbarButton commandbarButton = mainForm.GetReferenceEditorElectronicLocationsCommandbarManager().GetCommandbar(MainFormReferenceEditorElectronicLocationsCommandbarId.Toolbar).GetCommandbarMenu(MainFormReferenceEditorElectronicLocationsCommandbarMenuId.Tools)
                .AddCommandbarButton("CopyLocationPathToClipboard", CopyLocationPathToClipboardLocalizations.CopyLocationPathToClipboard, CommandbarItemStyle.ImageAndText, SwissAcademic.Citavi.Shell.Properties.Resources.Copy);

            base.OnHostingFormLoaded(hostingForm);
        }

        // 当用户点击插件添加的按钮时，此方法被调用
        protected override void OnBeforePerformingCommand(SwissAcademic.Controls.BeforePerformingCommandEventArgs e)
        {
            // 检查被点击的按钮是否是我们想要的那个
            switch (e.Key)
            {
                case "CopyLocationPathToClipboard":
                    {
                        // 是我们的按钮，执行复制操作
                        e.Handled = true; // 告诉Citavi，这个命令我们已经处理了
                        Function.CopyLocationClipboard(); // 调用执行复制的方法
                    }
                    break;
                case "commandbarButton":
                    { // 是我们的按钮，执行复制操作
                        e.Handled = true; // 告诉Citavi，这个命令我们已经处理了
                        Function.CopyLocationClipboard(); // 调用执行复制的方法

                    }
                    break;
                case "CopyLocationPathToClipboard2":
                    { // 是我们的按钮，执行复制操作
                        e.Handled = true; // 告诉Citavi，这个命令我们已经处理了
                        Function.CopyLocationClipboard(); // 调用执行复制的方法

                    }
                    break;
            }
                base.OnBeforePerformingCommand(e);
        }
    }
}
