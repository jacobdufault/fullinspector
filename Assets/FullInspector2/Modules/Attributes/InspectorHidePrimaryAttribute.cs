using System;

namespace FullInspector {
    /// <summary>
    /// Do not display the primary inspector. Only attribute property editors will be shown for
    /// the given field or property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InspectorHidePrimaryAttribute : Attribute, IInspectorAttributeOrder {
        double IInspectorAttributeOrder.Order {
            get {
                return double.MaxValue;
            }
        }
    }
}