using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FullSerializer.Internal;
using UnityEngine;

namespace FullInspector.Internal {
    public class AttributePropertyEditor : IPropertyEditor, IPropertyEditorEditAPI, IPropertyEditorDefaultFoldoutState {

        public bool DisplaysStandardLabel {
            get { return false || (_showPrimary && _editors.Count == 0); }
        }

        public PropertyEditorChain EditorChain {
            get;
            set;
        }

        private List<IAttributePropertyEditor> _editors;
        private bool _showPrimary;
        private bool _showTopLevelFoldout;
        private bool _indent;
        private bool _defaultFoldoutState;

        private AttributePropertyEditor(List<IAttributePropertyEditor> editors, bool showPrimary, bool showTopLevelFoldout, bool indent, bool defaultFoldoutState) {
            _editors = editors;
            _showPrimary = showPrimary;
            _showTopLevelFoldout = showTopLevelFoldout;
            _indent = indent;
            _defaultFoldoutState = defaultFoldoutState;
        }

        public object OnSceneGUI(object element) {
            for (int i = 0; i < _editors.Count; ++i) {
                element = _editors[i].OnSceneGUI(element);
            }

            if (_showPrimary) {
                element = NextEditor.OnSceneGUI(element);
            }

            return element;
        }

        private IPropertyEditor NextEditor {
            get {
                return EditorChain.GetNextEditor(this);
            }
        }

        public bool DefaultFoldoutState {
            get {
                return _defaultFoldoutState;
            }
        }

        private void DisableFoldouts(fiGraphMetadata metadata) {
            if (_editors.Count > 0) {
                if (_showTopLevelFoldout == false) {
                    metadata.GetPersistentMetadata<fiDropdownMetadata>().ForceDisable();
                }

                for (int i = 0; i < _editors.Count; ++i) {
                    metadata.Enter(i).Metadata.GetPersistentMetadata<fiDropdownMetadata>().ForceDisable();
                }
            }

            if (_showPrimary) {
                metadata.Enter("Primary").Metadata.GetPersistentMetadata<fiDropdownMetadata>().ForceDisable();
            }
        }

        public object Edit(Rect region, GUIContent label, object element, fiGraphMetadata metadata) {
            DisableFoldouts(metadata);

            if (_indent) {
                region.x += fiRectUtility.IndentHorizontal;
                region.width -= fiRectUtility.IndentHorizontal;
            }

            var heights = new List<float>(_editors.Count);
            for (int i = 0; i < _editors.Count; ++i) {
                heights.Add(_editors[i].GetElementHeight(label, element, metadata.Enter(i)));
            }

            Rect editRegion = region;
            for (int i = 0; i < _editors.Count; ++i) {
                editRegion.height = heights[i];
                element = _editors[i].Edit(editRegion, label, element, metadata.Enter(i));
                editRegion.y += editRegion.height;
            }

            if (_showPrimary) {
                editRegion.height = NextEditor.GetElementHeight(label, element, metadata.Enter("Primary"));
                element = NextEditor.Edit(editRegion, label, element, metadata.Enter("Primary"));
            }

            return element;
        }

        public float GetElementHeight(GUIContent label, object element, fiGraphMetadata metadata) {
            float height = 0;

            for (int i = 0; i < _editors.Count; ++i) {
                height += _editors[i].GetElementHeight(label, element, metadata.Enter(i));
            }

            if (_showPrimary) {
                height += NextEditor.GetElementHeight(label, element, metadata.Enter("Primary"));
            }

            return height;
        }

        public GUIContent GetFoldoutHeader(GUIContent label, object element) {
            return label;
        }

        public bool CanEdit(Type dataType) {
            throw new NotSupportedException();
        }

        /// <summary>
        /// A map of attribute type to the IPropertyEditor type that is associated with that
        /// attribute (via the CustomAttributePropertyEditorAttribute annotation).
        /// </summary>
        private static Dictionary<Type, Type> _attributeEditorMappings = new Dictionary<Type, Type>();

        /// <summary>
        /// A map of attribute type to if that property editor should replace all other editors
        /// after it.
        /// </summary>
        private static Dictionary<Type, bool> _attributeReplaceMappings = new Dictionary<Type, bool>();

