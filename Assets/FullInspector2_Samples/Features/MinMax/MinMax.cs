
namespace FullInspector.Samples.MinMaxSample {
    public struct MinMax<TElement> {
        public TElement Min;
        public TElement Max;

        public TElement MinLimit;
        public TElement MaxLimit;

        /// <summary>
        /// Resets the Min and Max values to the MinLimit.
        /// </summary>
        public void ResetMin() {
            Min = MinLimit;
            Max = MinLimit;
        }
    }
}