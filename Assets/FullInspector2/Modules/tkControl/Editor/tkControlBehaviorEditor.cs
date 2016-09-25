using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    [CustomBehaviorEditor(typeof(tkCustomEditor), Inherit = true)]
    public sealed class tkControlBehaviorEditor<T> : BehaviorEditor<T> where T : UnityObject {
        private tkControlEditor GetControlEditor(object element, fiGraphMetadata graphMetadata) {
            tkControlPropertyEditor.fiLayoutPropertyEditorMetadata metadata;
            if (graphMetadata.TryGetMetadata(out metadata) == false) {
                metadata = graphMetadata.GetMetadata<tkControlPropertyEditor.fiLayoutPropertyEditorMetadata>();
                metadata.Layout = ((tkCustomEditor)element).GetEditor();
            }

            return metadata.Layout;
        }

        protected override void OnEdit(Rect rect, T behavior, fiGraphMetadata metadata) {
            fiEditorGUI.tkControl(rect, GUIContent.none, behavior, metadata, GetControlEditor(behavior, metadata));
        }

        protected override float OnGetHeight(T behavior, fiGraphMetadata metadata) {
            return fiEditorGUI.tkControlHeight(GUIContent.none, behavior, metadata, GetControlEditor(behavior, metadata));
        }

        protected override void OnSceneGUI(T behavior) {
        }
    }
}