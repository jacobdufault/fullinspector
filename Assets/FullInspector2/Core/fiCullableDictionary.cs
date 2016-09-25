using System.Collections.Generic;
using UnityEngine;

namespace FullInspector.Internal {
    public interface ICullableDictionary<TKey, TValue> {
        TValue this[TKey key] { get; set; }
        bool TryGetValue(TKey key, out TValue value);
        void BeginCullZone();
        void EndCullZone();
        IEnumerable<KeyValuePair<TKey, TValue>> Items { get; }
        bool IsEmpty { get; }
    }

    //TODO: use cullable dictionary in persistent metadata

    /// <summary>
    /// A cullable dictionary is like a normal dictionary except that items inside of it can
    /// be removed if they are not used after a cull cycle.
    /// </summary>
    public class CullableDictionary<TKey, TValue, TDictionary> : ICullableDictionary<TKey, TValue>
        where TDictionary : IDictionary<TKey, TValue>, new() {

        /// <summary>
        /// Items that have been used and will therefore *not* be culled.
        /// </summary>
        [SerializeField]
        private TDictionary _primary;

        /// <summary>
        /// The items that we will cull if EndCullZone is called.
        /// </summary>
        [SerializeField]
        private TDictionary _culled;

        /// <summary>
        /// Are we currently culling data?
        /// </summary>
        [SerializeField]
        private bool _isCulling;

        public CullableDictionary() {
            _primary = new TDictionary();
            _culled = new TDictionary();
        }

        public TValue this[TKey key] {
            get {
                TValue value;
                if (TryGetValue(key, out value) == false) {
                    throw new KeyNotFoundException("" + key);
                }
                return value;
            }
            set {
                _culled.Remove(key);
                _primary[key] = value;
            }
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> Items {
            get {
                foreach (var item in _primary) yield return item;
                foreach (var item in _culled) yield return item;
            }
        }

        /// <summary>
        /// Add an item to the dictionary.
        /// </summary>
        public void Add(TKey key, TValue value) {
            _primary.Add(key, value);
        }

        /// <summary>
        /// Attempts to fetch a value for the given key. If a value is found, then it will
        /// not be culled on the next cull-cycle.
        /// </summary>
        public bool TryGetValue(TKey key, out TValue value) {
            if (_culled.TryGetValue(key, out value)) {

                // If we access the value in the culled set, then we want to make sure to move
                // it into the primary set so we don't cull it.
                _culled.Remove(key);
                _primary.Add(key, value);

                return true;
            }

            return _primary.TryGetValue(key, out value);
        }

        /// <summary>
        /// Begin a call zone. This is fine to call multiple times.
        /// </summary>
        public void BeginCullZone() {
            if (_isCulling == false) {
                fiUtility.Swap(ref _primary, ref _culled);
                _isCulling = true;
            }
        }

        /// <summary>
        /// Clears out all unused items. This method is harmless if called outside of
        /// BeginCullZone().
        /// </summary>
        /// <returns>Everything that was *not* culled.</returns>
        public void EndCullZone() {
            if (_isCulling) _isCulling = false;

            if (fiSettings.EmitGraphMetadataCulls) {
                // sigh: Only run the foreach loop if we actually have content to emit,
                // otherwise we will allocate an iterator needlessly.
                if (_culled.Count > 0) {
                    foreach (var item in _culled) {
                        Debug.Log("fiGraphMetadata culling \"" + item.Key + "\"");
                    }
                }
            }

            _culled.Clear();
        }

        public bool IsEmpty {
            get {
                return _primary.Count == 0 && _culled.Count == 0;
            }
        }
    }
}