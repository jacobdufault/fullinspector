using System;

namespace FullInspector {
    // TODO: Pull these attributes out of Core.
    /// <summary>
    /// This allows a member to be conditionally hidden in the inspector depending upon the
    /// state of other variables in object. This does *not* change serialization behavior,
    /// only display behavior.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class InspectorShowIfAttribute : Attribute {
        /// <summary>
        /// The name of the member to use as a condition. The conditional needs to
        /// either be a boolean field, a boolean property with a getter, or a no-argument
        /// method that returns a boolean.
        /// </summary>
        public string ConditionalMemberName {
            set { ConditionalMemberNames = new[] { value }; }
        }

        /// <summary>
        /// The names of the members to use as a condition. You can control how these members
        /// are combined using the LogicalOperator Operator parameter.
        ///
        /// The conditional members need to either be a boolean field, a boolean property with
        /// a getter, or a no-argument method that returns a boolean.
        /// </summary>
        public string[] ConditionalMemberNames;

        /// <summary>
        /// Determines how multiple boolean values are combined to determine if the property
        /// is shown.
        /// </summary>
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
        public InspectorShowIfAttribute(string conditionalMemberName) {
            Operator = fiLogicalOperator.AND;
            ConditionalMemberName = conditionalMemberName;
        }

        /// <summary>
        /// This allows a member to be conditionally shown in the inspector depending upon the
        /// state of other variables in object. This does *not* change serialization behavior,
        /// only display behavior.
        /// </summary>
        /// <param name="conditionalMemberNames">The names of the members to use as a condition.
        /// You can control how these members are combined using the LogicalOperator Operator parameter.
        ///
        /// Each conditional needs to either be a boolean field, a boolean property with a
        /// getter, or a no-argument method that returns a boolean.
        /// </param>
        /// <param name="op">Determines how multiple boolean values are combined to determine if
        /// the property is hidden.</param>
        public InspectorShowIfAttribute(fiLogicalOperator op, params string[] conditionalMemberNames) {
            Operator = op;
            ConditionalMemberNames = conditionalMemberNames;
        }
    }

    /// <summary>
    /// This allows a member to be conditionally hidden in the inspector depending upon the
    /// state of other variables in object. This does *not* change serialization behavior,
    /// only display behavior.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class InspectorHideIfAttribute : Attribute {
        /// <summary>
        /// The name of the member to use as a condition. The conditional needs to
        /// either be a boolean field, a boolean property with a getter, or a no-argument
        /// method that returns a boolean.
        /// </summary>
        public string ConditionalMemberName {
            set { ConditionalMemberNames = new string[] { value }; }
        }

        /// <summary>
        /// The names of the members to use as a condition. You can control how these members
        /// are combined using the LogicalOperator Operator parameter.
        ///
        /// The conditional members need to either be a boolean field, a boolean property with
        /// a getter, or a no-argument method that returns a boolean.
        /// </summary>
        public string[] ConditionalMemberNames;

        /// <summary>
        /// Determines how multiple boolean values are combined to determine if the property
        /// is hidden.
        /// </summary>
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
        public InspectorHideIfAttribute(string conditionalMemberName) {
            Operator = fiLogicalOperator.AND;
            ConditionalMemberName = conditionalMemberName;
        }

        /// <summary>
        /// This allows a member to be conditionally hidden in the inspector depending upon the
        /// state of other variables in object. This does *not* change serialization behavior,
        /// only display behavior.
        /// </summary>
        /// <param name="conditionalMemberNames">The names of the members to use as a condition.
        /// You can control how these members are combined using the LogicalOperator Operator parameter.
        ///
        /// Each conditional needs to either be a boolean field, a boolean property with a
        /// getter, or a no-argument method that returns a boolean.
        /// </param>
        /// <param name="op">Determines how multiple boolean values are combined to determine if
        /// the property is hidden.</param>
        public InspectorHideIfAttribute(fiLogicalOperator op, params string[] conditionalMemberNames) {
            Operator = op;
            ConditionalMemberNames = conditionalMemberNames;
        }
    }
}