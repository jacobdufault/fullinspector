using System;
using System.Reflection;
using FullSerializer.Internal;

namespace FullInspector {
    /// <summary>
    /// Set the display order of an field or property of an object. A field or
    /// property without an [InspectorOrder] attribute defaults to order
    /// double.MaxValue (which will appear after any ordered properties). The
    /// lower the order value, the higher the field or property will appear in
    /// the inspector. Each inheritance level receives its own order group.
    ///
    /// See fiSettings.EnableGlobalOrdering to make ordering happen globally
    /// instead of per-class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class InspectorOrderAttribute : Attribute {
        /// <summary>
        /// The ordering of this member relative to other ordered
        /// fields/properties.
        /// </summary>
        public double Order;

        /// <summary>
        /// Set the order.
        /// </summary>
        /// <param name="order">
        /// The order in which to display this field or property. A field or
        /// property without an [InspectorOrder] attribute defaults to order
        /// double.MaxValue.
        /// </param>
        public InspectorOrderAttribute(double order) {
            Order = order;
        }

        /// <summary>
        /// Helper method to determine the inspector order for the given member.
        /// If the member does not have an [InspectorOrder] attribute, then the
        /// default order is returned.
        /// </summary>
        public static double GetInspectorOrder(MemberInfo memberInfo) {
            var attr = fsPortableReflection.GetAttribute<InspectorOrderAttribute>(memberInfo);
            if (attr == null) {
                return double.MaxValue;
            }

            return attr.Order;
        }
    }
}