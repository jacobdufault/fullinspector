using System;
using System.Collections.Generic;
using System.Linq;
using FullSerializer;
using UnityEngine;

namespace FullInspector.Internal {
    public class fiPersistentEditorStorage {
        #region Reading/Writing
        public static void Reset<T>(fiUnityObjectReference key) {
            fiBaseStorageComponent<T> storage;
            if (fiLateBindings.EditorUtility.IsPersistent(key.Target)) {
                storage = GetStorageDictionary<T>(PrefabStorage);
            }
            else {
                storage = GetStorageDictionary<T>(SceneStorage);
            }

            storage.Data.Remove(key.Target);
            fiLateBindings.EditorUtility.SetDirty(storage);
        }

        public static T Read<T>(fiUnityObjectReference key)
            where T : new() {
            fiBaseStorageComponent<T> storage;
            if (fiLateBindings.EditorUtility.IsPersistent(key.Target)) {
                storage = GetStorageDictionary<T>(PrefabStorage);
            }
            else {
                storage = GetStorageDictionary<T>(SceneStorage);
            }

            if (storage.Data.ContainsKey(key.Target)) {
                return storage.Data[key.Target];
            }

            var value = storage.Data[key.Target] = new T();
            fiLateBindings.EditorUtility.SetDirty(storage);
            return value;
        }

#if false
        // This code is commented out because it is no longer used. It used to
        // migrate data inside of the prefab into the scene object. Now, we just
        // don't store scene objects inside of the prefab (we lose the ability to
        // save the state from play-mode, but that's okay)

        /// <summary>
        /// Attempts to migrate prefab storage into scene storage.
        /// </summary>
        private static void MigratePrefabIntoSceneStorage<T>()
            where T : fiPersistentEditorStorageItem {
            // Only try to do the migration once (per type)
            if (_didMigrate.Add(typeof(T)) == false) return;

            // We cannot migrate data while playing -- scene storage will not be
            // persisted
            if (Application.isPlaying) {
                return;
            }

            var prefabStorage = PrefabStorage.GetComponent<fiBaseStorageComponent<T>>();
            var sceneStorage = SceneStorage.GetComponent<fiBaseStorageComponent<T>>();

            // Nothing to migrate.
            if (prefabStorage == null || prefabStorage.Data == null) {
                return;
            }

            // Migrate everything into scene storage
            var toRemove = new List<fiUnityObjectReference>();
            foreach (var entry in prefabStorage.Data) {
                if (AssetDatabase.Contains(entry.Key.Target)) {
                    // data should stay in prefab storage, as the UnityObject
                    // target does not live in the scene
                    continue;
                }

                if (sceneStorage.Data.ContainsKey(entry.Key) == false) {
                    // move directly into scene storage
                    sceneStorage.Data[entry.Key] = entry.Value;
                }
                else {
                    // copy into scene storage
                    sceneStorage.Data[entry.Key].CopyFromAndClear(prefabStorage.Data[entry.Key]);
                }

                toRemove.Add(entry.Key);
            }
            foreach (var item in toRemove) prefabStorage.Data.Remove(item);

            fiLateBindings.EditorUtility.SetDirty(sceneStorage);
            fiLateBindings.EditorUtility.SetDirty(prefabStorage);
        }
        private static HashSet<Type> _didMigrate = new HashSet<Type>();
#endif

        private static Dictionary<Type, Type> _cachedRealComponentTypes = new Dictionary<Type, Type>();
        private static fiBaseStorageComponent<T> GetStorageDictionary<T>(GameObject container) {
            Type realComponentType;
            if (_cachedRealComponentTypes.TryGetValue(typeof(fiBaseStorageComponent<T>), out realComponentType) == false) {
                realComponentType = fiRuntimeReflectionUtility.AllSimpleTypesDerivingFrom(typeof(fiBaseStorageComponent<T>)).FirstOrDefault();
                _cachedRealComponentTypes[typeof(fiBaseStorageComponent<T>)] = realComponentType;
            }

            if (realComponentType == null) {
                throw new InvalidOperationException("Unable to find derived component type for " +
                    typeof(fiBaseStorageComponent<T>).CSharpName());
            }

            var component = container.GetComponent(realComponentType);
            if (component == null) {
                component = container.AddComponent(realComponentType);
            }

            return (fiBaseStorageComponent<T>)component;
        }
        #endregion Reading/Writing

        #region Scene Storage
        private const string SceneStorageName = "fiPersistentEditorStorage";
        private static GameObject _cachedSceneStorage;
        public static GameObject SceneStorage {
            get {
                if (_cachedSceneStorage == null) {
                    _cachedSceneStorage = GameObject.Find(SceneStorageName);

                    if (_cachedSceneStorage == null) {
                        // If we use new GameObject(), then for a split second
                        // Unity will show the game object in the hierarchy,
                        // which is bad UX.
                        _cachedSceneStorage = fiLateBindings.EditorUtility.CreateGameObjectWithHideFlags(SceneStorageName, HideFlags.HideInHierarchy);
                    }
                }

                return _cachedSceneStorage;
            }
        }
        #endregion Scene Storage

        #region Prefab Storage
        private static string PrefabPath = fiUtility.CombinePaths(fiSettings.RootGeneratedDirectory, "fiPersistentEditorStorage.prefab");
        private static GameObject _cachedPrefabStorage;
        public static GameObject PrefabStorage {
            get {
                if (_cachedPrefabStorage == null) {
                    // Try finding the current prefab
                    _cachedPrefabStorage = (GameObject)fiLateBindings.AssetDatabase.LoadAssetAtPath(PrefabPath, typeof(GameObject));

                    // Failed to find it; create a new one
                    if (_cachedPrefabStorage == null) {
                        var tmp = new GameObject();
                        _cachedPrefabStorage = fiLateBindings.PrefabUtility.CreatePrefab(PrefabPath, tmp);
                        fiUtility.DestroyObject(tmp);

                        Debug.Log("Created new editor storage object at " + PrefabPath +
                            "; this should only happen once. Please report a bug if it keeps on " +
                            "occurring.", _cachedPrefabStorage);
                    }
                }

                return _cachedPrefabStorage;
            }
        }
        #endregion Prefab Storage
    }
}