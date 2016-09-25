using System;

namespace FullInspector {
    /// <summary>
    /// A comment attribute adds a comment to an object that is viewable in the inspector.
    /// </summary>
    [Obsolete("Use [InspectorComment] instead of [Comment]")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class |
        AttributeTargets.Struct)]
    public class CommentAttribute : Attribute, IInspectorAttributeOrder {
        public string Comment;

        public CommentType Type;

        public double Order = 100;

        public CommentAttribute(string comment)
            : this(CommentType.Info, comment) {
        }

        public CommentAttribute(CommentType type, string comment) {
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