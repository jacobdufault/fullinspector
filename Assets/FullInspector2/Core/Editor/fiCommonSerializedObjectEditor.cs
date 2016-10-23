using System;
using System.Collections.Generic;
using FullInspector.BackupService;
using FullInspector.Internal;
using FullInspector.Rotorz.ReorderableList;
using FullSerializer;
using FullSerializer.Internal;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector {
    [Obsolete("Please use fiCommonSerializedObjectEditor instead", true)]
    public class FullInspectorCommonSerializedObjectEditor : Editor { }

    /// <summary>
    /// An editor that provides a good inspector experience for types which
    /// derive from ISerializedObject.
    /// </summary>
    public class fiCommonSerializedObjectEditor : Editor {
        private static Dictionary<Type, string> s_cachedTypeTooltips = new Dictionary<Type, string>();
        private static string GetTooltipText(Type type) {
            string tooltip;
            if (!s_cachedTypeTooltips.TryGetValue(type, out tooltip)) {
                tooltip = InspectorTooltipAttribute.GetTooltip(type.Resolve());
                s_cachedTypeTooltips[type] = tooltip;
            }
            return tooltip;
        }

        public override bool RequiresConstantRepaint() {
            // When we're playing and code modifies the inspector for an object,
            // we want to always show the latest data
            return EditorApplication.isPlaying || fiEditorUtility.ShouldInspectorRedraw.Enabled;
        }

        /// <summary>
        /// This is accessed by the BaseBehaviorEditor (using reflection) to
        /// determine if the editor should show the value for _serializedState.
        /// </summary>
        private static bool _editorShowSerializedState;

        [MenuItem("Window/Full Inspector/Show Serialized State &d", false, 0)]
        protected static void ViewSerializedState() {
            _editorShowSerializedState = !_editorShowSerializedState;
            fiEditorUtility.RepaintAllEditors();
        }

        private static fiGraphMetadata SerializedStateMetadata = new fiGraphMetadata();

        private static void DrawSerializedState(ISerializedObject behavior) {
            if (_editorShowSerializedState) {
                var flags = ReorderableListFlags.HideAddButton | ReorderableListFlags.DisableReordering;

                EditorGUILayout.HelpBox("The following is raw serialization data. Only change it " +
                    "if you know what you're doing or you could corrupt your object!",
                    MessageType.Warning);

                ReorderableListGUI.Title("Serialized Keys");
                ReorderableListGUI.ListField(new ListAdaptor<string>(
                    behavior.SerializedStateKeys ?? new List<string>(),
                    DrawItem, GetItemHeight, SerializedStateMetadata), flags);

                ReorderableListGUI.Title("Serialized Values");
                ReorderableListGUI.ListField(new ListAdaptor<string>(
                    behavior.SerializedStateValues ?? new List<string>(),
                    DrawItem, GetItemHeight, SerializedStateMetadata), flags);

                ReorderableListGUI.Title("Serialized Object References");
                ReorderableListGUI.ListField(new ListAdaptor<UnityObject>(
                    behavior.SerializedObjectReferences ?? new List<UnityObject>(),
                    DrawItem, GetItemHeight, SerializedStateMetadata), flags);
            }
        }

        private static float GetItemHeight(string item, fiGraphMetadataChild metadata) {
            return EditorStyles.label.CalcHeight(GUIContent.none, 100);
        }

        private static string DrawItem(Rect position, string item, fiGraphMetadataChild metadata) {
            return EditorGUI.TextField(position, item);
        }

        private static float GetItemHeight(UnityObject item, fiGraphMetadataChild metadata) {
            return EditorStyles.label.CalcHeight(GUIContent.none, 100);
        }

        private static UnityObject DrawItem(Rect position, UnityObject item, fiGraphMetadataChild metadata) {
            return EditorGUI.ObjectField(position, item, typeof(UnityObject),
                /*allowSceneObjects:*/true);
        }

        public void OnSceneGUI() {
            if (target == null)
                return;

            BehaviorEditor.Get(target.GetType()).SceneGUI(target);
        }

        public void OnEnable() {
            BehaviorEditor.Get(target.GetType()).OnEditorActivate(target);
        }

        public void OnDisable() {
            BehaviorEditor.Get(target.GetType()).OnEditorDeactivate(target);
        }

        private static void ShowBackupButton(UnityObject target) {
            if (target is CommonBaseBehavior == false) {
                return;
            }

            var behavior = (CommonBaseBehavior)target;

            if (fiStorageManager.HasBackups(behavior)) {
                // TODO: find a better location for these calls
                fiStorageManager.MigrateStorage();
                fiStorageManager.RemoveInvalidBackups();

                EditorGUILayout.Space();

                const float marginVertical = 5f;
                const float marginHorizontalRight = 13f;
                const float marginHorizontalLeft = 2f;

                Rect boxed = EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                GUILayout.Space(marginHorizontalRight);
                EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                GUILayout.Space(marginVertical);
                GUI.Box(boxed, GUIContent.none);

                {
                    List<fiSerializedObject> toRemove = new List<fiSerializedObject>();

                    GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                    fiBackupEditorGUILayout.DrawBackupsFor(behavior, toRemove);
                    GUILayout.EndVertical();

                    foreach (fiSerializedObject rem in toRemove) {
                        fiStorageManager.RemoveBackup(rem);
                    }
                }

                GUILayout.Space(marginVertical);
                EditorGUILayout.EndVertical();
                GUILayout.Space(marginHorizontalLeft);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }
        }

        /// <summary>
        /// Draws an open script button for the given object.
        /// </summary>
        private static void DrawOpenScriptButton(UnityObject element) {
            MonoScript monoScript;
            if (fiEditorUtility.TryGetMonoScript(element, out monoScript)) {
                var label = new GUIContent("Script", GetTooltipText(element.GetType()));
                Rect rect = EditorGUILayout.GetControlRect(false, EditorStyles.objectField.CalcHeight(label, 100));

                EditorGUIUtility.labelWidth = fiGUI.PushLabelWidth(label, rect.width);
                /*MonoScript newScript = (MonoScript)*/
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.ObjectField(rect, label, monoScript, typeof(MonoScript), false);
                EditorGUI.EndDisabledGroup();
                EditorGUIUtility.labelWidth = fiGUI.PopLabelWidth();

                /* TODO: Support replacing the script with another one.
                if (newScript != monoScript &&
                    element is MonoBehaviour && element is ISerializedObject) {
                    var root = ((MonoBehaviour)element).gameObject;
                    var newInstance = root.AddComponent(newScript.GetClass());
                    var newSerialized = new SerializedObject(newInstance);

                    var prop = newSerialized.GetIterator();
                    bool enterChildren = true;
                    while (prop.Next(enterChildren)) {
                        enterChildren = false;
                        newSerialized.CopyFromSerializedProperty(prop);
                    }
                    newSerialized.ApplyModifiedProperties();
                    fiUtility.DestroyObject(element);
                }*/
            }
        }

        private static void CheckForNewBaseBehaviorType(Type type) {
            // If this is a new BaseBehavior type, it might have been converted
            // from a MonoBehavior. Additionally, there might have been prefab
            // instances. The prefab instances can lose their state given some
            // serialization race conditions. This code removes the race
            // condition by explicitly serializing all of the instances.
            //
            // Here's a description of the race which occurs after changing the
            // MonoBehaviour to BaseBehavior:
            //
            // 1. Unity deserializes the FI objects. FI does an early return here
            //    because there is no data to deserialize.
            // 2. When you click on a prefab instance in the inspector, that item
            //    will get serialized. Unity will then go to the prefab and
            //    serialize it as well, invoking the serialization callbacks for
            //    those two objects.
            // 3. The current update loop finishes.
            // 4. Unity pushes out the prefab serialization changes to the rest
            //    of the prefab instances and calls their deserialization
            //    callbacks. Because these objects have never had Serialize
            //    called on them, they will blindly accept the prefab changes.
            // 5. When these objects are next serialized, they will use the
            //    incorrect prefab data. Null check, sometimes there is no prefab
            // state.
            if (fiPrefabManager.Storage == null || fiPrefabManager.Storage.SeenBaseBehaviors == null)
                return;

            if (fiPrefabManager.Storage.SeenBaseBehaviors.Contains(type.CSharpName()) == false) {
                fiLog.Log(typeof(fiCommonSerializedObjectEditor),
                         "Saving all BaseBehaviors of type " + type.CSharpName());

                fiPrefabManager.Storage.SeenBaseBehaviors.Add(type.CSharpName());
                EditorUtility.SetDirty(fiPrefabManager.Storage);
                fiSaveManager.SaveAll(type);
            }
        }

        public static void ShowInspectorForSerializedObject(UnityObject target) {
            ShowInspectorForSerializedObject(new[] { target });
        }

        public static void ShowInspectorForSerializedObject(UnityObject[] targets) {
            CheckForNewBaseBehaviorType(targets[0].GetType());
            DrawOpenScriptButton(targets[0]);

            // TODO: How do we show a mixed value?
            // TODO: We could have a global call of some sort?
            // TODO: We have the edit path w.r.t. metadata, so we can compute the
            //       deltas at the start and then export it as a list which
            //       property editors can check against.

            // Capture object state if we are doing multi-object editing.
            var preeditState = new SavedObject();
            if (fiSettings.EnableMultiEdit && targets.Length > 1)
                preeditState = new SavedObject(targets[0]);

            // Run the editor
            BehaviorEditor.Get(targets[0].GetType()).EditWithGUILayout(targets[0]);

            // Inspector for the serialized state
            var inspectedObject = targets[0] as ISerializedObject;
            if (inspectedObject != null) {
                EditorGUI.BeginChangeCheck();
                DrawSerializedState(inspectedObject);
                if (EditorGUI.EndChangeCheck()) {
                    EditorUtility.SetDirty(targets[0]);
                    inspectedObject.RestoreState();
                }
            }

            // Apply changes to other objects that are being edited.
            if (fiSettings.EnableMultiEdit && targets.Length > 1) {
                var posteditState = new SavedObject(targets[0]);
                var delta = new SavedObjectDelta(posteditState, preeditState);
                for (int i = 1; i < targets.Length; ++i) {
                    UnityObject obj = targets[i];
                    delta.ApplyChanges(ref obj);
                }
            }
        }

        public override void OnInspectorGUI() {
            ShowBackupButton(target);
            ShowInspectorForSerializedObject(targets);
        }

        public override bool HasPreviewGUI() {
            IBehaviorEditor editor = BehaviorEditor.Get(target.GetType());
            return editor is fiIInspectorPreview;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background) {
            fiIInspectorPreview preview = BehaviorEditor.Get(target.GetType()) as fiIInspectorPreview;
            if (preview != null) {
                preview.OnPreviewGUI(r, background);
            }
        }

        public override void OnPreviewSettings() {
            fiIInspectorPreview preview = BehaviorEditor.Get(target.GetType()) as fiIInspectorPreview;
            if (preview != null) {
                preview.OnPreviewSettings();
            }
        }
    }
}