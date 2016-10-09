using System.Collections.Generic;
using UnityEngine;

namespace FullInspector.Samples.Other.Constructors {
    [AddComponentMenu("Full Inspector Samples/Other/Non-Default Constructors")]
    public class SampleNonDefaultConstructorBehavior : BaseBehavior<FullSerializerSerializer> {
        public IInterface Interface;
        public List<IInterface> Interfaces;
    }
}