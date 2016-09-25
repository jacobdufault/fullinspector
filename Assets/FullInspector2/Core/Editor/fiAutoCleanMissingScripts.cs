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
                // NOTE: If this approach doesn't work, then we can use RemoveComponent with
                // the specific component type to remove. This is more similar to how RemoveMetadata
                // works.
                fiEditorUtility.RemoveMissingScripts(fiPersistentEditorStorage.SceneStorage);
                EditorUtility.SetDirty(fiPersistentEditorStorage.SceneStorage);
                fiEditorUtility.RemoveMissingScripts(fiPersistentEditorStorage.PrefabStorage);
                EditorUtility.SetDirty(fiPersistentEditorStorage.PrefabStorage);

                if (fiPrefabManager.Storage != null) {
                    fiEditorUtility.RemoveMissingScripts(fiPrefabManager.Storage.gameObject);
                    EditorUtility.SetDirty(fiPrefabManager.Storage);
                }
                if (fiSceneManager.Storage != null) {
                    fiEditorUtility.RemoveMissingScripts(fiSceneManager.Storage.gameObject);
                    EditorUtility.SetDirty(fiSceneManager.Storage);
                }
            });
        }
    }
}