using UnityEditor;

namespace FullInspector.Internal {
    [CustomEditor(typeof(CommonBaseScriptableObject), editorForChildClasses: true)]
    public class CommonBaseScriptableObjectEditor : FullInspectorCommonSerializedObjectEditor {
    }
}