using System;
using System.Linq;
using System.Reflection;
using FullSerializer.Internal;

namespace FullInspector.Internal {
    /// <summary>
    /// Implements ICustomAttributeProvider with the given set of attribute objects.
    /// </summary>
#if !UNITY_EDITOR && UNITY_METRO
    public class fiAttributeProvider {
        public static MemberInfo Create(params object[] attributes) {
            return null;
        }
    }
#else
    public class fiAttributeProvider : MemberInfo {
        private readonly object[] _attributes;

        public static MemberInfo Create(params object[] attributes) {
            return new fiAttributeProvider(attributes);
        }

        private fiAttributeProvider(object[] attributes) {
            _attributes = attributes;
        }

        public override Type DeclaringType {
            get {
                throw new NotSupportedException();
            }
        }

        public override MemberTypes MemberType {
            get {
                throw new NotSupportedException();
            }
        }

        public override string Name {
            get {
                throw new NotSupportedException();
            }
        }

        public override Type ReflectedType {
            get {
                throw new NotSupportedException();
            }
        }

        public override object[] GetCustomAttributes(bool inherit) {
            return _attributes;
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) {
            return
                (from attr in _attributes
                 where attr.GetType().Resolve().IsAssignableFrom(attributeType.Resolve())
                 select attr).ToArray();
        }

        public override bool IsDefined(Type attributeType, bool inherit) {
            return
                (from attr in _attributes
                 where attr.GetType().Resolve().IsAssignableFrom(attributeType.Resolve())
                 select attr).Any();
        }
    }
#endif
}
