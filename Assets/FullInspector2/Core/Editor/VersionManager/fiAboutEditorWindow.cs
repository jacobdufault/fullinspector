using FullInspector.Internal.Versioning;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Internal {
    public class fiAboutEditorWindow : EditorWindow {
        [MenuItem("Window/Full Inspector/About")]
        public static void ShowWindow() {
            fiEditorWindowUtility.ShowFixedSizeUtility<fiAboutEditorWindow>("About Full Inspector", fiEditorImages.Logo.Decoded.width, 330);
        }

        public void OnGUI() {
            var logoRect = EditorGUILayout.GetControlRect(/*hasLabel:*/ false, /*height:*/ fiEditorImages.Logo.Decoded.height);
            GUI.DrawTexture(logoRect, fiEditorImages.Logo.Decoded, ScaleMode.ScaleToFit);

            EditorGUILayout.Space();

            var linksRect = EditorGUILayout.BeginHorizontal();
            linksRect.x += 4;
            linksRect.width -= 4;
            GUI.Box(linksRect, GUIContent.none);
            
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("View Product Website", EditorStyles.boldLabel)) {
                Application.OpenURL("http://jacobdufault.github.io/fullinspector/");
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("View Documentation", EditorStyles.boldLabel)) {
                Application.OpenURL("http://jacobdufault.github.io/fullinspector/guide");
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Report an Issue", EditorStyles.boldLabel)) {
                Application.OpenURL("http://www.github.com/jacobdufault/fullinspector/issues");
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Forum Topic", EditorStyles.boldLabel)) {
                Application.OpenURL("http://forum.unity3d.com/threads/full-inspector-inspector-and-serialization-for-structs-dicts-generics-interfaces.224270/");
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Download Samples", EditorStyles.boldLabel)) {
                Application.OpenURL("http://www.github.com/jacobdufault/fullinspectorsamples");
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Full Inspector is property of Jacob Dufault");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Current Version: " + fiVersionManager.CurrentVersion);
            GUILayout.EndHorizontal();
        }
    }
}