using SwissAcademic.Controls;
using System.Windows.Forms;

namespace SwissAcademic.Citavi.Shell
{
    public class AfterPerformingCommandEventArgs
    {
        // Constructors

        public AfterPerformingCommandEventArgs(Form form, string key, string listItemKey, object tag)
        {
            Form = form;
            Key = key;
            ListItemKey = listItemKey;
            Tag = tag;
        }

        // Properties

        public Form Form { get; }

        public string ListItemKey { get; }
        public string Key { get; }
        public object Tag { get; }

        // Methods

        public static AfterPerformingCommandEventArgs Of(BeforePerformingCommandEventArgs beforePerformingCommandEventArgs) => new AfterPerformingCommandEventArgs(beforePerformingCommandEventArgs.Form, beforePerformingCommandEventArgs.Key, beforePerformingCommandEventArgs.ListItemKey, beforePerformingCommandEventArgs.Tag);
    }
}
