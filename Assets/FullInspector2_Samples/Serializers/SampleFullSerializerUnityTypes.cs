using UnityEngine;

namespace FullInspector.Samples.FullSerializer {
    [AddComponentMenu("Full Inspector Samples/FullSerializer/Unity Types")]
    public class SampleFullSerializerUnityTypes : BaseBehavior<FullSerializerSerializer> {
        public struct UnityTypesContainer {
            public Bounds Bounds;
            public Color Color;
            public AnimationCurve AnimationCurve;
            public Vector2 Vector2;
            public Vector3 Vector3;
            public Vector4 Vector4;
            public LayerMask LayerMask;
            public Gradient Gradient;
        }

        public UnityTypesContainer UnityTypes;
    }
}