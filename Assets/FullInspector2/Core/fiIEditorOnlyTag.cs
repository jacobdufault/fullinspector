namespace FullInspector.Internal {
    /// <summary>
    /// Any UnityObject type tagged with this interface will *not* be included in a published
    /// build. This is useful if there are components that should be editor-only.
    /// </summary>
    /// <remarks>The EditorOnlyMonoBehaviorRemover processor does the actual MonoBehaviour removal</remarks>
    public interface fiIEditorOnlyTag { }
}