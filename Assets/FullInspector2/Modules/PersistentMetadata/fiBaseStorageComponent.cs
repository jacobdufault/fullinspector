using System;
using System.Collections.Generic;
using FullInspector.Internal;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector {
    [AddComponentMenu("")]
    public abstract class fiBaseStorageComponent<T> : MonoBehaviour, fiIEditorOnlyTag, ISerializationCallbackReceiver {

        [SerializeField]
        private List<UnityObject> _keys;
        [SerializeField]
        private List<T> _values;

        private IDictionary<UnityObject, T> _data;
        public IDictionary<UnityObject, T> Data {
            get {
                if (_data == null)
                    _data = new Dictionary<UnityObject, T>();
                return _data;
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            if (_keys == null || _values == null)
                return;

            _data = new Dictionary<UnityObject, T>();
            for (int i = 0; i < Math.Min(_keys.Count, _values.Count); ++i) {
                if (ReferenceEquals(_keys[i], null) == false) {
                    Data[_keys[i]] = _values[i];
                }
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            if (_data == null) {
                _keys = null;
                _values = null;
                return;
            }

            _keys = new List<UnityObject>(_data.Count);
            _values = new List<T>(_data.Count);
            foreach (var entry in _data) {
                // We do *not* check to see if entry.Key refers to a valid
                // UnityObject here. If we did, then the GetInstanceId()
                // restoration mechanism would not work properly.
                _keys.Add(entry.Key);
                _values.Add(entry.Value);
            }
        }
    }
}