using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Modules.Attributes {
    [CustomAttributePropertyEditor(typeof(VerifyPrefabTypeAttribute), ReplaceOthers = false)]
    public class VerifyPrefabTypeAttributeEditor<T> : AttributePropertyEditor<T, VerifyPrefabTypeAttribute>
        where T : UnityObject {

        private static bool IsFlagSet(VerifyPrefabTypeFlags flags, VerifyPrefabTypeFlags setFlag) {
            if ((flags & setFlag) == 0) {
                return false;
            }

            return true;
        }

        private bool IsValidInstance(T element, VerifyPrefabTypeAttribute attribute) {
            if (element == null) {
                return true;
            }


            PrefabType prefabType = PrefabUtility.GetPrefabType(element);
            switch (prefabType) {
                case PrefabType.None:
                    return IsFlagSet(attribute.PrefabType, VerifyPrefabTypeFlags.None);
                case PrefabType.Prefab:
                    return IsFlagSet(attribute.PrefabType, VerifyPrefabTypeFlags.Prefab);
                case PrefabType.ModelPrefab:
                    return IsFlagSet(attribute.PrefabType, VerifyPrefabTypeFlags.ModelPrefab);
                case PrefabType.PrefabInstance:
                    return IsFlagSet(attribute.PrefabType, VerifyPrefabTypeFlags.PrefabInstance);
                case PrefabType.ModelPrefabInstance:
                    return IsFlagSet(attribute.PrefabType, VerifyPrefabTypeFlags.ModelPrefabInstance);
                case PrefabType.MissingPrefabInstance:
                    return IsFlagSet(attribute.PrefabType, VerifyPrefabTypeFlags.MissingPrefabInstance);
                case PrefabType.DisconnectedPrefabInstance:
                    return IsFlagSet(attribute.PrefabType, VerifyPrefabTypeFlags.DisconnectedPrefabInstance);
                case PrefabType.DisconnectedModelPrefabInstance:
                    return IsFlagSet(attribute.PrefabType, VerifyPrefabTypeFlags.DisconnectedModelPrefabInstance);
            }

            return false;
        }

        protected override T Edit(Rect region, GUIContent label, T element, VerifyPrefabTypeAttribute attribute, fiGraphMetadata metadata) {
            if (IsValidInstance(element, attribute) == false) {
                region.height -= Margin;

                PrefabType actualPrefabType = PrefabUtility.GetPrefabType(element);
                EditorGUI.HelpBox(region, "This property needs to be a " + attribute.PrefabType + ", not a " + actualPrefabType, MessageType.Error);
            }

            return element;
        }

        private const float Margin = 2f;

        protected override float GetElementHeight(GUIContent label, T element, VerifyPrefabTypeAttribute attribute, fiGraphMetadata metadata) {
            if (IsValidInstance(element, attribute) == false) {
                return 33 + Margin;
            }

            return 0;
        }
    }
}