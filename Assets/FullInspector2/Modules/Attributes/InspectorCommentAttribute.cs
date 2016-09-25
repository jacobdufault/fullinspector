using System;

namespace FullInspector {
    /// <summary>
    /// Add a comment above the given field or property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class |
        AttributeTargets.Struct)]
    public class InspectorCommentAttribute : Attribute, IInspectorAttributeOrder {
        public string Comment;

        public CommentType Type;

        public double Order = 100;

        public InspectorCommentAttribute(string comment)
            : this(fiSettings.DefaultCommentType, comment) {
        }

        public InspectorCommentAttribute(CommentType type, string comment) {
            Type = type;
            Comment = comment;
        }

        double IInspectorAttributeOrder.Order {
            get {
                return Order;
            }
        }
    }
}