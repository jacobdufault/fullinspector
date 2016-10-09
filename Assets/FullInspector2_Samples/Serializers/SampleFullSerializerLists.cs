using System.Collections.Generic;
using UnityEngine;

namespace FullInspector.Samples.FullSerializer {
    [AddComponentMenu("Full Inspector Samples/FullSerializer/Lists")]
    public class SampleFullSerializerLists : BaseBehavior<FullSerializerSerializer> {
        public struct Container {
            public List<Transform> SubTransformList;
            public interface IFace { }
            public class DerivedA : IFace { public int A; }
            public class DerivedB : IFace { public string B; }
            public List<IFace> SubInterfaceList;
        }

        public List<int> IntList;
        public int[] IntArray;
    }
}