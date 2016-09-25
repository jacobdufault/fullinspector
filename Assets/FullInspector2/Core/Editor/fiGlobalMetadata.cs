using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullInspector.Internal {
    /// <summary>
    /// Returns an associated object for another object.
    /// </summary>
    internal static class fiGlobalMetadata {
        /// <summary>
        /// A mapping from the objects that contain metadata to their metadata.
        /// </summary>
        private static Dictionary<object, Dictionary<Type, object>> _items =
            new Dictionary<object, Dictionary<Type, object>>();

        /// <summary>
        /// Returns true if there is metadata for the given item.
        /// </summary>
        public static bool Has(object item) {
            return
                item == null ||
                _items.ContainsKey(item);
        }

        /// <summary>
        /// Sets the metadata instance for the given item and type.
        /// </summary>
        public static void Set<T>(object item, T metadataItem) where T : class {
            if (typeof(T) == typeof(fiGraphMetadata)) {
                Debug.LogWarning("Please use fiGraphMetadata.GetGlobal instead of fiGlobalMetadata, as it will persist across serialization cycles");
            }

            if (metadataItem == null) {
                throw new ArgumentNullException("metadataItem");
            }

            Dictionary<Type, object> metadata;
            if (_items.TryGetValue(item, out metadata) == false) {
                metadata = new Dictionary<Type, object>();
                _items[item] = metadata;
            }

            metadata[typeof(T)] = metadataItem;
        }

        /// <summary>
        /// Fetch a metadata instance for the given item.
        /// </summary>
        public static T Get<T>(object item) where T : class, new() {
            if (typeof(T) == typeof(fiGraphMetadata)) {
                Debug.LogWarning("Please use fiGraphMetadata.GetGlobal instead of fiGlobalMetadata, as it will persist across serialization cycles");
            }

            if (item == null) {
                return new T();
            }

            Dictionary<Type, object> metadata;
            if (_items.TryGetValue(item, out metadata) == false) {
                metadata = new Dictionary<Type, object>();
                _items[item] = metadata;
            }

            object result;
            if (metadata.TryGetValue(typeof(T), out result) == false) {
                result = new T();
                metadata[typeof(T)] = result;
            }

            return (T)result;
        }
    }
}