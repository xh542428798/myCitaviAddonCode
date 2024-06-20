using System;
using System.Linq;
using System.Windows.Forms;

namespace SwissAcademic.Citavi.Shell
{
    partial class CitaviAddOnEx<TFormBase>
    {
        private void Application_Idle(object sender, EventArgs e) => Application.OpenForms.OfType<TFormBase>().ForEach(form => OnApplicationIdle(form));

        private void Application_Exit(object sender, EventArgs e) => ObserveApplication(false);

        private void Engine_SettingsChanged(object sender, SettingsEventArgs e)
        {
            if (e.Name.Equals(nameof(Program.Engine.Settings.General.UICulture), StringComparison.OrdinalIgnoreCase))
            {
                Application.OpenForms.OfType<TFormBase>().ForEach(form => OnLocalizing(form));
            }
        }

        private void Project_SettingsChanged(object sender, ProjectSettingsEventArgs e)
        {
            if (e?.ProjectSettingsType == ProjectSettingsType.ColorScheme && sender is Project project)
            {
                Application.OpenForms.OfType<TFormBase>().Where(form => form.GetProject() == project).ForEach(form => OnChangingColorScheme(form, form.GetProject().ProjectSettings.ColorScheme));
            }
        }

        private void Forms_Added(object sender, ListChangedEventArgs args)
        {
            foreach (var form in args.Forms.OfType<TFormBase>())
            {
                ObserveForm(form, true);
                OnHostingFormLoaded(form);
                ChangedToolClickHandler(form);
                if (form.GetProject() is Project project) ObserveProject(project, true);
            }
        }

        private void Forms_Closed(object sender, FormClosedEventArgs args)
        {
            if (sender is TFormBase tFormBase)
            {
                ObserveForm(tFormBase, false);
                OnHostingFormClosed(tFormBase);
                if (tFormBase.GetProject() is Project project) ObserveProject(project, false);
            }
        }
    }
}
