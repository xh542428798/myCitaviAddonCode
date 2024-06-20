using Infragistics.Win.UltraWinToolbars;
using SwissAcademic.Controls;
using SwissAcademic.Drawing;
using System;
using System.Windows.Forms;

namespace SwissAcademic.Citavi.Shell
{
    partial class CitaviAddOnEx<TFormBase>
    {
        private void ObserveApplication(bool start)
        {
            if (IsUnSupportedAddonHostingForm)
            {
                if (start)
                {
                    Application.OpenForms.AddListChangedEventHandler(Forms_Added, ListChangedType.Added);
                    Application.Idle += Application_Idle;
                    Application.ApplicationExit += Application_Exit;
                    Program.Engine.SettingsChanged += Engine_SettingsChanged;
                }
                else
                {
                    Application.OpenForms.RemoveListChangedEventHandler(Forms_Added, ListChangedType.Added);
                    Application.Idle -= Application_Idle;
                    Application.ApplicationExit -= Application_Exit;
                    Program.Engine.SettingsChanged -= Engine_SettingsChanged;
                }
            }
        }

        private void ObserveForm(Form form, bool start)
        {
            if (start)
            {
                form.FormClosed += Forms_Closed;
            }
            else
            {
                form.FormClosed -= Forms_Closed;
            }
        }

        private void ObserveProject(Project project, bool start)
        {
            try
            {
                if (project == null || project.IsDisposed) return;

                if (start)
                {
                    project.SettingsChanged += Project_SettingsChanged;
                }
                else
                {
                    project.SettingsChanged -= Project_SettingsChanged;
                }
            }
            catch (Exception)
            {
            }
        }

        private void ChangedToolClickHandler(TFormBase tFormBase)
        {
            var toolbarsManager = tFormBase.GetToolbarsManager();
            var registredDelegates = toolbarsManager?.RemoveEventHandlersFromEvent("ToolClick");
            ToolClickEventHandler clickEventHandler = (sender, args) =>
            {
                var e = new BeforePerformingCommandEventArgs(tFormBase, args.Tool.Key, args.ListToolItem?.Key, null);
                OnBeforePerformingCommand(tFormBase, e);
                if (!e.Handled)
                {
                    foreach (var del in registredDelegates)
                    {
                        del.DynamicInvoke(sender, args);
                    }
                }
                OnAfterPerformingCommand(tFormBase, AfterPerformingCommandEventArgs.Of(e));
            };

            toolbarsManager?.AddEventHandlerForEvent("ToolClick", clickEventHandler);
        }

        public virtual void OnApplicationIdle(TFormBase form)
        {
        }

        public virtual void OnAfterPerformingCommand(TFormBase form, AfterPerformingCommandEventArgs e)
        {
        }

        public virtual void OnBeforePerformingCommand(TFormBase form, BeforePerformingCommandEventArgs e)
        {
        }

        public virtual void OnChangingColorScheme(TFormBase form, ColorScheme colorScheme)
        {
        }

        public virtual void OnHostingFormLoaded(TFormBase form)
        {
        }

        public virtual void OnHostingFormClosed(TFormBase form)
        {
        }

        public virtual void OnLocalizing(TFormBase form)
        {
        }
    }
}
