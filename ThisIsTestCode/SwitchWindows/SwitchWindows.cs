using System;
using System.Collections.Generic;
using System.Windows.Forms;

using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Shell;
using SwissAcademic.Controls;
using System.Runtime.InteropServices;
using System.Threading;

namespace SwitchWindowsAddon
{
    public class SwitchWindows : CitaviAddOn<MainForm>
    {
        private const int WM_HOTKEY = 0x0312;
        private const int MOD_ALT = 0x0001;
        private const int MOD_SHIFT = 0x0004;
        private const int VK_W = 0x57;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private Thread hotkeyThread;
        private bool isRunning;
        private ManualResetEvent hotkeyStopEvent;
        private bool hotkeyPaused;

        public override void OnHostingFormLoaded(MainForm mainForm)
        {
            var button = mainForm.GetMainCommandbarManager().GetReferenceEditorCommandbar(MainFormReferenceEditorCommandbarId.Menu).GetCommandbarMenu(MainFormReferenceEditorCommandbarMenuId.Window).InsertCommandbarButton(4, "Switch to table view", "Switch active window to table view");
            button.Shortcut = (System.Windows.Forms.Shortcut)(System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.W);

            hotkeyStopEvent = new ManualResetEvent(false);
            // Start the hotkey thread
            hotkeyThread = new Thread(HotkeyThreadMethod);
            hotkeyThread.Start();
        }

        public virtual void OnHostingFormClosed(MainForm mainForm)
        {
            // Stop the hotkey thread when the hosting form is closed
            isRunning = false;
            hotkeyStopEvent.Set();

            if (hotkeyThread != null)
            {
                hotkeyThread.Join();
                hotkeyThread = null;
            }

            // Release the stop event
            hotkeyStopEvent.Dispose();
            hotkeyStopEvent = null;
        }

        private void HotkeyThreadMethod()
        {
            // Register the hotkey with the system
            RegisterHotKey(IntPtr.Zero, 1, MOD_ALT | MOD_SHIFT, VK_W);

            isRunning = true;
            while (isRunning)
            {
                if (!hotkeyPaused && PeekMessage(out var msg, IntPtr.Zero, WM_HOTKEY, WM_HOTKEY, 0))
                {
                    // The hotkey was pressed, execute your code here
                    //MainForm form = Program.ActiveProjectShell.PrimaryMainForm;

                    List<string> formNameList = new List<string>();
                    for (int i = 0; i < Application.OpenForms.Count; i++)
                    {
                        Form myform = Application.OpenForms[i];
                        if (!myform.Visible) continue;
                        formNameList.Add(myform.Name);
                        //form.Activate();
                        //MessageBox.Show(myform.Name);
                    }

                    // Get the active form
                    Form myActivaform = Program.ActiveProjectShell.ActiveForm;
                    //MessageBox.Show(myActivaform.Name);
                    // Set the next form to be activated
                    int currentIndex = formNameList.IndexOf(myActivaform.Name);
                    int nextIndex = (currentIndex + 1) % formNameList.Count; // Get the index of the next form in a circular manner
                    string nextFormName = formNameList[nextIndex];
                    // Activate the next form
                    Form nextForm = Application.OpenForms[nextFormName];

                    // Pause hotkey listening
                    hotkeyPaused = true;

                    // Bring the next form to the front
                    // Bring the next form to the front
                    if (!nextForm.IsDisposed)
                    {
                        nextForm.Invoke(new Action(() => nextForm.BringToFront()));
                    }

                    // Resume hotkey listening
                    hotkeyPaused = false;

                    // Consume all remaining hotkey messages
                    while (PeekMessage(out msg, IntPtr.Zero, WM_HOTKEY, WM_HOTKEY, 0))
                    {
                        // Do nothing, just consume the message
                    }
                }

                // Wait for the stop event or the next message
                hotkeyStopEvent.WaitOne(100);
            }

            // Unregister the hotkey when the thread is stopped
            UnregisterHotKey(IntPtr.Zero, 1);
        }

        [DllImport("user32.dll")]
        private static extern bool PeekMessage(out Message msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);
    }
}
