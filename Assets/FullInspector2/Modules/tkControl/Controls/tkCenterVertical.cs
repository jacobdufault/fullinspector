using UnityEngine;

namespace FullInspector {
    public partial class tk<T, TContext> {
        /// <summary>
        /// Centers the layout rule within it vertically.
        /// </summary>
        public class CenterVertical : tkControl<T, TContext> {
            [ShowInInspector]
            private readonly tkControl<T, TContext> _centered;

            public CenterVertical(tkControl<T, TContext> centered) {
                _centered = centered;
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata) {
                float padding = rect.height - _centered.GetHeight(obj, context, metadata);
                rect.y += padding / 2;
                rect.height -= padding;

                return _centered.Edit(rect, obj, context, metadata);
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata) {
                return _centered.GetHeight(obj, context, metadata);
            }
        }
    }
}