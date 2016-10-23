using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using FullInspector.Internal;
using FullSerializer;
using FullSerializer.Internal;
using UnityEngine;

namespace FullInspector {
    /// <summary>
    /// A PropertyMetadata describes a discovered property or field in a
    /// TypeMetadata.
    /// </summary>
    public sealed class InspectedProperty {
        /// <summary>
        /// The member info that we read to and write from.
        /// </summary>
        public MemberInfo MemberInfo {
            get;
            private set;
        }

        /// <summary>
        /// The cached name of the property/field.
        /// </summary>
        public string Name;

        /// <summary>
        /// The cached display name of the property/field. This may be overriden
        /// by the user.
        /// </summary>
        // TODO: convert this to a GUIContent instance w/ tooltip info
        public string DisplayName;

        /// <summary>
        /// Returns true if this property has a public component to it.
        /// </summary>
        public bool IsPublic {
            get {
                if (_isPublicCache == null) {
                    FieldInfo field = MemberInfo as FieldInfo;
                    if (field != null) {
                        _isPublicCache = field.IsPublic;
                    }

                    PropertyInfo property = MemberInfo as PropertyInfo;
                    if (property != null) {
                        _isPublicCache = property.GetGetMethod(/*nonPublic:*/ false) != null ||
                            property.GetSetMethod(/*nonPublic:*/ false) != null;
                    }

                    if (_isPublicCache.HasValue == false) {
                        _isPublicCache = false;
                    }
                }

                return _isPublicCache.Value;
            }
        }
        private bool? _isPublicCache;

        /// <summary>
        /// Returns true if this InspectedProperty is both a *property* and an
        /// *auto-property*. Otherwise this will return false.
        /// </summary>
        public bool IsAutoProperty {
            get {
                if (_isAutoPropertyCache == null) {
                    if (MemberInfo is PropertyInfo == false) {
                        _isAutoPropertyCache = false;
                    }
                    else {
                        PropertyInfo property = (PropertyInfo)MemberInfo;

                        _isAutoPropertyCache =
                            property.CanWrite && property.CanRead &&
                            fsPortableReflection.HasAttribute<CompilerGeneratedAttribute>(property.GetGetMethod(/*nonPublic:*/true), /*shouldCache:*/false);
                    }
                }
                return _isAutoPropertyCache.Value;
            }
        }
        private bool? _isAutoPropertyCache;

        /// <summary>
        /// Is this property static?
        /// </summary>
        public bool IsStatic {
            get;
            private set;
        }

        /// <summary>
        /// Returns true if it looks like the property can be written to. This
        /// does *not* guarantee that a write will actually be successful (for
        /// example, a property can throw a NotImplementedException()).
        /// </summary>
        public bool CanWrite {
            get;
            private set;
        }

        /// <summary>
        /// Writes a value to the property that this property metadata
        /// represents, using given object instance as the context.
        /// </summary>
        public void Write(object context, object value) {
            try {
                FieldInfo field = MemberInfo as FieldInfo;
                PropertyInfo property = MemberInfo as PropertyInfo;

                if (field != null) {
                    // We can never write to constants.
                    if (field.IsLiteral) {
                        return;
                    }

                    field.SetValue(context, value);
                }
                else if (property != null) {
                    MethodInfo setMethod = property.GetSetMethod(/*nonPublic:*/ true);
                    if (setMethod != null) {
                        setMethod.Invoke(context, new object[] { value });
                    }
                }
            }
            catch (Exception e) {
                Debug.LogWarning("Caught exception when writing property " +
                    Name + " with context=" + fiUtility.ToString(context) +
                    " and value=" + fiUtility.ToString(value));
                Debug.LogException(e);
            }
        }

        /// <summary>
        /// Reads a value from the property that this property metadata
        /// represents, using the given object instance as the context.
        /// </summary>
        public object Read(object context) {
            try {
                if (MemberInfo is PropertyInfo) {
                    return ((PropertyInfo)MemberInfo).GetValue(context, new object[] { });
                }
                else {
                    return ((FieldInfo)MemberInfo).GetValue(context);
                }
            }
            catch (Exception e) {
                Debug.LogWarning("Caught exception when reading property " + Name + " with " +
                                    " context=" + context + "; returning default value for " +
                                    StorageType.CSharpName());
                Debug.LogException(e);

                return DefaultValue;
            }
        }

        /// <summary>
        /// The default value for the storage type. The default value is not
        /// always null as structs need special support.
        /// </summary>
        public object DefaultValue {
            get {
                if (StorageType.Resolve().IsValueType) {
                    return InspectedType.Get(StorageType).CreateInstance();
                }

                return null;
            }
        }

        /// <summary>
        /// The type of value that is stored inside of the property. For example,
        /// for an int field, StorageType will be typeof(int).
        /// </summary>
        public Type StorageType;

        /// <summary>
        /// Initializes a new instance of the PropertyMetadata class from a
        /// property member.
        /// </summary>
        public InspectedProperty(PropertyInfo property) {
            MemberInfo = property;
            StorageType = property.PropertyType;
            CanWrite = property.GetSetMethod(/*nonPublic:*/ true) != null;
            IsStatic = (property.GetGetMethod(/*nonPublic:*/ true) ?? property.GetSetMethod(/*nonPublic:*/ true)).IsStatic;

            SetupNames();
        }

        /// <summary>
        /// Initializes a new instance of the PropertyMetadata class from a field
        /// member.
        /// </summary>
        public InspectedProperty(FieldInfo field) {
            MemberInfo = field;
            StorageType = field.FieldType;
            CanWrite = field.IsLiteral == false;
            IsStatic = field.IsStatic;

            SetupNames();
        }

        private void SetupNames() {
            Name = MemberInfo.Name;

            // Setup the display name. Allow the user to override it.
            var attr = fsPortableReflection.GetAttribute<InspectorNameAttribute>(MemberInfo);
            if (attr != null) {
                DisplayName = attr.DisplayName;
            }
            if (string.IsNullOrEmpty(DisplayName)) {
                DisplayName = fiDisplayNameMapper.Map(Name);
            }
        }

        /// <summary>
        /// Determines whether the specified object is equal to this one.
        /// </summary>
        public override bool Equals(System.Object obj) {
            // If parameter is null return false.
            if (obj == null) {
                return false;
            }

            // If parameter cannot be cast to PropertyMetadata return false.
            InspectedProperty p = obj as InspectedProperty;
            if ((System.Object)p == null) {
                return false;
            }

            // Return true if the fields match:
            return (StorageType == p.StorageType) && (Name == p.Name);
        }

        /// <summary>
        /// Determines whether the specified object is equal to this one.
        /// </summary>
        public bool Equals(InspectedProperty p) {
            // If parameter is null return false:
            if ((object)p == null) {
                return false;
            }

            // Return true if the fields match:
            return (StorageType == p.StorageType) && (Name == p.Name);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms
        /// and data structures like a hash table.
        /// </returns>
        public override int GetHashCode() {
            return StorageType.GetHashCode() ^ Name.GetHashCode();
        }
    }
}