using UnityEditor;
using UnityEngine;

namespace FullInspector.Samples.FullSerializer {
    [CustomPropertyEditor(typeof(CustomTypeEditorNonGeneric))]
    public class SampleFullSerializerCustomTypeEditorEditors :
        PropertyEditor<CustomTypeEditorNonGeneric> {
        public override CustomTypeEditorNonGeneric Edit(Rect region, GUIContent label, CustomTypeEditorNonGeneric element, fiGraphMetadata metadata) {
            EditorGUI.HelpBox(region, label.text + ": This is the non-generic editor", MessageType.Info);
            return element;
        }

        public override float GetElementHeight(GUIContent label, CustomTypeEditorNonGeneric element, fiGraphMetadata metadata) {
            return 30;
        }
    }

    [CustomPropertyEditor(typeof(CustomTypeEditorGeneric<,>))]
    public class SampleFullSerializerCustomTypeEditorEditors<T1, T2> :
        PropertyEditor<CustomTypeEditorGeneric<T1, T2>> {

        public override CustomTypeEditorGeneric<T1, T2> Edit(Rect region, GUIContent label, CustomTypeEditorGeneric<T1, T2> element, fiGraphMetadata metadata) {
            EditorGUI.HelpBox(region, string.Format(label.text + ": This is the non-generic editor (T1={0}, T2={1})", typeof(T1).Name, typeof(T2).Name), MessageType.Info);
            return element;
        }

        public override float GetElementHeight(GUIContent label, CustomTypeEditorGeneric<T1, T2> element, fiGraphMetadata metadata) {
            return 30;
        }
    }

    [CustomPropertyEditor(typeof(ICustomTypeEditorInherited), Inherit = true)]
    public class ICustomTypeEditorInheritedEditor<TDerived> : PropertyEditor<ICustomTypeEditorInherited> {

        public override ICustomTypeEditorInherited Edit(Rect region, GUIContent label, ICustomTypeEditorInherited element, fiGraphMetadata metadata) {
            EditorGUI.HelpBox(region, string.Format(label.text + ": This is the inherited editor (TDerived={0})", typeof(TDerived).Name), MessageType.Info);
            return element;
        }

        public override float GetElementHeight(GUIContent label, ICustomTypeEditorInherited element, fiGraphMetadata metadata) {
            return 30;
        }
    }
}