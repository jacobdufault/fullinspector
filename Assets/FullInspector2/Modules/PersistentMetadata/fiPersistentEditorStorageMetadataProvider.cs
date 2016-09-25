using System;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    public interface fiIGraphMetadataStorage {
        void RestoreData(fiUnityObjectReference target);
    }

    public abstract class fiPersistentEditorStorageMetadataProvider<TItem, TStorage> : fiIPersistentMetadataProvider
        where TItem : new()
        where TStorage : fiIGraphMetadataStorage, new() {

        public void RestoreData(fiUnityObjectReference target) {
            fiPersistentEditorStorage.Read<TStorage>(target).RestoreData(target);
        }

        public void Reset(fiUnityObjectReference target) {
            fiPersistentEditorStorage.Reset<TStorage>(target);
        }

        public Type MetadataType {
            get { return typeof(TItem); }
        }
    }
}