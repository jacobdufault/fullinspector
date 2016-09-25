using System;

namespace FullInspector {
    /// <summary>
    /// The ShowInInspectorAttribute causes the given field or property to be shown in the
    /// inspector, even if it is not public. This is the inverse of Unity's [HideInInspector]
    /// attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ShowInInspectorAttribute : Attribute {
    }
}