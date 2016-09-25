using UnityEditor;

namespace FullInspector.Internal {
    public class fiCoreMenuItems {
        // We support backup for *all* components derived from BaseBehavior
        [MenuItem("CONTEXT/CommonBaseBehavior/Reset Metadata")]
        public static void BackupBaseBehavior(MenuCommand command) {
            var unityObject = command.context;

            if (unityObject != null) {
                fiPersistentMetadata.Reset(new fiUnityObjectReference(unityObject, /*tryRestore:*/false));
            }
        }
    }
}