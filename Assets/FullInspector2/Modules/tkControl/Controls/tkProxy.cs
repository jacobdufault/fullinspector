using UnityEngine;

namespace FullInspector {
    public partial class tk<T, TContext> {
        /// <summary>
        /// Does nothing by itself. However, this can be used for applying a style to layout style controls
        /// which override the Add method.
        /// </summary>
        public class StyleProxy : tkControl<T, TContext> {
            public tkControl<T, TContext> Control;

            public StyleProxy() {
            }
            public StyleProxy(tkControl<T, TContext> control) {
                Control = control;
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata) {
                return Control.Edit(rect, obj, context, metadata);
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata) {
                return Control.GetHeight(obj, context, metadata);
            }
        }
    }
}