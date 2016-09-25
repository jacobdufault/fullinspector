using System;

namespace FullInspector {
    /// <summary>
    /// Draws a header above a property, with some nice text to go along with it. This is an
    /// analog to Unity's [Header] attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InspectorHeaderAttribute : Attribute, IInspectorAttributeOrder {
        /// <summary>
        /// The ordering of this item in the set of attributes. A low number will display before
        /// (above) a higher number.
        /// </summary>
        public double Order = 75;

        /// <summary>
        /// The displayed header.
        /// </summary>
        public string Header;

        /// <summary>
        /// Draws a header above the given field or property/
        /// </summary>
        /// <param name="header">The header to display.</param>
        public InspectorHeaderAttribute(string header) {
            Header = header;
        }

        double IInspectorAttributeOrder.Order {
            get {
                return Order;
            }
        }
    }
}