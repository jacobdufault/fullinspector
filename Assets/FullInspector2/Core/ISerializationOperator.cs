using UnityObject = UnityEngine.Object;

namespace FullInspector {
    /// <summary>
    /// An ISerializationOperator provides a set of common serialization routines for serializers.
    /// </summary>
    public interface ISerializationOperator {
        /// <summary>
        /// Retrieve an object that has been stored with StoreObjectReference.
        /// </summary>
        /// <param name="storageId">The id that was returned from the store operation.</param>
        /// <returns>The object that was previously stored. The object may have been destroyed (for
        /// example, when an object becomes a prefab it loses links to non-prefab objects), so make
        /// sure that you handle a null return value correctly.</returns>
        UnityObject RetrieveObjectReference(int storageId);

        /// <summary>
        /// Returns an integer that can be used to fetch the given object after Unity has gone
        /// through a serialization cycle.
        /// </summary>
        /// <param name="obj">The object to get an identifier for.</param>
        /// <returns>An integer that uniquely identifies the given obj. obj can be recovered with
        /// RetrieveObject</returns>
        int StoreObjectReference(UnityObject obj);
    }
}