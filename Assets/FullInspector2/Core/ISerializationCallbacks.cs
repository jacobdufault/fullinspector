namespace FullInspector {
    /// <summary>
    /// Extend this interface on any BaseBehavior or BaseScriptableObject type to receive callbacks
    /// for when Full Inspector runs serialization. These callbacks may or may not get invoked on
    /// the main Unity thread.
    /// </summary>
    /// <remarks>
    /// These functions will *not* get invoked if the type does not extend ISerializedObject (either
    /// BaseScriptableObject, BaseBehavior, or BaseObject). Use the serializer-specific callbacks for that.
    /// </remarks>
    public interface ISerializationCallbacks {
        /// <summary>
        /// Called right before FI runs serialization.
        /// </summary>
        void OnBeforeSerialize();

        /// <summary>
        /// Called right after FI runs serialization.
        /// </summary>
        void OnAfterSerialize();

        /// <summary>
        /// Called right before FI runs deserialization.
        /// </summary>
        void OnBeforeDeserialize();

        /// <summary>
        /// Called right after FI runs deserialization.
        /// </summary>
        void OnAfterDeserialize();
    }
}