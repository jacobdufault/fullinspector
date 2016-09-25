using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules {
    [CustomPropertyEditor(typeof(PropertyEditor<DateTimeOffset>))]
    public class DateTimeOffsetPropertyEditor : PropertyEditor<DateTimeOffset> {
        public override DateTimeOffset Edit(Rect region, GUIContent label, DateTimeOffset element, fiGraphMetadata metadata) {
            string updated = EditorGUI.TextField(region, label, element.ToString("o"));

            DateTimeOffset result;
            if (DateTimeOffset.TryParse(updated, null, DateTimeStyles.RoundtripKind, out result)) {
                return result;
            }

            return element;
        }

        public override float GetElementHeight(GUIContent label, DateTimeOffset element, fiGraphMetadata metadata) {
            return EditorStyles.label.CalcHeight(label, 100);
        }
    }
}