using System;

namespace FullInspector {
    /// <summary>
    /// Draws the regular property editor but with a disabled GUI. With the current implementation
    /// this is not compatible with other attribute editors.
    /// </summary>
    // TODO: rename to [InspectorReadOnly]
    // TODO: implement this inside of the core so we can support multiple attribute editors
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InspectorDisabledAttribute : Attribute {
    }

#if false
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InspectorDisabledIfAttribute : Attribute {
        /// <summary>
        /// The name of the member to use as a condition. The conditional needs to
        /// either be a boolean field, a boolean property with a getter, or a no-argument
        /// method that returns a boolean.
        /// </summary>
        public string ConditionalMemberName {
            set { ConditionalMemberNames = new[] { value }; }
        }

        // TODO
        public string[] ConditionalMemberNames;
        public fiLogicalOperator Operator;

        /// <summary>
        /// This allows a member to be conditionally hidden in the inspector depending upon the
        /// state of other variables in object. This does *not* change serialization behavior,
        /// only display behavior.
        /// </summary>
        /// <param name="conditionalMemberName">The name of the member to use as a condition.
        /// The conditional needs to either be a boolean field, a boolean property with a
        /// getter, or a no-argument method that returns a boolean.
        /// </param>
        public InspectorDisabledIfAttribute(string conditionalMemberName) {
            ConditionalMemberName = conditionalMemberName;
        }
    }
#endif
}