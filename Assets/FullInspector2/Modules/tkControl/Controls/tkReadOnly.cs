namespace FullInspector {
    public partial class tk<T, TContext> {
        /// <summary>
        /// The control will be drawn with a read-only interface.
        /// </summary>
        public class ReadOnly : ReadOnlyIf {
            public ReadOnly()
                : base(Val(o => true)) {
            }
        }
    }
}