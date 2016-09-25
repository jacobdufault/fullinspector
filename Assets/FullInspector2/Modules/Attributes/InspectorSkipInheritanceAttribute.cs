using System;

namespace FullInspector {
    /// <summary>
    /// Prevent the drop-down type selection editor from being shown. This is especially useful
    /// for fields of type object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class InspectorSkipInheritanceAttribute : Attribute, IInspectorAttributeOrder {
        double IInspectorAttributeOrder.Order {
            get {
                return double.MinValue;
            }
        }
    }
}