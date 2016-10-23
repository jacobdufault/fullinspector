using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    /// <summary>
    /// Utility functions for working with prefab overrides.
    /// </summary>
    public static class fiPrefabTools {
        /// <summary>
        /// Returns true if the period separate property path contains the given
        /// property name.
        /// </summary>
        private static bool ContainsPropertyName(string propertyPath,
                                                 string propertyName) {
            string[] paths = propertyPath.Split('.');
            for (int i = 0; i < paths.Length; ++i) {
                if (paths[i] == propertyName) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to extract the name of serialized key for the given property
        /// modification.
        /// </summary>
        /// <param name="obj">
        /// The object that that modification is applied to.
        /// </param>
        /// <param name="mod">The modification.</param>
        /// <param name="keyName">
        /// An output parameter containing the name of the key that the
        /// modification maps to.
        /// </param>
        /// <returns>True if the key was found, false otherwise.</returns>
        private static bool TryExtractPropertyName(
            ISerializedObject obj,
            PropertyModification mod,
            out string keyName) {
            //-
            // We want to extract 2 from _serializedStateValues.Array.data[2]
            // We could probably use a regular expression, but this is fine for now
            if (mod.propertyPath.StartsWith("_serializedStateValues.Array.data[")) {
                string front = mod.propertyPath.Remove(0, "_serializedStateValues.Array.data".Length + 1);
                string num = front.Substring(0, front.Length - 1);

                int index;
                if (int.TryParse(num, out index) &&
                    index >= 0 && index < obj.SerializedStateKeys.Count) {
                    keyName = obj.SerializedStateKeys[index];
                    return true;
                }
            }

            keyName = string.Empty;
            return false;
        }

        /// <summary>
        /// Reverts the given property on the instance to the prefab value.
        /// </summary>
        /// <param name="instance">
        /// The prefab instance to revert the value on.
        /// </param>
        /// <param name="property">The property to revert.</param>
        public static void RevertValue(object instance,
                                       InspectedProperty property) {
            // We only want top-level components
            if (instance is MonoBehaviour == false) {
                return;
            }

            // Not a prefab
            var prefabGameObject = (GameObject)PrefabUtility.GetPrefabParent(((MonoBehaviour)instance).gameObject);
            if (prefabGameObject == null) {
                return;
            }

            // Get all of the property modifications on the object. If there are
            // no property modifications, then there is nothing to revert.
            PropertyModification[] mods = PrefabUtility.GetPropertyModifications((UnityObject)instance);
            if (mods == null) {
                return;
            }

            ISerializedObject serializedInstance = (ISerializedObject)instance;

            bool removed = false;

            for (int i = 0; i < mods.Length; ++i) {
                PropertyModification mod = mods[i];

                // A property modification can take one of two forms. It can
                // either be modifying a Unity serialized value or a Full
                // Inspector serialized value.

                // Check to see if it's a Full Inspector serialized value. If it
                // is, then we lookup the key that the modification is associated
                // with and, if we find said key, and that the key is equal to
                // the property we are checking for, then we return true.
                string serializedPropertyName;
                if (TryExtractPropertyName(serializedInstance, mod, out serializedPropertyName) &&
                    serializedPropertyName == property.Name) {
                    removed = true;
                }

                // Check to see if it is a Unity serialized value. We have to do
                // a dotted comparison because the propertyPath may be associated
                // with, ie, an array, which in that case the path is something
                // like "values.Array._items[0]" while property.Name is just
                // "values".
                if (ContainsPropertyName(mod.propertyPath, property.Name)) {
                    removed = true;
                }

                if (removed) {
                    ArrayUtility.RemoveAt(ref mods, i);
                    PrefabUtility.SetPropertyModifications((UnityObject)instance, mods);
                    break;
                }
            }
        }

        /// <summary>
        /// Returns true if the given property on the given object instance has a
        /// prefab override.
        /// </summary>
        /// <param name="instance">The object instance.</param>
        /// <param name="property">The property to check.</param>
        /// <returns>
        /// True if the property is prefab override, false otherwise.
        /// </returns>
        /// <remarks>
        /// Currently, this method only works for MonoBehavior targets.
        /// </remarks>
        public static bool HasPrefabDiff(object instance, InspectedProperty property) {
            // For prefab differences, we rely upon the internal Unity mechanisms
            // for identifying when an object has a prefab diff. We are able to
            // do this because we only support top-level prefab differences.
            //
            // One of the current issues with this mechanism is when an array is
            // serialized by Unity, and only part of the array tracks the prefab,
            // then the inspector will show the entire array in bold (when only
            // the one part should be).

            // We only want top-level components
            if (instance is MonoBehaviour == false) {
                return false;
            }

            // If there is no prefab, then we don't show anything in bold.
            var prefabGameObject = (GameObject)PrefabUtility.GetPrefabParent(((MonoBehaviour)instance).gameObject);
            if (prefabGameObject == null) {
                return false;
            }

            // If the prefab doesn't have this component, then the entire
            // component should be in bold.
            var prefab = prefabGameObject.GetComponent(instance.GetType());
            if (prefab == null) {
                return true;
            }

            // Get all of the property modifications on the object. If there are
            // no property modifications, then nothing should be in bold.
            PropertyModification[] mods = PrefabUtility.GetPropertyModifications((UnityObject)instance);
            if (mods == null) {
                return false;
            }

            var serializedInstance = instance as ISerializedObject;
            if (serializedInstance != null) {
                foreach (PropertyModification mod in mods) {
                    // A property modification can take one of two forms. It can
                    // either be modifying a Unity serialized value or a Full
                    // Inspector serialized value.

                    // Check to see if it's a Full Inspector serialized value. If
                    // it is, then we lookup the key that the modification is
                    // associated with and, if we find said key, and that the key
                    // is equal to the property we are checking for, then we
                    // return true.
                    string serializedPropertyName;
                    if (TryExtractPropertyName(serializedInstance, mod, out serializedPropertyName) &&
                        serializedPropertyName == property.Name) {
                        return true;
                    }

                    // Check to see if it is a Unity serialized value. We have to
                    // do a dotted comparison because the propertyPath may be
                    // associated with, ie, an array, which in that case the path
                    // is something like "values.Array._items[0]" while
                    // property.Name is just "values".
                    if (ContainsPropertyName(mod.propertyPath, property.Name)) {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}