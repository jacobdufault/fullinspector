using System;
using System.Collections.Generic;
using System.Reflection;
using FullInspector.Internal;
using FullSerializer.Internal;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector {
    /// <summary>
    /// This partial implementation contains methods which are used to determine
    /// if a MemberInfo can be serialized by Unity or if it can be serialized by
    /// Full Inspector.
    /// </summary>
    public partial class InspectedType {
        /// <summary>
        /// A simple type is a type that is either primitive, a string, or a
        /// non-generic non-abstract class composed of other simple types.
        /// </summary>
        private static bool IsSimpleTypeThatUnityCanSerialize(Type type) {
            if (IsPrimitiveSkippedByUnity(type)) {
                return false;
            }

            if (type.Resolve().IsPrimitive) {
                return true;
            }

            if (type == typeof(string)) {
                return true;
            }

            // TODO: Enable more complex detection for types which Unity can
            //       serialize by uncommenting this block. First, test it,
            //       though.
            /*
            if (type.IsClass && type.IsGenericType == false && type.IsAbstract == false) {
                BindingFlags flags =
                        BindingFlags.Instance |
                        BindingFlags.FlattenHierarchy |
                        BindingFlags.Public |
                        BindingFlags.NonPublic;

                // The type might contain a property that should be serialized.
                if (type.GetProperties(flags).Length > 0) {
                    return false;
                }

                // The type can have fields composed only of other simple types.
                // TODO: will this recurse infinitely for recursive types?
                foreach (var field in type.GetFields(flags)) {
                    if (IsSimpleType(field.FieldType) == false) {
                        return false;
                    }
                }

                return true;
            }
            */

            return false;
        }

        /// <summary>
        /// Returns true if the primitive is *not* serialized by Unity
        /// </summary>
        private static bool IsPrimitiveSkippedByUnity(Type type) {
            return
                type == typeof(ushort) ||
                type == typeof(uint) ||
                type == typeof(ulong) ||
                type == typeof(sbyte);
        }

        /// <summary>
        /// Returns true if the given type can be serialized by Unity. This
        /// function is conservative and may not return true if the type can be
        /// serialized by unity. However, it will *not* return true if the type
        /// cannot be serialized by unity.
        /// </summary>
        public static bool IsSerializedByUnity(InspectedProperty property) {
            // Properties are *not* serialized by Unity
            if (property.MemberInfo is PropertyInfo) {
                return false;
            }

            // statics are *not* serialized by Unity
            if (property.IsStatic) {
                return false;
            }

            // readonly fields are *not* serialized by Unity
            FieldInfo fieldInfo = property.MemberInfo as FieldInfo;
            if (fieldInfo.IsInitOnly) {
                return false;
            }

            // If the attribute is not public and doesn't have a [SerializeField]
            // attribute, then Unity will not serialize it, regardless of type.
            if (property.IsPublic == false &&
                property.MemberInfo.IsDefined(typeof(SerializeField), /*inherit:*/ true) == false) {
                return false;
            }

            Type type = property.StorageType;

            return
                // Basic primitive types
                IsSimpleTypeThatUnityCanSerialize(type) ||

                // A non-generic UnityObject derived type
                (typeof(UnityObject).IsAssignableFrom(type) && type.Resolve().IsGenericType == false) ||

                // Array (but not a multidimensional one)
                (type.IsArray && type.GetElementType().IsArray == false && IsSimpleTypeThatUnityCanSerialize(type.GetElementType())) ||

                // Lists of already serializable types
                (
                    type.Resolve().IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(List<>) &&
                    IsSimpleTypeThatUnityCanSerialize(type.GetGenericArguments()[0])
                );
        }

        /// <summary>
        /// Returns true if the given property should be serialized.
        /// </summary>
        /// <remarks>This assumes that the property is r/w!</remarks>
        public static bool IsSerializedByFullInspector(InspectedProperty property) {
            if (property.IsStatic) {
                return false;
            }

            // Do not bother serializing BaseObject derived types - they will
            // handle it properly themselves. Serializing them will only lead to
            // wasted storage space.
            if (typeof(BaseObject).Resolve().IsAssignableFrom(property.StorageType.Resolve())) {
                return false;
            }

            MemberInfo member = property.MemberInfo;

            // if it has NonSerialized, then we *don't* serialize it
            if (fsPortableReflection.HasAttribute<NonSerializedAttribute>(member, /*shouldCache:*/false) ||
                fsPortableReflection.HasAttribute<NotSerializedAttribute>(member, /*shouldCache:*/false)) {
                return false;
            }

            // Don't serialize it if it has one of the custom opt-out annotations
            // either
            var optOut = fiInstalledSerializerManager.SerializationOptOutAnnotations;
            for (int i = 0; i < optOut.Length; ++i) {
                if (member.IsDefined(optOut[i], true)) {
                    return false;
                }
            }

            // if we have a [SerializeField] or [Serializable] attribute, then we
            // *do* serialize
            if (fsPortableReflection.HasAttribute<SerializeField>(member, /*shouldCache:*/false) ||
                fsPortableReflection.HasAttribute<SerializableAttribute>(member, /*shouldCache:*/false)) {
                return true;
            }

            // Serialize if we have a custom opt-in annotation
            var optIn = fiInstalledSerializerManager.SerializationOptInAnnotations;
            for (int i = 0; i < optIn.Length; ++i) {
                if (member.IsDefined(optIn[i], /*inherit:*/ true)) {
                    return true;
                }
            }

            if (property.MemberInfo is PropertyInfo) {
                // perf: property.IsAutoProperty is lazily computed, so if we
                //       have SerializeAutoProperties set to false we can avoid
                //       computing any auto-property checks by putting the
                //       SerializeAutoProperties check before the IsAutoProperty
                //       check

                // If we're not serializing auto-properties, then we will not
                // serialize any properties by default
                if (fiSettings.SerializeAutoProperties == false) return false;

                // If it's not an auto property, then we are not going to
                // serialize it by default
                if (property.IsAutoProperty == false) return false;
            }

            return property.IsPublic;
        }
    }
}