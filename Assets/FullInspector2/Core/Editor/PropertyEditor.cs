using FullInspector.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FullSerializer.Internal;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector {
    /// <summary>
    /// Manages the discovery of PropertyEditor class instances.
    /// </summary>
    public sealed class PropertyEditor {
        private struct CachedType {
            /// <summary>
            /// The type of the edited object.
            /// </summary>
            public Type EditedType;

            /// <summary>
            /// The attributes associated with the edited type.
            /// </summary>
            public ICustomAttributeProvider EditedAttributes;
        }
        private class CachedTypeComparator : IEqualityComparer<CachedType> {
            public bool Equals(CachedType x, CachedType y) {
                return
                    x.EditedType == y.EditedType &&
                    x.EditedAttributes == y.EditedAttributes;
            }

            public int GetHashCode(CachedType obj) {
                int hash = obj.EditedType.GetHashCode();
                if (obj.EditedAttributes != null) {
                    hash += 17 * obj.EditedAttributes.GetHashCode();
                }
                return hash;
            }
        }

        /// <summary>
        /// A list of all types that have a CustomPropertyEditorAttribute attribute.
        /// </summary>
        private static List<Type> _editorTypes;

        /// <summary>
        /// Cached property editors.
        /// </summary>
        private static Dictionary<CachedType, PropertyEditorChain> _cachedPropertyEditors =
            new Dictionary<CachedType, PropertyEditorChain>(new CachedTypeComparator());

        static PropertyEditor() {
            // fetch all CustomPropertyEditorAttribute types
            _editorTypes = new List<Type>();
            foreach (var editorType in
                from assembly in fiRuntimeReflectionUtility.GetUserDefinedEditorAssemblies()
                from type in assembly.GetTypesWithoutException()

                where type.IsAbstract == false
                where type.IsInterface == false

                where fsPortableReflection.HasAttribute<CustomPropertyEditorAttribute>(type)

                select type) {

                if (typeof(IPropertyEditor).IsAssignableFrom(editorType) == false) {
                    Debug.LogWarning(string.Format("{0} has a {1} attribute but does not extend {2}",
                        editorType, typeof(CustomPropertyEditorAttribute).Name,
                        typeof(IPropertyEditor).Name));
                    continue;
                }

                var attr = fsPortableReflection.GetAttribute<CustomPropertyEditorAttribute>(editorType);
                if (typeof(UnityObject).IsAssignableFrom(attr.PropertyType) &&
                    attr.DisableErrorOnUnityObject == false) {

                    Debug.LogError("Please derive from BehaviorEditor (not PropertyEditor) for " + editorType + " (which is editing type " + attr.PropertyType + ")");
                    continue;
                }

                _editorTypes.Add(editorType);
            }
        }

        /// <summary>
        /// If there are multiple user-defined property editors that report that they can edit a
        /// specific type, we sort the applicability of the property editor based on how close it's
        /// reported edited type is to the actual property type. This allows for, say, the
        /// IListPropertyEditor to override the ICollectionPropertyEditor.
        /// </summary>
        private static void SortByPropertyTypeRelevance(List<IPropertyEditor> editors) {
            editors.Sort((a, b) => {
                Type targetA = fsPortableReflection.GetAttribute<CustomPropertyEditorAttribute>(a.GetType()).PropertyType;
                Type targetB = fsPortableReflection.GetAttribute<CustomPropertyEditorAttribute>(b.GetType()).PropertyType;

                if (targetA.HasParent(targetB)) {
                    return -1;
                }

                return 1;
            });
        }

        /// <summary>
        /// Returns a set of property editors that can be used to edit the given property type.
        /// </summary>
        private static PropertyEditorChain GetCachedEditors(Type propertyType, ICustomAttributeProvider attributes) {
            var cachedType = new CachedType {
                EditedType = propertyType,
                EditedAttributes = attributes
            };

            PropertyEditorChain chain;

            if (_cachedPropertyEditors.TryGetValue(cachedType, out chain) == false) {
                chain = new PropertyEditorChain();
                _cachedPropertyEditors[cachedType] = chain;

                IPropertyEditor editor;

                if ((editor = AttributePropertyEditor.TryCreate(propertyType, attributes)) != null) chain.AddEditor(editor);

                // arrays always need special handling; we don't support overriding them
                if ((editor = ArrayPropertyEditor.TryCreate(propertyType, attributes)) != null) chain.AddEditor(editor);

                // support layout editors above custom editors
                // notably this enables the layout editor to be the highest-priority, ie, above inherited editors
                if ((editor = tkControlPropertyEditor.TryCreate(propertyType, attributes)) != null) chain.AddEditor(editor);

                // user-defined property editors
                var added = new List<IPropertyEditor>();
                foreach (Type editorType in _editorTypes) {
                    editor = PropertyEditorTools.TryCreateEditor(propertyType, editorType, attributes, false);
                    if (editor != null) {
                        added.Add(editor);
                    }
                }
                SortByPropertyTypeRelevance(added);
                foreach (IPropertyEditor toAdd in added) chain.AddEditor(toAdd);

                // no user-defined editors so let's try to see if we can integrate a PropertyDrawer
                if (added.Count == 0)
                    if ((editor = fiGenericPropertyDrawerPropertyEditorManager.TryCreate(propertyType)) != null) chain.AddEditor(editor);

                // enums come after generic & inherited to allow them to be overridden
                if ((editor = EnumPropertyEditor.TryCreate(propertyType)) != null) chain.AddEditor(editor);

                // try and create an editor for nullable types
                if ((editor = NullablePropertyEditor.TryCreate(propertyType, attributes)) != null) chain.AddEditor(editor);

                // try and create an editor for abstract/interface type
                if ((editor = AbstractTypePropertyEditor.TryCreate(propertyType)) != null) chain.AddEditor(editor);

                // try and create a reflected editor; will only fail for arrays or collections,
                // which should be covered by the array editor
                if ((editor = ReflectedPropertyEditor.TryCreate(propertyType, attributes)) != null) chain.AddEditor(editor);
            }

            return chain;
        }

        /// <summary>
        /// Returns a PropertyEditorChain that can edit the given type. The PropertyEditorChain will
        /// contain all of the IPropertyEditor instances which reported that they could edit the
        /// given type (with the associated set of attributes). To get an actual property editor,
        /// use chain.FirstEditor or a similar method.
        /// </summary>
        /// <param name="propertyType">The type of property/field that is being edited.</param>
        /// <param name="editedAttributes">Provides attributes that may override the default
        /// property editor. This parameter can safely be set to null.</param>
        /// <returns>A property editor chain composed of property editors which can edit the given
        /// property type.</returns>
        public static PropertyEditorChain Get(Type propertyType,
            ICustomAttributeProvider editedAttributes) {

            return GetCachedEditors(propertyType, editedAttributes);
        }
    }
}