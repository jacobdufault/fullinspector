using FullInspector.Internal;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector {
    /// <summary>
    /// Helper class you can derive from to write a custom tk control for a behavior. This wraps
    /// some of the boilerplate.
    /// </summary>
    public abstract class tkCustomBehaviorEditor<T> : BehaviorEditor<T> where T : UnityObject {
        private tkControlEditor GetControlEditor(T element, fiGraphMetadata graphMetadata) {
            tkControlPropertyEditor.fiLayoutPropertyEditorMetadata metadata;
            if (graphMetadata.TryGetMetadata(out metadata) == false) {
                metadata = graphMetadata.GetMetadata<tkControlPropertyEditor.fiLayoutPropertyEditorMetadata>();
                metadata.Layout = GetEditor(element);
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

        protected abstract tkControlEditor GetEditor(T behavior);
    }
}