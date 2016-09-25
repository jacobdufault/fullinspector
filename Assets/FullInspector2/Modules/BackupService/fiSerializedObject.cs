using System;
using System.Collections.Generic;
using FullInspector.Internal;

namespace FullInspector.BackupService {
    /// <summary>
    /// A serialized object, ie, a backup.
    /// </summary>
    [Serializable]
    public class fiSerializedObject {
        /// <summary>
        /// The object that the backup is applied to.
        /// </summary>
        public fiUnityObjectReference Target;

        /// <summary>
        /// When the backup was made (computed from DateTime.Now).
        /// </summary>
        public string SavedAt;

        /// <summary>
        /// Only used in the editor -- if true, then the deserialized state should be shown.
        /// </summary>
        public bool ShowDeserialized;

        /// <summary>
        /// The deserialized state. Potentially null.
        /// </summary>
        public fiDeserializedObject DeserializedState;

        /// <summary>
        /// The serialized members.
        /// </summary>
        public List<fiSerializedMember> Members = new List<fiSerializedMember>();

        /// <summary>
        /// The serialized object references.
        /// </summary>
        public List<fiUnityObjectReference> ObjectReferences = new List<fiUnityObjectReference>();
    }

    /// <summary>
    /// An item that has been serialized.
    /// </summary>
    [Serializable]
    public class fiSerializedMember {
        /// <summary>
        /// The property or field name.
        /// </summary>
        public string Name;

        /// <summary>
        /// The serialized state.
        /// </summary>
        public string Value;

        /// <summary>
        /// A shared object instance (with fiDeserializedMember) that tells the deserialization
        /// engine if this property should be restored upon backup restore.
        /// </summary>
        public fiEnableRestore ShouldRestore = new fiEnableRestore() {
            Enabled = true
        };
    }
}