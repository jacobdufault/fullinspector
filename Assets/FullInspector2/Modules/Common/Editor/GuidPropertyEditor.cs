using System;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules {
    [CustomPropertyEditor(typeof(Guid))]
    public class GuidPropertyEditor : PropertyEditor<Guid> {
        private const float Split = .7f;
        private const int Margin = 3;

        public override Guid Edit(Rect region, GUIContent label, Guid element, fiGraphMetadata metadata) {
            // create the two rects
            float splitWidth = region.width * Split;
            Rect guidEditor = new Rect(region.xMin, region.yMin, splitWidth, region.height);
            Rect randGuid = new Rect(region.xMin + splitWidth + Margin, region.yMin,
                region.width - splitWidth - Margin, region.height);

            string updatedGuid = EditorGUI.TextField(guidEditor, label, element.ToString());

            if (GUI.Button(randGuid, "New GUID")) {
                return Guid.NewGuid();
            }

            try {
                return new Guid(updatedGuid);
            }
            catch (ArgumentNullException e) {
                Debug.LogError("Null GUID; " + e);
            }
            catch (FormatException e) {
                Debug.LogError("Bad GUID format; " + e);
            }

            // failed to create a new GUID for some reason; return the original one
            return element;
        }

        public override float GetElementHeight(GUIContent label, Guid element, fiGraphMetadata metadata) {
            return EditorStyles.textField.CalcHeight(label, 1000);
        }
    }
}