using SwissAcademic.Controls;
using SwissAcademic.Drawing;
using System;
using System.Windows.Forms;

namespace SwissAcademic.Citavi.Shell
{
    public abstract partial class CitaviAddOnEx<TFormBase>
    {
        // Properties

        public sealed override AddOnHostingForm HostingForm => Enum.TryParse(typeof(TFormBase).Name, true, out AddOnHostingForm addOnHostingForm)
                                                               ? addOnHostingForm
                                                               : AddOnHostingForm.None;
        // Methods

        sealed protected override void OnApplicationIdle(Form form)
        {
            if (form is TFormBase tFormBase)
            {
                OnApplicationIdle(tFormBase);
            }
        }

        sealed protected override void OnBeforePerformingCommand(BeforePerformingCommandEventArgs args)
        {
            if (args.Form is TFormBase tFormBase)
            {
                OnBeforePerformingCommand(tFormBase, args);
            }
        }

        sealed protected override void OnChangingColorScheme(Form form, ColorScheme colorScheme)
        {
            if (form is TFormBase tFormBase)
            {
                OnChangingColorScheme(tFormBase, colorScheme);
            }
        }

        sealed protected override void OnHostingFormLoaded(Form form)
        {
            if (form is TFormBase tFormBase)
            {
                OnHostingFormLoaded(tFormBase);
                form.FormClosed += Form_FormClosed;
            }
        }

        sealed protected override void OnLocalizing(Form form)
        {
            if (form is TFormBase tFormBase)
            {
                OnLocalizing(tFormBase);
            }
        }

        private void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sender is Form form)
            {
                form.FormClosed -= Form_FormClosed;
            }

            if (sender is TFormBase tFormBase)
            {
                OnHostingFormClosed(tFormBase);
            }
        }
    }
}