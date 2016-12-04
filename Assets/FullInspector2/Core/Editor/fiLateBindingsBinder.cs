using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Internal {

    // note: See the docs on fiLateBindings This is just the actual injection
    //       code which only gets run if we're in an editor
    //
    // note: If there is ever a binding that doesn't occur quickly enough, then
    //       we can use reflection to discover it immediately

    [InitializeOnLoad]
    public class fiLateBindingsBinder {
        static fiLateBindingsBinder() {
            fiLateBindings._Bindings._AssetDatabase_LoadAssetAtPath = AssetDatabase.LoadAssetAtPath;

            fiLateBindings._Bindings._EditorApplication_isPlaying = () => EditorApplication.isPlaying;
            fiLateBindings._Bindings._EditorApplication_isCompilingOrChangingToPlayMode = () => EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode;
            fiLateBindings._Bindings._EditorApplication_InvokeOnEditorThread = a => {
                EditorApplication.CallbackFunction updateFn = null;
                updateFn = () => {
                    a();
                    EditorApplication.update -= updateFn;
                };
                EditorApplication.update += updateFn;
            };
            fiLateBindings._Bindings._EditorApplication_Callbacks = new List<Action>();
            fiLateBindings._Bindings._EditorApplication_CallbacksToBeAdded = new List<Action>();
            fiLateBindings._Bindings._EditorApplication_CallbacksToBeRemoved = new List<Action>();
            fiLateBindings._Bindings._EditorApplication_AddUpdateAction = a => fiLateBindings._Bindings._EditorApplication_CallbacksToBeAdded.Add(a);
            fiLateBindings._Bindings._EditorApplication_RemUpdateAction = a => fiLateBindings._Bindings._EditorApplication_CallbacksToBeRemoved.Add(a);
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;

            fiLateBindings._Bindings._EditorApplication_timeSinceStartup = () => EditorApplication.timeSinceStartup;

            fiLateBindings._Bindings._EditorPrefs_GetString = EditorPrefs.GetString;
            fiLateBindings._Bindings._EditorPrefs_SetString = EditorPrefs.SetString;

            fiLateBindings._Bindings._EditorUtility_SetDirty = EditorUtility.SetDirty;
            fiLateBindings._Bindings._EditorUtility_InstanceIdToObject = EditorUtility.InstanceIDToObject;
            fiLateBindings._Bindings._EditorUtility_IsPersistent = EditorUtility.IsPersistent;
            fiLateBindings._Bindings._EditorUtility_CreateGameObjectWithHideFlags = (name, flags) => EditorUtility.CreateGameObjectWithHideFlags(name, flags);

            fiLateBindings._Bindings._EditorGUI_BeginChangeCheck = EditorGUI.BeginChangeCheck;
            fiLateBindings._Bindings._EditorGUI_EndChangeCheck = EditorGUI.EndChangeCheck;
            fiLateBindings._Bindings._EditorGUI_BeginDisabledGroup = EditorGUI.BeginDisabledGroup;
            fiLateBindings._Bindings._EditorGUI_EndDisabledGroup = EditorGUI.EndDisabledGroup;
            fiLateBindings._Bindings._EditorGUI_Foldout = EditorGUI.Foldout;
            fiLateBindings._Bindings._EditorGUI_HelpBox = (rect, message, commentType) => EditorGUI.HelpBox(rect, message, (MessageType)commentType);
            fiLateBindings._Bindings._EditorGUI_IntSlider = EditorGUI.IntSlider;
            fiLateBindings._Bindings._EditorGUI_Popup = EditorGUI.Popup;
            fiLateBindings._Bindings._EditorGUI_Slider = EditorGUI.Slider;

            fiLateBindings.EditorGUIUtility.standardVerticalSpacing = EditorGUIUtility.standardVerticalSpacing;
            fiLateBindings.EditorGUIUtility.singleLineHeight = EditorGUIUtility.singleLineHeight;

            fiLateBindings._Bindings._EditorStyles_label = () => EditorStyles.label;
            fiLateBindings._Bindings._EditorStyles_foldout = () => EditorStyles.foldout;

            fiLateBindings._Bindings._fiEditorGUI_PushHierarchyMode = state => fiEditorGUI.PushHierarchyMode(state);
            fiLateBindings._Bindings._fiEditorGUI_PopHierarchyMode = () => fiEditorGUI.PopHierarchyMode();

            fiLateBindings._Bindings._PrefabUtility_CreatePrefab = (string path, GameObject template) => PrefabUtility.CreatePrefab(path, template);
            fiLateBindings._Bindings._PrefabUtility_IsPrefab = unityObj => PrefabUtility.GetPrefabType(unityObj) == PrefabType.Prefab;
            fiLateBindings._Bindings._PrefabUtility_IsPrefabInstance = unityObj => PrefabUtility.GetPrefabType(unityObj) == PrefabType.PrefabInstance;

            fiLateBindings._Bindings._PropertyEditor_Edit =
                (objType, attrs, rect, label, obj, metadata, skippedEditors) =>
                    PropertyEditor.Get(objType, attrs).SkipUntilNot(skippedEditors).Edit(rect, label, obj, metadata);
            fiLateBindings._Bindings._PropertyEditor_GetElementHeight =
                (objType, attrs, label, obj, metadata, skippedEditors) =>
                    PropertyEditor.Get(objType, attrs).SkipUntilNot(skippedEditors).GetElementHeight(label, obj, metadata);

            fiLateBindings._Bindings._PropertyEditor_EditSkipUntilNot =
                (skipUntilNot, objType, attrs, rect, label, obj, metadata) =>
                    PropertyEditor.Get(objType, attrs).SkipUntilNot(skipUntilNot).Edit(rect, label, obj, metadata);
            fiLateBindings._Bindings._PropertyEditor_GetElementHeightSkipUntilNot =
                (skipUntilNot, objType, attrs, label, obj, metadata) =>
                    PropertyEditor.Get(objType, attrs).SkipUntilNot(skipUntilNot).GetElementHeight(label, obj, metadata);

            fiLateBindings._Bindings._Selection_activeObject = () => Selection.activeObject;
            fiLateBindings._Bindings._Selection_activeSelection = () => {
                if (Selection.activeObject is GameObject)
                    return ((GameObject)Selection.activeObject).GetComponents<MonoBehaviour>();

                // Not sure if this ever actually triggers, but just to be safe.
                if (Selection.activeObject is MonoBehaviour)
                    return ((MonoBehaviour)Selection.activeObject).gameObject.GetComponents<MonoBehaviour>();

                return new[] { Selection.activeObject };
            };
        }

        private static void OnEditorUpdate() {
            // Remove callbacks from _EditorApplication_Callbacks.
            for (int i = 0; i < fiLateBindings._Bindings._EditorApplication_CallbacksToBeRemoved.Count; i++) {
                var c = fiLateBindings._Bindings._EditorApplication_CallbacksToBeRemoved[i];
                fiLateBindings._Bindings._EditorApplication_Callbacks.Remove(c);
            }
            fiLateBindings._Bindings._EditorApplication_CallbacksToBeRemoved.Clear();

            // Add new callbacks to _EditorApplication_Callbacks.
            fiLateBindings._Bindings._EditorApplication_Callbacks.AddRange(fiLateBindings._Bindings._EditorApplication_CallbacksToBeAdded);
            fiLateBindings._Bindings._EditorApplication_CallbacksToBeAdded.Clear();

            // Invoke all _EditorApplication_Callbacks instances.
            for (int i = 0; i < fiLateBindings._Bindings._EditorApplication_Callbacks.Count; i++) {
                fiLateBindings._Bindings._EditorApplication_Callbacks[i]();
            }
        }

        public static void EnsureLoaded() {
            // no-op, but it ensures that the static constructor has executed
        }
    }
}