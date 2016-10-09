using UnityEngine;

namespace FullInspector.Samples.Button {
    public class ButtonContainerType {
        [InspectorOrder(1)]
        public int Member1;

        [InspectorButton]
        [InspectorName("A custom name")]
        [InspectorOrder(2)]
        public void Method(int val) { }

        [InspectorOrder(3)]
        public int Member2;
    }

    [AddComponentMenu("Full Inspector Samples/Other/Buttons")]
    public class ButtonSampleBehavior : BaseBehavior<FullSerializerSerializer> {
        public ButtonContainerType ImplementedType;

        [InspectorButton]
        public static void PublicStaticButton() {
            Debug.Log("PublicStaticButton");
        }

        [InspectorButton]
        private static void PrivateStaticButton() {
            Debug.Log("PrivateStaticButton");
        }

        [InspectorButton]
        private void PrivateButton() {
            Debug.Log("PrivateButton " + this);
        }

        [InspectorButton]
        public void PublicButton() {
            Debug.Log("PublicButton " + this);
        }
    }
}