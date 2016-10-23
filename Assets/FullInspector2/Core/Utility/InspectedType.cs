// The MIT License (MIT)
//
// Copyright (c) 2013-2014 Jacob Dufault
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using FullInspector.Internal;
using FullSerializer;
using FullSerializer.Internal;
using UnityEngine;
using UnityEngine.Serialization;

namespace FullInspector {
    /// <summary>
    /// Provides a view of an arbitrary type that unifies a number of discrete
    /// concepts in the CLR. Arrays and Collection types have special support,
    /// but their APIs are unified by the InspectedType so that they can be
    /// treated as if they were a regular type.
    /// </summary>
    public sealed partial class InspectedType {
        static InspectedType() {
            InitializePropertyRemoval();
        }

        /// <summary>
        /// Returns true if the type represented by this metadata contains a
        /// default constructor.
        /// </summary>
        public bool HasDefaultConstructor {
            get {
                if (_hasDefaultConstructorCache.HasValue == false) {
                    // arrays are considered to have a default constructor
                    if (_isArray) {
                        _hasDefaultConstructorCache = true;
                    }

                    // value types (ie, structs) always have a default
                    // constructor
                    else if (ReflectedType.Resolve().IsValueType) {
                        _hasDefaultConstructorCache = true;
                    }
                    else {
                        // consider private constructors as well
                        var ctor = ReflectedType.GetDeclaredConstructor(fsPortableReflection.EmptyTypes);
                        _hasDefaultConstructorCache = ctor != null;
                    }
                }

                return _hasDefaultConstructorCache.Value;
            }
        }
        private bool? _hasDefaultConstructorCache;

        /// <summary>
        /// Creates a new instance of the type that this metadata points back to.
        /// If this type has a default constructor, then Activator.CreateInstance
        /// will be used to construct the type (or Array.CreateInstance if it an
        /// array). Otherwise, an uninitialized object created via
        /// FormatterServices.GetSafeUninitializedObject is used to construct the
        /// instance.
        /// </summary>
        public object CreateInstance() {
            // Unity requires special construction logic for types that derive
            // from ScriptableObject. The normal inspector reflection logic will
            // create ScriptableObject instances if
            // fiSettings.AutomaticReferenceInstantation has been set to true.
            if (typeof(ScriptableObject).IsAssignableFrom(ReflectedType)) {
                return ScriptableObject.CreateInstance(ReflectedType);
            }

            // HACK: Constructing components is tricky, as they require a
            //       GameObject context. We fetch a GameObject from the active
            //       selection for the context to add the component to. If that
            //       doesn't work, then we construct an unformatted instance,
            //       which will be reported to the underlying system as null.
            //
            // TODO: Can this support multi-object selection? Very, very dirty.
            if (typeof(Component).IsAssignableFrom(ReflectedType)) {
                var activeGameObject = fiLateBindings.Selection.activeObject as GameObject;
                if (activeGameObject != null) {
                    // Try to fetch an existing instance
                    Component component = activeGameObject.GetComponent(ReflectedType);
                    if (component != null) {
                        return component;
                    }

                    // Failed -- add a fake "dead" instance that isn't attached
                    // to anything.
#if !(!UNITY_EDITOR && (UNITY_WP8 || UNITY_METRO))
                    return FormatterServices.GetSafeUninitializedObject(ReflectedType);
#endif
                }

#if !UNITY_EDITOR && (UNITY_WP8 || UNITY_METRO)
                throw new InvalidOperationException("InspectedType.CreateInstance is not supported for " +
                    ReflectedType + " on this Unity platform. FormatterServices is required for " +
                    "construction. Consider adding a default constructor");
#else
                Debug.LogWarning("No selected game object; constructing an unformatted instance (which will be null) for " + ReflectedType);
                return FormatterServices.GetSafeUninitializedObject(ReflectedType);
#endif
            }

            if (HasDefaultConstructor == false) {
#if !UNITY_EDITOR && (UNITY_WP8 || UNITY_METRO)
                throw new InvalidOperationException("InspectedType.CreateInstance is not supported for " +
                    ReflectedType + " on this Unity platform. FormatterServices is required for " +
                    "construction. Consider adding a default constructor");
#else
                return FormatterServices.GetSafeUninitializedObject(ReflectedType);
#endif
            }

