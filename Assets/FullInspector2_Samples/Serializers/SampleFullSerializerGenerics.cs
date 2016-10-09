using UnityEngine;

namespace FullInspector.Samples.FullSerializer {
    public class GenericHolder<T1> {
        public T1 Value;
    }

    public struct GenericHolder<T1, T2> {
        public T1 Value1;
        public T2 Value2;
    }

    [AddComponentMenu("Full Inspector Samples/FullSerializer/Generics")]
    public class SampleFullSerializerGenerics : BaseBehavior<FullSerializerSerializer> {
        public GenericHolder<int> GenericInt;
        public GenericHolder<int, string> GenericIntString;
        public GenericHolder<GenericHolder<int>> GenericGenericInt;
    }
}