using System;

namespace FullInspector {
    /// <summary>
    /// This attribute forces indices to be shown next to the items inside of the edited
    /// collection. This is only applicable to collection types (lists, arrays, dictionaries, etc).
    /// </summary>
    [Obsolete("Use [InspectorCollectionRotorzFlags(ShowIndices=true)] instead")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class InspectorCollectionShowIndicesAttribute : Attribute {
    }
}
