using UnityEngine;

namespace FullInspector.Samples.FullSerializer {
    [AddComponentMenu("Full Inspector Samples/FullSerializer/Simple Types")]
    public class SampleFullSerializerSimpleTypes : BaseBehavior<FullSerializerSerializer> {
        public struct StructContainer {
            public int IntValue;
            public float FloatValue;
            public string StringValue;
            public bool BoolValue;
        }

        public int IntValue;
        public float FloatValue;
        public string StringValue;
        public bool BoolValue;
        public StructContainer StructValue;
    }
}