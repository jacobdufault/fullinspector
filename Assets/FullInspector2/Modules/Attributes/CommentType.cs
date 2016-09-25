namespace FullInspector {
    /// <summary>
    /// A comment can have an associated message box info header associated with it. This enum
    /// describes the header options.
    /// </summary>
    public enum CommentType {
        /// <summary>
        /// No header. This is the default comment style.
        /// </summary>
        None = 0,

        /// <summary>
        /// Display an "info" image next to the comment text.
        /// </summary>
        Info = 1,

        /// <summary>
        /// Display a "warning" image next to the comment text.
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Display an "error" image next to the comment text.
        /// </summary>
        Error = 3,
    }
}