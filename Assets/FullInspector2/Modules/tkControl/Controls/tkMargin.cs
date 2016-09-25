using UnityEngine;

namespace FullInspector {
    public partial class tk<T, TContext> {
        public class Margin : tkControl<T, TContext> {
            [ShowInInspector]
            private readonly Value<float> _left, _top, _right, _bottom;
            [ShowInInspector]
            private readonly tkControl<T, TContext> _control;

            public Margin(Value<float> margin, tkControl<T, TContext> control)
                : this(margin, margin, margin, margin, control) {
            }

            public Margin(Value<float> left, Value<float> top, tkControl<T, TContext> control)
                : this(left, top, left, top, control) {
            }

            public Margin(Value<float> left, Value<float> top, Value<float> right, Value<float> bottom, tkControl<T, TContext> control) {
                _left = left;
                _top = top;
                _right = right;
                _bottom = bottom;
                _control = control;
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata) {
                var left = _left.GetCurrentValue(obj, context);
                var right = _right.GetCurrentValue(obj, context);
                var top = _top.GetCurrentValue(obj, context);
                var bottom = _bottom.GetCurrentValue(obj, context);

                rect.x += left;
                rect.width -= left + right;
                rect.y += top;
                rect.height -= top + bottom;
                return _control.Edit(rect, obj, context, metadata);
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata) {
                var top = _top.GetCurrentValue(obj, context);
                var bottom = _bottom.GetCurrentValue(obj, context);

                return _control.GetHeight(obj, context, metadata) + top + bottom;
            }
        }
    }
}