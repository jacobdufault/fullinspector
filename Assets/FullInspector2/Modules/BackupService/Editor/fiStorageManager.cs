using System.Linq;
using System.Collections.Generic;
using FullInspector.Internal;
using UnityEditor;
using UnityEngine;
using FullInspector.StoragesManager;

namespace FullInspector.BackupService {

    /// <summary>
    /// This class provides a unified API for accessing backups across scene and
    /// prefab storage.
    /// </summary>
    public static class fiStorageManager {
        private const string SceneStorageName = "fiBackupSceneStorage";

        private static readonly string _PrefabStoragePath =
                fiUtility.CombinePaths(fiSettings.RootGeneratedDirectory, "fiBackupStorage.prefab");

        private static readonly fiStoragesManager<fiStorageComponent> _StoragesManager =
                new fiStoragesManager<fiStorageComponent>(SceneStorageName, _PrefabStoragePath);


        public static fiStorageComponent PrefabStorage {
            get { return _StoragesManager.PrefabStorage; }
        }

        /// <summary>
        /// Returns the storage component that is currently best suited for use.
        /// </summary>
        public static fiStorageComponent GetPersistentStorage(int instanceID) {
            var o = fiLateBindings.EditorUtility.InstanceIDToObject(instanceID);
            var isPersistent = AssetDatabase.Contains(o);

            // If we're playing, scene storage will not persist, so we *must* use prefab storage
            return isPersistent || Application.isPlaying
                ? _StoragesManager.PrefabStorage
                : _StoragesManager.GetStorage(o);
        }

        /// <summary>
        /// Removes the given backup instance.
        /// </summary>
        public static void RemoveBackup(fiSerializedObject serializedObj) {
            var storage = GetPersistentStorage(serializedObj.Target.Target.GetInstanceID());
            var hasObject = storage.Objects.Contains(serializedObj);
            if (hasObject) {
                Undo.RegisterCompleteObjectUndo(storage, "Undo Backup Removal");
                storage.Objects.Remove(serializedObj);
                storage.SetDirty();
            }
        }

        /// <summary>
        /// Removes backups that are no longer valid (their target got destroyed,
        /// etc).
        /// </summary>
        public static void RemoveInvalidBackups() {
            _StoragesManager.PrefabStorage.RemoveInvalidBackups();
            foreach (var sceneStorage in _StoragesManager.GetAllSceneStorages()) {
                sceneStorage.RemoveInvalidBackups();
            }
        }

        /// <summary>
        /// Attempts to migrate prefab storage and cross-scene references
        /// into correct scene storage.
        /// </summary>
        public static void MigrateStorage() {
            // We cannot migrate data while playing -- scene storage will not be
            // persisted.
            if (Application.isPlaying) {
                return;
            }
            var i = 0;
            //migrate cross-scene references
            //this will still show warnings, as Unity will show then the moment it detects cross-scene refs
            //we at least won't lose the backups.
            //currently, we don't have a similar solution for fiPersistendEditorStorage,
            //so it will be ugly and show warnings when moving objects around
            //and when saving. Lots of warnings.
            foreach (var storage in _StoragesManager.GetAllSceneStorages()) {
                while (i <  storage.Objects.Count) {
                    fiSerializedObject fiSerializedObject = storage.Objects[i];
                    var unityObject = fiSerializedObject.Target.Target;
                    var c = unityObject as Component;
                    if (c == null ||
                        !c.gameObject.scene.IsValid() ||
                        !c.gameObject.scene.isLoaded ||
                        c.gameObject.scene == storage.gameObject.scene) {
                        ++i;
                        continue;
                    }

                    var correctStorage = _StoragesManager.GetStorage(c.gameObject.scene);
                    correctStorage.Objects.Add(fiSerializedObject);
                    storage.Objects.RemoveAt(i);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(storage.gameObject.scene);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(correctStorage.gameObject.scene);
                }
            }

            // Nothing to migrate.
            if (_StoragesManager.PrefabStorage.Objects.Count == 0) {
                return;
            }

            // Migrate everything except prefabs into storage.
            //Debug.Log("_StoragesManager.PrefabStorage has " + _StoragesManager.PrefabStorage.Objects.Count + "  objects");
            while (i < _StoragesManager.PrefabStorage.Objects.Count) {
                var obj = _StoragesManager.PrefabStorage.Objects[i];
                var target = _StoragesManager.PrefabStorage.Objects[i].Target.Target;

                var sceneStorage = GetPersistentStorage(target.GetInstanceID());

                //this means that the object is persistent or we failed to retrieve a storage
                if (sceneStorage == _StoragesManager.PrefabStorage) {
                    ++i;
                    continue;
                }

                var o = fiLateBindings.EditorUtility.InstanceIDToObject(target.GetInstanceID());
                var state = new fiSerializedObject()
                {
                    Target = new fiUnityObjectReference(o, false),
                    SavedAt = obj.SavedAt
                };

                //Debug.Log("Migrating " + state.Target.Target.name + " to new storage", o);
                sceneStorage.Objects.Add(state);
                sceneStorage.SetDirty();
                EditorGUIUtility.PingObject(sceneStorage);
                _StoragesManager.PrefabStorage.Objects.Remove(obj);
                _StoragesManager.PrefabStorage.SetDirty();
                EditorGUIUtility.PingObject(_StoragesManager.PrefabStorage);
            }


        }

        /// <summary>
        /// Returns true if there is a backup for the given target.
        /// </summary>
        public static bool HasBackups(fiUnityObjectReference target) {
            return GetAllSerializedObjects().Any(s => Equals(s.Target, target));
        }

        /// <summary>
        /// Returns every serialized object.
        /// </summary>
        public static IEnumerable<fiSerializedObject> GetAllSerializedObjects() {

            foreach (var obj in _StoragesManager.GetAllSceneStorages().SelectMany(list => list.Objects)) {
                yield return obj;
            }

            foreach (var obj in _StoragesManager.PrefabStorage.Objects) {
                yield return obj;
            }
        }

    }
}
