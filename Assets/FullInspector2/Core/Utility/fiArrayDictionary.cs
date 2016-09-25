using System.Collections.Generic;

namespace FullInspector.Internal {
    /// <summary>
    /// A dictionary that does not use GetHashCode().
    /// </summary>
    public class fiArrayDictionary<TKey, TValue> {
        private List<KeyValuePair<TKey, TValue>> _elements = new List<KeyValuePair<TKey, TValue>>();

        public TValue this[TKey key] {
            set {
                // Replace existing item.
                for (int i = 0; i < _elements.Count; ++i) {
                    var element = _elements[i];
                    if (EqualityComparer<TKey>.Default.Equals(key, element.Key)) {
                        _elements[i] = new KeyValuePair<TKey, TValue>(key, value);
                        return;
                    }
                }

                // Add to end.
                _elements.Add(new KeyValuePair<TKey, TValue>(key, value));
            }
        }

        public bool Remove(TKey key) {
            // Replace existing item.
            for (int i = 0; i < _elements.Count; ++i) {
                var element = _elements[i];
                if (EqualityComparer<TKey>.Default.Equals(key, element.Key)) {
                    _elements.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public bool ContainsKey(TKey key) {
            TValue dummy;
            return TryGetValue(key, out dummy);
        }

        public bool TryGetValue(TKey key, out TValue value) {
            for (int i = 0; i < _elements.Count; ++i) {
                var element = _elements[i];
                if (EqualityComparer<TKey>.Default.Equals(key, element.Key)) {
                    value = element.Value;
                    return true;
                }
            }

            value = default(TValue);
            return false;
        }
    }
}