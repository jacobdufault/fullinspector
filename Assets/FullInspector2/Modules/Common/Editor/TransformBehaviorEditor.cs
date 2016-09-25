using FullInspector.LayoutToolkit;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules {

    /*
    public class BaseUnityBehaviorEditor<TBehavior> : BehaviorEditor<TBehavior> where TBehavior : UnityObject {
        private static ReflectedPropertyEditor _reflectedEditor = new ReflectedPropertyEditor(InspectedType.Get(typeof(TBehavior)));

        protected override void OnEdit(Rect rect, TBehavior behavior) {
            var metadata = fiGraphMetadata.GetGlobal<fiGraphMetadata>(behavior);
            _reflectedEditor.Edit(rect, GUIContent.none, behavior, metadata);
        }

        protected override float OnGetHeight(TBehavior behavior) {
            var metadata = fiGraphMetadata.GetGlobal<fiGraphMetadata>(behavior);
            return _reflectedEditor.GetElementHeight(GUIContent.none, behavior, metadata);
        }

        protected override void OnSceneGUI(TBehavior behavior) {
        }
    }

    [CustomBehaviorEditor(typeof(Transform))]
    public class TransformBehaviorEditor : BaseUnityBehaviorEditor<Transform> {
        static TransformBehaviorEditor() {
            InspectedType.RemoveProperty<Transform>("eulerAngles");
            InspectedType.RemoveProperty<Transform>("forward");
            InspectedType.RemoveProperty<Transform>("hasChanged");
            InspectedType.RemoveProperty<Transform>("localEulerAngles");
            InspectedType.RemoveProperty<Transform>("localPosition");
            InspectedType.RemoveProperty<Transform>("localRotation");
            InspectedType.RemoveProperty<Transform>("localToWorldMatrix");
            InspectedType.RemoveProperty<Transform>("lossyScale");
            InspectedType.RemoveProperty<Transform>("parent");
            InspectedType.RemoveProperty<Transform>("right");
            InspectedType.RemoveProperty<Transform>("root");
            InspectedType.RemoveProperty<Transform>("up");
            InspectedType.RemoveProperty<Transform>("worldToLocalMatrix");
        }
    }
    */

    [CustomBehaviorEditor(typeof(Transform))]
    public class TransformBehaviorEditor : BehaviorEditor<Transform> {
        private static fiLayout Layout;

        static TransformBehaviorEditor() {
            float vecHeight = EditorStyles.label.CalcHeight(GUIContent.none, 0);
            Layout = new fiVerticalLayout {
                { "Position", vecHeight },
                2,
                { "Rotation", vecHeight },
                2,
                { "Scale", vecHeight }
            };
        }

        protected override void OnEdit(Rect rect, Transform behavior, fiGraphMetadata metadata) {
            behavior.position = EditorGUI.Vector3Field(Layout.GetSectionRect("Position", rect), "Position", behavior.position);
            behavior.rotation = Quaternion.Euler(EditorGUI.Vector3Field(Layout.GetSectionRect("Rotation", rect), "Rotation", behavior.rotation.eulerAngles));
            behavior.localScale = EditorGUI.Vector3Field(Layout.GetSectionRect("Scale", rect), "Scale", behavior.localScale);
        }

        protected override float OnGetHeight(Transform behavior, fiGraphMetadata metadata) {
            return Layout.Height;
        }

        protected override void OnSceneGUI(Transform behavior) {
        }
    }
}