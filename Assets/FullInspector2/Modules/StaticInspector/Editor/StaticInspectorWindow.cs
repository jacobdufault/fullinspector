using System;
using FullInspector.Internal;
using FullSerializer.Internal;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules {
    public class StaticInspectorWindow : EditorWindow {
        [MenuItem("Window/Full Inspector/Static Inspector &i")]
        public static void Activate() {
            fiEditorWindowUtility.Show<StaticInspectorWindow>("Static Inspector");
        }

        protected void OnEnable() {
            fiEditorUtility.RepaintableEditorWindows.Add(this);
        }

        protected void OnDestroy() {
            fiEditorUtility.RepaintableEditorWindows.Remove(this);
        }

        /// <summary>
        /// The current scrolling position for the static inspector.
        /// </summary>
        private Vector2 _inspectorScrollPosition;

        /// <summary>
        /// The type that we are currently viewing the statics for. Unfortunately, we have to store
        /// this type as a string so that Unity can serialize it. It would be awesome to have FI
        /// serialization on EditorWindows, but oh well :P.
        /// </summary>
        private string _serializedInspectedType;
        private Type _inspectedType {
            get {
                return fsTypeCache.GetType(_serializedInspectedType);
            }
            set {
                if (value == null) {
                    _serializedInspectedType = string.Empty;
                }
                else {
                    _serializedInspectedType = value.FullName;
                }
            }
        }

        private static fiGraphMetadata Metadata = new fiGraphMetadata();

        public void OnGUI() {
            try {
                EditorGUIUtility.hierarchyMode = true;
                Type updatedType = _inspectedType;

                GUILayout.Label("Static Inspector", EditorStyles.boldLabel);

                {
                    var label = new GUIContent("Inspected Type");
                    var typeEditor = PropertyEditor.Get(typeof(Type), null);

                    updatedType = typeEditor.FirstEditor.EditWithGUILayout(label, _inspectedType, Metadata.Enter("TypeSelector"));
                }

                fiEditorGUILayout.Splitter(2);

                if (_inspectedType != null) {
                    _inspectorScrollPosition = EditorGUILayout.BeginScrollView(_inspectorScrollPosition);

                    var inspectedType = InspectedType.Get(_inspectedType);
                    foreach (InspectedProperty staticProperty in inspectedType.GetProperties(InspectedMemberFilters.StaticInspectableMembers)) {
                        var editorChain = PropertyEditor.Get(staticProperty.StorageType, staticProperty.MemberInfo);
                        IPropertyEditor editor = editorChain.FirstEditor;

                        GUIContent label = new GUIContent(staticProperty.Name);
                        object currentValue = staticProperty.Read(null);

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.GetControlRect(GUILayout.Width(8));
                        EditorGUILayout.BeginVertical();

                        GUI.enabled = staticProperty.CanWrite;
                        object updatedValue = editor.EditWithGUILayout(label, currentValue, Metadata.Enter(staticProperty.Name));
                        if (staticProperty.CanWrite) {
                            staticProperty.Write(null, updatedValue);
                        }

                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndScrollView();
                }

                // For some reason, the type selection popup window cannot force the rest of the
                // Unity GUI to redraw. We do it here instead -- this removes any delay after
                // selecting the type in the popup window and the type actually being displayed.
                if (fiEditorUtility.ShouldInspectorRedraw.Enabled) {
                    Repaint();
                }

                if (_inspectedType != updatedType) {
                    _inspectedType = updatedType;
                    Repaint();
                }

                EditorGUIUtility.hierarchyMode = false;
            }
            catch (ExitGUIException) { }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }
    }
}