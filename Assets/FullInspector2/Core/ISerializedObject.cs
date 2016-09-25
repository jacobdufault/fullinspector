using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace FullInspector {
    /// <summary>
    /// The API that the editor code needs to interact with UnityObjects.
    /// </summary>
    public interface ISerializedObject {
        /// <summary>
        /// Restore the last saved state. This method is a proxy fiISerializedObjectUtility.RestoreState (with appropriate generic parameters inserted).
        /// </summary>
        void RestoreState();

        /// <summary>
        /// Save the current state. This method is a proxy for fiISerializedObjectUtility.SaveState (with appropriate generic parameters inserted).
        /// </summary>
        void SaveState();

        /// <summary>
        /// Used to determine if the given object has been restored. This value should *not*
        /// be persisted by Unity.
        /// </summary>
        bool IsRestored { get; set; }

        /// <summary>
        /// This list contains a set of object references that were encountered during the
        /// serialization process in this object graph. These need to persist through a Unity
        /// serialization cycle.
        /// </summary>
        List<UnityObject> SerializedObjectReferences {
            get;
            set;
        }

        /// <summary>
        /// The serialized state for this UnityObject - the key values. The actual value is in
        /// SerializedStateValues at the same index.
        /// </summary>
        List<string> SerializedStateKeys {
            get;
            set;
        }

        /// <summary>
        /// The serialized state for this UnityObject - the actual values. The key for this value is
        /// in SerializedStateKeys at the same index.
        /// </summary>
        List<string> SerializedStateValues {
            get;
            set;
        }
    }

}