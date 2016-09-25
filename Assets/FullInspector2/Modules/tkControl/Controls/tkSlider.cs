using System;
using FullInspector.Internal;
using UnityEngine;

namespace FullInspector {
    public partial class tk<T, TContext> {
        /// <summary>
        /// This will render a slider.
        /// </summary>
        public class Slider : tkControl<T, TContext> {
            private readonly Value<float> _min;
            private readonly Value<float> _max;
            private readonly Func<T, TContext, float> _getValue;
            private readonly Action<T, TContext, float> _setValue;
            private readonly Value<fiGUIContent> _label;

            public Slider(
                Value<float> min, Value<float> max,
                Func<T, TContext, float> getValue, Action<T, TContext, float> setValue)
                : this(fiGUIContent.Empty, min, max, getValue, setValue) {
            }

            public Slider(
                Value<fiGUIContent> label,
                Value<float> min, Value<float> max,
                Func<T, TContext, float> getValue, Action<T, TContext, float> setValue) {

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
                value = fiLateBindings.EditorGUI.Slider(rect, _label.GetCurrentValue(obj, context), value, min, max);
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