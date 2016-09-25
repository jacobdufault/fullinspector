using System;
using System.Reflection;

namespace FullInspector.Internal {
    public static class ArrayPropertyEditor {
        public static IPropertyEditor TryCreate(Type dataType, ICustomAttributeProvider attributes) {
            if (dataType.IsArray == false) {
                return null;
            }

            Type elementType = dataType.GetElementType();

            Type editorType = typeof(ArrayPropertyEditor<>).MakeGenericType(elementType);
            return (IPropertyEditor)Activator.CreateInstance(editorType, new object[] { dataType, attributes });
        }
    }
}