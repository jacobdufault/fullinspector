using System;
using System.Collections.Generic;
using FullSerializer.Internal;

namespace FullInspector.Internal {
    /// <summary>
    /// Returns the serializer that is used for a given behavior type.
    /// </summary>
    public static class BehaviorTypeToSerializerTypeMap {
        private struct SerializationMapping {
            public Type BehaviorType;
            public Type SerializerType;
        }
        private static List<SerializationMapping> _mappings = new List<SerializationMapping>();

        /// <summary>
        /// Register a mapping for a particular behavior type to a given serializer type.
        /// </summary>
        /// <param name="behaviorType"></param>
        /// <param name="serializerType"></param>
        public static void Register(Type behaviorType, Type serializerType) {
            _mappings.Add(new SerializationMapping() {
                BehaviorType = behaviorType,
                SerializerType = serializerType
            });
        }

        /// <summary>
        /// Returns the serializer type that the given behavior type uses.
        /// </summary>
        public static Type GetSerializerType(Type behaviorType) {
            for (int i = 0; i < _mappings.Count; ++i) {
                var mapping = _mappings[i];
                if (mapping.BehaviorType.Resolve().IsAssignableFrom(behaviorType.Resolve())) {
                    return mapping.SerializerType;
                }
            }

            // No custom serializer, use the default one
            return fiInstalledSerializerManager.DefaultMetadata.SerializerType;
        }
    }
}
