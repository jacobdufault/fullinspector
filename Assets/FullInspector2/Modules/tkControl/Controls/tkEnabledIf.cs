using FullInspector.Internal;

namespace FullInspector {
    public partial class tk<T, TContext> {
        /// <summary>
        /// Draws the control with a read-only GUI if the predicate fails
        /// </summary>
        public class EnabledIf : ConditionalStyle {
            public EnabledIf(Value<bool> isEnabled) :
                base((o, c) => !isEnabled.GetCurrentValue(o, c),
                    (obj, context) => {
                        fiLateBindings.EditorGUI.BeginDisabledGroup(true);
                        return null;
                    },
                    (obj, context, state) => {
                        fiLateBindings.EditorGUI.EndDisabledGroup();
                    }) {
            }

            public EnabledIf(Value<bool>.Generator isEnabled)
                : this(Val(isEnabled)) {
            }
            public EnabledIf(Value<bool>.GeneratorNoContext isEnabled)
                : this(Val(isEnabled)) {
            }

        }
    }
}