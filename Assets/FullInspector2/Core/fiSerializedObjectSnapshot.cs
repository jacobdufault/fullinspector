using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    /// <summary>
    /// Stores the state of a serialized object (but only the Full Inspector data).
    /// </summary>
    public class fiSerializedObjectSnapshot {
        private readonly List<string> _keys;
        private readonly List<string> _values;
        private readonly List<UnityObject> _objectReferences;

        public fiSerializedObjectSnapshot(ISerializedObject obj) {
            _keys = new List<string>(obj.SerializedStateKeys);
            _values = new List<string>(obj.SerializedStateValues);
            _objectReferences = new List<UnityObject>(obj.SerializedObjectReferences);
        }

        public void RestoreSnapshot(ISerializedObject target) {
            target.SerializedStateKeys = new List<string>(_keys);
            target.SerializedStateValues = new List<string>(_values);
            target.SerializedObjectReferences = new List<UnityObject>(_objectReferences);
            target.RestoreState();
        }

        public bool IsEmpty {
            get {
                return
                    _keys.Count == 0 ||
                    _values.Count == 0;
            }
        }

        public override bool Equals(object obj) {
            var snapshot = obj as fiSerializedObjectSnapshot;

            if (ReferenceEquals(snapshot, null)) return false;

            return
                AreEqual(_keys, snapshot._keys) &&
                AreEqual(_values, snapshot._values) &&
                AreEqual(_objectReferences, snapshot._objectReferences);
        }

        public override int GetHashCode() {
            int hash = 13;
            hash = (hash * 7) + _keys.GetHashCode();
            hash = (hash * 7) + _values.GetHashCode();
            hash = (hash * 7) + _objectReferences.GetHashCode();
            return hash;
        }

        public static bool operator ==(fiSerializedObjectSnapshot a, fiSerializedObjectSnapshot b) {
            return Equals(a, b);
        }

        public static bool operator !=(fiSerializedObjectSnapshot a, fiSerializedObjectSnapshot b) {
            return Equals(a, b) == false;
        }

        private static bool AreEqual<T>(List<T> a, List<T> b) {
            if (a.Count != b.Count) return false;
            for (int i = 0; i < a.Count; ++i) {
                if (EqualityComparer<T>.Default.Equals(a[i], b[i]) == false) {
                    return false;
                }
            }
            return true;
        }
    }
}