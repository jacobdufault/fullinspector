using FullInspector.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.BackupService {
    /// <summary>
    /// The backup editor window.
    /// </summary>
    public class fiBackupEditorWindow : EditorWindow {
        [MenuItem("Window/Full Inspector/Backup Window")]
        public static void ShowWindow() {
            fiEditorWindowUtility.Show<fiBackupEditorWindow>("Backups");
        }

        public Component FindComponentInstance(Type componentType) {
            GameObject container = new GameObject();
            container.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSave;
            return container.AddComponent(componentType);
        }

        private Vector2 _scrollPosition;
        public string NameFilter = string.Empty;

        private bool PassesFilter(UnityObject target) {
            /*
            // filters by the currently selected object
            if (Selection.activeGameObject != null) {
                foreach (var component in Selection.activeGameObject.GetComponents(typeof(CommonBaseBehavior))) {
                    if (component == target) return true;
                }

                return false;
            }
            */

            return target.name.IndexOf(NameFilter, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private IEnumerable<UnityObject> Targets(IEnumerable<fiSerializedObject> backups) {
            var targets = new HashSet<UnityObject>();

            foreach (var backup in backups) {
                targets.Add(backup.Target.Target);
            }

            return targets.OrderBy(obj => obj.name);
        }

        private void OnGUI() {
            var savedHierarchyMode = EditorGUIUtility.hierarchyMode;
            EditorGUIUtility.hierarchyMode = false;

            fiStorageManager.MigrateStorage();
            fiStorageManager.RemoveInvalidBackups();

            Repaint();

            //GUILayout.Label("Full Inspector Dynamic Object Saved State Solution", EditorStyles.boldLabel);
            //fiEditorGUILayout.Splitter(3);

            //_filter = EditorGUILayout.ObjectField("Filter By Object", _filter, typeof(UnityObject), /*allowSceneObjects:*/true);
            //_filter = EditorGUILayout.TextField("Filter By Name", _filter);

            // For the custom search bar, see:
            // http: //answers.unity3d.com/questions/464708/custom-editor-search-bar.html

            GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
            GUILayout.Label("Filter", GUILayout.ExpandWidth(false));
            NameFilter = GUILayout.TextField(NameFilter, GUI.skin.FindStyle("ToolbarSeachTextField"));
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton"))) {
                // Remove focus if cleared
                NameFilter = "";
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (fiStorageManager.SerializedObjects.Count() == 0) {
                EditorGUILayout.HelpBox(
                    "It looks like you don't have any backups! To create one, simply right click a " +
                    "supported behavior and hit the Backup context menu item. This will work in both " +
                    "play and edit mode. If you do not see the backup menu item, then the behavior is " +
                    "not currently supported -- please create a new issue if you would like " +
                    "support.", MessageType.Info);
            }

            Rect dragDropArea = EditorGUILayout.BeginVertical();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUIStyle.none);
            var toRemove = new List<fiSerializedObject>();

            foreach (UnityObject target in Targets(fiStorageManager.SerializedObjects)) {
                if (PassesFilter(target) == false) {
                    continue;
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Target (" + target.GetType().Name + ")", EditorStyles.boldLabel);
                EditorGUILayout.ObjectField(target, target.GetType(), false, GUILayout.Width(200));
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(4);

                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                GUILayout.BeginVertical();
                fiBackupEditorGUILayout.DrawBackupsFor(target, toRemove);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                fiEditorGUILayout.Splitter(3);
            }

            foreach (fiSerializedObject backup in toRemove) {
                Undo.RecordObject(fiStorageManager.PersistentStorage, "Undo Backup Removal");
                fiStorageManager.RemoveBackup(backup);
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();

            // Do the backups last so we don't screw up the layout code by modifying the
            // backup data structure.
            UnityObject[] droppedObjects;
            if (fiEditorGUI.TryDragAndDropArea(dragDropArea, unityObj => unityObj is Component, out droppedObjects)) {
                foreach (var droppedObject in droppedObjects) {
                    fiBackupManager.CreateBackup((Component)droppedObject);
                }
            }

            EditorGUIUtility.hierarchyMode = savedHierarchyMode;
        }
    }
}