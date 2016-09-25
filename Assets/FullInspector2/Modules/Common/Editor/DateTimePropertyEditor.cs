using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules {
    [CustomPropertyEditor(typeof(DateTime))]
    public class DateTimePropertyEditor : PropertyEditor<DateTime> {
        public override DateTime Edit(Rect region, GUIContent label, DateTime element, fiGraphMetadata metadata) {
            string updated = EditorGUI.TextField(region, label, element.ToString("o"));

            DateTime result;
            if (DateTime.TryParse(updated, null, DateTimeStyles.RoundtripKind, out result)) {
                return result;
            }

            return element;
        }

        public override float GetElementHeight(GUIContent label, DateTime element, fiGraphMetadata metadata) {
            return EditorStyles.label.CalcHeight(label, 100);
        }
    }
}