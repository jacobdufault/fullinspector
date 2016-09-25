using System;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Internal {
    /// <summary>
    /// Provides a property editor for enums.
    /// </summary>
    internal class EnumPropertyEditor : IPropertyEditor, IPropertyEditorEditAPI {
        public bool DisplaysStandardLabel {
            get { return true; }
        }

        public PropertyEditorChain EditorChain {
            get;
            set;
        }
        
        public object OnSceneGUI(object element) {
            return element;
        }

        public object Edit(Rect region, GUIContent label, object element, fiGraphMetadata metadata) {
            Enum selected = (Enum)element;

            if (selected.GetType().IsDefined(typeof(FlagsAttribute), /*inherit:*/ true)) {
                return EditorGUI.EnumMaskField(region, label, selected);
            }

            return EditorGUI.EnumPopup(region, label, selected);
        }

        public float GetElementHeight(GUIContent label, object element, fiGraphMetadata metadata) {
            return EditorStyles.popup.CalcHeight(label, 100);
        }

        public GUIContent GetFoldoutHeader(GUIContent label, object element) {
            return label;
        }

        public bool CanEdit(Type dataType) {
            throw new NotSupportedException();
        }

        public static IPropertyEditor TryCreate(Type dataType) {
            if (dataType.IsEnum == false) {
                return null;
            }

            return new EnumPropertyEditor();
        }
    }
}