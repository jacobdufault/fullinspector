using System;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules.InspectorRange {
    [CustomAttributePropertyEditor(typeof(InspectorRangeAttribute), ReplaceOthers = true)]
    public class InspectorRangeAttributeEditor<TElement> : AttributePropertyEditor<TElement, InspectorRangeAttribute> {
        private static T Cast<T>(object o) {
            return (T)Convert.ChangeType(o, typeof(T));
        }

        protected override TElement Edit(Rect region, GUIContent label, TElement element, InspectorRangeAttribute attribute, fiGraphMetadata metadata) {
            if (float.IsNaN(attribute.Step) == false) {
                if (attribute.Step <= 0) {
                    Debug.LogWarning(attribute.Step + " is not a valid step. It must be greater than 0.");
                    attribute.Step = float.NaN;
                }
            }

            if (float.IsNaN(attribute.Step) == false) {
                return Cast<TElement>((int)(EditorGUI.Slider(region, label, Cast<float>(element), attribute.Min, attribute.Max) / attribute.Step) * attribute.Step);
            }

            return Cast<TElement>(EditorGUI.Slider(region, label, Cast<float>(element), attribute.Min, attribute.Max));
        }

        protected override float GetElementHeight(GUIContent label, TElement element, InspectorRangeAttribute attribute, fiGraphMetadata metadata) {
            return EditorStyles.label.CalcHeight(label, 100);
        }

        public override bool CanEdit(Type type) {
            return type == typeof(int) || type == typeof(double) || type == typeof(float) || type == typeof(decimal);
        }
    }
}