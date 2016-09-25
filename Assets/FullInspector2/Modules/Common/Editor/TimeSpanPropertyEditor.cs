using System;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules {
    [CustomPropertyEditor(typeof(TimeSpan))]
    public class TimeSpanPropertyEditor : PropertyEditor<TimeSpan> {
        public override TimeSpan Edit(Rect region, GUIContent label, TimeSpan element, fiGraphMetadata metadata) {
            if (string.IsNullOrEmpty(label.tooltip)) {
                var tooltip = string.Format("{0} days, {1} hours, {2} minutes, {3} seconds, {4} milliseconds",
                    element.Days, element.Hours, element.Minutes, element.Seconds, element.Milliseconds);
                label.tooltip = tooltip;
            }

            string updated = EditorGUI.TextField(region, label, element.ToString());

            TimeSpan result;
            if (TimeSpan.TryParse(updated, out result)) {
                return result;
            }

            return element;
        }

        public override float GetElementHeight(GUIContent label, TimeSpan element, fiGraphMetadata metadata) {
            return EditorStyles.label.CalcHeight(label, 100);
        }
    }
}