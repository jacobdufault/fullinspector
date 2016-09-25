using System;
using System.Collections.Generic;

namespace FullInspector.Internal {
    /// <summary>
    /// Provides access to an arbitrary set of singleton objects such that the objects can be
    /// accessed in generic functions.
    /// </summary>
    public static class fiSingletons {
        /// <summary>
        /// The singleton instances.
        /// </summary>
        private static Dictionary<Type, object> _instances = new Dictionary<Type, object>();

        /// <summary>
        /// Retrieve a singleton of the given type. This method creates the object if it has not
        /// already been created.
        /// </summary>
        /// <typeparam name="T">The type of object to fetch an instance of.</typeparam>
        /// <returns>An object of the given type.</returns>
        public static T Get<T>() {
            return (T)Get(typeof(T));
        }

        /// <summary>
        /// Retrieve a singleton of the given type. This method creates the object if it has not
        /// already been created.
        /// </summary>
        /// <param name="type">The type of the object to fetch.</param>
        /// <returns>An object of the given type.</returns>
        public static object Get(Type type) {
            object result;

            if (_instances.TryGetValue(type, out result) == false) {
                result = Activator.CreateInstance(type);
                _instances[type] = result;
            }

            return result;
        }
    }
}