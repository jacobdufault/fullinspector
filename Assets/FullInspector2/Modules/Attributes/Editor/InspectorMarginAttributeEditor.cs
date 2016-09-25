using UnityEngine;

namespace FullInspector.Modules.Attributes {
    [CustomAttributePropertyEditor(typeof(InspectorMarginAttribute), ReplaceOthers = false)]
    public class InspectorMarginAttributeEditor<T> : AttributePropertyEditor<T, InspectorMarginAttribute> {
        protected override T Edit(Rect region, GUIContent label, T element, InspectorMarginAttribute attribute, fiGraphMetadata metadata) {
            return element;
        }

        protected override float GetElementHeight(GUIContent label, T element, InspectorMarginAttribute attribute, fiGraphMetadata metadata) {
            return attribute.Margin;
        }
    }
}