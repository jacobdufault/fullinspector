using System;
using System.Collections;
using System.Collections.Generic;

namespace FullInspector {
    public static class fiGraphMetadataCallbacks {

        public static IList Cast<T>(IList<T> list) {
            if (list is IList) return (IList)list;
            return new ListWrapper<T>(list);
        }

        // IList<T> does not extend from IList but we need to
        // cast from IList<T> types to IList. If the IList<T>
        // does not extend IList, then we just use a proxy
        // object instead that forwards all of the IList
        // calls directly to the IList<T> instance.
        private sealed class ListWrapper<T> : IList {
            private readonly IList<T> _list;

            public ListWrapper(IList<T> list) {
                _list = list;
            }

            public int Add(object value) {
                _list.Add((T)value);
                return _list.Count - 1;
            }

            public void Clear() {
                _list.Clear();
            }

            public bool Contains(object value) {
                return _list.Contains((T)value);
            }

            public int IndexOf(object value) {
                return _list.IndexOf((T)value);
            }

            public void Insert(int index, object value) {
                _list.Insert(index, (T)value);
            }

            public bool IsFixedSize {
                get { return false; }
            }

            public bool IsReadOnly {
                get { return _list.IsReadOnly; }
            }

            public void Remove(object value) {
                _list.Remove((T)value);
            }

            public void RemoveAt(int index) {
                _list.RemoveAt(index);
            }

            public object this[int index] {
                get { return _list[index]; }
                set { _list[index] = (T)value; }
            }

            public void CopyTo(Array array, int index) {
                _list.CopyTo((T[])array, index);
            }

            public int Count {
                get { return _list.Count; }
            }

            public bool IsSynchronized {
                get { return false; }
            }

            public object SyncRoot {
                get { return this; }
            }

            public IEnumerator GetEnumerator() {
                return _list.GetEnumerator();
            }
        }

        public static Action<fiGraphMetadata, IList, int> ListMetadataCallback = (m, l, i) => { };
        public static Action<fiGraphMetadata, InspectedProperty> PropertyMetadataCallback = (m, p) => { };
    }
}