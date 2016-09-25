namespace FullInspector {
    /// <summary>
    /// Attributes can opt-in to this interface and request support for custom ordering in the
    /// inspector. Attributes are ordered in reverse for display; that is, a low attribute number
    /// will display before a high attribute number.
    /// </summary>
    public interface IInspectorAttributeOrder {
        /// <summary>
        /// The ordering of this item in the set of attributes. A low number will display before
        /// (above) a higher number.
        /// </summary>
        double Order {
            get;
        }
    }
}