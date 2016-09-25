using FullSerializer.Internal;
using System;
using System.Reflection;

namespace FullInspector {
    /// <summary>
    /// The core API that is used for serializing and deserializing objects.
    /// </summary>
    public abstract class BaseSerializer {
        /// <summary>
        /// Serialize the given object into a string.
        /// </summary>
        /// <param name="storageType">The type of field/property that is storing this value. For
        /// example, an object field may be storing an int instance. In that case, storageType will
        /// be typeof(object), while value.GetType() will result in typeof(int).</param>
        /// <param name="value">The object to serialize.</param>
        /// <param name="serializationOperator">Contains implementations for common serialization
        /// operations, such as storing a UnityObject reference.</param>
        /// <returns>The serialized state of the object that can be used to restore it.</returns>
        public abstract string Serialize(MemberInfo storageType, object value,
            ISerializationOperator serializationOperator);

        /// <summary>
        /// Deserialize the given serialized content.
        /// </summary>
        /// <param name="storageType">The type of field/property that is storing this value. For
        /// example, an object field may be storing an int instance. In that case, storageType will
        /// be typeof(object), while value.GetType() will result in typeof(int).</param>
        /// <param name="serializedState">The serialized state of the object, created by calling
        /// Serialize(target).</param>
        /// <param name="serializationOperator">Contains implementations for common serialization
        /// operations, such as storing a UnityObject reference.</param>
        public abstract object Deserialize(MemberInfo storageType, string serializedState,
            ISerializationOperator serializationOperator);

        /// <summary>
        /// Does this serializer support concurrent serialization/deserialization? By default, this is false.
        /// </summary>
        public virtual bool SupportsMultithreading {
            get { return false; }
        }

        /// <summary>
        /// Helper function that returns the type of object stored within the given member.
        /// </summary>
        protected static Type GetStorageType(MemberInfo member) {
            if (member is FieldInfo) {
                return ((FieldInfo)member).FieldType;
            }

            if (member is PropertyInfo) {
                return ((PropertyInfo)member).PropertyType;
            }

            if (fsPortableReflection.IsType(member)) {
                return fsPortableReflection.AsType(member);
            }

            throw new InvalidOperationException("Unknown member type " + member);
        }
    }
}