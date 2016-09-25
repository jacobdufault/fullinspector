using System;
using UnityObject = UnityEngine.Object;

namespace FullInspector {
    /// <summary>
    /// An ISerializationOperator that just throws NotSupportedExceptions, useful when serializing
    /// an object to disk where UnityObject references cannot be handled.
    /// </summary>
    public class NotSupportedSerializationOperator : ISerializationOperator {
        public UnityObject RetrieveObjectReference(int storageId) {
            throw new NotSupportedException("UnityEngine.Object references are not supported " +
                "with this serialization operator");
        }

        public int StoreObjectReference(UnityObject obj) {
            throw new NotSupportedException("UnityEngine.Object references are not supported " +
                "with this serialization operator (obj=" + obj + ")");
        }
    }
}