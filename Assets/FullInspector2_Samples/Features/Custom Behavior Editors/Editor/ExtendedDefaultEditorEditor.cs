using UnityEditor;
using UnityEngine;

namespace FullInspector.Samples {

    [CustomBehaviorEditor(typeof(ExtendedDefaultEditor))]
    public class ExtendedDefaultEditorEditor : DefaultBehaviorEditor<ExtendedDefaultEditor> {
        protected override void OnBeforeEdit(Rect rect, ExtendedDefaultEditor behavior, fiGraphMetadata metadata) {
            rect.height -= 3; // margin
            EditorGUI.HelpBox(rect, "Hello, this is a custom before section", MessageType.Info);
        }

        protected override float OnBeforeEditHeight(ExtendedDefaultEditor behavior, fiGraphMetadata metadata) {
            return 30;
        }

        protected override void OnAfterEdit(Rect rect, ExtendedDefaultEditor behavior, fiGraphMetadata metadata) {
            // margin
            rect.y += 3;
            rect.height -= 3;
            
            EditorGUI.HelpBox(rect, "Hello, this is a custom after section", MessageType.Info);
        }

        protected override float OnAfterEditHeight(ExtendedDefaultEditor behavior, fiGraphMetadata metadata) {
            return 30;
        }
    }

}