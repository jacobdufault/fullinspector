using FullInspector.BackupService;
using UnityEditor;
using UnityEditor.Callbacks;

namespace FullInspector.Internal {
    public static class fiAutoCleanMissingScripts {
        [DidReloadScripts]
        public static void AutoCleanupMissingScripts() {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            fiLateBindings.EditorApplication.InvokeOnEditorThread(() => {
                // Remove any potentially missing scripts.
                //
                // NOTE: If this approach doesn't work, then we can use
                //       RemoveComponent with the specific component type to
                // remove. This is more similar to how RemoveMetadata works.
                foreach (var storage in fiPersistentEditorStorage.GetAllCachedSceneStorages()) {
                    fiEditorUtility.RemoveMissingScripts(storage.gameObject);
                    EditorUtility.SetDirty(storage);
                }

                fiEditorUtility.RemoveMissingScripts(fiPersistentEditorStorage.PrefabStorage.gameObject);
                EditorUtility.SetDirty(fiPersistentEditorStorage.PrefabStorage);

                if (fiStorageManager.PrefabStorage != null) {
                    fiEditorUtility.RemoveMissingScripts(fiStorageManager.PrefabStorage .gameObject);
                    EditorUtility.SetDirty(fiStorageManager.PrefabStorage );
                }
            });
        }
    }
}