            if (_isArray) {
                // we have to start with a size zero array otherwise it will have
                // invalid data inside of it
                return Array.CreateInstance(ReflectedType.GetElementType(), 0);
            }

            try {
                return Activator.CreateInstance(ReflectedType, /*nonPublic:*/ true);
            }
#if (!UNITY_EDITOR && (UNITY_METRO)) == false
            catch (MissingMethodException e) {
                throw new InvalidOperationException("Unable to create instance of " + ReflectedType + "; there is no default constructor", e);
            }
#endif
            catch (TargetInvocationException e) {
                throw new InvalidOperationException("Constructor of " + ReflectedType + " threw an exception when creating an instance", e);
            }
            catch (MemberAccessException e) {
                throw new InvalidOperationException("Unable to access constructor of " + ReflectedType, e);
            }
        }

        /// <summary>
        /// Returns all fields/properties/methods on the type that pass the given
        /// filter.
        /// </summary>
        /// <param name="filter">
        /// The filter to use. You may be interested in some predefined filters
        /// available in the InspectedMemberFilters type.
        /// </param>
        public List<InspectedMember> GetMembers(IInspectedMemberFilter filter) {
            VerifyNotCollection();

            List<InspectedMember> members;
            if (_cachedMembers.TryGetValue(filter, out members) == false) {
                // Not in the cache. Run each item through the filter and then
                // cache the result.
                members = new List<InspectedMember>();
                for (int i = 0; i < _allMembers.Count; ++i) {
                    var member = _allMembers[i];

                    bool allow;
                    if (member.IsProperty) allow = filter.IsInterested(member.Property);
                    else allow = filter.IsInterested(member.Method);

                    if (allow) {
                        members.Add(member);
                    }
                }

                _cachedMembers[filter] = members;
            }

            return members;
        }

        /// <summary>
        /// Returns all fields/properties on the type that pass the given filter.
        /// </summary>
        /// <param name="filter">
        /// The filter to use. You may be interested in some predefined filters
        /// available in the InspectedMemberFilters type.
        /// </param>
        public List<InspectedProperty> GetProperties(IInspectedMemberFilter filter) {
            VerifyNotCollection();

            List<InspectedProperty> properties;
            if (_cachedProperties.TryGetValue(filter, out properties) == false) {
                var members = GetMembers(filter);
                properties = (from member in members
                              where member.IsProperty
                              select member.Property).ToList();
                _cachedProperties[filter] = properties;
            }
            return properties;
        }

        /// <summary>
        /// Returns all methods on the type that pass the filter.
        /// </summary>
        /// <param name="filter">
        /// The filter to use. You may be interested in some predefined filters
        /// available in the InspectedMemberFilters type.
        /// </param>
        public List<InspectedMethod> GetMethods(IInspectedMemberFilter filter) {
            VerifyNotCollection();

            List<InspectedMethod> methods;
            if (_cachedMethods.TryGetValue(filter, out methods) == false) {
                var members = GetMembers(filter);
                methods = (from member in members
                           where member.IsMethod
                           select member.Method).ToList();
                _cachedMethods[filter] = methods;
            }
            return methods;
        }

        /// <summary>
        /// Verifies that the current type is not a collection.
        /// </summary>
        private void VerifyNotCollection() {
            if (IsCollection) {
                throw new InvalidOperationException("Operation not valid -- " + ReflectedType + " is a collection");
            }
        }

        private List<InspectedMember> _allMembers;
        private Dictionary<IInspectedMemberFilter, List<InspectedMember>> _cachedMembers;
        private Dictionary<IInspectedMemberFilter, List<InspectedProperty>> _cachedProperties;
        private Dictionary<IInspectedMemberFilter, List<InspectedMethod>> _cachedMethods;

