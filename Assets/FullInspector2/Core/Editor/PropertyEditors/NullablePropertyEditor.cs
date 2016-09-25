using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Internal {
    /// <summary>
    /// A property editor for nullable types.
    /// </summary>
    // TODO: eventually we need to make sure that this works for generic nullable types, but at
    //       the moment they cause an internal compiler error within the Mono C# compiler. The
    //       potential error point is the constructor where we fetch the struct type.
    public class NullablePropertyEditor : IPropertyEditor, IPropertyEditorEditAPI {
        public bool DisplaysStandardLabel {
            get { return true; }
        }

        private InspectedType _elementType;

        public NullablePropertyEditor(Type elementType) {
            _elementType = InspectedType.Get(elementType);
        }

        public object Edit(Rect region, GUIContent label, object element, fiGraphMetadata metadata) {
            // draw the nullable type toggle
            {
                int labelHeight = (int)EditorStyles.label.CalcHeight(GUIContent.none, 100);

                Rect toggleRegion = new Rect(region);
                toggleRegion.height = labelHeight;
                region.y += toggleRegion.height;
                region.height -= toggleRegion.height;

                if (EditorGUI.Toggle(toggleRegion, label, element != null)) {
                    if (element == null) {
                        element = _elementType.CreateInstance();
                        GUI.changed = true;
                    }
                }
                else {
                    element = null;
                }
            }

            // no element; no editor
            if (element == null) {
                return null;
            }

            // we have a value for the nullable type; draw the property editor
            {
                Rect selectedRegion = new Rect(region);
                selectedRegion = fiRectUtility.IndentedRect(selectedRegion);
                region.y += selectedRegion.height;
                region.height -= selectedRegion.height;

                // show custom editor
                PropertyEditorChain chain = PropertyEditor.Get(element.GetType(), null);
                IPropertyEditor editor = chain.SkipUntilNot(typeof(NullablePropertyEditor));

                return editor.Edit(selectedRegion, GUIContent.none, element, metadata.Enter("NullableEditor"));
            }
        }

        public float GetElementHeight(GUIContent label, object element, fiGraphMetadata metadata) {
            float height = EditorStyles.label.CalcHeight(label, 100);

            if (element != null) {
                PropertyEditorChain chain = PropertyEditor.Get(element.GetType(), null);
                IPropertyEditor editor = chain.SkipUntilNot(typeof(NullablePropertyEditor));

                height += fiRectUtility.IndentVertical;
                height += editor.GetElementHeight(GUIContent.none, element, metadata.Enter("NullableEditor"));
            }

            return height;
        }

        public GUIContent GetFoldoutHeader(GUIContent label, object element) {
            return new GUIContent(label.text + " (" + fiReflectionUtility.GetObjectTypeNameSafe(element) + ")");
        }

        public object OnSceneGUI(object element) {
            if (element != null) {
                PropertyEditorChain chain = PropertyEditor.Get(element.GetType(), null);
                IPropertyEditor editor = chain.SkipUntilNot(typeof(NullablePropertyEditor));

                return editor.OnSceneGUI(element);
            }
            return element;
        }


        public static IPropertyEditor TryCreate(Type type, ICustomAttributeProvider attributes) {
            if (type.IsNullableType()) {
                return new NullablePropertyEditor(type.GetGenericArguments()[0]);
            }

            if (attributes != null &&
                type.IsClass && attributes.IsDefined(typeof(InspectorNullableAttribute), /*inherit:*/true)) {
                return new NullablePropertyEditor(type);
            }

            return null;
        }

        public PropertyEditorChain EditorChain {
            get;
            set;
        }

        public bool CanEdit(Type dataType) {
            throw new NotSupportedException();
        }
    }
}