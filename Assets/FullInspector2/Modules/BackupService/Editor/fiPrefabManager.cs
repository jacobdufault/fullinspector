using FullInspector.Internal;
using UnityEditor;
using UnityEngine;

namespace FullInspector.BackupService {
    /// <summary>
    /// Manages backups stored in prefab storage container. The prefab container stores backups
    /// when Unity is in play-mode and when the backup target does not live in the scene (an
    /// example would be a backup targeting another prefab).
    /// </summary>
    public static class fiPrefabManager {
        private static string PrefabPath = fiUtility.CombinePaths(fiSettings.RootGeneratedDirectory, "fiBackupStorage.prefab");
        private static fiStorageComponent _storage;

        public static fiStorageComponent Storage {
            get {
                if (_storage == null) {
                    GameObject prefabGameObject = null;

                    // Try finding the current prefab
                    prefabGameObject = (GameObject)AssetDatabase.LoadAssetAtPath(PrefabPath, typeof(GameObject));

                    // Failed to find it; create a new one
                    if (prefabGameObject == null) {
                        var cloned = new GameObject();
                        prefabGameObject = PrefabUtility.CreatePrefab(PrefabPath, cloned);
                        fiUtility.DestroyObject(cloned);

                        prefabGameObject.AddComponent<fiStorageComponent>();

                        Debug.Log("Created new backup persistent storage object at " + PrefabPath +
                            "; this should only happen once. Please report a bug if it keeps on " +
                            "occurring.", prefabGameObject);
                    }

                    _storage = prefabGameObject.GetComponent<fiStorageComponent>();
                }

                return _storage;
            }
        }

        public static void SetDirty() {
            EditorUtility.SetDirty(Storage);
            AssetDatabase.SaveAssets();
        }
    }
}