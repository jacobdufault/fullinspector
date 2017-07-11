using FullSerializer.Internal;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Internal {
    // Enables fiInspectorOnly on MonoBehavior Also enables MonoBehavior for
    // extending tkCustomEditor

    [CanEditMultipleObjects]
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class fiInspectorOnly_MonoBehaviourEditor : Editor {
        private readonly UnityEngine.Object[] _targetArr = new UnityEngine.Object[1];
        public override void OnInspectorGUI() {
            if (fsPortableReflection.HasAttribute<fiInspectorOnlyAttribute>(target.GetType()) || target is tkCustomEditor) {
                _targetArr[0] = target;
                fiCommonSerializedObjectEditor.ShowInspectorForSerializedObject(_targetArr);
            }
            else {
                base.OnInspectorGUI();
            }
        }
    }
}