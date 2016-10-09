using FullInspector.Internal;
using UnityEngine;

namespace FullInspector.Samples.Other.Delegates {
    [AddComponentMenu("Full Inspector Samples/Other/Delegates")]
    public class DelegateSampleBehavior : BaseBehavior<FullSerializerSerializer> {
        [InspectorMargin(10)]
        [InspectorComment("SerializedAction and SerializedFunc are pretty nifty - they are fully type " +
            "safe (up to 9 generic arguments) while also properly supporting both contra and " +
            "covariance.")]
        [InspectorHidePrimary]
        [ShowInInspector]
        private int __inspectorVar;

        [InspectorMargin(10)]
        [InspectorComment("Notice that this action, which takes a BaseBehavior, also works on functions " +
            "which take a supertype of BaseBehavior, such as MonoBehaviour")]
        public SerializedAction<int, CommonBaseBehavior> IntBaseBehaviorAction;

        [InspectorMargin(10)]
        [InspectorComment("Notice that this generator function -- which returns types of BaseBehavior, " +
            "can accept a function that also returns only DelegateSampleBehavior (which is a " +
            "subclass of BaseBehavior)")]
        public SerializedFunc<CommonBaseBehavior> BaseBehaviorGenerator;

        public void MyIntMonoBehaviourConsumer(int a, MonoBehaviour b) {
            Debug.Log(string.Format("MyIntMonoBehaviourConsumer({0}, {1}) called", a, b));

        }

        public void MyIntBaseBehaviorConsumer(int a, BaseBehavior<FullSerializerSerializer> b) {
            Debug.Log(string.Format("MyIntBaseBehaviorConsumer({0}, {1}) called", a, b));
        }

        public BaseBehavior<FullSerializerSerializer> MyBaseBehaviorGenerator() {
            return this;
        }

        public DelegateSampleBehavior MyDelegateSampleGenerator() {
            return this;
        }
    }
}