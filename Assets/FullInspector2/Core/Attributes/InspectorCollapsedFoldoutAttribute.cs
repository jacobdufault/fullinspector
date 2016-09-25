using System;

namespace FullInspector {
    /// <summary>
    /// Forces the given field or property to be collapsed in the inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class InspectorCollapsedFoldoutAttribute : Attribute {
    }
}