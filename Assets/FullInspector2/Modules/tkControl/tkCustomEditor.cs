namespace FullInspector {
    /// <summary>
    /// If a type extends this interface, then the given tkControlEditor will
    /// be used for rendering the inspector instead of the typical reflected
    /// inspector. This allows for extremely easy inspector customization.
    /// </summary>
    public interface tkCustomEditor {
        tkControlEditor GetEditor();
    }
}