using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules.Common {
    [CustomAttributePropertyEditor(typeof(InspectorTextAreaAttribute), ReplaceOthers = true)]
    public class InspectorTextAreaAttributeEditor : AttributePropertyEditor<string, InspectorTextAreaAttribute> {
        protected override string Edit(Rect region, GUIContent label, string element, InspectorTextAreaAttribute attribute, fiGraphMetadata metadata) {
            // note: Unity does *not* provide a label override for TextArea, so we have to handle it ourselves.

            // We don't have an empty label
            if (string.IsNullOrEmpty(label.text) == false || label.image != null) {
                region = EditorGUI.PrefixLabel(region, label);
            }

            // No label
            return EditorGUI.TextArea(region, element);
        }

        protected override float GetElementHeight(GUIContent label, string element, InspectorTextAreaAttribute attribute, fiGraphMetadata metadata) {
            return attribute.Height;
        }
    }
}