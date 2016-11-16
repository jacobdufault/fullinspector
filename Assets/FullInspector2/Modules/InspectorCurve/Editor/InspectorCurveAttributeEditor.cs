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
            if (attribute.TimeStart > attribute.TimeEnd) {
                Debug.Log("TimeStart cannot be larger than TimeEnd. Resetting to 0 and 1");
                attribute.TimeStart = 0;
                attribute.TimeEnd = 1;
            }

            if (attribute.ValueStart > attribute.ValueEnd) {
                Debug.Log("ValueStart cannot be larger than TimeStart. Resetting to 0 and 1");
                attribute.ValueStart = 0;
               attribute.ValueEnd =1;
            }


            var curveRange = new Rect(attribute.TimeStart, attribute.ValueStart, attribute.TimeEnd, attribute.ValueEnd);
            var curve = element == null ?
                AnimationCurve.Linear(0, 0, 1, 1) :
                Cast<AnimationCurve>(element);
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