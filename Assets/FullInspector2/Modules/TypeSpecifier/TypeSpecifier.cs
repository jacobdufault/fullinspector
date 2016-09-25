using System;

namespace FullInspector {
    // TODO: When Unity updates their C# compiler, make this a struct. However, it is currently a class
    //       because it causes internal compiler errors otherwise when it is used.
    /// <summary>
    /// Provides a dropdown in the Unity editor for selecting a type that is a derived type from TBaseType.
    /// </summary>
    /// <typeparam name="TBaseType">The base type that Type must be assignable to.</typeparam>
    public class TypeSpecifier<TBaseType> {
        /// <summary>
        /// A type that is assignable from TBaseType.
        /// </summary>
        public Type Type;

        public TypeSpecifier() { }

        public TypeSpecifier(Type type) {
            Type = type;
        }

        public static implicit operator Type(TypeSpecifier<TBaseType> specifier) {
            return specifier.Type;
        }

        public static implicit operator TypeSpecifier<TBaseType>(Type type) {
            return new TypeSpecifier<TBaseType> {
                Type = type
            };
        }

        public override bool Equals(object obj) {
            var typeSpec = obj as TypeSpecifier<TBaseType>;
            return typeSpec != null && Type == typeSpec.Type;
        }

        public override int GetHashCode() {
            return Type.GetHashCode();
        }
    }
}