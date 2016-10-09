using UnityEngine;

namespace FullInspector.Samples.FullSerializer {
    [AddComponentMenu("Full Inspector Samples/FullSerializer/Properties")]
    public class SampleFullSerializerProperties : BaseBehavior<FullSerializerSerializer> {
        public struct Container {
            public int SubValue { get; set; }
        }

        public int Value { get; set; }
        public Container ContainedValue { get; set; }
    }
}