        /// <summary>
        /// Initializes a new instance of the TypeMetadata class from a type. Use
        /// TypeCache to get instances of TypeMetadata; do not use this
        /// constructor directly.
        /// </summary>
        internal InspectedType(Type type) {
            ReflectedType = type;

            // determine if we are a collection or array; recall that arrays
            // implement the ICollection interface, however

            _isArray = type.IsArray;
            IsCollection = _isArray || type.IsImplementationOf(typeof(ICollection<>));

            // We're not a collection, so lookup the properties on this type
            if (IsCollection == false) {
                _cachedMembers = new Dictionary<IInspectedMemberFilter, List<InspectedMember>>();
                _cachedProperties = new Dictionary<IInspectedMemberFilter, List<InspectedProperty>>();
                _cachedMethods = new Dictionary<IInspectedMemberFilter, List<InspectedMethod>>();

                _allMembers = new List<InspectedMember>();

                // Add the parent members first. They will be sorted properly and
                // the like.
                if (ReflectedType.Resolve().BaseType != null) {
                    var inspectedParentType = InspectedType.Get(ReflectedType.Resolve().BaseType);
                    _allMembers.AddRange(inspectedParentType._allMembers);
                }

                // Add local properties.
                var localMembers = CollectUnorderedLocalMembers(type).ToList();
                if (fiSettings.EnableGlobalOrdering == false) {
                    StableSort(localMembers, (a, b) => {
                        double orderA = InspectorOrderAttribute.GetInspectorOrder(a.MemberInfo);
                        double orderB = InspectorOrderAttribute.GetInspectorOrder(b.MemberInfo);
                        return Math.Sign(orderA - orderB);
                    });
                }
                _allMembers.AddRange(localMembers);

                if (fiSettings.EnableGlobalOrdering) {
                    StableSort(_allMembers, (a, b) => {
                        double orderA = InspectorOrderAttribute.GetInspectorOrder(a.MemberInfo);
                        double orderB = InspectorOrderAttribute.GetInspectorOrder(b.MemberInfo);
                        return Math.Sign(orderA - orderB);
                    });
                }

                // Add our property names in
                _nameToProperty = new Dictionary<string, InspectedProperty>();
                _formerlySerializedAsPropertyNames = new Dictionary<string, InspectedProperty>();
                foreach (var member in _allMembers) {
                    if (member.IsProperty == false) continue;

                    if (fiSettings.EmitWarnings && _nameToProperty.ContainsKey(member.Name)) {
                        Debug.LogWarning("Duplicate property with name=" + member.Name +
                            " detected on " + ReflectedType.CSharpName());
                    }

                    _nameToProperty[member.Name] = member.Property;

                    foreach (FormerlySerializedAsAttribute attr in
                        member.MemberInfo.GetCustomAttributes(typeof(FormerlySerializedAsAttribute), /*inherit:*/ true)) {
                        _nameToProperty[attr.oldName] = member.Property;
                    }
                }
            }
        }

        /// <summary>
        /// Performs a stable sort on the list.
        /// </summary>
        public static void StableSort<T>(IList<T> list, Func<T, T, int> comparator) {
            // insertion sort; see http://www.csharp411.com/c-stable-sort/
            for (int j = 1; j < list.Count; ++j) {
                T key = list[j];

                int i = j - 1;
                for (; i >= 0 && comparator(list[i], key) > 0; i--) {
                    list[i + 1] = list[i];
                }
                list[i + 1] = key;
            }
        }

        private static List<InspectedMember> CollectUnorderedLocalMembers(Type reflectedType) {
            var members = new List<InspectedMember>();

            foreach (MemberInfo member in reflectedType.GetDeclaredMembers()) {
                PropertyInfo property = member as PropertyInfo;
                FieldInfo field = member as FieldInfo;

                // Properties
                if (property != null) {
                    // If either the get or set methods are overridden, then the
                    // property is not considered local and will appear on a
                    // parent type.
                    var getMethod = property.GetGetMethod(/*nonPublic:*/ true);
                    var setMethod = property.GetSetMethod(/*nonPublic:*/ true);
                    if ((getMethod != null && getMethod != getMethod.GetBaseDefinition()) ||
                        (setMethod != null && setMethod != setMethod.GetBaseDefinition())) {
                        continue;
                    }

                    members.Add(new InspectedMember(new InspectedProperty(property)));
                }

                // Fields
                else if (field != null) {
                    members.Add(new InspectedMember(new InspectedProperty(field)));
                }
            }

            // While split field/property and method handling apart so that
            // methods will always appear at the end of the members list.
            // Otherwise, the methods will go wherever C# reflection feels like.

            foreach (MethodInfo method in reflectedType.GetDeclaredMethods()) {
                // This is a method override. Skip it as it is not a "local"
                // property -- it will appear in a parent type.
                if (method != method.GetBaseDefinition()) {
                    continue;
                }

                members.Add(new InspectedMember(new InspectedMethod(method)));
            }

            return members;
        }

