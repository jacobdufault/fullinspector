using UnityEngine;

namespace FullInspector {
    public partial class tk<T, TContext> {
        /// <summary>
        /// This horizontally indents the given subcontrol.
        /// </summary>
        public class Indent : tkControl<T, TContext> {
            [ShowInInspector]
            private readonly Value<float> _indent;
            [ShowInInspector]
            private readonly tkControl<T, TContext> _control;

            public Indent(tkControl<T, TContext> control)
                : this(15, control) {
            }

            public Indent(Value<float> indent, tkControl<T, TContext> control) {
                _indent = indent;
                _control = control;
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata) {
                var indent = _indent.GetCurrentValue(obj, context);

                rect.x += indent;
                rect.width -= indent;
                return _control.Edit(rect, obj, context, metadata);
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata) {
                return _control.GetHeight(obj, context, metadata);
            }
        }
    }
}