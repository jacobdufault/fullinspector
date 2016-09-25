using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules {
    [CustomPropertyEditor(typeof(ICustomAttributeProvider), Inherit = true)]
    public class ICustomAttributeProviderPropertyEditor : PropertyEditor<ICustomAttributeProvider> {
        public override float GetElementHeight(GUIContent label, ICustomAttributeProvider element, fiGraphMetadata metadata) {
            return EditorStyles.label.CalcHeight(label, Screen.width);
        }

        public override ICustomAttributeProvider Edit(Rect region, GUIContent label, ICustomAttributeProvider element, fiGraphMetadata metadata) {
            string displayed = "<no attribute provider>";
            if (element != null) {
                displayed = element.ToString();
            }

            EditorGUI.LabelField(region, displayed);

            return element;
        }
    }
}