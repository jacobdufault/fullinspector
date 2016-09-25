using System;
using System.Collections.Generic;
using System.IO;
using FullInspector.Internal;
using FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Modules {
    public class fiSharedInstanceSelectorWindow : EditorWindow {
        /// <summary>
        /// Shows a new selection window for a SharedInstance type.
        /// </summary>
        /// <param name="instanceType">The generic SharedInstance parameter; that actual instance type.</param>
        /// <param name="sharedInstanceType">The generic SharedInstance type itself.</param>
        /// <param name="onSelected">Method to invoke when a new SharedInstance has been selected.</param>
        public static void Show(Type instanceType, Type sharedInstanceType, Action<UnityObject> onSelected) {
            var window = fiEditorWindowUtility.ShowUtility<fiSharedInstanceSelectorWindow>("Shared Instance Selector (" + instanceType.CSharpName() + ")");
            window._instanceType = instanceType;
            window._sharedInstanceType = sharedInstanceType;
            window._onSelected = onSelected;
            window.Focus();
        }

        private Action<UnityObject> _onSelected;
        private Type _instanceType;
        private Type _sharedInstanceType;

        private Vector2 _scroll;
        private Dictionary<int, bool> _foldouts = new Dictionary<int, bool>();
        private string _searchString = string.Empty;

        public void OnEnable() {
            fiEditorUtility.ShouldInspectorRedraw.Push();
        }
        public void OnDestroy() {
            // We delay the inspector redraw pop so that the inspector will redraw one more time
            // after the window is closed so it gets all of the updated state.
            EditorApplication.delayCall += () => fiEditorUtility.ShouldInspectorRedraw.Pop();
        }

        private string PathEditorPreferencesKey {
            get {
                return "FullInspector_LastSharedInstancePathKey_" + _instanceType.CSharpName();
            }
        }

        private void CreateNewScriptableObject() {
            Type actualInstanceType = fiSharedInstanceUtility.GetSerializableType(_sharedInstanceType);
            if (actualInstanceType != null) {
                string assetPath =
                    EditorUtility.SaveFilePanelInProject("Select Shared Instance Path (" + _instanceType.CSharpName() + ")",
                        Guid.NewGuid().ToString(), "asset", "", EditorPrefs.GetString(PathEditorPreferencesKey, ""));

                if (string.IsNullOrEmpty(assetPath) == false) {
                    EditorPrefs.SetString(PathEditorPreferencesKey, Path.GetDirectoryName(assetPath));

                    ScriptableObject asset = ScriptableObject.CreateInstance(actualInstanceType);
                    AssetDatabase.CreateAsset(asset, assetPath);
                }
            }
        }

        public void OnGUI() {
            // If we went through an serialization cycle, then the types will be null so we have to close the selection
            // window.
            if (_sharedInstanceType == null || _instanceType == null) {
                // we have to close the window *outside* of OnGUI otherwise Unity will
                // report cleanup errors
                EditorApplication.delayCall += () => { if (this != null) Close(); };
                return;
            }

            GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));

            if (GUILayout.Button("Create New Instance", EditorStyles.toolbarButton, GUILayout.Width(125))) {
                CreateNewScriptableObject();
            }

            GUILayout.FlexibleSpace();

            GUILayout.Label("Filter", EditorStyles.toolbarButton, GUILayout.ExpandWidth(false));
            _searchString = GUILayout.TextField(_searchString, EditorStyles.toolbarTextField, GUILayout.MaxWidth(250), GUILayout.MinWidth(125));
            GUILayout.EndHorizontal();

            List<UnityObject> availableInstances = fiEditorUtility.GetAllAssetsOfType(_sharedInstanceType);


            GUILayout.BeginHorizontal();
            GUILayout.Label("Select Existing Instance (" + availableInstances.Count + " available)", EditorStyles.boldLabel);

            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical();
            GUILayout.Space(5);
            if (GUILayout.Button("Select null", GUILayout.MaxWidth(125))) {
                _onSelected(null);
                Close();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);


            for (int i = 0; i < availableInstances.Count; ++i) {
                if (_foldouts.ContainsKey(i) == false) _foldouts[i] = false;

                if (availableInstances[i] == null) {
                    continue;
                }

                if (availableInstances[i].name.Contains(_searchString) == false) {
                    continue;
                }

                EditorGUILayout.BeginHorizontal();

                var foldoutRect = EditorGUILayout.GetControlRect(GUILayout.Width(11));
                foldoutRect.y += 2;
                _foldouts[i] = EditorGUI.Foldout(foldoutRect, _foldouts[i], GUIContent.none);

                if (GUILayout.Button(availableInstances[i].name)) {
                    _onSelected(availableInstances[i]);
                    Close();
                }

                string assetPath = AssetDatabase.GetAssetPath(availableInstances[i]);
                if (string.IsNullOrEmpty(assetPath) == false) {
                    GUI.color = Color.red;
                    if (GUILayout.Button(new GUIContent("X"), GUILayout.Width(18))) {
                        if (EditorUtility.DisplayDialog("Confirm Deletion", "Are you sure that you want to delete \"" + availableInstances[i].name + "\"?", "Yes", "No")) {
                            AssetDatabase.DeleteAsset(assetPath);
                            fiEditorUtilityCache.ClearCache();
                        }
                    }
                    GUI.color = Color.white;
                }

                EditorGUILayout.EndHorizontal();

                if (_foldouts[i]) {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20f);

                    var editor = BehaviorEditor.Get(availableInstances[i].GetType());
                    editor.EditWithGUILayout(availableInstances[i]);

                    GUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }
}