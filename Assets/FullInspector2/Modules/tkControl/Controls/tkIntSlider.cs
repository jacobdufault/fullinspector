using System;
using FullInspector.Internal;
using UnityEngine;

namespace FullInspector {
    public partial class tk<T, TContext> {
        /// <summary>
        /// This will render an int slider.
        /// </summary>
        public class IntSlider : tkControl<T, TContext> {
            private readonly Value<int> _min;
            private readonly Value<int> _max;
            private readonly Func<T, TContext, int> _getValue;
            private readonly Action<T, TContext, int> _setValue;
            private readonly Value<fiGUIContent> _label;

            public IntSlider(
                Value<int> min, Value<int> max,
                Func<T, TContext, int> getValue, Action<T, TContext, int> setValue)
                : this(fiGUIContent.Empty, min, max, getValue, setValue) {
            }

            public IntSlider(
                Value<fiGUIContent> label,
                Value<int> min, Value<int> max,
                Func<T, TContext, int> getValue, Action<T, TContext, int> setValue) {

                _label = label;
                _min = min;
                _max = max;
                _getValue = getValue;
                _setValue = setValue;
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata) {
                var value = _getValue(obj, context);
                var min = _min.GetCurrentValue(obj, context);
                var max = _max.GetCurrentValue(obj, context);

                fiLateBindings.EditorGUI.BeginChangeCheck();
                value = fiLateBindings.EditorGUI.IntSlider(rect, _label.GetCurrentValue(obj, context), value, min, max);
                if (fiLateBindings.EditorGUI.EndChangeCheck()) {
                    _setValue(obj, context, value);
                }

                return obj;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata) {
                return fiLateBindings.EditorGUIUtility.singleLineHeight;
            }
        }
    }
}