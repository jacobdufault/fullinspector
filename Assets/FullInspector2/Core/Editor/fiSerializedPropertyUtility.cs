using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Internal {
    public class fiSerializedPropertyUtility {
        public static object ReadFieldOrProperty(object obj, string name) {
            // TODO: can we use/add fiRuntimeReflectionUtility.ReadField<>()?
            // + add support for properties, rename to ReadMember?

            // We cannot use BindingFlags.FlattenHierarchy because that does
            // *not* include private members in the parent type. Instead, we scan
            // fields/properties for each inheritance level which *will* include
            // private members on parent types.

            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var type = obj.GetType();

            while (type != null) {
                var field = type.GetField(name, flags);
                if (field != null) {
                    return field.GetValue(obj);
                }

                var prop = type.GetProperty(name, flags);
                if (prop != null) {
                    return prop.GetValue(obj, null);
                }

                type = type.BaseType;
            }

            return null;
        }

        public static MemberInfo GetFieldOrProperty(Type type, string name) {
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            while (type != null) {
                var field = type.GetField(name, flags);
                if (field != null) {
                    return field;
                }

                var prop = type.GetProperty(name, flags);
                if (prop != null) {
                    return prop;
                }

                type = type.BaseType;
            }

            return null;
        }

        public static void WriteFieldOrProperty(object obj, string name, object value) {
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var type = obj.GetType();

            while (type != null) {
                var field = type.GetField(name, flags);
                if (field != null) {
                    field.SetValue(obj, value);
                    break;
                }

                var prop = type.GetProperty(name, flags);
                if (prop != null) {
                    prop.SetValue(obj, value, null);
                    break;
                }

                type = type.BaseType;
            }
        }

        private static object ReadArrayIndex(object obj, int index) {
            var list = (IList)obj;
            if (index >= list.Count) return null;
            return list[index];
        }
        private static void WriteArrayIndex(object obj, int index, object value) {
            var list = (IList)obj;
            if (index >= 0 && index < list.Count) {
                list[index] = value;
            }
        }

        public static void WriteTarget(SerializedProperty property, object value) {
            object context = property.serializedObject.targetObject;

            string[] names = property.propertyPath.Replace("Array.data", "").Split('.');
            for (int i = 0; i < names.Length - 1; ++i) {
                string name = names[i];

                // array
                if (name[0] == '[') {
                    name = name.Substring(1);
                    name = name.Remove(name.Length - 1);
                    int index = int.Parse(name);
                    context = ReadArrayIndex(context, index);
                }

                // member
                else {
                    context = ReadFieldOrProperty(context, name);
                }
            }

            var last = names[names.Length - 1];
            if (last[0] == '[') {
                string idx = last.Substring(1);
                idx = idx.Remove(idx.Length - 1);
                int index = int.Parse(idx);
                WriteArrayIndex(context, index, value);
            }
            else {
                WriteFieldOrProperty(context, last, value);
            }
        }

        /// <summary>
        /// Returns the object that this serialized property is currently
        /// storing.
        /// </summary>
        public static object GetTarget(SerializedProperty property) {
            object result = property.serializedObject.targetObject;

            string[] names = property.propertyPath.Replace("Array.data", "").Split('.');
            for (int i = 0; i < names.Length; ++i) {
                string name = names[i];

                // array
                if (name[0] == '[') {
                    name = name.Substring(1);
                    name = name.Remove(name.Length - 1);
                    int index = int.Parse(name);
                    result = ReadArrayIndex(result, index);
                }

                // member
                else {
                    result = ReadFieldOrProperty(result, name);
                }

                // reading the property from reflection failed for some reason --
                // we have to return null
                if (result == null) return null;
            }

            return result;
        }

        public static void RevertPrefabContextMenu(Rect region, SerializedProperty property) {
            if (Event.current.type == EventType.ContextClick &&
                region.Contains(Event.current.mousePosition) &&
                property.prefabOverride) {
                Event.current.Use();

                var content = new GUIContent("Revert Value to Prefab");

                GenericMenu menu = new GenericMenu();
                menu.AddItem(content, /*on:*/false, () => {
                    PropertyModification[] fixedMods = PrefabUtility.GetPropertyModifications(property.serializedObject.targetObject);
                    for (int i = 0; i < fixedMods.Length; ++i) {
                        if (fixedMods[i].propertyPath.StartsWith(property.propertyPath)) {
                            ArrayUtility.RemoveAt(ref fixedMods, i);
                        }
                    }

                    PrefabUtility.SetPropertyModifications(property.serializedObject.targetObject, fixedMods);
                });
                menu.ShowAsContext();
            }
        }

        public static fiGraphMetadataChild GetMetadata(SerializedProperty property) {
            return fiPersistentMetadata.GetMetadataFor(property.serializedObject.targetObject).Enter(property.propertyPath, null);
        }
    }
}