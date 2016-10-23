using FullInspector.Internal;
using UnityEditor;
using UnityEngine;

namespace FullInspector.BackupService {
    /// <summary>
    /// Manages the backup storage that lives in the scene.
    /// </summary>
    public class fiSceneManager {
        private const string SceneStorageName = "fiBackupSceneStorage";
        private static fiStorageComponent _storage;

        private static void FindAndRemoveStaleObjects() {
            GameObject oldObject = GameObject.Find(SceneStorageName);
            if (oldObject != null && oldObject.GetComponent<fiStorageComponent>() == null)
                fiUtility.DestroyObject(oldObject);
        }

        public static fiStorageComponent Storage {
            get {
                if (_storage == null) {
                    FindAndRemoveStaleObjects();
                    _storage = GameObject.FindObjectOfType<fiStorageComponent>();

                    if (_storage == null) {
                        // If we use new GameObject(), then for a split second
                        // Unity will show the game object in the hierarchy,
                        // which is bad UX.
                        var obj = EditorUtility.CreateGameObjectWithHideFlags(SceneStorageName,
                            HideFlags.HideInHierarchy);
                        _storage = obj.AddComponent<fiStorageComponent>();
                    }
                }

                return _storage;
            }
        }
    }
}