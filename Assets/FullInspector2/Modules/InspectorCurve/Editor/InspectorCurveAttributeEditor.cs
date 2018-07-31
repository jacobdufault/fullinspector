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
            if (attribute.Width <= 0) {
                Debug.Log("Invalid width. Resetting to 1");
                attribute.Width = 1;
            }

            if (attribute.Height <= 0) {
                Debug.Log("Invalid height. Resetting to 1");
                attribute.Height = 1;
            }

            var curveRange = new Rect(attribute.X, attribute.Y, attribute.Width, attribute.Height);
            var curve = element == null ?
                AnimationCurve.Linear(0, 1, 1, 1) :
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