using UnityEditor;
using UnityEngine;

namespace FullInspector.Samples.FullSerializer {
    [CustomBehaviorEditor(typeof(SampleFullSerializerCustomBehaviorEditor))]
    public class SampleFullSerializerCustomBehaviorEditorEditor :
        BehaviorEditor<SampleFullSerializerCustomBehaviorEditor> {

        protected override void OnEdit(Rect rect, SampleFullSerializerCustomBehaviorEditor behavior, fiGraphMetadata metadata) {
            EditorGUI.HelpBox(rect, "This is the custom editor for SampleFullSerializerCustomBehaviorEditor", MessageType.Info);
        }

        protected override float OnGetHeight(SampleFullSerializerCustomBehaviorEditor behavior, fiGraphMetadata metadata) {
            return 30;
        }

        protected override void OnSceneGUI(SampleFullSerializerCustomBehaviorEditor behavior) {
        }
    }

}