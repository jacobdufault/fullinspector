using FullSerializer.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FullInspector.Internal {
    public static class PropertyEditorTools {
        /// <summary>
        /// Returns true if the given dataType matches the given propertyType. If the dataType is
        /// generic and the propertyType is a generic definition, then the propertyType will be
        /// instantiated with the same generic arguments as dataType.
        /// </summary>
        private static bool CanEdit(Type dataType, CustomPropertyEditorAttribute propertyTypeAttribute) {
            if (propertyTypeAttribute == null) {
                return true;
            }

            Type propertyType = propertyTypeAttribute.PropertyType;
            if (dataType.IsGenericType && propertyType.IsGenericTypeDefinition) {
                // I don't believe this will ever throw, but just in case we wrap it in a try/catch
                // block.
                try {
                    propertyType = propertyType.MakeGenericType(dataType.GetGenericArguments());
                }
                catch { }
            }

            return dataType == propertyType ||
                (propertyTypeAttribute.Inherit && propertyType.IsAssignableFrom(dataType));
        }

        private static Type[] NonEmptyConstructorArgs = new Type[] { typeof(Type), typeof(ICustomAttributeProvider) };

        /// <summary>
        /// Creates a new instance of the given editorType. It is assumed that editorType extends
        /// IPropertyEditor.
        /// </summary>
        private static bool TryCreateInstance(Type editorType, Type usedEditedType,
            Type actualEditedType, ICustomAttributeProvider attributes, out IPropertyEditor editor) {

            if (editorType.GetConstructor(fsPortableReflection.EmptyTypes) == null &&
                editorType.GetConstructor(NonEmptyConstructorArgs) == null) {

                Debug.LogWarning("Type " + editorType + " can serve as a property editor if it " +
                    "has a default constructor or a constructor that takes a Type and an ICustomAttributeProvider arguments");
                editor = null;
                return false;
            }

            if (CanEdit(usedEditedType,
                fsPortableReflection.GetAttribute<CustomPropertyEditorAttribute>(editorType)) == false) {
                editor = null;
                return false;
            }

            try {
                if (editorType.GetConstructor(NonEmptyConstructorArgs) != null) {
                    editor = (IPropertyEditor)Activator.CreateInstance(editorType, new object[] { actualEditedType, attributes });
                }
                else {
                    editor = (IPropertyEditor)Activator.CreateInstance(editorType);
                }
            }
            catch (Exception e) {
                Debug.LogException(e);
                editor = null;
                return false;
            }

            if (editor.CanEdit(actualEditedType) == false) {
                editor = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Attempt to create a new IPropertyEditor instance.
        /// </summary>
        /// <param name="usedEditedType">The data type that is used for comparison with the property
        /// editor.</param>
        /// <param name="actualEditedType">The actual data type that that usedEditedType was derived
        /// from.</param>
        /// <param name="editorType">The type of editor we are trying to create.</param>
        /// <returns></returns>
        private static IPropertyEditor TryCreateSpecificEditor(Type usedEditedType, Type actualEditedType, Type editorType, ICustomAttributeProvider attributes) {
            //-
            //-
            //-
            // the type can be instantiated directly and is non-generic
            if (editorType.IsGenericTypeDefinition == false) {
                IPropertyEditor editor;
                TryCreateInstance(editorType, usedEditedType, actualEditedType, attributes, out editor);
                return editor;
            }

            //-
            //-
            //-
            // we have to construct a closed generic editor type
            else {
                int expectedGenericCount = editorType.GetGenericArguments().Length;


                // The edited type itself
                if (expectedGenericCount == 1) {
                    try {
                        Type createdEditor = editorType.MakeGenericType(usedEditedType);
                        IPropertyEditor editor;
                        if (TryCreateInstance(createdEditor, usedEditedType, actualEditedType, attributes, out editor)) {
                            return editor;
                        }
                    }
                    catch { }
                }

                Type[] usedEditedTypeGenericArguments = usedEditedType.GetGenericArguments();

                // try the regular generic arguments
                if (expectedGenericCount == usedEditedTypeGenericArguments.Length) {
                    try {
                        Type createdEditor = editorType.MakeGenericType(usedEditedTypeGenericArguments);
                        IPropertyEditor editor;
                        if (TryCreateInstance(createdEditor, usedEditedType, actualEditedType, attributes, out editor)) {
                            return editor;
                        }
                    }
                    catch { }
                }

                // try including the edited type
                if (expectedGenericCount == (usedEditedTypeGenericArguments.Length) + 1) {
                    try {
                        List<Type> arguments = new List<Type>();
                        arguments.Add(actualEditedType);
                        arguments.AddRange(usedEditedTypeGenericArguments);

                        Type createdEditor = editorType.MakeGenericType(arguments.ToArray());
                        IPropertyEditor editor;
                        if (TryCreateInstance(createdEditor, usedEditedType, actualEditedType, attributes, out editor)) {
                            return editor;
                        }
                    }
                    catch { }
                }

                return null;
            }
        }

        /// <summary>
        /// Attempts to create a property editor for the given edited data type from the given
        /// editor type.
        /// </summary>
        /// <param name="editedType">The type that is being edited.</param>
        /// <param name="editorType">The editor type.</param>
        /// <param name="attributes">The attributes that were specified for the type.</param>
        /// <param name="forceInherit">Should inheritance behavior be forced? The expected value is false.</param>
        /// <returns>A property editor that can edit the given edited type.</returns>
        public static IPropertyEditor TryCreateEditor(Type editedType, Type editorType, ICustomAttributeProvider attributes, bool forceInherit) {
            // If our editor isn't inherited, then we only want to create a specific editor
            var customPropertyEditorAttribute = fsPortableReflection.GetAttribute<CustomPropertyEditorAttribute>(editorType);
            if (!forceInherit && (customPropertyEditorAttribute == null || customPropertyEditorAttribute.Inherit == false)) {
                return TryCreateSpecificEditor(editedType, editedType, editorType, attributes);
            }

            // Otherwise we want to try to create a property editor from any of the edited type's
            // associated types.
            Type baseType = editedType;

            while (baseType != null) {
                IPropertyEditor editor = TryCreateSpecificEditor(baseType, editedType, editorType, attributes);
                if (editor != null) {
                    return editor;
                }

                foreach (Type iface in baseType.GetInterfaces()) {
                    editor = TryCreateSpecificEditor(iface, editedType, editorType, attributes);
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