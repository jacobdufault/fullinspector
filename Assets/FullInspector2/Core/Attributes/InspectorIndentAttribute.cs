using System;

namespace FullInspector {
    /// <summary>
    /// Indents the given editor. This can be useful combined with [InspectorHeader] to draw
    /// an indented region in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InspectorIndentAttribute : Attribute, IInspectorAttributeOrder {
        /// <summary>
        /// The ordering of this item in the set of attributes. A low number will display before
        /// (above) a higher number.
        /// </summary>
        public double Order = 100;

        double IInspectorAttributeOrder.Order {
            get {
                return Order;
            }
        }
    }
}