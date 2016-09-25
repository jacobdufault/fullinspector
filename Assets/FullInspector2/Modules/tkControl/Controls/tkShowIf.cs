using UnityEngine;

namespace FullInspector {
    public partial class tk<T, TContext> {
        /// <summary>
        /// The control will displayed only if the given predicate returns true.
        /// </summary>
        public class ShowIf : tkControl<T, TContext> {
            private readonly Value<bool> _shouldDisplay;

            [ShowInInspector]
            private readonly tkControl<T, TContext> _control;

            public ShowIf(Value<bool> shouldDisplay, tkControl<T, TContext> control) {
                _shouldDisplay = shouldDisplay;
                _control = control;
            }

            public ShowIf(Value<bool>.Generator shouldDisplay, tkControl<T, TContext> control)
                : this(new Value<bool>(shouldDisplay), control) {
            }
            public ShowIf(Value<bool>.GeneratorNoContext shouldDisplay, tkControl<T, TContext> control)
                : this(new Value<bool>(shouldDisplay), control) {
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata) {
                return _control.Edit(rect, obj, context, metadata);
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata) {
                return _control.GetHeight(obj, context, metadata);
            }

            public override bool ShouldShow(T obj, TContext context, fiGraphMetadata metadata) {
                return _shouldDisplay.GetCurrentValue(obj, context);
            }
        }
    }
}