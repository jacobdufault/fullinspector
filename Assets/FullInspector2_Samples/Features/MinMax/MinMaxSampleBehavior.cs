using UnityEngine;

namespace FullInspector.Samples.MinMaxSample {
    [AddComponentMenu("Full Inspector Samples/Other/MinMax")]
    public class MinMaxSampleBehavior : BaseBehavior<FullSerializerSerializer> {
        public MinMax<float> FloatMinMax;
        public MinMax<int> IntMinMax;

        protected void Reset() {
            FloatMinMax.MinLimit = 0;
            FloatMinMax.MaxLimit = 100;
            FloatMinMax.ResetMin();

            IntMinMax.MinLimit = 33;
            IntMinMax.MaxLimit = 88;
            IntMinMax.ResetMin();
        }
    }
}