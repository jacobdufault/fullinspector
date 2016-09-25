using FullInspector.Internal;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules.Attributes {
    [CustomAttributePropertyEditor(typeof(InspectorSkipInheritanceAttribute), ReplaceOthers = true)]
    public class InspectorSkipInheritanceAttributeEditor<T> : AttributePropertyEditor<T, InspectorSkipInheritanceAttribute> {

        private static IPropertyEditor GetEditor(object element) {
            if (element == null) {
                return PropertyEditor.Get(typeof(T), null).FirstEditor;
            }
            return PropertyEditor.Get(element.GetType(), null).SkipUntilNot(typeof(AbstractTypePropertyEditor));
        }

        protected override T Edit(Rect region, GUIContent label, T element, InspectorSkipInheritanceAttribute attribute, fiGraphMetadata metadata) {
            if (element == null) {
                GUI.Label(region, label);
                return element;
            }

            return GetEditor(element).Edit(region, label, element, metadata.Enter("InspectorSkipInheritance"));
        }

        protected override float GetElementHeight(GUIContent label, T element, InspectorSkipInheritanceAttribute attribute, fiGraphMetadata metadata) {
            if (element == null) {
                return EditorGUIUtility.singleLineHeight;
            }

            return GetEditor(element).GetElementHeight(label, element, metadata.Enter("InspectorSkipInheritance"));
        }

        public override GUIContent GetFoldoutHeader(GUIContent label, object element) {
            return GetEditor(element).GetFoldoutHeader(label, element);
        }

        protected override T OnSceneGUI(T element, InspectorSkipInheritanceAttribute attribute) {
            return GetEditor(element).OnSceneGUI(element);
        }
    }
}