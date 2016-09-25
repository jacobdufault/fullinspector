using UnityEngine;

namespace FullInspector {
    public partial class tk<T, TContext> {
        /// <summary>
        /// This will render a box around the edited rectangle.
        /// </summary>
        public class Box : tkControl<T, TContext> {
            [ShowInInspector]
            private readonly tkControl<T, TContext> _control;

            public Box(tkControl<T, TContext> control) {
                _control = control;
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata) {
                GUI.Box(rect, "");
                return _control.Edit(rect, obj, context, metadata);
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata) {
                return _control.GetHeight(obj, context, metadata);
            }
        }
    }
}