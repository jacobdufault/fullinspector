using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector {
    public static class IBehaviorEditorExtensions {
        /// <summary>
        /// This method makes it easy to use a typical behavior editor as a
        /// GUILayout style method, where the rect is taken care of.
        /// </summary>
        /// <param name="editor">The editor that is being used.</param>
        /// <param name="element">The element that is being edited.</param>
        public static void EditWithGUILayout<T>(this IBehaviorEditor editor, T element)
            where T : UnityObject {
            float height = editor.GetHeight(element);
            Rect region = EditorGUILayout.GetControlRect(false, height);
            if (Event.current.type != EventType.Layout) {
                editor.Edit(region, element);
            }
        }
    }
}