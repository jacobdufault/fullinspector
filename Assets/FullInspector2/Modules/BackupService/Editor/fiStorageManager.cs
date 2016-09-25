using System.Collections.Generic;
using FullInspector.Internal;
using UnityEditor;
using UnityEngine;

namespace FullInspector.BackupService {
    /// <summary>
    /// This class provides a unified API for accessing backups across scene and prefab storage.
    /// </summary>
    public static class fiStorageManager {
        /// <summary>
        /// Returns the storage component that is currently best suited for use.
        /// </summary>
        public static fiStorageComponent PersistentStorage {
            get {
                // If we're playing, scene storage will not persist, so we *must* use prefab storage
                if (Application.isPlaying) {
                    return fiPrefabManager.Storage;
                }

                return fiSceneManager.Storage;
            }
        }

        /// <summary>
        /// Removes the given backup instance.
        /// </summary>
        public static void RemoveBackup(fiSerializedObject serializedObj) {
            fiSceneManager.Storage.Objects.Remove(serializedObj);
            if (fiPrefabManager.Storage.Objects.Remove(serializedObj)) {
                fiPrefabManager.SetDirty();
            }
        }

        /// <summary>
        /// Removes backups that are no longer valid (their target got destroyed, etc).
        /// </summary>
        public static void RemoveInvalidBackups() {
            fiSceneManager.Storage.RemoveInvalidBackups();
            fiPrefabManager.Storage.RemoveInvalidBackups();
        }

        /// <summary>
        /// Attempts to migrate prefab storage into scene storage.
        /// </summary>
        public static void MigrateStorage() {
            // We cannot migrate data while playing -- scene storage will not be persisted.
            if (Application.isPlaying) {
                return;
            }

            // Nothing to migrate.
            if (fiPrefabManager.Storage.Objects.Count == 0) {
                return;
            }

            // Migrate everything except prefabs into storage.
            int i = 0;
            while (i < fiPrefabManager.Storage.Objects.Count) {
                fiSerializedObject obj = fiPrefabManager.Storage.Objects[i];

                // If the target object is persistent (ie, not from the scene or a temporary object),
                // then we want to keep it in the prefab storage.
                if (AssetDatabase.Contains(obj.Target.Target)) {
                    ++i;
                }

                // This appears to be a scene object, so migrate it to scene storage.
                else {
                    fiSceneManager.Storage.Objects.Add(obj);
                    fiPrefabManager.Storage.Objects.RemoveAt(i);
                    fiPrefabManager.SetDirty();
                }
            }
        }

        /// <summary>
        /// Returns true if there is a backup for the given behavior.
        /// </summary>
        public static bool HasBackups(CommonBaseBehavior behavior) {
            // TODO: Maybe this should be triggering? If a user reports a bug about having
            //       backups not work then this is probably the cause...
            if (fiSceneManager.Storage == null || fiPrefabManager.Storage == null)
                return false;

            for (int i = 0; i < fiSceneManager.Storage.Objects.Count; ++i) {
                var backup = fiSceneManager.Storage.Objects[i];
                if (backup.Target.Target == behavior) {
                    return true;
                }
            }

            for (int i = 0; i < fiPrefabManager.Storage.Objects.Count; ++i) {
                var backup = fiPrefabManager.Storage.Objects[i];
                if (backup.Target.Target == behavior) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns every serialized object.
        /// </summary>
        public static IEnumerable<fiSerializedObject> SerializedObjects {
            get {
                foreach (var obj in fiSceneManager.Storage.Objects) {
                    yield return obj;
                }

                foreach (var obj in fiPrefabManager.Storage.Objects) {
                    yield return obj;
                }
            }
        }
    }
}