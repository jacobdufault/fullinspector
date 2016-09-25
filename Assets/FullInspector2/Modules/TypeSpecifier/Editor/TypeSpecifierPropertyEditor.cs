using FullInspector.Internal;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules {
    [CustomPropertyEditor(typeof(TypeSpecifier<>))]
    public class TypeSpecifierPropertyEditor<TType> : PropertyEditor<TypeSpecifier<TType>> {
        private static TypeDropdownOptionsManager Options =
            new TypeDropdownOptionsManager(typeof(TType), /*allowUncreatableTypes:*/ true);

        public override TypeSpecifier<TType> Edit(Rect region, GUIContent label, TypeSpecifier<TType> element, fiGraphMetadata metadata) {
            if (element == null) element = new TypeSpecifier<TType>();

            int index = Options.GetIndexForType(element.Type);
            if (index == -1) index = 0;

            int newIndex = EditorGUI.Popup(region, label, index, Options.GetDisplayOptions());
            element.Type = Options.GetTypeForIndex(newIndex, element.Type);

            return element;
        }

        public override float GetElementHeight(GUIContent label, TypeSpecifier<TType> element, fiGraphMetadata metadata) {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}