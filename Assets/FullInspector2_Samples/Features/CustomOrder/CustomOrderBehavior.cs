using UnityEngine;

namespace FullInspector.Samples.Other.CustomOrder {
    public struct Struct {
        [InspectorOrder(0)]
        [InspectorComment("Structs/classes also get their own order groups")]
        [InspectorHidePrimary]
        [ShowInInspector]
        private int __inspectorComment;

        [InspectorOrder(2)]
        public int Two;

        [InspectorOrder(1)]
        public int One;
    }

    [AddComponentMenu("Full Inspector Samples/Other/Custom Order")]
    public partial class CustomOrderBehavior : BaseBehavior<FullSerializerSerializer> {
        [InspectorOrder(2)]
        public int Two;

        [InspectorOrder(2.2)]
        public Struct TwoPtTwo;
    }

    public partial class CustomOrderBehavior {
        [InspectorOrder(1)]
        public int One;

        [InspectorOrder(2.1)]
        public int TwoPt1 { get; set; }
    }

    public partial class CustomOrderBehavior {
        [InspectorOrder(3)]
        public int Three { get; private set; }
    }

    public partial class CustomOrderBehavior {
        [InspectorOrder(-10)]
        public int NegativeTen;
    }

    public partial class CustomOrderBehavior {
        public int NonOrdered;
    }
}