using FullInspector.Internal;

namespace FullInspector {
    public partial class tk<T, TContext> {
        /// <summary>
        /// The control will be drawn with a read-only UI if the predicate returns true.
        /// </summary>
        public class ReadOnlyIf : ConditionalStyle {
            public ReadOnlyIf(Value<bool> isReadOnly) :
                base(isReadOnly.GetCurrentValue,
                    (obj, context) => {
                        fiLateBindings.EditorGUI.BeginDisabledGroup(true);
                        return null;
                    },
                    (obj, context, state) => {
                        fiLateBindings.EditorGUI.EndDisabledGroup();
                    }) {
            }

            public ReadOnlyIf(Value<bool>.Generator isReadOnly)
                : this(Val(isReadOnly)) {
            }
            public ReadOnlyIf(Value<bool>.GeneratorNoContext isReadOnly)
                : this(Val(isReadOnly)) {
            }

        }
    }
}