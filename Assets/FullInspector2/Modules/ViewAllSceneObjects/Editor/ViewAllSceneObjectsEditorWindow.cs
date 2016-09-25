using FullInspector.Internal;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules {
    public class ViewAllSceneObjectsEditorWindow : EditorWindow {
        [MenuItem("Window/Full Inspector/Developer/View All Scene GameObjects")]
        public static void ShowWindow() {
            fiEditorWindowUtility.Show<ViewAllSceneObjectsEditorWindow>("Scene Objects");
        }

        private Vector2 _scroll;

        public void Update() {
            Repaint();
        }

        public void OnGUI() {
            EditorGUILayout.HelpBox("All GameObjects in the scene will be displayed below. You " +
                "can destroy the object using the red \"X\" button. Note: this will display " +
                "*all* objects in the scene, including internal Unity ones, such as the preview " +
                "camera.", MessageType.Info);

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            foreach (var obj in Resources.FindObjectsOfTypeAll<GameObject>()) {
                // Skip all non-scene objects
                if (EditorUtility.IsPersistent(obj)) {
                    continue;
                }

                GUILayout.BeginHorizontal();

                EditorGUILayout.ObjectField(obj, typeof(GameObject), /*allowSceneObjects:*/ true);

                var color = GUI.color;
                GUI.color = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(20))) {
                    fiUtility.DestroyObject(obj);
                }
                GUI.color = color;

                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
