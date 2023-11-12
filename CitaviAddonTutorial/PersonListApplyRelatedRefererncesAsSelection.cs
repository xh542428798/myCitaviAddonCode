using SwissAcademic.Citavi.Shell;
using SwissAcademic.Citavi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitaviAddonTest
{
    public class PersonListApplyRelatedRefererncesAsSelection
                :
        CitaviAddOn
    {
        public override AddOnHostingForm HostingForm
        {
            get { return AddOnHostingForm.PersonList; }
        }

        protected override void OnHostingFormLoaded(System.Windows.Forms.Form hostingForm)
        {
            var personList = (PersonList)hostingForm;

            var menuEntry = personList.GetCommandbar(PersonListCommandbarId.Menu).GetCommandbarMenu(PersonListCommandbarMenuId.Persons).AddCommandbarButton("ApplyRelatedReferencesAsSelection", "Apply related references as selection");
            menuEntry.HasSeparator = true;
            menuEntry.Image = SwissAcademic.Citavi.Shell.Properties.Resources.Filter;

            base.OnHostingFormLoaded(hostingForm);
        }

        protected override void OnBeforePerformingCommand(SwissAcademic.Controls.BeforePerformingCommandEventArgs e)
        {
            switch (e.Key)
            {
                case "ApplyRelatedReferencesAsSelection":
                    {
                        e.Handled = true;

                        var personList = (PersonList)e.Form;

                        var references = new List<Reference>();

                        foreach (var person in personList.GetSelectedPersons())
                        {
                            foreach (var reference in person.References)
                            {
                                if (!references.Contains(reference))
                                {
                                    references.Add(reference);
                                }
                            }
                        }

                        var mainForm = personList.ProjectShell.PrimaryMainForm;

                        var referenceFilter = new ReferenceFilter(references, "List of persons");
                        var filters = new List<ReferenceFilter> { referenceFilter };

                        mainForm.ReferenceEditorFilterSet.Filters.ReplaceBy(filters);

                        Program.ActivateForm(mainForm);
                    }
                    break;
            }

            base.OnBeforePerformingCommand(e);
        }
    }
}
