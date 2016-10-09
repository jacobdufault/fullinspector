using System.Collections.Generic;
using UnityEngine;

namespace FullInspector.Samples.FullSerializer {
    [AddComponentMenu("Full Inspector Samples/FullSerializer/Dictionaries")]
    public class SampleFullSerializerDictionaries : BaseBehavior<FullSerializerSerializer> {
        public struct Container {
            public Dictionary<string, GameObject> StrGoDict;
        }

        public Dictionary<string, string> StrStrDict;

        public enum Enum { ValueA, ValueB, ValueC };
        public Dictionary<Enum, Transform> EnumTransformDict;
    }
}