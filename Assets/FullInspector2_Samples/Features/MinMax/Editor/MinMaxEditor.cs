using System;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Samples.MinMaxSample {
    [CustomPropertyEditor(typeof(MinMax<>))]
    public class MinMaxEditor<TElement> : PropertyEditor<MinMax<TElement>> {
        /// <summary>
        /// Formats a float so that it shows up to two decimal places if they are non-zero.
        /// </summary>
        public static string FormatFloat(float num) {
            var s = string.Format("{0:0.00}", num).TrimEnd('0');
            if (s.EndsWith(".")) {
                return s.TrimEnd('.');
            }
            return s;
        }

        private static float ToFloat(TElement element) {
            return (float)Convert.ChangeType(element, typeof(float));
        }

        private static TElement FromFloat(float f) {
            return (TElement)Convert.ChangeType(f, typeof(TElement));
        }

        public override MinMax<TElement> Edit(Rect region, GUIContent label, MinMax<TElement> element, fiGraphMetadata metadata) {
            float min = ToFloat(element.Min);
            float max = ToFloat(element.Max);
            float minLimit = ToFloat(element.MinLimit);
            float maxLimit = ToFloat(element.MaxLimit);

            string labelText = label.text + string.Format(" ({0}/{2} - {1}/{3})",
                FormatFloat(min), FormatFloat(max),
                FormatFloat(minLimit), FormatFloat(maxLimit));
            var updatedLabel = new GUIContent(labelText, label.image, label.tooltip);

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4
            EditorGUI.MinMaxSlider(updatedLabel, region, ref min, ref max, minLimit, maxLimit);
#else
            EditorGUI.MinMaxSlider(region, updatedLabel, ref min, ref max, minLimit, maxLimit);
#endif

            return new MinMax<TElement>() {
                Min = FromFloat(min),
                Max = FromFloat(max),
                MinLimit = FromFloat(minLimit),
                MaxLimit = FromFloat(maxLimit)
            };
        }

        public override float GetElementHeight(GUIContent label, MinMax<TElement> element, fiGraphMetadata metadata) {
            return EditorStyles.largeLabel.CalcHeight(label, 100);
        }
    }
}