using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules.Attributes {
    [CustomAttributePropertyEditor(typeof(InspectorHeaderAttribute), ReplaceOthers = false)]
    public class InspectorHeaderAttributeEditor<T> : AttributePropertyEditor<T, InspectorHeaderAttribute> {
        protected override T Edit(Rect region, GUIContent label, T element, InspectorHeaderAttribute attribute, fiGraphMetadata metadata) {
            GUI.Label(region, attribute.Header, EditorStyles.boldLabel);
            return element;
        }

        protected override float GetElementHeight(GUIContent label, T element, InspectorHeaderAttribute attribute, fiGraphMetadata metadata) {
            return EditorStyles.boldLabel.CalcHeight(label, 100);
        }
    }
}