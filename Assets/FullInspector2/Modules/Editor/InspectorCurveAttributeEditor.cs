using System;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules {
    [CustomAttributePropertyEditor(typeof(InspectorCurveAttribute), ReplaceOthers = true)]
    public class InspectorCurveAttributeEditor<TElement> : AttributePropertyEditor<TElement, InspectorCurveAttribute> {
        private static T Cast<T>(object o) {
            return (T)Convert.ChangeType(o, typeof(T));
        }

        protected override TElement Edit(Rect region, GUIContent label, TElement element, InspectorCurveAttribute attribute, fiGraphMetadata metadata) {
            if (attribute.xMin >= attribute.xMax) {
                Debug.Log("xMin cannot be larger than xMax. Resetting to 0 and 1");
                attribute.xMin = 0;
                attribute.xMax = 1;
            }

            if (attribute.yMin >= attribute.yMax) {
                Debug.Log("yMin cannot be larger than yMax. Resetting to 0 and 1");
                attribute.yMin = 0;
               attribute.yMax =1;
            }

            var curveRange = new Rect(attribute.xMin, attribute.yMin, attribute.xMax, attribute.yMax);
            var curve = Cast<AnimationCurve>(element);
            return Cast<TElement>(EditorGUI.CurveField(region, label, curve, Color.green, curveRange));
        }

        protected override float GetElementHeight(GUIContent label, TElement element, InspectorCurveAttribute attribute, fiGraphMetadata metadata) {
            return EditorStyles.largeLabel.CalcHeight(label, 100);
        }

        public override bool CanEdit(Type type) {
            return type == typeof(AnimationCurve);
        }
    }
}