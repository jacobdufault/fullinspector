using UnityEngine;

namespace FullInspector.Samples.FullSerializer {
    [AddComponentMenu("Full Inspector Samples/FullSerializer/Nullable Unity Types")]
    public class SampleFullSerializerNullableUnityTypes : BaseBehavior<FullSerializerSerializer> {
        public struct UnityTypesContainer {
            public Bounds? Bounds;
            public Color? Color;
            public Vector2? Vector2;
            public Vector3? Vector3;
            public Vector4? Vector4;
            public LayerMask? LayerMask;
        }

        public UnityTypesContainer UnityTypes;

        public Bounds? Bounds;
        public Color? Color;
        public Vector2? Vector2;
        public Vector3? Vector3;
        public Vector4? Vector4;
        public LayerMask? LayerMask;
    }
}