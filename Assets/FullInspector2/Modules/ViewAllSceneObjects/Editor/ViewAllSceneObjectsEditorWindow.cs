using System;
using System.Linq;
using System.Collections.Generic;
using FullInspector.Internal;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FullInspector.Modules {
    public class ViewAllSceneObjectsEditorWindow : EditorWindow {
        [MenuItem("Window/Full Inspector/Developer/View All Scene GameObjects")]
        public static void ShowWindow() {
            fiEditorWindowUtility.Show<ViewAllSceneObjectsEditorWindow>("Scene Objects");
        }

        private Vector2 _scroll;
        private string _searchString = string.Empty;
        private int _totalDisplayed;

        private IOrderedEnumerable<GameObject> _allGameObjects;
        private Dictionary<string, int> _totalSceneObjects;

        private readonly Dictionary<string, bool> _scenesFoldoutState = new Dictionary<string, bool>();


        private void OnEnable() {
            RefreshObjects();
        }


        private void RefreshObjects() {
            _allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>().
                                        Where(o => o != null && !EditorUtility.IsPersistent(o)).
                                        OrderBy(o => o.scene.name);

            _totalSceneObjects = new Dictionary<string, int> {[string.Empty] = 0};

            foreach (var go in _allGameObjects) {
                if (go.scene.name == null) {
                    _totalSceneObjects[string.Empty]++;
                } else {
                    if (_totalSceneObjects.ContainsKey(go.scene.name)) {
                        _totalSceneObjects[go.scene.name]++;
                    } else {
                        _totalSceneObjects[go.scene.name] = 0;
                    }
                }
            }
        }

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
            if (GUILayout.Button("Refresh", GUILayout.ExpandWidth(true))) {
                RefreshObjects();
            }

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            var currentParsingScene = default(Scene);
            var didDrawDefaultSceneHeader = false;
            var isFiltering = _searchString != string.Empty;

            foreach (var obj in _allGameObjects) {

                var currentSceneName = obj.scene.name;
                if (currentSceneName == null) {
                    currentSceneName = string.Empty;
                }

                if (!_scenesFoldoutState.ContainsKey(currentSceneName)) {
                    _scenesFoldoutState[currentSceneName] = true;
                }

                if (!_totalSceneObjects.ContainsKey(currentSceneName)) {
                    RefreshObjects();
                }

                var sceneHeader = didDrawDefaultSceneHeader ? "Scene: " + currentSceneName : "No scene";
                sceneHeader += " (Total: " + _totalSceneObjects[currentSceneName] + ")";

                if (!didDrawDefaultSceneHeader) {
                    didDrawDefaultSceneHeader = true;
                    _scenesFoldoutState[currentSceneName] = EditorGUILayout.Foldout(_scenesFoldoutState[currentSceneName], sceneHeader, true);
                } else if (currentParsingScene != obj.scene) {
                    currentParsingScene = obj.scene;
                    _scenesFoldoutState[currentSceneName] = EditorGUILayout.Foldout(_scenesFoldoutState[currentSceneName], sceneHeader, true);
                }

                if (isFiltering && !obj.name.ToUpper().Contains(_searchString.ToUpper())) {
                    continue;
                }

                _totalDisplayed++;

                if (!_scenesFoldoutState[currentSceneName]) {
                    continue;
                }

                GUILayout.BeginHorizontal();
                var contentColor = GUI.contentColor;
                var bgColor = GUI.backgroundColor;

                EditorGUILayout.ObjectField(obj, typeof(GameObject), /*allowSceneObjects:*/ true);

                GUI.contentColor = Color.white;

                var enabledPingButton = (obj.hideFlags & HideFlags.HideInHierarchy) != HideFlags.HideInHierarchy;

                EditorGUI.BeginDisabledGroup(!enabledPingButton);
                if (enabledPingButton) {
                    GUI.backgroundColor = EditorGUIUtility.isProSkin ? Color.blue : Color.cyan;
                }
                if (GUILayout.Button("ping", GUILayout.Width(40))) {
                    EditorGUIUtility.PingObject(obj);
                }
                EditorGUI.EndDisabledGroup();

                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(20))) {
                    var scene = obj.gameObject.scene;
                    Undo.DestroyObjectImmediate(obj);
                    if (scene.IsValid() && scene.isLoaded) {
                        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
                    }
                }

                GUI.contentColor = contentColor;
                GUI.backgroundColor = bgColor;

                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
    }
}