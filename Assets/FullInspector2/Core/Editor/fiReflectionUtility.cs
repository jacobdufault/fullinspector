using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FullSerializer;
using FullSerializer.Internal;
using UnityEngine;

namespace FullInspector.Internal {
    public static class fiReflectionUtility {
        // TODO: move this to fiEditorReflectionUtility

        /// <summary>
        /// A cache of all types that derive the key type in the AppDomain. The cache is
        /// automatically destroyed upon assembly loads.
        /// </summary>
        private static Dictionary<Type, List<DisplayedType>> _creatableTypeCache;
        public struct DisplayedType {
            public Type Type;
            public string DisplayName;

            public DisplayedType(Type type) {
                Type = type;
                DisplayName = string.Empty;

                var attr = fsPortableReflection.GetAttribute<InspectorDropdownNameAttribute>(type, /*shouldCache:*/false);
                if (attr != null)
                    DisplayName = attr.DisplayName;

                if (string.IsNullOrEmpty(DisplayName))
                    DisplayName = type.CSharpName();
            }
        }

        static fiReflectionUtility() {
            _creatableTypeCache = new Dictionary<Type, List<DisplayedType>>();

            AppDomain.CurrentDomain.AssemblyLoad += (o, e) => {
                _creatableTypeCache = new Dictionary<Type, List<DisplayedType>>();
            };
        }

        /// <summary>
        /// Returns all types that derive from the base type. This includes generic type
        /// definitions, which when returned will have appropriate constructor values injected.
        /// </summary>
        /// <param name="baseType">The base parent type.</param>
        public static List<DisplayedType> GetCreatableTypesDeriving(Type baseType) {
            return (from displayedType in GetTypesDeriving(baseType)
                    let type = displayedType.Type

                    // cannot be abstract, an interface, or an open generic type
                    where type.IsAbstract == false
                    where type.IsInterface == false
                    where type.IsGenericTypeDefinition == false

                    select displayedType).ToList();
        }
        public static List<DisplayedType> GetTypesDeriving(Type baseType) {
            List<DisplayedType> result;
            if (_creatableTypeCache.TryGetValue(baseType, out result) == false) {
                result = new List<DisplayedType>();
                _creatableTypeCache[baseType] = result;

                // fast path for types which are sealed and have no derived types
                if (baseType.IsSealed) {
                    result.Add(new DisplayedType(baseType));
                }

                else {
                    // non-generic types
                    foreach (var type in from assembly in fiRuntimeReflectionUtility.GetUserDefinedEditorAssemblies()
                                         from type in assembly.GetTypesWithoutException()

                                             // extends the base type
                                         where baseType.IsAssignableFrom(type)

                                         select type) {
                        result.Add(new DisplayedType(type));
                    }

                    // generic types
                    if (baseType.IsGenericType) {
                        Type baseTypeGenericDefinition = baseType.GetGenericTypeDefinition();
                        Type[] baseTypeGenericArguments = baseType.GetGenericArguments();

                        foreach (var openGenericType in from assembly in fiRuntimeReflectionUtility.GetUserDefinedEditorAssemblies()
                                                        from type in assembly.GetTypesWithoutException()
                                                        where type.GetExactImplementation(baseTypeGenericDefinition) != null
                                                        select type) {

                            Type constructed;
                            if (TryConstructAssignableGenericType(openGenericType,
                                baseType, baseTypeGenericDefinition, baseTypeGenericArguments,
                                out constructed)) {
                                result.Add(new DisplayedType(constructed));
                            };
                        }
                    }

                    result.Sort((a, b) => a.DisplayName.CompareTo(b.DisplayName));
                }
            }

            return result;
        }

        /// <summary>
        /// A candidate type that can potentially be used as a generic parameter argument.
        /// </summary>
        private struct GenericParameterCandidate {
            /// <summary>
            /// The actual type that will be the generic parameter.
            /// </summary>
            public Type Type;

            /// <summary>
            /// The name of the generic parameter that this type came from.
            /// </summary>
            public string SourceParameterName;
        }

