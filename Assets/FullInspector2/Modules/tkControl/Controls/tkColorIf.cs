using UnityEngine;

namespace FullInspector {
    public partial class tk<T, TContext> {
        /// <summary>
        /// This will change the color of the subcontrol if the given predicate passes.
        /// </summary>
        public class ColorIf : ConditionalStyle {
            public ColorIf(Value<bool> shouldActivate, Value<UnityEngine.Color> color) :
                base(shouldActivate.GetCurrentValue,
                    (obj, context) => {
                        var savedColor = GUI.color;
                        GUI.color = color.GetCurrentValue(obj, context);
                        return savedColor;
                    },
                    (obj, context, state) => {
                        GUI.color = (UnityEngine.Color)state;
                    }) {
            }

            public ColorIf(Value<bool>.Generator shouldActivate, Value<UnityEngine.Color> color)
                : this(Val(shouldActivate), color) {
            }
            public ColorIf(Value<bool>.GeneratorNoContext shouldActivate, Value<UnityEngine.Color> color)
                : this(Val(shouldActivate), color) {
            }

        }
    }
}