using System;

namespace FullInspector {
    /// <summary>
    /// Draws the regular property editor but with a disabled GUI. With the
    /// current implementation this is not compatible with other attribute
    /// editors.
    /// </summary>
    // TODO: rename to [InspectorReadOnly]
    // TODO: Find a way to support multiple primary attribute editors.
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InspectorDisabledAttribute : Attribute {
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InspectorDisabledIfAttribute : Attribute {
        /// <summary>
        /// The name of the member to use as a condition. The conditional needs
        /// to either be a boolean field, a boolean property with a getter, or a
        /// no-argument method that returns a boolean.
        /// </summary>
        public string ConditionalMemberName {
            set { ConditionalMemberNames = new[] { value }; }
        }

        /// <summary>
        /// A sequence of named boolean values to fetch from the object instance.
        /// Each condition is combined using |Operator|.
        /// </summary>
        public string[] ConditionalMemberNames;

        /// <summary>
        /// How |ConditionalMemberNames| should be combined into one value.
        /// </summary>
        public fiLogicalOperator Operator;

        public InspectorDisabledIfAttribute(fiLogicalOperator op, params string[] memberNames) {
            Operator = op;
            ConditionalMemberNames = memberNames;
        }

        /// <summary>
        /// This allows a member to be conditionally hidden in the inspector
        /// depending upon the state of other variables in object. This does
        /// *not* change serialization behavior, only display behavior.
        /// </summary>
        /// <param name="conditionalMemberName">
        /// The name of the member to use as a condition. The conditional needs
        /// to either be a boolean field, a boolean property with a getter, or a
        /// no-argument method that returns a boolean.
        /// </param>
        public InspectorDisabledIfAttribute(string conditionalMemberName) {
            ConditionalMemberName = conditionalMemberName;
        }
    }
}