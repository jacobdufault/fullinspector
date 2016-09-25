using System;
using System.Reflection;
namespace FullInspector {
    /// <summary>
    /// An InspectedMember is either an InspectedMethod or an InspectedProperty. This also contains
    /// some common API functions between the two types.
    /// </summary>
    public struct InspectedMember {
        private InspectedProperty _property;
        private InspectedMethod _method;

        /// <summary>
        /// Returns the property value. Throws an exception if this is not a property.
        /// </summary>
        public InspectedProperty Property {
            get {
                if (IsProperty == false) {
                    throw new InvalidOperationException("Member is not a property");
                }

                return _property;
            }
        }

        /// <summary>
        /// Returns the method value. Throws an exception if this is not a method.
        /// </summary>
        public InspectedMethod Method {
            get {
                if (IsMethod == false) {
                    throw new InvalidOperationException("Member is not a method");
                }

                return _method;
            }
        }

        /// <summary>
        /// Is this member a method?
        /// </summary>
        public bool IsMethod { 
            get { return _method != null; }
        }

        /// <summary>
        /// Is this member a field or property?
        /// </summary>
        public bool IsProperty { 
            get { return _property != null; }
        }

        /// <summary>
        /// The actual (*not* display) name of the property.
        /// </summary>
        public string Name {
            get {
                return MemberInfo.Name;
            }
        }

        /// <summary>
        /// Returns the MemberInfo of the member.
        /// </summary>
        public MemberInfo MemberInfo {
            get {
                if (IsMethod) return _method.Method;
                else return _property.MemberInfo;
            }
        }

        /// <summary>
        /// Construct an either containing an A value.
        /// </summary>
        public InspectedMember(InspectedProperty property) {
            _property = property;
            _method = null;
        }

        /// <summary>
        /// Construct an either containing a B value.
        /// </summary>
        public InspectedMember(InspectedMethod method) {
            _property = null;
            _method = method;
        }
    }
}