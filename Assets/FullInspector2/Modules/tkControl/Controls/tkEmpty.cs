using UnityEngine;

namespace FullInspector {
    public partial class tk<T, TContext> {
        /// <summary>
        /// A rule that does nothing. The returned height can be customized.
        /// </summary>
        public class Empty : tkControl<T, TContext> {
            [ShowInInspector]
            private readonly Value<float> _height;

            public Empty() :
                this(0) {
            }

            public Empty(Value<float> height) {
                _height = height;
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata) {
                return obj;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata) {
                return _height.GetCurrentValue(obj, context);
            }
        }
    }
}