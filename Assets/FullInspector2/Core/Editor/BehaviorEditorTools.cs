using FullSerializer.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullInspector.Internal {
    public static class BehaviorEditorTools {
        /// <summary>
        /// Returns true if the given dataType matches the given behaviorType. If the dataType is
        /// generic and the behaviorType is a generic definition, then the behaviorType will be
        /// instantiated with the same generic arguments as dataType.
        /// </summary>
        private static bool CanEdit(Type dataType, CustomBehaviorEditorAttribute behaviorTypeAttribute) {
            if (behaviorTypeAttribute == null) {
                return true;
            }

            Type behaviorType = behaviorTypeAttribute.BehaviorType;
            if (dataType.IsGenericType && behaviorType.IsGenericTypeDefinition) {
                // I don't believe this will ever throw, but just in case we wrap it in a try/catch
                // block.
                try {
                    behaviorType = behaviorType.MakeGenericType(dataType.GetGenericArguments());
                }
                catch { }
            }

            return dataType == behaviorType ||
                (behaviorTypeAttribute.Inherit && behaviorType.IsAssignableFrom(dataType));
        }

        /// <summary>
        /// Creates a new instance of the given editorType. It is assumed that editorType extends
        /// IBehaviorEditor.
        /// </summary>
        private static bool TryCreateInstance(Type editorType, Type usedEditedType,
            Type actualEditedType, out IBehaviorEditor editor) {

            if (editorType.GetConstructor(fsPortableReflection.EmptyTypes) == null) {
                Debug.LogWarning("Type " + editorType + " can serve as a behavior editor if it " +
                    "has a default constructor");
                editor = null;
                return false;
            }

            if (CanEdit(usedEditedType,
                fsPortableReflection.GetAttribute<CustomBehaviorEditorAttribute>(editorType)) == false) {

                editor = null;
                return false;
            }

            try {
                editor = (IBehaviorEditor)Activator.CreateInstance(editorType);
            }
            catch (Exception e) {
                Debug.LogException(e);
                editor = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Attempt to create a new IBehaviorEditor instance.
        /// </summary>
        /// <param name="usedEditedType">The data type that is used for comparison with the behavior
        /// editor.</param>
        /// <param name="actualEditedType">The actual data type that that usedEditedType was derived
        /// from.</param>
        /// <param name="editorType">The type of editor we are trying to create.</param>
        /// <returns></returns>
        private static IBehaviorEditor TryCreateSpecificEditor(Type usedEditedType, Type actualEditedType, Type editorType) {
            //-
            //-
            //-
            // the type can be instantiated directly and is non-generic
            if (editorType.IsGenericTypeDefinition == false) {
                IBehaviorEditor editor;
                TryCreateInstance(editorType, usedEditedType, actualEditedType, out editor);
                return editor;
            }

            //-
            //-
            //-
            // we have to construct a closed generic editor type
            else {
                int expectedGenericCount = editorType.GetGenericArguments().Length;

                // Just the edited type itself
                if (expectedGenericCount == 1) {
                    try {
                        Type createdEditor = editorType.MakeGenericType(usedEditedType);
                        IBehaviorEditor editor;
                        if (TryCreateInstance(createdEditor, usedEditedType, actualEditedType, out editor)) {
                            return editor;
                        }
                    }
                    catch { }
                }

                var usedEditTypeGenericArguments = usedEditedType.GetGenericArguments();

                // try the regular generic arguments
                if (expectedGenericCount == usedEditTypeGenericArguments.Length) {
                    try {
                        Type createdEditor = editorType.MakeGenericType(usedEditTypeGenericArguments);
                        IBehaviorEditor editor;
                        if (TryCreateInstance(createdEditor, usedEditedType, actualEditedType, out editor)) {
                            return editor;
                        }
                    }
                    catch { }
                }

                // try including the edited type
                if (expectedGenericCount == (usedEditTypeGenericArguments.Length + 1)) {
                    try {
                        List<Type> arguments = new List<Type>();
                        arguments.Add(actualEditedType);
                        arguments.AddRange(usedEditTypeGenericArguments);

                        Type createdEditor = editorType.MakeGenericType(arguments.ToArray());
                        IBehaviorEditor editor;
                        if (TryCreateInstance(createdEditor, usedEditedType, actualEditedType, out editor)) {
                            return editor;
                        }
                    }
                    catch { }
                }

                return null;
            }
        }

        /// <summary>
        /// Attempts to create a behavior editor for the given edited data type from the given
        /// editor type.
        /// </summary>
        /// <param name="editedType">The type that is being edited.</param>
        /// <param name="editorType">The editor type.</param>
        /// <returns>A behavior editor that can edit the given edited type.</returns>
        public static IBehaviorEditor TryCreateEditor(Type editedType, Type editorType) {
            // If our editor isn't inherited, then we only want to create a specific editor
            var customBehaviorEditorAttribute = fsPortableReflection.GetAttribute<CustomBehaviorEditorAttribute>(editorType);
            if (customBehaviorEditorAttribute == null || customBehaviorEditorAttribute.Inherit == false) {
                return TryCreateSpecificEditor(editedType, editedType, editorType);
            }

            // Otherwise we want to try to create a behavior editor from any of the edited type's
            // associated types.
            Type baseType = editedType;

            while (baseType != null) {
                IBehaviorEditor editor = TryCreateSpecificEditor(baseType, editedType, editorType);
                if (editor != null) {
                    return editor;
                }

                foreach (Type iface in baseType.GetInterfaces()) {
                    editor = TryCreateSpecificEditor(iface, editedType, editorType);
                    if (editor != null) {
                        return editor;
                    }
                }

                baseType = baseType.BaseType;
            }

            return null;
        }
    }
}