using FullInspector.Internal;
using UnityEngine;

namespace FullInspector {
    public partial class tk<T, TContext> {
        /// <summary>
        /// Provides access to the currently edited value.
        /// </summary>
        public class Context : tkControl<T, TContext> {
            [ShowInInspector]
            private tkControl<T, TContext> _control;

            [ShowInInspector]
            private readonly fiStackValue<T> _data = new fiStackValue<T>();

            public tkControl<T, TContext> With(tkControl<T, TContext> control) {
                _control = control;
                return this;
            }

            public T Data {
                get { return _data.Value; }
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata) {
                _data.Push(obj);
                obj = _control.Edit(rect, obj, context, metadata);
                _data.Pop();
                return obj;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata) {
                _data.Push(obj);
                var height = _control.GetHeight(obj, context, metadata);
                _data.Pop();
                return height;
            }
        }
    }
}