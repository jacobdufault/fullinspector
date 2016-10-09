using UnityEngine;

namespace FullInspector.Samples.FullSerializer {
    [AddComponentMenu("Full Inspector Samples/FullSerializer/Custom Behavior Editor")]
    public class SampleFullSerializerCustomBehaviorEditor : BaseBehavior<FullSerializerSerializer> {
        // neither of these values will be shown in the inspector because there is a custom editor
        // for this behavior
        public int A;
        public float B;
    }
}