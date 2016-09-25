using FullInspector.Internal;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules.Attributes {
    [CustomAttributePropertyEditor(typeof(InspectorCommentAttribute), ReplaceOthers = false)]
    public class InspectorCommentAttributeEditor<T> : AttributePropertyEditor<T, InspectorCommentAttribute> {
        private const float Margin = 2f;

        private static MessageType MapCommentType(CommentType commentType) {
            return (MessageType)commentType;
        }

        protected override T Edit(Rect region, GUIContent label, T element, InspectorCommentAttribute attribute, fiGraphMetadata metadata) {
            region.height = GetCommentHeight(attribute);
            EditorGUI.HelpBox(region, attribute.Comment, MapCommentType(attribute.Type));
            return element;
        }

        private float GetCommentHeight(InspectorCommentAttribute attribute) {
            return fiCommentUtility.GetCommentHeight(attribute.Comment, attribute.Type);
        }

        protected override float GetElementHeight(GUIContent label, T element, InspectorCommentAttribute attribute, fiGraphMetadata metadata) {
            return GetCommentHeight(attribute) + Margin;
        }
    }
}