        /// <summary>
        /// The type that this metadata is modeling, ie, the type that the
        /// metadata was constructed off of.
        /// </summary>
        public Type ReflectedType {
            get;
            private set;
        }

        /// <summary>
        /// True if the base type is a collection. If true, accessing Properties
        /// will throw an exception.
        /// </summary>
        public bool IsCollection {
            get;
            private set;
        }

        public Type ElementType {
            get {
                if (_elementType != null)
                    return _elementType;

                if (!IsCollection)
                    throw new InvalidOperationException("Only collections have ElementTypes for " + ReflectedType.CSharpName());

                if (_isArray)
                    _elementType = ReflectedType.GetElementType();
                else
                    _elementType = ReflectedType.GetInterface(typeof(ICollection<>)).GetGenericArguments()[0];

                return _elementType;
            }
        }
        private Type _elementType;

        /// <summary>
        /// True if the base type is an array. If true, accessing Properties will
        /// throw an exception. IsCollection is also true if _isArray is true.
        /// </summary>
        private bool _isArray;

        /// <summary>
        /// The categories that are used for this type. If the type has no
        /// categories defined, then this will be empty.
        /// </summary>
        public Dictionary<string, List<InspectedMember>> GetCategories(IInspectedMemberFilter filter) {
            VerifyNotCollection();

            Dictionary<string, List<InspectedMember>> categories;
            if (_categoryCache.TryGetValue(filter, out categories) == false) {
                var defaultCategories = (from oattribute in ReflectedType.Resolve().GetCustomAttributes(typeof(InspectorCategoryAttribute), /*inherit:*/true)
                                         let attribute = (InspectorCategoryAttribute)oattribute
                                         select attribute.Category).ToList();

                // Not in the cache - actually compute the result.
                // NOTE: we update the cache before actually doing the
                //       computation - if for whatever reason there is an error,
                //       we will not redo this computation and just return an
                //       empty result.
                categories = new Dictionary<string, List<InspectedMember>>();
                _categoryCache[filter] = categories;

                foreach (var member in GetMembers(filter)) {
                    var memberCategories =
                        (from oattribute in member.MemberInfo.GetCustomAttributes(typeof(InspectorCategoryAttribute), /*inherit:*/true)
                         let attribute = (InspectorCategoryAttribute)oattribute
                         select attribute.Category).ToList();
                    if (memberCategories.Count == 0)
                        memberCategories = defaultCategories;

                    foreach (string category in memberCategories) {
                        if (categories.ContainsKey(category) == false)
                            categories[category] = new List<InspectedMember>();
                        categories[category].Add(member);
                    }
                }
            }

            return categories;
        }
        private Dictionary<IInspectedMemberFilter, Dictionary<string, List<InspectedMember>>> _categoryCache = new Dictionary<IInspectedMemberFilter, Dictionary<string, List<InspectedMember>>>();

        private Dictionary<string, InspectedProperty> _nameToProperty;

        /// <summary>
        /// Looks up the given property by name. Returns null if not found.
        /// </summary>
        /// <param name="name">The name of the property to lookup.</param>
        /// <returns>The property with the given name.</returns>
        public InspectedProperty GetPropertyByName(string name) {
            VerifyNotCollection();

            InspectedProperty property;

            // no such member with the name
            if (_nameToProperty.TryGetValue(name, out property) == false) {
                return null;
            }

            // we found a property!
            return property;
        }

        private Dictionary<string, InspectedProperty> _formerlySerializedAsPropertyNames;

        public InspectedProperty GetPropertyByFormerlySerializedName(string name) {
            VerifyNotCollection();

            InspectedProperty property;

            // no such member with the name
            if (_formerlySerializedAsPropertyNames.TryGetValue(name, out property) == false) {
                return null;
            }

            // we found a property!
            return property;
        }
    }
}