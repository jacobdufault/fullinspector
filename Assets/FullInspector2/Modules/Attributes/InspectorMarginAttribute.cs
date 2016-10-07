using System;

namespace FullInspector {
    /// <summary>
    /// Adds whitespace above the given field or property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InspectorMarginAttribute : Attribute, IInspectorAttributeOrder {
        public int Margin;

        public double Order = 0;

        public InspectorMarginAttribute(int margin) {
            Margin = margin;
        }

        double IInspectorAttributeOrder.Order {
            get {
                return Order;
            }
        }
    }
}