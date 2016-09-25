using System;

namespace FullInspector {
    /// <summary>
    /// Use this if you wish for each item inside of a collection to have a dropdown arrow. This is
    /// disabled by default as it can cause multiple dropdown arrows to be shown next to each-other
    /// in certain scenarios.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class InspectorCollectionShowItemDropdownAttribute : Attribute {
        public bool IsCollapsedByDefault = true;
    }
}
