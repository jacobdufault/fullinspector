using UnityEditor;

namespace FullInspector.Internal {
    [CustomEditor(typeof(CommonBaseBehavior), /*editorForChildClasses:*/ true)]
    [CanEditMultipleObjects]
    public class CommonBaseBehaviorEditor : fiCommonSerializedObjectEditor {
    }
}