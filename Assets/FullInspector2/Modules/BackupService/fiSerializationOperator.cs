using System;
using System.Collections.Generic;
using FullInspector.Internal;
using UnityObject = UnityEngine.Object;

namespace FullInspector.BackupService {
    /// <summary>
    /// An ISerializationOperator that uses a list for reference storage and targets
    /// fiPersistentObjects instead of regular UnityObject references.
    /// </summary>
    public class fiSerializationOperator : ISerializationOperator {
        /// <summary>
        /// A list of objects that have been serialized.
        /// </summary>
        public List<fiUnityObjectReference> SerializedObjects;

        public UnityObject RetrieveObjectReference(int storageId) {
            if (SerializedObjects == null) {
                throw new InvalidOperationException("SerializedObjects cannot be  null");
            }

            // The storageId is invalid; we can only return null.
            if (storageId < 0 || storageId >= SerializedObjects.Count) {
                return null;
            }

            fiUnityObjectReference found = SerializedObjects[storageId];

            // Unity is funny; it overrides null so such that ReferenceEquals(found, null) can
            // return false when found == null is true. If the serialized value became null (such as
            // when it becomes a prefab and references a non-prefab object), we want to return the
            // actual, correct, proper null value, instead of some whatever magical thing. The bad
            // null value will cause property assignment exceptions (because it is of type
            // UnityEngine.Component)
            if (found == null || found.Target == null) {
                return null;
            }

            return found.Target;
        }

        public int StoreObjectReference(UnityObject obj) {
            if (SerializedObjects == null) {
                throw new InvalidOperationException("SerializedObjects cannot be null");
            }

            // We don't have to bother storing null
            if (obj == null) {
                return -1;
            }

            int index = SerializedObjects.Count;
            SerializedObjects.Add(new fiUnityObjectReference(obj, /*tryRestore:*/ true));
            return index;
        }
    }
}