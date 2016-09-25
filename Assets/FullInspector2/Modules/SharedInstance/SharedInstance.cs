namespace FullInspector {
    /// <summary>
    /// A SharedInstance{T} contains an instance of type T whose instance is shared across multiple MonoBehaviour instances.
    /// </summary>
    /// <typeparam name="TInstance">The object type to store.</typeparam>
    /// <typeparam name="TSerializer">The type of serializer to use.</typeparam>
    public class SharedInstance<TInstance, TSerializer> : BaseScriptableObject<TSerializer>
        where TSerializer : BaseSerializer {

        /// <summary>
        /// The shared object instance.
        /// </summary>
        public TInstance Instance;
    }
}