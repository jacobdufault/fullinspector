using UnityEngine;

namespace FullInspector.Samples {
    [AddComponentMenu("Full Inspector Samples/Other/Categories")]
    public class Categories : BaseBehavior<FullSerializerSerializer> {
        [InspectorCategory("Category A")]
        public int a0, a1, a2, a3, a4;

        [InspectorCategory("Category B")]
        public int b0, b1, b2, b3, b4;

        [InspectorCategory("Category A")]
        [InspectorCategory("Category B")]
        public int common0, common1;

        public int notShown;
    }
}