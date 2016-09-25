using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FullInspector.Modules {
    [CustomPropertyEditor(typeof(KeyValuePair<,>))]
    public class KeyValuePairPropertyEditor<TKey, TValue> : PropertyEditor<KeyValuePair<TKey, TValue>> {
        private readonly PropertyEditorChain _keyEditor = PropertyEditor.Get(typeof(TKey), null);
        private readonly PropertyEditorChain _valueEditor = PropertyEditor.Get(typeof(TValue), null);
        private float _widthPercentage = 0.3f;

        public KeyValuePairPropertyEditor(Type editedType, ICustomAttributeProvider attributes) {
            if (attributes != null) {
                var attrs = attributes.GetCustomAttributes(typeof(InspectorKeyWidthAttribute), /*inherit:*/true);
                if (attrs != null && attrs.Length >= 1) {
                    _widthPercentage = ((InspectorKeyWidthAttribute)attrs[0]).WidthPercentage;
                }
            }
        }

        /// <summary>
        /// Splits the given rect into two rects that are divided horizontally.
        /// </summary>
        /// <param name="rect">The rect to split</param>
        /// <param name="percentage">The horizontal percentage that the rects are split at</param>
        /// <param name="margin">How much space that should be between the two rects</param>
        /// <param name="left">The output left-hand side rect</param>
        /// <param name="right">The output right-hand side rect</param>
        private static void SplitRect(Rect rect, float percentage, float margin, out Rect left, out Rect right) {
            left = new Rect(rect);
            left.width *= percentage;

            right = new Rect(rect);
            right.x += left.width + margin;
            right.width -= left.width + margin;
        }


        public override KeyValuePair<TKey, TValue> Edit(Rect region, GUIContent label, KeyValuePair<TKey, TValue> element, fiGraphMetadata metadata) {
            Rect keyRect, valueRect;
            SplitRect(region, /*percentage:*/ _widthPercentage, /*margin:*/ 5, out keyRect, out valueRect);

            keyRect.height = _keyEditor.FirstEditor.GetElementHeight(label, element.Key, metadata.Enter("Key"));
            valueRect.height = _valueEditor.FirstEditor.GetElementHeight(GUIContent.none, element.Value, metadata.Enter("Value"));

            var newKey = _keyEditor.FirstEditor.Edit(keyRect, label, element.Key, metadata.Enter("Key"));
            var newValue = _valueEditor.FirstEditor.Edit(valueRect, GUIContent.none, element.Value, metadata.Enter("Value"));

            return new KeyValuePair<TKey, TValue>(newKey, newValue);
        }

        public override float GetElementHeight(GUIContent label, KeyValuePair<TKey, TValue> element, fiGraphMetadata metadata) {
            float keyHeight = _keyEditor.FirstEditor.GetElementHeight(label, element.Key, metadata.Enter("Key"));
            float valueHeight = _valueEditor.FirstEditor.GetElementHeight(GUIContent.none, element.Value, metadata.Enter("Value"));

            return Math.Max(keyHeight, valueHeight);
        }
    }
}