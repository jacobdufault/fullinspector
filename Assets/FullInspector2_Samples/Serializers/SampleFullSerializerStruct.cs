using UnityEngine;

namespace FullInspector.Samples.FullSerializer {
    [AddComponentMenu("Full Inspector Samples/FullSerializer/Structs")]
    public class SampleFullSerializerStruct : BaseBehavior<FullSerializerSerializer> {
        public struct MyStruct {
            public int A;
            public float B;
            public string C;
        }

        public MyStruct StructValue;
    }
}