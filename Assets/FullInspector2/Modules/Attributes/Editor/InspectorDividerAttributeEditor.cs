using FullInspector.Internal;
using UnityEngine;

namespace FullInspector.Modules.Attributes {
    [CustomAttributePropertyEditor(typeof(InspectorDividerAttribute), ReplaceOthers = false)]
    public class InspectorDividerAttributeEditor<T> : AttributePropertyEditor<T, InspectorDividerAttribute> {
        protected override T Edit(Rect region, GUIContent label, T element, InspectorDividerAttribute attribute, fiGraphMetadata metadata) {
            fiEditorGUI.Splitter(region);
            return element;
        }

        protected override float GetElementHeight(GUIContent label, T element, InspectorDividerAttribute attribute, fiGraphMetadata metadata) {
            return 2;
        }
    }
}