using FullInspector.Internal;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.BackupService {
    /// <summary>
    /// Contains editing logic common to the backup window and the inline backup viewer.
    /// </summary>
    public class fiBackupEditorGUILayout {

        public static void DrawBackupsFor(UnityObject target, List<fiSerializedObject> toRemove) {
            bool showSpace = false;

            fiEditorGUI.PushHierarchyMode(false);

            fiGraphMetadata metadata = fiPersistentMetadata.GetMetadataFor(target);

            foreach (fiSerializedObject backup in fiStorageManager.SerializedObjects) {
                if (backup.Target.Target != target) {
                    continue;
                }

                if (showSpace) EditorGUILayout.Space();
                showSpace = true;

                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                //GUILayout.Space(5);

                backup.ShowDeserialized = EditorGUILayout.Foldout(backup.ShowDeserialized,
                    "Backup state on " + backup.SavedAt);

                if (GUILayout.Button("Restore \u2713", GUILayout.Width(100))) {
                    fiBackupManager.RestoreBackup(backup);
                }

                Color savedColor = GUI.color;
                GUI.color = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(20))) {
                    toRemove.Add(backup);
                }
                GUI.color = savedColor;
                EditorGUILayout.EndHorizontal();

                if (backup.ShowDeserialized) {
                    if (backup.DeserializedState == null) {
                        backup.DeserializedState = new fiDeserializedObject(backup);
                    }

                    GUILayout.Space(3);

                    DisplayDeserializedObject(backup.DeserializedState, metadata);
                }
                else if (backup.DeserializedState != null) {
                    backup.DeserializedState = null;
                }

                // In what has to be an extremely annoying GUILayout issue, the indentation changes
                // when you have a Begin/End Horizontal after the End/Begin horizontal for the
                // previous horizontal.
                if (backup.ShowDeserialized == false) {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.EndHorizontal();
                }
            }

            fiEditorGUI.PopHierarchyMode();
        }

        public static void DisplayDeserializedObject(fiDeserializedObject obj, fiGraphMetadata metadata) {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical();

            for (int i = 0; i < obj.Members.Count; ++i) {
                fiDeserializedMember member = obj.Members[i];

                PropertyEditorChain editor = PropertyEditor.Get(
                    member.InspectedProperty.StorageType,
                    member.InspectedProperty.MemberInfo);

                GUILayout.BeginHorizontal();

                member.ShouldRestore.Enabled = GUILayout.Toggle(member.ShouldRestore.Enabled, GUIContent.none, GUILayout.Width(15));

                GUI.enabled = false;

                string label = member.InspectedProperty.DisplayName;
                if (member.ShouldRestore.Enabled) {
                    editor.FirstEditor.EditWithGUILayout(new GUIContent(label), member.Value, metadata.Enter(label));
                }
                else {
                    GUILayout.Label(new GUIContent(label + " (will not restore)"));
                }

                GUI.enabled = true;

                GUILayout.EndHorizontal();

                if (i != obj.Members.Count - 1) {
                    fiEditorGUILayout.Splitter(1);
                }
            }

            GUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }
}