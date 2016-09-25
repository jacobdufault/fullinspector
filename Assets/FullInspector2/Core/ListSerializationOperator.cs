using System;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    /// <summary>
    /// An ISerializationOperator that uses a list for reference storage.
    /// </summary>
    public class ListSerializationOperator : ISerializationOperator {
        /// <summary>
        /// A list of objects that have been serialized.
        /// </summary>
        public List<UnityObject> SerializedObjects;

        public UnityObject RetrieveObjectReference(int storageId) {
            if (SerializedObjects == null) {
                throw new InvalidOperationException("SerializedObjects cannot be null");
            }

            // The storageId is invalid; we can only return null.
            if (storageId < 0 || storageId >= SerializedObjects.Count) {
                return null;
            }

            return SerializedObjects[storageId];
        }

        public int StoreObjectReference(UnityObject obj) {
            if (SerializedObjects == null) {
                throw new InvalidOperationException("SerializedObjects cannot be null");
            }

            // We don't have to bother storing null
            if (ReferenceEquals(obj, null)) {
                return -1;
            }

            int index = SerializedObjects.Count;
            SerializedObjects.Add(obj);
            return index;
        }
    }
}