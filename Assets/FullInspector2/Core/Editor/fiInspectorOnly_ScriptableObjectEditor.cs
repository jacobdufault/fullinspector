using FullSerializer.Internal;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Internal {
    // Enables fiInspectorOnly on ScriptableObjects Also enables ScriptableObject
    // for extending tkCustomEditor

    [CanEditMultipleObjects]
    [CustomEditor(typeof(ScriptableObject), editorForChildClasses: true, isFallback = true)]
    public class fiInspectorOnly_ScriptableObjectEditor : Editor {
        public override void OnInspectorGUI() {
            if (fsPortableReflection.HasAttribute<fiInspectorOnlyAttribute>(target.GetType()) || target is tkCustomEditor) {
                BehaviorEditor.Get(target.GetType()).EditWithGUILayout(target);
            }
            else {
                base.OnInspectorGUI();
            }
        }
    }
}