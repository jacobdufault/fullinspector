using System;

namespace FullInspector {
    /// <summary>
    /// This will cause Full Inspector to treat the given target class as a
    /// nullable property, ie, it does not have to have an instance allocated. If
    /// you're using a struct, just mark the type nullable with ?, ie, obj?, and
    /// the nullable editor will automatically be used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class InspectorNullableAttribute : Attribute { }
}