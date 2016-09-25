using System;
using FullInspector.Internal;

namespace FullInspector {
    public partial class tk<T, TContext> {
        /// <summary>
        /// This enables running code before/after running the actual control editing logic assuming some
        /// predicate first passes the test. Typically you'll want to use classes derived from this one,
        /// like ColorIf, instead of this.
        /// </summary>
        public class ConditionalStyle : tkStyle<T, TContext> {
            private readonly Func<T, TContext, bool> _shouldActivate;
            private readonly Func<T, TContext, object> _activate;
            private readonly Action<T, TContext, object> _deactivate;
            private readonly fiStackValue<bool> _activatedStack = new fiStackValue<bool>();
            private readonly fiStackValue<object> _activationStateStack = new fiStackValue<object>();

            /// <summary>
            /// Create a new conditional style.
            /// </summary>
            /// <param name="shouldActivate">Used to determine if the modifier should activate.</param>
            /// <param name="activate">This is the activation logic. The return value will be passed into deactivate.</param>
            /// <param name="deactivate">The deactivate function.</param>
            public ConditionalStyle(
                Func<T, TContext, bool> shouldActivate,
                Func<T, TContext, object> activate,
                Action<T, TContext, object> deactivate) {
                
                _shouldActivate = shouldActivate;
                _activate = activate;
                _deactivate = deactivate;
            }

            public override void Activate(T obj, TContext context) {
                bool shouldActivate = _shouldActivate(obj, context);
                _activatedStack.Push(shouldActivate);

                if (shouldActivate) {
                    _activationStateStack.Push(_activate(obj, context));
                }
            }

            public override void Deactivate(T obj, TContext context) {
                bool activated = _activatedStack.Pop();
                if (activated) {
                    _deactivate(obj, context, _activationStateStack.Pop());
                }
            }
        }
    }
}