using SwissAcademic.Citavi.Shell;
using SwissAcademic.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

// Addon.cs
using System.Reflection;
using System.Windows.Forms;

namespace AutoRef
{
    public class Addon : CitaviAddOnEx<MacroEditorForm>
    {
        private MacroEditorForm _trackedDefaultEditor;

        public override void OnHostingFormLoaded(MacroEditorForm macroEditorForm)
        {
            // 为了方便调试，我们直接在这里启动一个高频检查
            StartHighFrequencyTracking();
        }

        private void StartHighFrequencyTracking()
        {
            var checkTimer = new Timer();
            checkTimer.Interval = 100; // 提高检查频率到100毫秒
            checkTimer.Tick += (s, e) =>
            {
                // 不需要停止，让它一直运行
                CheckDefaultEditor();
            };
            checkTimer.Start();
        }

        private void CheckDefaultEditor()
        {
            var programType = typeof(Program);
            var defaultEditorField = programType.GetField("_macroEditor", BindingFlags.Static | BindingFlags.NonPublic);

            if (defaultEditorField != null)
            {
                var currentDefaultEditor = defaultEditorField.GetValue(null) as MacroEditorForm;

                // --- 开始侦探模式 ---
                // 打印出每次检查的所有状态
                var currentInfo = currentDefaultEditor == null ? "NULL" : $"Text='{currentDefaultEditor.Text}', Visible={currentDefaultEditor.Visible}";
                var trackedInfo = _trackedDefaultEditor == null ? "NULL" : $"Text='{_trackedDefaultEditor.Text}', Visible={_trackedDefaultEditor.Visible}";

                // 我们只在状态发生变化时打印，避免刷屏
                // 这里用一个简单的静态变量来记录上一次的状态
                var lastInfo = (string)AppDomain.CurrentDomain.GetData("LastEditorInfo");
                if (lastInfo != currentInfo)
                {
                    System.Diagnostics.Debug.WriteLine($"[AutoRef] Current: {currentInfo} | Tracked: {trackedInfo}");
                    AppDomain.CurrentDomain.SetData("LastEditorInfo", currentInfo);
                }
                // --- 侦探模式结束 ---


                // 我们的判断条件
                bool isNewHiddenEditor = !ReferenceEquals(currentDefaultEditor, _trackedDefaultEditor) && currentDefaultEditor != null && !currentDefaultEditor.Visible;

                if (isNewHiddenEditor)
                {
                    System.Diagnostics.Debug.WriteLine($"[AutoRef] !!! 发现新的隐藏编辑器! 准备处理...");

                    _trackedDefaultEditor = currentDefaultEditor;

                    // 核心逻辑：预处理宏代码
                    var timer = new Timer();
                    timer.Interval = 50;
                    timer.Tick += (s, e) =>
                    {
                        timer.Stop();
                        System.Diagnostics.Debug.WriteLine($"[AutoRef] 正在预处理代码...");
                        var processedCode = _trackedDefaultEditor.PreprocessMacroCode();
                        _trackedDefaultEditor.MacroCode = processedCode;
                        System.Diagnostics.Debug.WriteLine($"[AutoRef] 代码预处理完成!");
                    };
                    timer.Start();
                }
            }
        }
    }
}