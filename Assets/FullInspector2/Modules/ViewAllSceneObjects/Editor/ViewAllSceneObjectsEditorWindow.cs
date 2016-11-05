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
        private string _searchString = string.Empty;
        private int _totalDisplayed;

        public void Update() {
            Repaint();
        }

        public void OnGUI() {
            EditorGUILayout.HelpBox("All GameObjects in the scene will be displayed below. You " +
                "can destroy the object using the red \"X\" button. Note: this will display " +
                "*all* objects in the scene, including internal Unity ones, such as the preview " +
                "camera.", MessageType.Info);

            GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
            GUILayout.Label("Filter", GUILayout.ExpandWidth(false));
            _searchString = GUILayout.TextField(_searchString, GUI.skin.FindStyle("ToolbarSeachTextField"));
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton"))) {
                // Remove focus if cleared
                _searchString = "";
                GUI.FocusControl(null);
            }
            GUILayout.Label("Found " + _totalDisplayed, GUILayout.ExpandWidth(false));
            _totalDisplayed = 0;
            GUILayout.EndHorizontal();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            foreach (var obj in Resources.FindObjectsOfTypeAll<GameObject>()) {
                // Skip all non-scene objects
                if (EditorUtility.IsPersistent(obj)) {
                    continue;
                }

                if (_searchString != string.Empty && !obj.name.ToUpper().Contains(_searchString.ToUpper())) {
                    continue;
                }

                _totalDisplayed++;

                GUILayout.BeginHorizontal();
                var contentColor = GUI.contentColor;
                var bgColor = GUI.backgroundColor;

                EditorGUILayout.ObjectField(obj, typeof(GameObject), /*allowSceneObjects:*/ true);

                GUI.contentColor = Color.white;

                var enabledPingButton = (obj.hideFlags & HideFlags.HideInHierarchy) != HideFlags.HideInHierarchy;

                EditorGUI.BeginDisabledGroup(!enabledPingButton);
                if (enabledPingButton) {
                    GUI.backgroundColor = Color.blue;
                }
                if (GUILayout.Button("ping", GUILayout.Width(40))) {
                    EditorGUIUtility.PingObject(obj);
                }
                EditorGUI.EndDisabledGroup();

                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(20))) {
                    fiUtility.DestroyObject(obj);
                }

                GUI.contentColor = contentColor;
                GUI.backgroundColor = bgColor;

                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
    }
}