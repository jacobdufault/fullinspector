using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Internal {
    public class fiLogEditorWindow : EditorWindow {
        static fiLogEditorWindow() {
            fiLog.Log("", "-- RELOADED --");
        }

        [MenuItem("Window/Full Inspector/Developer/Log")]
        public static void ShowWindow() {
            var window = fiEditorWindowUtility.Show<fiLogEditorWindow>("fiLog");
            window.autoRepaintOnSceneChange = true;
        }

        private Vector2 _scroll;
        private List<string> _messages = new List<string>();
        private string _filter = string.Empty;

        private IEnumerable<string> FilteredMessages {
            get {
                return from message in _messages
                       where string.IsNullOrEmpty(_filter) || 
                             message.IndexOf(_filter, StringComparison.OrdinalIgnoreCase) >= 0
                       select message;
            }
        }
        public void OnGUI() {
            fiLog.InsertAndClearMessagesTo(_messages);

            _filter = GUILayout.TextField(_filter, GUI.skin.FindStyle("ToolbarSeachTextField"));

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear")) {
                _messages.Clear();
            }
            if (GUILayout.Button("Copy to Clipboard", GUILayout.ExpandWidth(false))) {
                EditorGUIUtility.systemCopyBuffer = string.Join(Environment.NewLine, FilteredMessages.ToArray());
                Debug.Log("Copied contents to clipboard");
            }
            EditorGUILayout.EndHorizontal();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            foreach (var message in FilteredMessages) { 
                int controlId = GUIUtility.GetControlID(FocusType.Passive) + 1;
                EditorGUILayout.TextArea(message, EditorStyles.label);
                if (GUIUtility.hotControl == controlId) {
                    // automatically deselect any selected text control
                    GUIUtility.hotControl = -1;
                }
            }

            EditorGUILayout.EndScrollView();
        }
    }
}