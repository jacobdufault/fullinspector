using System;
using System.Collections.Generic;
using System.Linq;
using FullSerializer;
using FullInspector.StoragesManager;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {

    public class fiPersistentEditorStorage {

        private const string SceneStorageName = "fiPersistentEditorStorage";

        private static readonly string _PrefabStoragePath =
                fiUtility.CombinePaths(fiSettings.RootGeneratedDirectory, "fiPersistentEditorStorage.prefab");

        private static readonly fiStoragesManager<fiPersistentEditorStorageComponent> _StoragesManager =
            new fiStoragesManager<fiPersistentEditorStorageComponent>(SceneStorageName, _PrefabStoragePath);

        #region Reading/Writing
        public static void Reset<T>(fiUnityObjectReference key) {
            fiBaseStorageComponent<T> storage;
            if (fiLateBindings.EditorUtility.IsPersistent(key.Target)) {
                storage = GetStorageDictionary<T>(PrefabStorage);
            }
            else {
                storage = GetStorageDictionary<T>(GetStorage(key.Target));
            }

            if (storage != null) {
                storage.Data.Remove(key.Target);
                fiLateBindings.EditorUtility.SetDirty(storage);
            }
        }


        public static T Read<T>(fiUnityObjectReference key)
            where T : new() {

            fiBaseStorageComponent<T> storage = null;
            if (fiLateBindings.EditorUtility.IsPersistent(key.Target)) {
                storage = GetStorageDictionary<T>(PrefabStorage);
            } else {
                storage = GetStorageDictionary<T>(GetStorage(key.Target));
            }

            if (storage != null) {
                if (storage.Data.ContainsKey(key.Target)) {
                    return storage.Data[key.Target];
                }

                var value = storage.Data[key.Target] = new T();
                fiLateBindings.EditorUtility.SetDirty(storage);

                return value;
            }

            return default(T);
        }

        private static Dictionary<Type, Type> _cachedRealComponentTypes = new Dictionary<Type, Type>();
        private static fiBaseStorageComponent<T> GetStorageDictionary<T>(fiPersistentEditorStorageComponent container) {
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
                component = container.gameObject.AddComponent(realComponentType);
            }

            return (fiBaseStorageComponent<T>)component;
        }
        #endregion

        public static IEnumerable<fiPersistentEditorStorageComponent> GetAllCachedSceneStorages() {
            return _StoragesManager.GetAllSceneStorages();
        }

        public static fiPersistentEditorStorageComponent GetStorage(UnityObject o) {
            return _StoragesManager.GetStorage(o);
        }

        public static fiPersistentEditorStorageComponent PrefabStorage {
            get { return _StoragesManager.PrefabStorage; }
        }

    }
}
