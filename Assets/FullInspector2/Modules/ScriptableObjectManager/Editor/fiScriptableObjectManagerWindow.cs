using System;
using System.IO;
using System.Linq;
using FullSerializer;
using UnityEditor;
using UnityEngine;
using tk = FullInspector.tk<FullInspector.Internal.fiScriptableObjectManagerWindow, FullInspector.tkDefaultContext>;

namespace FullInspector.Internal {
    /// <summary>
    /// Provides a nice interface for interacting with and managing scriptable
    /// object instances
    /// </summary>
    public class fiScriptableObjectManagerWindow : EditorWindow {
        [MenuItem("Window/Full Inspector/Scriptable Object Manager &o")]
        public static void ShowWindow() {
            fiEditorWindowUtility.Show<fiScriptableObjectManagerWindow>("Scriptable Object Manager");
        }

        private const string PathPreferencesKey = "FullInspector_ScriptableObjectCreatorWindow_Path";

        private static void CreateNewScriptableObject(Type instanceType) {
            string assetPath =
                EditorUtility.SaveFilePanelInProject("Select Path (" + instanceType.CSharpName() + ")",
                    Guid.NewGuid().ToString(), "asset", "", EditorPrefs.GetString(PathPreferencesKey, "Assets"));

            if (string.IsNullOrEmpty(assetPath) == false) {
                EditorPrefs.SetString(PathPreferencesKey, Path.GetDirectoryName(assetPath));

                ScriptableObject asset = ScriptableObject.CreateInstance(instanceType);
                AssetDatabase.CreateAsset(asset, assetPath);
            }
        }

        private int _index;
        private Type[] _types;
        private GUIContent[] _labels;
        private Type[] _filteredTypes;
        private GUIContent[] _filteredLabels;
        private bool _useGlobalFilter = true;

        public void OnEnable() {
            _types =
                (from type in fiRuntimeReflectionUtility.AllSimpleCreatableTypesDerivingFrom(typeof(ScriptableObject))
                 where type.Assembly.FullName.Contains("UnityEngine") == false
                 where type.Assembly.FullName.Contains("UnityEditor") == false
                 select type).ToArray();

            _labels = _types.Select(t => new GUIContent(t.FullName)).ToArray();

            var filters = fiSettings.TypeSelectionDefaultFilters;
            if (filters != null) {
                _filteredTypes = (from type in _types
                                  where filters.Any(t => type.FullName.ToUpper().Contains(t.ToUpper()))
                                  select type).ToArray();
                _filteredLabels = _filteredTypes.Select(t => new GUIContent(t.FullName)).ToArray();
            } else {
                //to avoid special checks in case we have no filters
                _filteredTypes = new Type[0];
                _filteredLabels = new GUIContent[0];

                _useGlobalFilter = false;
            }

            _index = 0;

            fiEditorUtility.RepaintableEditorWindows.Add(this);
        }


        private Type[] GetTypes() {
            return _useGlobalFilter
                ? _filteredTypes
                : _types;
        }


        private GUIContent[] GetLabels() {
            return _useGlobalFilter
                ? _filteredLabels
                : _labels;
        }

        private tkControlEditor Editor = new tkControlEditor(
            new tk.VerticalGroup {
            new tk.Empty(5),

            new tk.HorizontalGroup {
                {
                    150,
                    new tk.Label("ScriptableObject Type", FontStyle.Bold)
                },

                15,

                {
                    true,
                    new tk.CenterVertical(new tk.Popup(fiGUIContent.Empty,
                        /* get options */ tk.Val(o => o.GetLabels()),
                        /* get index */ tk.Val(o => o._index),
                        /* set index */
                        (o, c, v) => {
                            o._index = v;
                            return o;
                        }))
                },

                15,

                {
                    65,
                    new tk.Button("Create",
                        (o, c) => CreateNewScriptableObject(o.GetTypes()[o._index])) {
                            Style = new tk.EnabledIf(o => o.GetTypes().Length > 0)
                        }
                }
            },

            tk.PropertyEditor.Create("Instances",
                fiAttributeProvider.Create(new InspectorCollectionRotorzFlagsAttribute {
                    HideRemoveButtons = true,
                    HideAddButton = true,
                    DisableReordering = true
                }),
                (o, c) => fiEditorUtility.GetAllAssetsOfType(o.GetTypes()[o._index]),
                (o, c, v) => {}),
            }
        );

        public void OnGUI() {
            if (fiSettings.TypeSelectionDefaultFilters != null) {
                GUILayout.BeginHorizontal();
                _useGlobalFilter = EditorGUILayout.Toggle("global filter", _useGlobalFilter, GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();
            }

            if (GetTypes().Length == 0) {
                EditorGUILayout.HelpBox("There are no derived ScriptableObject types to manage.", MessageType.Error);
                return;
            }

            fiEditorGUI.PushHierarchyMode(false);

            var metadata = fiPersistentMetadata.GetMetadataFor(this);
            float height = fiEditorGUI.tkControlHeight(GUIContent.none, this, metadata, Editor);

            var rect = EditorGUILayout.GetControlRect(false, height);
            fiEditorGUI.tkControl(rect, GUIContent.none, this, metadata, Editor);

            if (fiEditorUtility.ShouldInspectorRedraw.Enabled) {
                Repaint();
            }

            fiEditorGUI.PopHierarchyMode();
        }
    }
}
