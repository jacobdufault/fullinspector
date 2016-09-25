using System;
using UnityEngine;

namespace FullInspector.Internal {
    [Serializable]
    public class tkFoldoutMetadata : IGraphMetadataItemPersistent {
        public bool IsExpanded;

        bool IGraphMetadataItemPersistent.ShouldSerialize() {
            return IsExpanded == false;
        }
    }

    // A component for Unity to store the data within
    [AddComponentMenu("")]
    public class tkFoldoutMetadataStorageComponent : fiBaseStorageComponent<tkFoldoutGraphMetadataSerializer> { }

    // To serialize the graph metadata
    [Serializable] public class tkFoldoutGraphMetadataSerializer : fiGraphMetadataSerializer<tkFoldoutMetadata> { }

    // To provide the presistent metadata system information about our types
    public class tkFoldoutMetadataProvider : fiPersistentEditorStorageMetadataProvider<tkFoldoutMetadata, tkFoldoutGraphMetadataSerializer> { }
}