using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules.Attributes {
    [CustomAttributePropertyEditor(typeof(InspectorDisabledAttribute), ReplaceOthers = true)]
    public class InspectorDisabledAttributeEditor<T> : AttributePropertyEditor<T, InspectorDisabledAttribute> {
        protected override T Edit(Rect region, GUIContent label, T element, InspectorDisabledAttribute attribute, fiGraphMetadata metadata) {
            PropertyEditorChain chain = PropertyEditor.Get(typeof(T), null);

            EditorGUI.BeginDisabledGroup(true);
            element = chain.FirstEditor.Edit(region, label, element, metadata.Enter("InspectorDisabledAttribute"));
            EditorGUI.EndDisabledGroup();

            return element;
        }

        protected override float GetElementHeight(GUIContent label, T element, InspectorDisabledAttribute attribute, fiGraphMetadata metadata) {
            PropertyEditorChain chain = PropertyEditor.Get(typeof(T), null);
            return chain.FirstEditor.GetElementHeight(label, element, metadata.Enter("InspectorDisabledAttribute"));
        }
    }

#if false
    [CustomAttributePropertyEditor(typeof(InspectorDisabledIfAttribute), ReplaceOthers = true)]
    public class InspectorDisabledIfAttributeEditor<T> : AttributePropertyEditor<T, InspectorDisabledIfAttribute> {
        protected override T Edit(Rect region, GUIContent label, T element, InspectorDisabledIfAttribute attribute, fiGraphMetadata metadata) {
            PropertyEditorChain chain = PropertyEditor.Get(typeof(T), null);

            bool disabled = fiReflectionUtility.GetBooleanReflectedMember(typeof(T), element, attribute.ConditionalMemberName, true);
            EditorGUI.BeginDisabledGroup(disabled);
            element = chain.FirstEditor.Edit(region, label, element, metadata.Enter("InspectorDisabledIfAttribute"));
            EditorGUI.EndDisabledGroup();

            return element;
        }

        protected override float GetElementHeight(GUIContent label, T element, InspectorDisabledIfAttribute attribute, fiGraphMetadata metadata) {
            PropertyEditorChain chain = PropertyEditor.Get(typeof(T), null);
            return chain.FirstEditor.GetElementHeight(label, element, metadata.Enter("InspectorDisabledIfAttribute"));
        }
    }
#endif
}
