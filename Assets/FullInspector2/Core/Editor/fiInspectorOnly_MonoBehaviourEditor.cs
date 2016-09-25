using FullSerializer.Internal;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Internal {
    // Enables fiInspectorOnly on MonoBehavior
    // Also enables MonoBehavior for extending tkCustomEditor

    [CanEditMultipleObjects]
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class fiInspectorOnly_MonoBehaviourEditor : Editor {
        public override void OnInspectorGUI() {
            if (fsPortableReflection.HasAttribute<fiInspectorOnlyAttribute>(target.GetType()) || target is tkCustomEditor) {
                FullInspectorCommonSerializedObjectEditor.ShowInspectorForSerializedObject(target);
            }

            else {
                base.OnInspectorGUI();
            }
        }
    }
}
