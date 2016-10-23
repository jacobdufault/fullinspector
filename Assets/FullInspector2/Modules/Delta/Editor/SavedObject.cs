using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FullSerializer;
using FullSerializer.Internal;
using UnityEngine;

namespace FullInspector.Internal {
    public struct SavedObject {
        public SavedObject(object o) {
            state = Create(o);
        }

        public Dictionary<ObjectDataPath[], object> state;

        /// <summary>
        /// Returns true if the given object should be saved directly inside of
        /// the state dictionary.
        /// </summary>
        private static bool IsPrimitiveValue(object obj) {
            return obj == null ||
                   obj.GetType() == typeof(string) ||
                   obj.GetType().Resolve().IsPrimitive ||
                   obj.GetType().Resolve().IsEnum;
        }

        private static Dictionary<ObjectDataPath[], object> AllocateDict() {
            return new Dictionary<ObjectDataPath[], object>(
                new ObjectDataPath.EqualityComparer());
        }

        public static Dictionary<ObjectDataPath[], object> Create(object obj) {
            // TODO: Use ObjectReferenceEqualityComparator.Instance when it works
            //       with structs
            var seenObjects = new HashSet<object>();
            return Create(new List<ObjectDataPath>(), obj, seenObjects);
        }

        // TODO: Fix
        //       transform.position.normalized.normalized.normalized.normalized.normalized...
        public static Dictionary<ObjectDataPath[], object> CreateWithPath(List<ObjectDataPath> toUs, ObjectDataPath nav, object obj, HashSet<object> seenObjects) {
            toUs.Add(nav);
            var result = Create(toUs, obj, seenObjects);
            toUs.RemoveAt(toUs.Count - 1);
            return result;
        }

        private static ObjectDataPath[] CreateNavigation(List<ObjectDataPath> history, ObjectDataPath finalNav) {
            var result = new ObjectDataPath[history.Count + 1];
            for (int i = 0; i < history.Count; ++i)
                result[i] = history[i];
            result[result.Length - 1] = finalNav;
            return result;
        }

        private class ObjectReferenceEqualityComparator : IEqualityComparer<object> {
            bool IEqualityComparer<object>.Equals(object x, object y) {
                return ReferenceEquals(x, y);
            }

            int IEqualityComparer<object>.GetHashCode(object obj) {
                return RuntimeHelpers.GetHashCode(obj);
            }

            public static readonly IEqualityComparer<object> Instance = new ObjectReferenceEqualityComparator();
        }

        public static Dictionary<ObjectDataPath[], object> Create(List<ObjectDataPath> toUs, object obj, HashSet<object> seenObjects) {
            /*
            Debug.Log(string.Join(", ", toUs.Select(t => {
                if (t.byProperty == null) return "";
                return t.byProperty.Name;
            }).ToArray()));
            Debug.Log("seenObjects.Count = " + seenObjects.Count);
            Debug.Log("seenObjects.Contains " + obj + " = " + seenObjects.Contains(obj));
            */

            Dictionary<ObjectDataPath[], object> thisLevel = AllocateDict();

            if (seenObjects.Add(obj) == false)
                return thisLevel;

            // Write the type.
            thisLevel.Add(CreateNavigation(toUs, new ObjectDataPath(toUs.ToArray())), obj.GetType());

            List<InspectedProperty> properties = InspectedType.Get(obj.GetType()).GetProperties(InspectedMemberFilters.InspectableMembers);
            for (int i = 0; i < properties.Count; ++i) {
                InspectedProperty property = properties[i];
                object valueForProperty = property.Read(obj);

                if (IsPrimitiveValue(valueForProperty)) {
                    thisLevel.Add(CreateNavigation(toUs, new ObjectDataPath(property)), valueForProperty);
                    continue;
                }

                if (InspectedType.Get(property.StorageType).IsCollection) {
                    if (valueForProperty is IList) {
                        var listForProperty = (IList)valueForProperty;
                        for (int j = 0; j < listForProperty.Count; ++j) {
                            object listElement = listForProperty[j];

                            if (IsPrimitiveValue(listElement)) {
                                thisLevel.Add(CreateNavigation(toUs, new ObjectDataPath(property, j)), listElement);
                                continue;
                            }

                            var listChildValues = CreateWithPath(toUs, new ObjectDataPath(property, j), listElement, seenObjects);
                            foreach (var entry in listChildValues)
                                thisLevel.Add(entry.Key, entry.Value);
                        }
                    }
                    else if (valueForProperty is IDictionary) {
                        var dictForProperty = (IDictionary)valueForProperty;
                        IDictionaryEnumerator enumerator = dictForProperty.GetEnumerator();
                        while (enumerator.MoveNext()) {
                            object key = enumerator.Key;
                            object value = enumerator.Value;

                            if (value == null || value.GetType().IsPrimitive || value is string) {
                                thisLevel.Add(CreateNavigation(toUs, new ObjectDataPath(property, key)), value);
                            }
                            else {
                                toUs.Add(new ObjectDataPath(property, key));
                                var dictChildValues = Create(toUs, value, seenObjects);
                                toUs.RemoveAt(toUs.Count - 1);
                                foreach (var entry in dictChildValues)
                                    thisLevel.Add(entry.Key, entry.Value);
                            }
                        }
                    }
                    else {
                        Debug.LogError("Please file a bug (with an example) requesting multiedit support for " + valueForProperty.GetType().CSharpName());
                    }
                    continue;
                }

                // Navigate children.
                var childValues = CreateWithPath(toUs, new ObjectDataPath(property), valueForProperty, seenObjects);
                foreach (var entry in childValues)
                    thisLevel.Add(entry.Key, entry.Value);
            }

            return thisLevel;
        }
    }
}