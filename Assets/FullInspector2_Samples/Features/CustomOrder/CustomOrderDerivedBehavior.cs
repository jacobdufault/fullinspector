using UnityEngine;

namespace FullInspector.Samples.Other.CustomOrder {
    [AddComponentMenu("Full Inspector Samples/Other/Custom Order Derived")]
    public partial class CustomOrderDerivedBehavior : CustomOrderBehavior {
        [InspectorOrder(2)]
        public int DerivedTwo;
    }

    public partial class CustomOrderDerivedBehavior {
        [InspectorOrder(1)]
        public int DerivedOne;

        [InspectorOrder(2.1)]
        public int DerivedTwoPt1 { get; set; }
    }

    public partial class CustomOrderDerivedBehavior {
        [InspectorOrder(3)]
        public int DerivedThree { get; private set; }
    }

    public partial class CustomOrderDerivedBehavior {
        [InspectorOrder(-10)]
        public int DerivedNegativeTen;
    }

    public partial class CustomOrderDerivedBehavior {
        public int DerivedNonOrdered;
    }
}