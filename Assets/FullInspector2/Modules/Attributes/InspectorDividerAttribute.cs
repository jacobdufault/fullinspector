using System;

namespace FullInspector {
    /// <summary>
    /// Draws a divider (horizontal line) above the given field or property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InspectorDividerAttribute : Attribute, IInspectorAttributeOrder {
        public double Order = 50;

        double IInspectorAttributeOrder.Order {
            get {
                return Order;
            }
        }
    }
}