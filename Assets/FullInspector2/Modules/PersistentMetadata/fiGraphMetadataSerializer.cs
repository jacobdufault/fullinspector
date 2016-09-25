using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    public class fiGraphMetadataSerializer<TPersistentData> : fiIGraphMetadataStorage, ISerializationCallbackReceiver
       where TPersistentData : IGraphMetadataItemPersistent {

        [SerializeField]
        private string[] _keys;
        [SerializeField]
        private TPersistentData[] _values;
        [SerializeField]
        private UnityObject _target;

        public void RestoreData(fiUnityObjectReference target) {
            _target = target.Target;

            if (_keys != null && _values != null) {
                fiPersistentMetadata.GetMetadataFor(target).Deserialize(_keys, _values);
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            if (ReferenceEquals(_target, null))
                return;

            // Unity's null equality check is buggy. It doesn't always return
            // true if the object has been destroyed if the type of _target is
            // UnityObject.
            try {
                var target = new fiUnityObjectReference(_target, /*tryRestore:*/ false);
                if (fiPersistentMetadata.HasMetadata(target) == false)
                    return;

                var metadata = fiPersistentMetadata.GetMetadataFor(target);
                if (metadata.ShouldSerialize()) {
                    metadata.Serialize(out _keys, out _values);
                }
            }
            catch (MissingReferenceException e) {
                fiLog.Log(typeof(fiGraphMetadataSerializer<TPersistentData>),
                          "Caught exception {0}", e);
            }
        }
    }

}