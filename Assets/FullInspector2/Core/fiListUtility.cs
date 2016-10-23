using System;
using System.Collections;
using System.Collections.Generic;

namespace FullInspector {
    /// <summary>
    /// Helper functions that unify IList operations across arrays and "actual"
    /// lists.
    /// </summary>
    public static class fiListUtility {
        private static T GetDefault<T>() {
            if (typeof(T) == typeof(string))
                return (T)(object)"";
            return default(T);
        }

        public static void Add(ref IList list, Type elementType) {
            if (list.GetType().IsArray) {
                Array arr = Array.CreateInstance(elementType, list.Count + 1);
                for (int i = 0; i < list.Count; ++i)
                    arr.SetValue(list[i], i);
                list = arr;
            }
            else {
                UnityEngine.Debug.Log("Adding " + elementType + " to " + list);
                list.Add(InspectedType.Get(elementType).CreateInstance());
            }
        }

        public static void RemoveAt(ref IList list, Type elementType, int index) {
            if (list.GetType().IsArray) {
                Array arr = Array.CreateInstance(elementType, list.Count - 1);
                int j = 0;
                for (int i = 0; i < list.Count - 1; ++i) {
                    if (i == index)
                        continue;
                    arr.SetValue(list[i], j++);
                }
                list = arr;
            }
            else {
                list.RemoveAt(index);
            }
        }

        public static void Add<T>(ref IList list) {
            if (list.GetType().IsArray) {
                T[] arr = (T[])list;
                Array.Resize(ref arr, arr.Length + 1);
                list = arr;
            }
            else {
                list.Add(GetDefault<T>());
            }
        }

        public static void InsertAt<T>(ref IList list, int index) {
            if (list.GetType().IsArray) {
                var wrappedList = new List<T>((IList<T>)list);
                wrappedList.Insert(index, GetDefault<T>());
                list = wrappedList.ToArray();
            }
            else {
                list.Insert(index, GetDefault<T>());
            }
        }

        public static void RemoveAt<T>(ref IList list, int index) {
            if (list.GetType().IsArray) {
                var wrappedList = new List<T>((IList<T>)list);
                wrappedList.RemoveAt(index);
                list = wrappedList.ToArray();
            }
            else {
                list.RemoveAt(index);
            }
        }
    }
}