        static AttributePropertyEditor() {
            // fetch all CustomAttributePropertyEditorAttribute types
            _attributeEditorMappings = new Dictionary<Type, Type>();
            _attributeReplaceMappings = new Dictionary<Type, bool>();

            foreach (Type attributeHolder in
                from assembly in fiRuntimeReflectionUtility.GetUserDefinedEditorAssemblies()
                from type in assembly.GetTypesWithoutException()

                where type.IsAbstract == false
                where type.IsInterface == false

                where fsPortableReflection.HasAttribute<CustomAttributePropertyEditorAttribute>(type)

                select type) {

                if (typeof(IAttributePropertyEditor).IsAssignableFrom(attributeHolder) == false) {
                    Debug.LogWarning(string.Format("{0} has a {1} attribute but does not extend {2}",
                        attributeHolder, typeof(CustomAttributePropertyEditorAttribute).Name,
                        typeof(IAttributePropertyEditor).Name));
                    continue;
                }

                var attribute = fsPortableReflection.GetAttribute<CustomAttributePropertyEditorAttribute>(attributeHolder);

                _attributeEditorMappings[attribute.AttributeActivator] = attributeHolder;
                _attributeReplaceMappings[attribute.AttributeActivator] = attribute.ReplaceOthers;
            }
        }

        public static IPropertyEditor TryCreate(Type editedType, ICustomAttributeProvider editedAttributes) {
            if (editedAttributes == null) {
                return null;
            }

            List<IAttributePropertyEditor> editors = new List<IAttributePropertyEditor>();
            bool replace = false;
            bool showTopLevelFoldout = false;
            bool indent = false;
            bool defaultFoldoutState = true;
            bool hasAttribute = false;

            foreach (object attribute in
                editedAttributes.GetCustomAttributes(/*inherit:*/ true)) {

                if (attribute is InspectorIndentAttribute) {
                    indent = true;
                    hasAttribute = true;
                    continue;
                }

                if (attribute is InspectorCollapsedFoldoutAttribute) {
                    defaultFoldoutState = false;
                    hasAttribute = true;
                    continue;
                }

                if (_attributeEditorMappings.ContainsKey(attribute.GetType())) {
                    Type editorType = _attributeEditorMappings[attribute.GetType()];
                    IPropertyEditor editor = PropertyEditorTools.TryCreateEditor(editedType, editorType, editedAttributes, /*forceInherit:*/ true);
                    if (editor == null) {
                        Debug.LogWarning(string.Format("Failed to create attribute property " +
                            "editor {0} for {1}", editedAttributes, editorType));
                        continue;
                    }

                    // does this attribute editor hide the primary editor?
                    bool replacePrimary = _attributeReplaceMappings[attribute.GetType()];
                    replace = replace || replacePrimary;

                    // HACK: We hard-code support for disabling drop-downs for fields that only
                    //       serve as white-space between other fields (ie, they are annotated with
                    //       [InspectorMargin, InspectorHidePrimary]). This could be generalized
                    //       pretty easily, but I don't see the need, as it should only ever be
                    //       needed for InspectorMarginAttribute.
                    if (attribute.GetType() != typeof(InspectorMarginAttribute) && replacePrimary == false) {
                        showTopLevelFoldout = true;
                    }

                    // add it to the list of editors
                    IAttributePropertyEditor attributeEditor = (IAttributePropertyEditor)editor;
                    attributeEditor.Attribute = (Attribute)attribute;
                    editors.Add(attributeEditor);
                    hasAttribute = true;
                }
            }

            if (hasAttribute == false) {
                return null;
            }

            // Sort the editors by their opt-in ordering
            editors = editors.OrderBy(editor => {
                var attributeOrder = editor.Attribute as IInspectorAttributeOrder;
                if (attributeOrder != null) {
                    return attributeOrder.Order;
                }

                return double.MaxValue;
            }).ToList();

            return new AttributePropertyEditor(editors, replace == false, showTopLevelFoldout, indent, defaultFoldoutState);
        }
    }
}