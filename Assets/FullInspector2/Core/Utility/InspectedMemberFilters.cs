using FullInspector.Internal;
using FullSerializer.Internal;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace FullInspector {
    public static class InspectedMemberFilters {
        private class AllFilter : IInspectedMemberFilter {
            public bool IsInterested(InspectedProperty property) {
                return true;
            }

            public bool IsInterested(InspectedMethod method) {
                return true;
            }
        }
        public static IInspectedMemberFilter All = new AllFilter();

        private class FullInspectorSerializedPropertiesFilter : IInspectedMemberFilter {
            public bool IsInterested(InspectedProperty property) {
                return
                    property.CanWrite &&
                    InspectedType.IsSerializedByFullInspector(property) &&
                    InspectedType.IsSerializedByUnity(property) == false;
            }

            public bool IsInterested(InspectedMethod method) {
                return false;
            }
        }
        public static IInspectedMemberFilter FullInspectorSerializedProperties = new FullInspectorSerializedPropertiesFilter();

        private class InspectableMembersFilter : IInspectedMemberFilter {
            public bool IsInterested(InspectedProperty property) {
                return IsPropertyTypeInspectable(property) && ShouldDisplayProperty(property);
            }

            public bool IsInterested(InspectedMethod method) {
                return method.Method.IsDefined(typeof(InspectorButtonAttribute), /*inherit:*/ true);
            }
        }
        public static IInspectedMemberFilter InspectableMembers = new InspectableMembersFilter();


        private class StaticInspectableMembersFilter : IInspectedMemberFilter {
            public bool IsInterested(InspectedProperty property) {
                return property.IsStatic && IsPropertyTypeInspectable(property);
            }

            public bool IsInterested(InspectedMethod method) {
                return method.Method.IsDefined(typeof(InspectorButtonAttribute), /*inherit:*/ true);
            }
        }
        public static IInspectedMemberFilter StaticInspectableMembers = new StaticInspectableMembersFilter();

        private class ButtonMembersFilter : IInspectedMemberFilter {
            public bool IsInterested(InspectedProperty property) {
                return false;
            }

            public bool IsInterested(InspectedMethod method) {
                return method.Method.IsDefined(typeof(InspectorButtonAttribute), /*inherit:*/ true);
            }
        }
        public static IInspectedMemberFilter ButtonMembers = new ButtonMembersFilter();

        /// <summary>
        /// Returns true if the given property should be displayed in the inspector. This method
        /// assumes that the property type is inspectable.
        /// </summary>
        private static bool ShouldDisplayProperty(InspectedProperty property) {
            var memberInfo = property.MemberInfo;

            // If ShowInInspector is present, we will *always* display the attribute.
            if (memberInfo.IsDefined(typeof(ShowInInspectorAttribute), /*inherit:*/ true)) {
                return true;
            }

            // note: we do opt-out serialization annotations before opt-in annotations
            //       so that we can still serialize a field but not display it in the
            //       inspector (as the serialize annotations automatically cause a field
            //       to be displayed)
            if (memberInfo.IsDefined(typeof(HideInInspector), /*inherit:*/ true) ||
                memberInfo.IsDefined(typeof(NotSerializedAttribute), /*inherit:*/true) ||
                fiInstalledSerializerManager.SerializationOptOutAnnotations.Any(t => memberInfo.IsDefined(t, /*inherit*/true))) {

                return false;
            }

            // Show the property if any of the opt-in annotations are present.
            if (property.IsStatic == false &&
                fiInstalledSerializerManager.SerializationOptInAnnotations.Any(t => memberInfo.IsDefined(t, /*inherit*/true))) {
                return true;
            }

            if (property.MemberInfo is PropertyInfo && fiSettings.InspectorRequireShowInInspector) {
                return false;
            }

            return
                // IsSerializedByFullInspector will return false for BaseObject types, so we want to
                // special case support for them being inspected.
                typeof(BaseObject).Resolve().IsAssignableFrom(property.StorageType.Resolve()) ||
                InspectedType.IsSerializedByFullInspector(property) ||
                InspectedType.IsSerializedByUnity(property);
        }

        /// <summary>
        /// Returns true if the property type itself is inspectable. This does not necessarily
        /// mean that the property should be displayed in the inspector -- just that the FI editing
        /// engine can handle it.
        /// </summary>
        private static bool IsPropertyTypeInspectable(InspectedProperty property) {
            // We never inspect delegates
            if (typeof(Delegate).IsAssignableFrom(property.StorageType)) {
                return false;
            }

            if (property.MemberInfo is FieldInfo) {
                // Don't inspect compiler generated fields (an example would be a backing field
                // for an automatically generated property).
                if (property.MemberInfo.IsDefined(typeof(CompilerGeneratedAttribute), /*inherit:*/ false)) {
                    return false;
                }
            }

            else if (property.MemberInfo is PropertyInfo) {
                var propertyInfo = (PropertyInfo)property.MemberInfo;

                // If we cannot read from the property, then there is no sense in displaying it --
                // we will have no value to display
                if (propertyInfo.CanRead == false) {
                    return false;
                }

                // hack?: We only display r/w properties declared on Unity types
                // note: This may rely on the fact that we collect members locally per inheritance
                //       level (does DeclaringType change? I'm not sure).
                // note: We also check for UnityEditor since some users use FI in non-standard
                //       ways -- ie, potentially for types that are not available at runtime and
                //       hence may be in the UnityEditor namespace.
                var @namespace = propertyInfo.DeclaringType.Namespace;
                if (@namespace != null &&
                    (@namespace.StartsWith("UnityEngine") || @namespace.StartsWith("UnityEditor"))) {

                    if (propertyInfo.CanWrite == false) {
                        return false;
                    }
                }

                // If the property is named "Item", it might be the this[int] indexer, which in that
                // case we don't serialize it We cannot just compare with "Item" because of explicit
                // interfaces, where the name of the property will be the full method name.
                if (propertyInfo.Name.EndsWith("Item")) {
                    ParameterInfo[] parameters = propertyInfo.GetIndexParameters();
                    if (parameters.Length > 0) {
                        return false;
                    }
                }
            }

            return true;
        }

    }


}