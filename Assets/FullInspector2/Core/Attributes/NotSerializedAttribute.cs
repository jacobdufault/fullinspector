using System;

namespace FullInspector {
    /// <summary>
    /// Identical to a [NonSerialized] attribute except that it is also usable on properties
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class NotSerializedAttribute : Attribute {
    }
}