using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using FullInspector.Internal;
using UnityEngine.SceneManagement;

namespace FullInspector.StoragesManager {

    public class fiStoragesManager<T>
        where T : MonoBehaviour {
        private readonly Dictionary<Scene, T> _Storages = new Dictionary<Scene, T>();
        private readonly string SceneStorageName;
        private readonly string PrefabStoragePath;

        private bool _didInitialRefresh;

        private List<Scene> _lastSeenScenes = new List<Scene>();


        public fiStoragesManager(string sceneStorageName, string prefabStoragePath) {
            SceneStorageName = sceneStorageName;
            PrefabStoragePath = prefabStoragePath;
        }

        private T _prefabStorage;
        public T PrefabStorage {
            get {
                if (_prefabStorage == null) {
                    GameObject prefabGameObject = null;

                    // Try finding the current prefab
                    prefabGameObject = fiLateBindings.AssetDatabase.LoadAssetAtPath(PrefabStoragePath, typeof(GameObject)) as GameObject;

                    // Failed to find it; create a new one
                    if (prefabGameObject == null) {
                        var cloned = new GameObject();
                        prefabGameObject = fiLateBindings.PrefabUtility.CreatePrefab(PrefabStoragePath, cloned);
                        fiUtility.DestroyObject(cloned);

                        prefabGameObject.AddComponent<T>();

                        Debug.Log("Created new backup persistent storage object at " + PrefabStorage +
                                  "; this should only happen once. Please report a bug if it keeps on " +
                                  "occurring.", prefabGameObject);
                    }

                    _prefabStorage = prefabGameObject.GetComponent<T>();

                    if (_prefabStorage == null) {
                        _prefabStorage = prefabGameObject.AddComponent<T>();
                    }
                }

                return _prefabStorage;
            }
        }


        public IEnumerable<T> GetAllSceneStorages() {
            RefreshStorages();
            return _Storages.Select(t => t.Value);
        }


        public T GetStorage(Scene scene) {
            RefreshStorages();
            T storage;
            if (_Storages.TryGetValue(scene, out storage)) {
                return storage;
            }

            return _prefabStorage;
        }

        public T GetStorage(Object o) {
            RefreshStorages();

            Scene scene;
            var c = o as Component;
            if (c != null) {
                scene = c.gameObject.scene;
            } else {
                var g = o as GameObject;
                if (g != null) {
                    scene = g.scene;
                } else {
                    return PrefabStorage;
                }
            }

            if (!scene.IsValid() || !scene.isLoaded || scene.name == "DontDestroyOnLoad") {
                return PrefabStorage;
            }

            return _Storages[scene];
        }


        /// <summary>
        /// This will ensure that <see cref="_Storages"/> always has fresh
        /// references for each storage in the loaded scenes.
        /// </summary>
        /// <param name="forced"></param>
        private void RefreshStorages(bool forced = false) {
            //always refresh at least once
            if (_didInitialRefresh && !UpdateLastSeenScenes()) {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                var hasNullStorages = _Storages.Any(s => s.Value == null || s.Value.gameObject == null);
                var mustRefresh = false;
                if (!hasNullStorages) {
                    if (_Storages.Count == 0) {
                        mustRefresh = true;
                    } else {
                        for (var i = 0; i < SceneManager.sceneCount; i++) {
                            Scene scene = SceneManager.GetSceneAt(i);
                            if (scene.isLoaded) {
                                if (_Storages.ContainsKey(scene)) {
                                    continue;
                                }

                                //we'll have to refresh if we stored keys for scenes that were unloaded
                            } else if (!_Storages.ContainsKey(scene)) {
                                continue;
                            }

                            mustRefresh = true;
                            break;
                        }
                    }
                }

                if (!forced && !mustRefresh && !hasNullStorages) {
                    return;
                }
            }

            _didInitialRefresh = true;

            //get fresh references for all storages that already exist in the currently loaded scenes
            _Storages.Clear();
            foreach (var found in Object.FindObjectsOfType<T>()) {
                var scene = found.gameObject.scene;
                if (!_Storages.Keys.Contains(scene)) {
                    _Storages[scene] = found;
                } else {
                    //Debug.LogWarning("scene " + scene.name + " already has a storage, but found another. Destroying...");
                    Object.DestroyImmediate(found.gameObject);
                    if (!fiLateBindings.EditorApplication.isPlaying) {
                        fiLateBindings.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
                    }
                }
            }

            for (var i = 0; i < SceneManager.sceneCount; i++) {
                Scene scene = SceneManager.GetSceneAt(i);
                if (_Storages.ContainsKey(scene) || !scene.isLoaded) {
                    continue;
                }

                //if we are here, it means that this scene doesn't have a storage, so we created one
                var obj = fiLateBindings.EditorUtility.CreateGameObjectWithHideFlags(SceneStorageName, HideFlags.HideInHierarchy);
                SceneManager.MoveGameObjectToScene(obj.gameObject, scene);
                var storage = obj.AddComponent<T>();
                _Storages[scene] = storage;
                //Debug.Log("No storage for scene " + scene.name + ". New one was created", storage.gameObject);
            }

        }


        private bool UpdateLastSeenScenes() {
            List<Scene> currentlyLoaded = new List<Scene>();
            for (int i = 0; i < SceneManager.sceneCount; i++) {
                Scene s = SceneManager.GetSceneAt(i);
                if (s.isLoaded) {
                    currentlyLoaded.Add(s);
                }
            }

            List<Scene> lastSeenLoadedScenes = _lastSeenScenes.Where(s => s.isLoaded).ToList();
            if (lastSeenLoadedScenes.Count != _lastSeenScenes.Count) {
                _lastSeenScenes = lastSeenLoadedScenes;
            }

            if (currentlyLoaded.All(s => _lastSeenScenes.Contains(s))) {
                return false;
            }

            _lastSeenScenes.Clear();
            foreach (var scene in currentlyLoaded) {
                _lastSeenScenes.Add(scene);
            }

            return true;
        }

    }
}
