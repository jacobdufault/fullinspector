using System;
using System.Collections.Generic;

namespace FullInspector {
    public partial class InspectedType {
        /// <summary>
        /// Cache from Type to the respective InspectedType.
        /// </summary>
        private static Dictionary<Type, InspectedType> _cachedMetadata =
            new Dictionary<Type, InspectedType>();

        /// <summary>
        /// Finds the associated InspectedType for the given type.
        /// </summary>
        /// <param name="type">The type to find the type metadata for.</param>
        /// <returns>A TypeMetadata that models the given type.</returns>
        public static InspectedType Get(Type type) {
            InspectedType metadata;
            if (_cachedMetadata.TryGetValue(type, out metadata) == false) {
                metadata = new InspectedType(type);
                _cachedMetadata[type] = metadata;
            }
            return metadata;
        }

        /// <summary>
        /// Reset the cached set of metadata. Should only be used in tests, as this will
        /// significantly impact performance.
        /// </summary>
        public static void ResetCacheForTesting() {
            _cachedMetadata = new Dictionary<Type, InspectedType>();
        }
    }
}