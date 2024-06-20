using System;
using System.Collections;

namespace SwissAcademic.Citavi.Shell
{
    internal delegate void ListChangedEventHandler(object sender, ListChangedEventArgs args);

    internal class ObservableArrayList : ArrayList
    {
        // Events

        public event ListChangedEventHandler Added;

        protected void OnAdded(ListChangedEventArgs args) => Added?.Invoke(this, args);

        public event ListChangedEventHandler Removed;

        protected void OnRemoved(ListChangedEventArgs args) => Removed?.Invoke(this, args);

        // Methods

        public override int Add(object value)
        {
            var result = base.Add(value);
            OnAdded(new ListChangedEventArgs(ListChangedType.Added, value));
            return result;
        }

        public override void AddRange(ICollection objects)
        {
            base.AddRange(objects);
            OnAdded(new ListChangedEventArgs(ListChangedType.Added, objects)); ;
        }

        public override void Remove(object value)
        {
            base.Remove(value);
            OnRemoved(new ListChangedEventArgs(ListChangedType.Removed, value));
        }

        public override void RemoveRange(int index, int count)
        {
            var objects = GetRange(index, count);
            base.RemoveRange(index, count);
            OnRemoved(new ListChangedEventArgs(ListChangedType.Removed, objects));
        }
    }

    internal class ListChangedEventArgs : EventArgs
    {
        // Constructors

        private ListChangedEventArgs()
        {
            ChangedType = ListChangedType.Unknown;
            Forms = null;
        }

        public ListChangedEventArgs(ListChangedType changedType, params object[] items)
        {
            ChangedType = changedType;
            Forms = items;
        }

        // Properties

        public new ListChangedEventArgs Empty => new ListChangedEventArgs();

        public ListChangedType ChangedType { get; }

        public object[] Forms { get; }
    }

    public enum ListChangedType
    {
        Added,
        Removed,
        Unknown
    }
}