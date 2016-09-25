using System;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Internal {
    public class fiEditorGUILayout {
        // see http://answers.unity3d.com/questions/216584/horizontal-line.html

        private static readonly GUIStyle splitter;

        static fiEditorGUILayout() {
            splitter = new GUIStyle();
            splitter.normal.background = EditorGUIUtility.whiteTexture;
            splitter.stretchWidth = true;
            splitter.margin = new RectOffset(0, 0, 7, 7);
        }

        private static readonly Color splitterColor = EditorGUIUtility.isProSkin ?
            new Color(0.157f, 0.157f, 0.157f) : new Color(0.5f, 0.5f, 0.5f);


        public static void Splitter(Color rgb, float thickness) {
            Rect position = GUILayoutUtility.GetRect(GUIContent.none, splitter,
                GUILayout.Height(thickness));

            if (Event.current.type == EventType.Repaint) {
                Color restoreColor = GUI.color;
                GUI.color = rgb;
                splitter.Draw(position, false, false, false, false);
                GUI.color = restoreColor;
            }
        }

        public static void Splitter(float thickness, GUIStyle splitterStyle) {
            Rect position = GUILayoutUtility.GetRect(GUIContent.none, splitterStyle,
                GUILayout.Height(thickness));

            if (Event.current.type == EventType.Repaint) {
                Color restoreColor = GUI.color;
                GUI.color = splitterColor;
                splitterStyle.Draw(position, false, false, false, false);
                GUI.color = restoreColor;
            }
        }

        public static void Splitter(float thickness) {
            Splitter(thickness, splitter);
        }


        public static void WithIndent(float pixels, Action code) {
            GUILayout.BeginHorizontal();
            GUILayout.Space(pixels);
            GUILayout.BeginVertical();
            code();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
    }
}