using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    /// <summary>
    /// Full Inspector has to support running in both DLL and source code mode.
    /// This sometimes introduces issues when non-editor code has to access
    /// editor-related code. This is achieved via a late-binding mechanism; the
    /// editor code will automatically inject the relevant pointers into this
    /// data structure. If the binding does not exist yet, then a warning will be
    /// emitted.
    /// </summary>
    public static class fiLateBindings {
        public static class _Bindings {
            public static Func<string, Type, UnityObject> _AssetDatabase_LoadAssetAtPath;

            public static Func<bool> _EditorApplication_isPlaying;
            public static Func<bool> _EditorApplication_isCompilingOrChangingToPlayMode;
            public static Action<Action> _EditorApplication_InvokeOnEditorThread;
            public static Action<Action> _EditorApplication_AddUpdateAction;
            public static Action<Action> _EditorApplication_RemUpdateAction;
            public static List<Action> _EditorApplication_Callbacks;
            public static List<Action> _EditorApplication_CallbacksToBeAdded;
            public static List<Action> _EditorApplication_CallbacksToBeRemoved;
            public static Func<double> _EditorApplication_timeSinceStartup;

            public static Func<string, string, string> _EditorPrefs_GetString;
            public static Action<string, string> _EditorPrefs_SetString;

            public static Action<UnityObject> _EditorUtility_SetDirty;
            public static Func<int, UnityObject> _EditorUtility_InstanceIdToObject;
            public static Func<UnityObject, bool> _EditorUtility_IsPersistent;
            public static Func<string, HideFlags, GameObject> _EditorUtility_CreateGameObjectWithHideFlags;

            public static Action _EditorGUI_BeginChangeCheck;
            public static Func<bool> _EditorGUI_EndChangeCheck;
            public static Action<bool> _EditorGUI_BeginDisabledGroup;
            public static Action _EditorGUI_EndDisabledGroup;

            public delegate bool _EditorGUI_Foldout_Type(Rect rect, bool status, GUIContent label, bool toggleOnLabelClick, GUIStyle style);
            public static _EditorGUI_Foldout_Type _EditorGUI_Foldout;
            public static Action<Rect, string, CommentType> _EditorGUI_HelpBox;
            public delegate T _EditorGUI_Slider_Type<T>(Rect position, GUIContent label, T value, T leftValue, T rightValue);
            public static _EditorGUI_Slider_Type<int> _EditorGUI_IntSlider;
            public delegate int _EditorGUI_PopupType(Rect position, GUIContent label, int selectedIndex, GUIContent[] displayedOptions);
            public static _EditorGUI_PopupType _EditorGUI_Popup;
            public static _EditorGUI_Slider_Type<float> _EditorGUI_Slider;

            public static Func<GUIStyle> _EditorStyles_label;
            public static Func<GUIStyle> _EditorStyles_foldout;

            public static Action<bool> _fiEditorGUI_PushHierarchyMode;
            public static Action _fiEditorGUI_PopHierarchyMode;

            public static Func<string, GameObject, GameObject> _PrefabUtility_CreatePrefab;
            public static Func<UnityObject, bool> _PrefabUtility_IsPrefab;
            public static Func<UnityObject, bool> _PrefabUtility_IsPrefabInstance;

            // too many parameters for Func
            public delegate object _PropertyEditor_Edit_Type(Type objType, MemberInfo attrs, Rect rect, GUIContent label, object obj, fiGraphMetadataChild metadata, Type[] skippedEditors);
            public delegate float _PropertyEditor_GetElementHeight_Type(Type objType, MemberInfo attrs, GUIContent label, object obj, fiGraphMetadataChild metadata, Type[] skippedEditors);
            public static _PropertyEditor_Edit_Type _PropertyEditor_Edit;
            public static _PropertyEditor_GetElementHeight_Type _PropertyEditor_GetElementHeight;

            public delegate object _PropertyEditor_EditSkipUntilNot_Type(Type[] skipUntilNot, Type objType, MemberInfo attrs, Rect rect, GUIContent label, object obj, fiGraphMetadataChild metadata);
            public delegate float _PropertyEditor_GetElementHeightSkipUntilNot_Type(Type[] skipUntilNot, Type objType, MemberInfo attrs, GUIContent label, object obj, fiGraphMetadataChild metadata);
            public static _PropertyEditor_EditSkipUntilNot_Type _PropertyEditor_EditSkipUntilNot;
            public static _PropertyEditor_GetElementHeightSkipUntilNot_Type _PropertyEditor_GetElementHeightSkipUntilNot;

            public static Func<UnityObject> _Selection_activeObject;
            public static Func<UnityObject[]> _Selection_activeSelection;
        }

        public static class AssetDatabase {
            public static UnityObject LoadAssetAtPath(string path, Type type) {
                if (VerifyBinding("AssetDatabase.LoadAssetAtPath", _Bindings._AssetDatabase_LoadAssetAtPath)) {
                    return _Bindings._AssetDatabase_LoadAssetAtPath(path, type);
                }
                return null;
            }
        }

        public static class EditorApplication {
            public static bool isPlaying {
                get {
                    if (VerifyBinding("EditorApplication.isPlaying", _Bindings._EditorApplication_isPlaying)) {
                        return _Bindings._EditorApplication_isPlaying();
                    }
                    return true;
                }
            }

            public static bool isCompilingOrChangingToPlayMode {
                get {
                    if (VerifyBinding("EditorApplication.isCompilingOrChangingToPlayMode", _Bindings._EditorApplication_isCompilingOrChangingToPlayMode)) {
                        return _Bindings._EditorApplication_isCompilingOrChangingToPlayMode();
                    }
                    return true;
                }
            }

            public static double timeSinceStartup {
                get {
                    if (VerifyBinding("EditorApplication.timeSinceStartup", _Bindings._EditorApplication_timeSinceStartup)) {
                        return _Bindings._EditorApplication_timeSinceStartup();
                    }
                    return 0;
                }
            }

            public static void InvokeOnEditorThread(Action func) {
                if (VerifyBinding("EditorApplication.InvokeOnEditorThread", _Bindings._EditorApplication_InvokeOnEditorThread)) {
                    _Bindings._EditorApplication_InvokeOnEditorThread(func);
                }
            }

            public static void AddUpdateFunc(Action func) {
                if (VerifyBinding("EditorApplication.AddUpdateFunc", _Bindings._EditorApplication_AddUpdateAction)) {
                    _Bindings._EditorApplication_AddUpdateAction(func);
                }
            }

            public static void RemUpdateFunc(Action func) {
                if (VerifyBinding("EditorApplication.RemUpdateFunc", _Bindings._EditorApplication_RemUpdateAction)) {
                    _Bindings._EditorApplication_RemUpdateAction(func);
                }
            }
        }

        public static class EditorPrefs {
            public static string GetString(string key, string defaultValue) {
                if (VerifyBinding("EditorPrefs.GetString", _Bindings._EditorPrefs_GetString)) {
                    return _Bindings._EditorPrefs_GetString(key, defaultValue);
                }
                return defaultValue;
            }

            public static void SetString(string key, string value) {
                if (VerifyBinding("EditorPrefs.SetString", _Bindings._EditorPrefs_SetString)) {
                    _Bindings._EditorPrefs_SetString(key, value);
                }
            }
        }

        public static class EditorUtility {
            public static void SetDirty(UnityObject unityObject) {
                if (VerifyBinding("EditorUtility.SetDirty", _Bindings._EditorUtility_SetDirty)) {
                    if (unityObject != null) {
                        _Bindings._EditorUtility_SetDirty(unityObject);
                    }
                }
            }

            public static UnityObject InstanceIDToObject(int instanceId) {
                if (VerifyBinding("EditorUtility.InstanceIdToObject", _Bindings._EditorUtility_InstanceIdToObject)) {
                    return _Bindings._EditorUtility_InstanceIdToObject(instanceId);
                }
                return null;
            }

            public static bool IsPersistent(UnityObject unityObject) {
                if (VerifyBinding("EditorUtility.IsPersistent", _Bindings._EditorUtility_IsPersistent)) {
                    return _Bindings._EditorUtility_IsPersistent(unityObject);
                }
                return false;
            }

            public static GameObject CreateGameObjectWithHideFlags(string name, HideFlags hideFlags) {
                if (VerifyBinding("EditorUtility.CreateGameObjectWithHideFlags", _Bindings._EditorUtility_CreateGameObjectWithHideFlags)) {
                    return _Bindings._EditorUtility_CreateGameObjectWithHideFlags(name, hideFlags);
                }

                var go = new GameObject(name);
                go.hideFlags = hideFlags;
                return go;
            }
        }

        public static class EditorGUI {
            public static void BeginChangeCheck() {
                if (VerifyBinding("EditorGUI.BeginChangeCheck", _Bindings._EditorGUI_BeginDisabledGroup)) {
                    _Bindings._EditorGUI_BeginChangeCheck();
                }
            }

            public static bool EndChangeCheck() {
                if (VerifyBinding("EditorGUI.EndChangeCheck", _Bindings._EditorGUI_EndDisabledGroup)) {
                    return _Bindings._EditorGUI_EndChangeCheck();
                }
                return false;
            }

            public static void BeginDisabledGroup(bool disabled) {
                if (VerifyBinding("EditorGUI.BeginDisabledGroup", _Bindings._EditorGUI_BeginDisabledGroup)) {
                    _Bindings._EditorGUI_BeginDisabledGroup(disabled);
                }
            }

            public static void EndDisabledGroup() {
                if (VerifyBinding("EditorGUI.EndDisabledGroup", _Bindings._EditorGUI_EndDisabledGroup)) {
                    _Bindings._EditorGUI_EndDisabledGroup();
                }
            }

            public static bool Foldout(Rect rect, bool state, GUIContent label, bool toggleOnLabelClick, GUIStyle style) {
                if (VerifyBinding("EditorGUI.Foldout", _Bindings._EditorGUI_Foldout)) {
                    return _Bindings._EditorGUI_Foldout(rect, state, label, toggleOnLabelClick, style);
                }
                return true;
            }

            public static void HelpBox(Rect rect, string message, CommentType commentType) {
                if (VerifyBinding("EditorGUI.HelpBox", _Bindings._EditorGUI_HelpBox)) {
                    _Bindings._EditorGUI_HelpBox(rect, message, commentType);
                }
            }

            public static int IntSlider(Rect position, GUIContent label, int value, int leftValue, int rightValue) {
                if (VerifyBinding("EditorGUI.IntSlider", _Bindings._EditorGUI_IntSlider)) {
                    return _Bindings._EditorGUI_IntSlider(position, label, value, leftValue, rightValue);
                }
                return value;
            }

            public static int Popup(Rect position, GUIContent label, int selectedIndex, GUIContent[] displayedOptions) {
                if (VerifyBinding("EditorGUI.Popup", _Bindings._EditorGUI_Popup)) {
                    return _Bindings._EditorGUI_Popup(position, label, selectedIndex, displayedOptions);
                }
                return selectedIndex;
            }

            public static float Slider(Rect position, GUIContent label, float value, float leftValue, float rightValue) {
                if (VerifyBinding("EditorGUI.Slider", _Bindings._EditorGUI_Slider)) {
                    return _Bindings._EditorGUI_Slider(position, label, value, leftValue, rightValue);
                }
                return value;
            }
        }

        public static class EditorGUIUtility {
            public static float standardVerticalSpacing = 2f;
            public static float singleLineHeight = 16f;
        }

        public static class EditorStyles {
            public static GUIStyle label {
                get {
                    if (VerifyBinding("EditorStyles.label", _Bindings._EditorStyles_label)) {
                        return _Bindings._EditorStyles_label();
                    }

                    return new GUIStyle();
                }
            }

            public static GUIStyle foldout {
                get {
                    if (VerifyBinding("EditorStyles.foldout", _Bindings._EditorStyles_foldout)) {
                        return _Bindings._EditorStyles_foldout();
                    }

                    return new GUIStyle();
                }
            }
        }

        public static class fiEditorGUI {
            public static void PushHierarchyMode(bool state) {
                if (VerifyBinding("fiEditorGUI.PushHierarchyMode", _Bindings._fiEditorGUI_PushHierarchyMode)) {
                    _Bindings._fiEditorGUI_PushHierarchyMode(state);
                }
            }

            public static void PopHierarchyMode() {
                if (VerifyBinding("fiEditorGUI.PopHierarchyMode", _Bindings._fiEditorGUI_PopHierarchyMode)) {
                    _Bindings._fiEditorGUI_PopHierarchyMode();
                }
            }
        }

        public static class PrefabUtility {
            public static GameObject CreatePrefab(string path, GameObject template) {
                if (VerifyBinding("PrefabUtility.CreatePrefab", _Bindings._PrefabUtility_CreatePrefab)) {
                    return _Bindings._PrefabUtility_CreatePrefab(path, template);
                }
                return null;
            }

            public static bool IsPrefabInstance(UnityObject unityObject) {
                if (ReferenceEquals(unityObject, null))
                    return false;

                if (VerifyBinding("PrefabUtility.IsPrefabInstance", _Bindings._PrefabUtility_IsPrefabInstance)) {
                    return _Bindings._PrefabUtility_IsPrefabInstance(unityObject);
                }
                return false;
            }

            /// <summary>
            /// Returns true if UnityEditor.PrefabUtility.GetPrefabType(unityObj)
            /// == UnityEditor.PrefabType.Prefab
            /// </summary>
            public static bool IsPrefab(UnityObject unityObject) {
                if (VerifyBinding("PrefabUtility.IsPrefab", _Bindings._PrefabUtility_IsPrefab)) {
                    return _Bindings._PrefabUtility_IsPrefab(unityObject);
                }
                return false;
            }
        }

        public static class PropertyEditor {
            public static object Edit(Type objType, MemberInfo attrs, Rect rect, GUIContent label, object obj, fiGraphMetadataChild metadata, params Type[] skippedEditors) {
                if (VerifyBinding("PropertyEditor.Edit", _Bindings._PropertyEditor_Edit)) {
                    return _Bindings._PropertyEditor_Edit(objType, attrs, rect, label, obj, metadata, skippedEditors);
                }

                return obj;
            }

            public static float GetElementHeight(Type objType, MemberInfo attrs, GUIContent label, object obj, fiGraphMetadataChild metadata, params Type[] skippedEditors) {
                if (VerifyBinding("PropertyEditor.GetElementHeight", _Bindings._PropertyEditor_GetElementHeight)) {
                    return _Bindings._PropertyEditor_GetElementHeight(objType, attrs, label, obj, metadata, skippedEditors);
                }

                return 0;
            }

            public static object EditSkipUntilNot(Type[] skipUntilNot, Type objType, MemberInfo attrs, Rect rect, GUIContent label, object obj, fiGraphMetadataChild metadata) {
                if (VerifyBinding("PropertyEditor.EditSkipUntilNot", _Bindings._PropertyEditor_EditSkipUntilNot)) {
                    return _Bindings._PropertyEditor_EditSkipUntilNot(skipUntilNot, objType, attrs, rect, label, obj, metadata);
                }

                return obj;
            }

            public static float GetElementHeightSkipUntilNot(Type[] skipUntilNot, Type objType, MemberInfo attrs, GUIContent label, object obj, fiGraphMetadataChild metadata) {
                if (VerifyBinding("PropertyEditor.GetElementHeightSkipUntilNot", _Bindings._PropertyEditor_GetElementHeightSkipUntilNot)) {
                    return _Bindings._PropertyEditor_GetElementHeightSkipUntilNot(skipUntilNot, objType, attrs, label, obj, metadata);
                }

                return 0;
            }
        }

        public static class Selection {
            public static UnityObject activeObject {
                get {
                    if (VerifyBinding("Selection.activeObject", _Bindings._Selection_activeObject)) {
                        return _Bindings._Selection_activeObject();
                    }
                    return null;
                }
            }

            public static UnityObject[] activeSelection {
                get {
                    if (VerifyBinding("Selection.activeSelection", _Bindings._Selection_activeSelection)) {
                        return _Bindings._Selection_activeSelection();
                    }
                    return null;
                }
            }
        }

        private static bool VerifyBinding(string name, object obj) {
            if (obj == null) {
                if (fiUtility.IsEditor && fiSettings.EmitWarnings) {
                    Debug.LogWarning("There is no binding for " + name + " even though we are in an editor");
                }

                return false;
            }

            return true;
        }
    }
}