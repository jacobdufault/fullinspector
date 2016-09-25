using UnityEngine;

namespace FullInspector.Modules.Attributes {
    [CustomAttributePropertyEditor(typeof(InspectorHidePrimaryAttribute), ReplaceOthers = true)]
    public class InspectorHidePrimaryAttributeEditor<T> :
        AttributePropertyEditor<T, InspectorHidePrimaryAttribute> {

        protected override T Edit(Rect region, GUIContent label, T element, InspectorHidePrimaryAttribute attribute, fiGraphMetadata metadata) {
            return element;
        }

        protected override float GetElementHeight(GUIContent label, T element, InspectorHidePrimaryAttribute attribute, fiGraphMetadata metadata) {
            return 0;
        }
    }
}