using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    internal static class EditorOnlyMonoBehaviorRemover {
        [PostProcessScene(-1)]
        public static void ClearData() {
            // [PostProcessScene] methods are called when Unity enters play-mode, but this is a
            // situation where we don't want to destroy the EditorOnly behaviors, as we are still
            // in the editor.
            if (EditorApplication.isPlayingOrWillChangePlaymode) {
                return;
            }

            var derivedTypes = fiRuntimeReflectionUtility.AllSimpleTypesDerivingFrom(typeof(fiIEditorOnlyTag));
            foreach (var type in derivedTypes) {
                var behaviors = GameObject.FindObjectsOfType(type);
                foreach (var behavior in behaviors) {
                    UnityObject.DestroyImmediate(behavior);
                }
            }
        }
    }
}