        /// <summary>
        /// Helper method to return the index of the Type in the array that has the given Name (or
        /// - 1 if the item is not in the array).
        /// </summary>
        private static int GetIndexOfName(Type[] types, string name) {
            for (int i = 0; i < types.Length; ++i) {
                if (types[i].Name == name) {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Orders the given types parameter array so that the order most closely matches the given
        /// genericArguments array
        /// </summary>
        /// <param name="candidateTypes">The types that have been collected as potential candidates
        /// for generic parameter arguments</param>
        /// <param name="openGenericType">The type that we are going to use the candidateTypes for
        /// constructing a closed generic type on</param>
        /// <returns>An array of types that can (hopefully) be used to construct a closed generic
        /// type from the given openGenericType.</returns>
        private static Type[] SelectBestTypeParameters(
            List<GenericParameterCandidate> candidateTypes, Type openGenericType) {
            Type[] genericArguments = openGenericType.GetGenericArguments();

            // figure out how many generic parameters we actually need
            int requiredParameters = 0;
            foreach (var type in genericArguments) {
                if (type.IsGenericParameter) {
                    ++requiredParameters;
                }
            }

            var ordered = new Type[requiredParameters];

            // match types into the ordered array based on their name
            var overflow = new List<Type>();
            for (int i = 0; i < candidateTypes.Count; ++i) {
                int index = GetIndexOfName(genericArguments, candidateTypes[i].SourceParameterName);
                if (index == -1) {
                    overflow.Add(candidateTypes[i].Type);
                }
                else {
                    ordered[index] = candidateTypes[i].Type;
                }
            }

            // there were some types that didn't match by name; just insert them first come first
            // serve
            int start = 0;
            foreach (var type in overflow) {
                while (start < ordered.Length && ordered[start] != null) {
                    ++start;
                }

                if (start >= ordered.Length) {
                    break;
                }

                ordered[start] = type;
            }

            return ordered;
        }

        /// <summary>
        /// Attempts to create an instance of openGenericType such that it is can be assigned to
        /// baseType.
        /// </summary>
        /// <param name="openGenericType">An open generic type that derives baseType</param>
        /// <param name="baseType">A generic type with fully populated type parameters.</param>
        /// <param name="baseTypeGenericDefinition">Just baseType.GetGenericTypeDefinition()</param>
        /// <param name="baseTypeGenericArguments">Just baseType.GetGenericArguments()</param>
        /// <param name="constructedType">If this function returns true, then this value is set to
        /// the created type that is assignable to baseType and an instance of
        /// openGenericType.</param>
        /// <returns>True if a type was constructed, false otherwise.</returns>
        private static bool TryConstructAssignableGenericType(Type openGenericType,
            Type baseType, Type baseTypeGenericDefinition, Type[] baseTypeGenericArguments,
            out Type constructedType) {

            // The base type populated with openGenericType's generic parameters.
            //
            // For example, if
            //- openGenericType = Derived{T1} : Iface{object, T1}
            //- then
            //- typeBaseType = IFace{object, T1}
            //- currentParameters = new Type[] { typeof(object), T1 }
            Type typeBaseType = GetExactImplementation(openGenericType, baseTypeGenericDefinition);
            Type[] currentParameters = typeBaseType.GetGenericArguments();

            // We want to iterate over all of the current generic parameters for typeBaseType. If
            // some of them are unspecified, then we will use those types from the baseType to
            // construct our constructedType instance.
            //
            // For example, if
            //- typeBaseType = Iface{object, T1, int, T3}
            //- baseType = Iface{object, int, int, double}
            //- then
            //- typeParams = new List{Type} { typeof(int, double) }.
            var typeParamCandidates = new List<GenericParameterCandidate>();
            for (int i = 0; i < currentParameters.Length; ++i) {
                if (currentParameters[i].IsGenericParameter) {
                    typeParamCandidates.Add(new GenericParameterCandidate() {
                        Type = baseTypeGenericArguments[i],
                        SourceParameterName = currentParameters[i].Name
                    });
                }
            }

            Type[] orderedTypeParams = SelectBestTypeParameters(typeParamCandidates, openGenericType);

            // Try to construct the generic type from our unspecified parameters. This could fail
            // for a number of reasons, but for now we will just ignore them and say we failed to
            // create the generic type. We accept that this method will never be fully perfect.
            try {
                Type createdType = openGenericType.MakeGenericType(orderedTypeParams);
                if (baseType.IsAssignableFrom(createdType)) {
                    constructedType = createdType;
                    return true;
                }
            }
            catch (Exception) { }

            constructedType = null;
            return false;
        }

        /// <summary>
        /// Searches for a particular implementation of the given parent type inside of the type.
        /// This is particularly useful if the interface type is an open type, ie, typeof(IFace{}),
        /// because this method will then return IFace{} but with appropriate type parameters
        /// inserted.
        /// </summary>
        /// <param name="type">The base type to search for interface</param>
        /// <param name="parentType">The parent type to search for. Can be an open generic
        /// type.</param>
        /// <returns>The actual interface type that the type contains, or null if there is no
        /// implementation of the given interfaceType on type.</returns>
        public static Type GetExactImplementation(this Type type, Type parentType) {
            if (parentType.IsGenericType && parentType.IsGenericTypeDefinition == false) {
                throw new ArgumentException("GetInterface requires that if the parent " +
                    "type is generic, then it must be the generic type definition, not a " +
                    "specific generic type instantiation");
            };

            while (type != null) {
                if ((type.IsGenericType && type.GetGenericTypeDefinition() == parentType) ||
                    (type == parentType)) {
                    return type;
                }

                foreach (var iface in type.GetInterfaces()) {
                    if ((iface.IsGenericType && iface.GetGenericTypeDefinition() == parentType) ||
                        (iface == parentType)) {
                        return iface;
                    }
                }

                type = type.BaseType;
            }

            return null;
        }


        /// <summary>
        /// Tries to fetch the given CSharpName of the given object type, or null if the object is
        /// null.
        /// </summary>
        public static string GetObjectTypeNameSafe(object obj) {
            if (obj == null) return "null";
            return obj.GetType().CSharpName();
        }

        /// <summary>
        /// Returns the value of a boolean, field, or property.
        /// </summary>
        public static bool GetBooleanReflectedMember(Type elementType, object element, string memberName, bool defaultValue) {
            MemberInfo[] members = elementType.GetFlattenedMember(memberName);
            if (members == null || members.Length == 0) {
                Debug.LogError("Could not find a member with name " + memberName + " on object type " + elementType);
                return true;
            }

            MemberInfo member = members[0];

            object result = defaultValue;

            try {
                if (member is FieldInfo) {
                    result = ((FieldInfo)member).GetValue(element);
                }
                else if (member is PropertyInfo) {
                    result = ((PropertyInfo)member).GetValue(element, null);
                }
                else if (member is MethodInfo) {
                    result = ((MethodInfo)member).Invoke(element, null);
                }
            }
            catch (Exception e) {
                Debug.LogError("When running " + member.ReflectedType + "::" + member.Name + ", caught exception " + e);
                Debug.LogException(e);
                result = defaultValue;
            }

            if (result.GetType() != typeof(bool)) {
                Debug.LogError(member.ReflectedType + "::" + member.Name + " needs to return a bool to be used with [InspectorDisplayIf]");
                result = defaultValue;
            }

            return (bool)result;
        }
    }
}