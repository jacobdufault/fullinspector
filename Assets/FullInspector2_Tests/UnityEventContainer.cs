using UnityEngine.Events;

namespace FullInspector.Tests {
    public class UnityEventContainer : BaseBehavior<FullSerializerSerializer> {
        public UnityEvent unityEvent = new UnityEvent();

        public void EventAction() {
        }
    }
}