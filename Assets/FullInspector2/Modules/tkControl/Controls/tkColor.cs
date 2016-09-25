namespace FullInspector {
    public partial class tk<T, TContext> {
        /// <summary>
        /// This will render the given control with the given color.
        /// </summary>
        public class Color : ColorIf {
            public Color(Value<UnityEngine.Color> color)
                : base(Val(o => true), color) {
            }
        }
    }
}