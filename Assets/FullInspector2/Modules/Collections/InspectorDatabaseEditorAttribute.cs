using System;

namespace FullInspector {
    /// <summary>
    /// Changes the default editor for IList{T} types to be one that only edits a single item in the
    /// collection at a single time. This can be extremely useful, if, for example, you're editing
    /// an extremely large list or just want to reduce information overload.
    /// </summary>
    [Obsolete("Please use [InspectorDatabaseEditor] instead")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SingleItemListEditorAttribute : Attribute {
    }

    /// <summary>
    /// Changes the default editor for IList{T} types to be one that only edits a single item in the
    /// collection at a single time. This can be extremely useful, if, for example, you're editing
    /// an extremely large list or just want to reduce information overload.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class InspectorDatabaseEditorAttribute : Attribute {
    }
}