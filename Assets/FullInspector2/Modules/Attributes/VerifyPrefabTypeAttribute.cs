using System;

namespace FullInspector {
    /// <summary>
    /// A simple verification attribute that ensures the UnityObject derived target is a prefab.
    /// </summary>
    // TODO: rename to InspectorVerifyPrefabType
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class VerifyPrefabTypeAttribute : Attribute {
        public VerifyPrefabTypeFlags PrefabType;

        public VerifyPrefabTypeAttribute(VerifyPrefabTypeFlags prefabType) {
            PrefabType = prefabType;
        }
    }

    /// <summary>
    /// The different prefab possibilities an object could be.
    /// </summary>
    [Flags]
    public enum VerifyPrefabTypeFlags {
        None = 1 << 0,
        Prefab = 1 << 1,
        ModelPrefab = 1 << 2,
        PrefabInstance = 1 << 3,
        ModelPrefabInstance = 1 << 4,
        MissingPrefabInstance = 1 << 5,
        DisconnectedPrefabInstance = 1 << 6,
        DisconnectedModelPrefabInstance = 1 << 7,
    }
}