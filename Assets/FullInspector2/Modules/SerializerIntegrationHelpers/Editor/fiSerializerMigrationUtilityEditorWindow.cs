using System;
using System.Collections.Generic;
using System.Linq;
using FullInspector.Internal;
using FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Modules {
    public class fiSerializerMigrationUtilityEditorWindow : EditorWindow {
        /// <summary>
        /// This is a utility class that wraps the UX that lets the user pick which UnityObjects they
        /// want to process.
        /// </summary>
        private class UnityObjectSelectionGroup {

            private class EnabledObject {
                public UnityObject Object;
                public bool Enabled;
            }

            private Vector2 _scroll;
            private List<EnabledObject> _objects;

            public IEnumerable<UnityObject> Selected {
                get {
                    return from obj in _objects
                           where obj.Enabled
                           select obj.Object;
                }
            }

            public UnityObjectSelectionGroup(IEnumerable<UnityObject> objects) {
                _objects = (from obj in objects
                            select new EnabledObject {
                                Object = obj,
                                Enabled = true
                            }).ToList();
            }

            public void OnGUI() {
                _scroll = GUILayout.BeginScrollView(_scroll);
                fiEditorGUILayout.WithIndent(25, () => {
                    foreach (EnabledObject obj in _objects) {
                        GUILayout.BeginHorizontal();
                        obj.Enabled = EditorGUILayout.ToggleLeft("Process?", obj.Enabled, GUILayout.Width(75));
                        EditorGUILayout.ObjectField(obj.Object, obj.Object.GetType(), /*allowSceneObjects:*/ true);
                        GUILayout.EndHorizontal();
                    }
                });
                GUILayout.EndScrollView();
            }
        }



        [MenuItem("Window/Full Inspector/Serializer Migration Utility")]
        public static void ShowWindow() {
            fiEditorWindowUtility.ShowUtility<fiSerializerMigrationUtilityEditorWindow>("Serializer Migration Utility");
        }

        private TypeSpecifier<BaseSerializer> _currentSerializer = new TypeSpecifier<BaseSerializer>();
        private TypeSpecifier<BaseSerializer> _newSerializer = new TypeSpecifier<BaseSerializer>();
        private fiGraphMetadata _metadata = new fiGraphMetadata();

        [NonSerialized]
        private UnityObjectSelectionGroup PersistentObjectSelections;
        [NonSerialized]
        private UnityObjectSelectionGroup SceneObjectSelections;

        private int _selectedMode = 1;
        private bool _disablePopups = false;




        private static GUIStyle RichLabel;
        private static void EnsureResources() {
            if (RichLabel == null) {
                RichLabel = new GUIStyle(EditorStyles.label);
                RichLabel.richText = true;
            }
        }

        private bool CheckAnnotationsPopup() {
            if (_disablePopups) return true;
            return EditorUtility.DisplayDialog("Annotations", "Have you also added annotations to your models so that they can be *deserialized* using " +
                _currentSerializer.Type.CSharpName() + " and then *serialized* using " + _newSerializer.Type.CSharpName() + "?", "Yes", "No, not yet");
        }

        private void DisplayPostSerializeMessage() {
            if (_disablePopups) return;
            EditorUtility.DisplayDialog("Important!", "Please go and change the serializers the behaviors use in code now. Do *NOT* inspect any of the migrated objects before this -- you will lose data if you do!", "Ok");
        }

        private void BeforeMigrate() {
            if (fiSerializationManager.DisableAutomaticSerialization == false) {
                ShowNotification(new GUIContent("Automatic serialization disabled until next serialization reload -- do NOT use the inspector"));
                fiSerializationManager.DisableAutomaticSerialization = true;
            }
        }

        public void OnEnable() {
            // UX: Set the current serializer to the default serializer if we have a default serializer
            if (fiInstalledSerializerManager.HasDefault) {
                _currentSerializer.Type = fiInstalledSerializerManager.DefaultMetadata.SerializerType;
            }
        }

        public void OnSelectionChange() {
            Repaint();
        }

        public void OnGUI() {
            EnsureResources();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            _disablePopups = GUILayout.Toggle(_disablePopups, "Disable Dialog Boxes");
            GUILayout.EndHorizontal();

            GUILayout.Label("<b><size=25>About</size></b> If you've decided to change serializers (for example, from Json.NET to Full Serializer), then this utility will assist your migration.", RichLabel);

            fiEditorGUILayout.Splitter(3);

            IPropertyEditor editor = PropertyEditor.Get(typeof(TypeSpecifier<BaseSerializer>), null).FirstEditor;

            GUILayout.Label("Select the <i>current</i> serializer and then the <i>new</i> serializer", RichLabel);

            fiEditorGUILayout.WithIndent(50, () => {
                WithTemporaryLabelWidth(120, () => {
                    editor.EditWithGUILayout(new GUIContent("Current Serializer"), _currentSerializer, _metadata.Enter(0));
                    editor.EditWithGUILayout(new GUIContent("New Serializer"), _newSerializer, _metadata.Enter(1));
                });
            });

            fiEditorGUILayout.Splitter(3);

            if (_currentSerializer.Type == null || _newSerializer.Type == null) return;
            if (_currentSerializer.Type == _newSerializer.Type) {
                EditorGUILayout.HelpBox("You cannot migrate to the same serializer", MessageType.Error);
                return;
            }

            _selectedMode = GUILayout.SelectionGrid(_selectedMode, new string[] { "Migrate Active Selection", "Migrate Scene Objects", "Migrate Persistent Objects" }, 3);

            if (_selectedMode == 0) {
                GameObject[] toMigrate = DisplaySelection();

                if (GUILayout.Button("Run Migration") && CheckAnnotationsPopup()) {
                    BeforeMigrate();
                    foreach (var obj in toMigrate) {
                        fiSerializerMigrationUtility.MigrateUnityObject(obj, _currentSerializer.Type, _newSerializer.Type);
                    }
                    DisplayPostSerializeMessage();
                }
            }

            else if (_selectedMode == 1) {
                DisplayScenesGUI();

                if (SceneObjectSelections == null) {
                    SceneObjectSelections = new UnityObjectSelectionGroup(fiSerializerMigrationUtility.GetSceneObjects());
                }

                GUILayout.Label("Scene Objects to Process", EditorStyles.boldLabel);
                SceneObjectSelections.OnGUI();

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Run Migration", GUILayout.ExpandWidth(true)) && CheckAnnotationsPopup()) {
                    BeforeMigrate();
                    foreach (var obj in SceneObjectSelections.Selected) {
                        fiSerializerMigrationUtility.MigrateUnityObject(obj, _currentSerializer.Type, _newSerializer.Type);
                    }
                    DisplayPostSerializeMessage();
                }
            }

            else if (_selectedMode == 2) {
                if (PersistentObjectSelections == null) {
                    PersistentObjectSelections = new UnityObjectSelectionGroup(fiSerializerMigrationUtility.GetPersistentObjects());
                }

                GUILayout.Label("Persistent GameObjects to Process", EditorStyles.boldLabel);
                PersistentObjectSelections.OnGUI();

                if (GUILayout.Button("Run Migration", GUILayout.ExpandWidth(true)) && CheckAnnotationsPopup()) {
                    BeforeMigrate();
                    foreach (var obj in PersistentObjectSelections.Selected) {
                        fiSerializerMigrationUtility.MigrateUnityObject(obj, _currentSerializer.Type, _newSerializer.Type);
                    }
                    DisplayPostSerializeMessage();
                }
            }
        }



        private Vector2 _selectionScroll;
        private GameObject[] DisplaySelection() {
            GUILayout.Label("GameObjects to Process", EditorStyles.boldLabel);

            _selectionScroll = GUILayout.BeginScrollView(_selectionScroll);
            fiEditorGUILayout.WithIndent(25, () => {
                foreach (var go in Selection.gameObjects) {
                    EditorGUILayout.ObjectField(go, go.GetType(), /*allowSceneObjects:*/ true);
                }
            });
            GUILayout.EndScrollView();

            return Selection.gameObjects;
        }

        private Vector2 _sceneListScroll;
        [NonSerialized]
        private List<string> _scenes;

        private void DisplayScenesGUI() {
            if (_scenes == null) _scenes = fiEditorUtility.GetAllScenes();

            GUILayout.Label("Quick Scene Loader", EditorStyles.boldLabel);

            _sceneListScroll = GUILayout.BeginScrollView(_sceneListScroll);
            fiEditorGUILayout.WithIndent(25, () => {
                foreach (var scene in _scenes) {

                    float buttonWidth = 50;

                    var rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
                    Rect buttonRect, labelRect;
                    SplitRect(rect, buttonWidth, 5, out buttonRect, out labelRect);

                    if (GUI.Button(buttonRect, "Load")) {
                        fiEditorUtility.OpenScene(scene);
                        SceneObjectSelections = null;
                    }

                    if (fiEditorUtility.CurrentScene == scene) {
                        EditorGUI.LabelField(labelRect, "<b>" + scene + "</b>", RichLabel);
                    }

                    else {
                        EditorGUI.LabelField(labelRect, scene);
                    }
                }
            });

            GUILayout.EndScrollView();
        }

        private static void SplitRect(Rect rect, float leftSize, float margin, out Rect left, out Rect right) {
            left = new Rect(rect);
            left.width = leftSize;

            right = new Rect(rect);
            right.x += left.width + margin;
            right.width -= left.width + margin;
        }


        private static void WithTemporaryLabelWidth(float width, Action code) {
            float saved = fiSettings.LabelWidthMax;
            fiSettings.LabelWidthMax = width;

            code();

            fiSettings.LabelWidthMax = saved;
        }
    }
}