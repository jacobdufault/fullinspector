using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules {
    [CustomAttributePropertyEditor(typeof(InspectorDisabledAttribute), ReplaceOthers = true)]
    public class InspectorDisabledAttributeEditor<T> : AttributePropertyEditor<T, InspectorDisabledAttribute> {
        protected override T Edit(Rect region, GUIContent label, T element, InspectorDisabledAttribute attribute, fiGraphMetadata metadata) {
            PropertyEditorChain chain = PropertyEditor.Get(typeof(T), null);

            EditorGUI.BeginDisabledGroup(true);
            element = chain.FirstEditor.Edit(region, label, element, metadata.NoOp());
            EditorGUI.EndDisabledGroup();

            return element;
        }

        protected override float GetElementHeight(GUIContent label, T element, InspectorDisabledAttribute attribute, fiGraphMetadata metadata) {
            PropertyEditorChain chain = PropertyEditor.Get(typeof(T), null);
            return chain.FirstEditor.GetElementHeight(label, element, metadata.NoOp());
        }
    }

    [CustomAttributePropertyEditor(typeof(InspectorDisabledIfAttribute), ReplaceOthers = true)]
    public class InspectorDisabledIfAttributeEditor<T> : AttributePropertyEditor<T, InspectorDisabledIfAttribute> {
        protected override T Edit(Rect region, GUIContent label, T element, InspectorDisabledIfAttribute attribute, fiGraphMetadata metadata) {
            bool disabled = fiLogicalOperatorSupport.ComputeValue(
                attribute.Operator, attribute.ConditionalMemberNames, metadata.Context);

            EditorGUI.BeginDisabledGroup(disabled);
            PropertyEditorChain chain = PropertyEditor.Get(typeof(T), null);
            element = chain.FirstEditor.Edit(region, label, element, metadata.NoOp());
            EditorGUI.EndDisabledGroup();

            return element;
        }

        protected override float GetElementHeight(GUIContent label, T element, InspectorDisabledIfAttribute attribute, fiGraphMetadata metadata) {
            PropertyEditorChain chain = PropertyEditor.Get(typeof(T), null);
            return chain.FirstEditor.GetElementHeight(label, element, metadata.NoOp());
        }
    }
}
