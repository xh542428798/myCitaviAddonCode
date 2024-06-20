using System.Collections;
using System.Reflection;
using System.Windows.Forms;

namespace SwissAcademic.Citavi.Shell
{
    internal static class FormsCollectionExt
    {
        // Fields

        private static readonly BindingFlags fieldBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

        // Methods

        public static void AddListChangedEventHandler(this FormCollection collection, ListChangedEventHandler eventHandler, ListChangedType changedType)
        {
            var innerListlistFieldInfo = collection.GetType().BaseType.GetField("list", fieldBindingFlags);
            var innerlist = innerListlistFieldInfo?.GetValue(Application.OpenForms);

            if (!(innerlist is ObservableArrayList))
            {
                var newInnerList = new ObservableArrayList();

                foreach (var item in innerlist as ArrayList)
                {
                    newInnerList.Add(item);
                }

                innerListlistFieldInfo.SetValue(Application.OpenForms, newInnerList);
            }

            if (innerListlistFieldInfo?.GetValue(Application.OpenForms) is ObservableArrayList currentInnerList)
            {
                switch (changedType)
                {
                    case ListChangedType.Added:
                        currentInnerList.Added += eventHandler;
                        break;

                    case ListChangedType.Removed:
                        currentInnerList.Removed += eventHandler;
                        break;
                }
            }
        }

        public static void RemoveListChangedEventHandler(this FormCollection collection, ListChangedEventHandler eventHandler, ListChangedType changedType)
        {
            var innerListlistFieldInfo = collection.GetType().BaseType.GetField("list", fieldBindingFlags);
            var innerlist = innerListlistFieldInfo?.GetValue(Application.OpenForms);

            if (!(innerlist is ObservableArrayList))
            {
                var newInnerList = new ObservableArrayList();
                foreach (var item in innerlist as ArrayList)
                {
                    newInnerList.Add(item);
                }

                innerListlistFieldInfo.SetValue(Application.OpenForms, newInnerList);
            }

            if (innerListlistFieldInfo?.GetValue(Application.OpenForms) is ObservableArrayList currentInnerList)
            {
                switch (changedType)
                {
                    case ListChangedType.Added:
                        currentInnerList.Added -= eventHandler;
                        break;

                    case ListChangedType.Removed:
                        currentInnerList.Removed -= eventHandler;
                        break;
                }
            }
        }
    }
}