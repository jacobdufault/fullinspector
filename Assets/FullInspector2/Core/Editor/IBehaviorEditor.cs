using FullInspector.Internal;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector {
    public interface IBehaviorEditor {
        void Edit(Rect rect, UnityObject behavior);
        float GetHeight(UnityObject behavior);
        void SceneGUI(UnityObject behavior);

        /// <summary>
        /// Notification that the inspector/editor is now active.
        /// </summary>
        void OnEditorActivate(UnityObject behavior);

        /// <summary>
        /// Notification that the inspector/editor is no longer active.
        /// </summary>
        void OnEditorDeactivate(UnityObject behavior);
    }

    public abstract class BehaviorEditor<TBehavior> : IBehaviorEditor
        where TBehavior : UnityObject {
        protected abstract void OnEdit(Rect rect, TBehavior behavior, fiGraphMetadata metadata);
        protected abstract float OnGetHeight(TBehavior behavior, fiGraphMetadata metadata);
        protected abstract void OnSceneGUI(TBehavior behavior);

        protected virtual void OnEditorActivate(UnityObject behavior) {
        }
        protected virtual void OnEditorDeactivate(UnityObject behavior) {
        }
        void IBehaviorEditor.OnEditorActivate(UnityObject behavior) {
            OnEditorActivate(behavior);
        }
        void IBehaviorEditor.OnEditorDeactivate(UnityObject behavior) {
            OnEditorDeactivate(behavior);
        }

        public void SceneGUI(UnityObject behavior) {
            Undo.RecordObject(behavior, "Scene GUI Modification");

            EditorGUI.BeginChangeCheck();

            // we don't want to get the IObjectPropertyEditor for the given
            // target, which extends UnityObject, so that we can actually edit
            // the property instead of getting a Unity reference field
            OnSceneGUI((TBehavior)behavior);

            // If the GUI has been changed, then we want to reserialize the
            // current object state. However, we don't bother doing this if we're
            // currently in play mode, as the serialized state changes will be
            // lost regardless.
            if (EditorGUI.EndChangeCheck()) {
                // We want to call OnValidate even if we are in play-mode,
                // though.
                fiRuntimeReflectionUtility.InvokeMethod(behavior.GetType(), "OnValidate", behavior, null);

                if (EditorApplication.isPlaying == false) {
                    fiLateBindings.EditorUtility.SetDirty(behavior);
                }
            }
        }

        public void Edit(Rect rect, UnityObject behavior) {
            Undo.RecordObject(behavior, "Inspector Modification");

            //-
            //-
            //-
            // Inspector based off of the property editor
            EditorGUI.BeginChangeCheck();

            // Seed the label width - sometimes we don't always go through the
            // property editor logic.
            float savedLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = fiGUI.PushLabelWidth(GUIContent.none, rect.width);

            // Run the editor
            OnEdit(rect, (TBehavior)behavior, fiPersistentMetadata.GetMetadataFor(behavior));

            EditorGUIUtility.labelWidth = savedLabelWidth;

            // If the GUI has been changed, then we want to reserialize the
            // current object state. However, we don't bother doing this if we're
            // currently in play mode, as the serialized state changes will be
            // lost regardless.
            if (EditorGUI.EndChangeCheck()) {
                fiSerializationManager.DirtyForceSerialize.Add(behavior);

                // We want to call OnValidate even if we are in play-mode,
                // though.
                fiRuntimeReflectionUtility.InvokeMethod(behavior.GetType(), "OnValidate", behavior, null);

                if (EditorApplication.isPlaying == false) {
                    fiLateBindings.EditorUtility.SetDirty(behavior);
                }
            }
        }

        public float GetHeight(UnityObject behavior) {
            return OnGetHeight((TBehavior)behavior, fiPersistentMetadata.GetMetadataFor(behavior));
        }
    }
}