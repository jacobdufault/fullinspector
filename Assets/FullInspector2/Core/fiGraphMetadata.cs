using System;
using System.Collections.Generic;
using FullInspector.Internal;
using UnityObject = UnityEngine.Object;

namespace FullInspector {
    /// <summary>
    /// A simple wrapper tye for fiGraphMetadata so that we can have a different
    /// API signature when calling IPropertyEditor.Edit/GetHeight so that it is
    /// obvious that you need to call metadata.Enter(identifier) instead of
    /// passing in metadata.
    /// </summary>
    public struct fiGraphMetadataChild {
        public fiGraphMetadata Metadata;
    }

    /// <summary>
    /// An item that can be used as metadata inside of the graph metadata engine.
    /// </summary>
    public interface IGraphMetadataItemNotPersistent { }
    public interface IGraphMetadataItemPersistent {
        /// <summary>
        /// Should this metadata item be serialized? If it is in the default
        /// state, then there is no need to serialize it as it can just be
        /// recreated next time.
        /// </summary>
        bool ShouldSerialize();
    }

    /// <summary>
    /// The graph metadata system allows for metadata storage based on the
    /// location of an item in the inspector, regardless of the actual object
    /// instance. The graph metadata system requires minor user support -- you
    /// simply have to pass an associated key when entering into child metadata
    /// items.
    /// </summary>
    public class fiGraphMetadata {
        #region Serialization
        /// <summary>
        /// Returns true if the metadata should be reserialized. Because metadata
        /// restoration is lazy, we only want to reserialize metadata if we have
        /// restored the actual metadata structure. Otherwise, if we serialize
        /// before restoring we will lose all of the persistent metadata.
        /// </summary>
        public bool ShouldSerialize() {
            // we can check to see if we restored by checking if we have any
            // children
            return
                _childrenInt.IsEmpty == false ||
                _childrenString.IsEmpty == false;
        }

        public void Serialize<TPersistentData>(out string[] keys_, out TPersistentData[] values_)
            where TPersistentData : IGraphMetadataItemPersistent {
            var keys = new List<string>();
            var values = new List<TPersistentData>();

            AddSerializeData(keys, values);

            keys_ = keys.ToArray();
            values_ = values.ToArray();
        }
        private void AddSerializeData<TPersistentData>(List<string> keys, List<TPersistentData> values)
            where TPersistentData : IGraphMetadataItemPersistent {
            foreach (var item in _metadata.Items) {
                if (item.Key == typeof(TPersistentData)) {
                    if (((IGraphMetadataItemPersistent)item.Value).ShouldSerialize()) {
                        keys.Add(_accessPath);
                        values.Add((TPersistentData)item.Value);
                    }
                }
            }

            foreach (var item in _childrenInt.Items) {
                item.Value.AddSerializeData(keys, values);
            }
            foreach (var item in _childrenString.Items) {
                item.Value.AddSerializeData(keys, values);
            }
        }

        private Dictionary<string, List<object>> _precomputedData;
        public void Deserialize<TPersistentData>(string[] keys, TPersistentData[] values) {
            for (int i = 0; i < keys.Length; ++i) {
                var key = keys[i];

                List<object> allValues;
                if (_precomputedData.TryGetValue(key, out allValues) == false) {
                    allValues = new List<object>();
                    _precomputedData[key] = allValues;
                }

                allValues.Add(values[i]);
            }
        }
        #endregion Serialization

        #region Culling
        /// <summary>
        /// In order to avoid accumulating large amounts of metadata that is no
        /// longer useful or used, the graph metadata system supports automatic
        /// culling of unused metadata. If you wrap a set of code in a
        /// Begin/EndCullZone function calls, then any metadata that is not used
        /// between the Begin/End calls will be automatically released. This
        /// includes child metadata items.
        /// </summary>
        /// <remarks>
        /// Note that BeginCullZone() and EndCullZone() do *not* stack. Calling
        /// BeginCullZone() multiple times is like calling it only once. Calling
        /// EndCullZone() without having first called BeginCullZone() will do
        /// nothing.
        /// </remarks>
        /// <remarks>
        /// You almost certainly will not need to use this function. The
        /// IPropertyEditorExtensions engine handles it automatically.
        /// </remarks>
        public void BeginCullZone() {
            _childrenInt.BeginCullZone();
            _childrenString.BeginCullZone();
            _metadata.BeginCullZone();
        }

        /// <summary>
        /// This ends a culling zone. See docs on BeginCullZone.
        /// </summary>
        /// <remarks>
        /// You should not need to use this function -- it is for internal
        /// purposes. The IPropertyEditorExtensions engine handles it
        /// automatically.
        /// </remarks>
        public void EndCullZone() {
            _childrenInt.EndCullZone();
            _childrenString.EndCullZone();
            _metadata.EndCullZone();
        }
        #endregion Culling

        /// <summary>
        /// The child metadata objects (at construction time).
        /// </summary>
        /// <remarks>
        /// This can go out of date if the metadata graph is adjusted by property
        /// editors! It's useful for debugging purposes, but don't rely on it for
        /// the actual edit graph.
        /// </remarks>
        /// <remarks>
        /// We use two dictionaries instead of just one (that takes an object
        /// key) to avoid boxing ints.
        /// </remarks>
        [ShowInInspector]
        private CullableDictionary<int, fiGraphMetadata, IntDictionary<fiGraphMetadata>> _childrenInt;
        [ShowInInspector]
        private CullableDictionary<string, fiGraphMetadata, Dictionary<string, fiGraphMetadata>> _childrenString;

        /// <summary>
        /// The actual metadata objects.
        /// </summary>
        [ShowInInspector]
        private CullableDictionary<Type, object, Dictionary<Type, object>> _metadata;

        /// <summary>
        /// Reference to parent data, for access via GetInheritedMetadata
        /// </summary>
        private fiGraphMetadata _parentMetadata;
        public fiGraphMetadata Parent {
            get { return _parentMetadata; }
        }

        private fiUnityObjectReference _targetObject;
        private UnityObject TargetObject {
            get {
                if (_targetObject != null && _targetObject.IsValid) return _targetObject.Target;
                if (_parentMetadata != null) return _parentMetadata.TargetObject;
                return null;
            }
        }

        /// <summary>
        /// The metadata context is simply the parent object which generated this
        /// metadata. For example, if there is a struct |S| with members |a|,
        /// |b|, |c|, then the context for the metadata on |a|, |b|, and |c| is
        /// the instance of |S| that owns them.
        ///
        /// Context can be gathered recursively by examining the |Parent|
        /// metadata instance.
        /// </summary>
        public object Context;

        private string _accessPath;
        public string Path {
            get { return _accessPath; }
        }

        public fiGraphMetadata() : this(null) {
        }

        public fiGraphMetadata(fiUnityObjectReference targetObject)
            : this(null, string.Empty) {
            _targetObject = targetObject;
        }

        private fiGraphMetadata(fiGraphMetadata parentMetadata, string accessKey) {
            _childrenInt = new CullableDictionary<int, fiGraphMetadata, IntDictionary<fiGraphMetadata>>();
            _childrenString = new CullableDictionary<string, fiGraphMetadata, Dictionary<string, fiGraphMetadata>>();
            _metadata = new CullableDictionary<Type, object, Dictionary<Type, object>>();
            _parentMetadata = parentMetadata;

            if (_parentMetadata == null) _precomputedData = new Dictionary<string, List<object>>();
            else _precomputedData = _parentMetadata._precomputedData;

            RebuildAccessPath(accessKey);

            if (_precomputedData.ContainsKey(_accessPath)) {
                foreach (var data in _precomputedData[_accessPath]) {
                    _metadata[data.GetType()] = data;
                }
            }
        }

        private void RebuildAccessPath(string accessKey) {
            _accessPath = "";
            if (_parentMetadata != null && string.IsNullOrEmpty(_parentMetadata._accessPath) == false) {
                _accessPath += _parentMetadata._accessPath + ".";
            }
            _accessPath += accessKey;
        }

        #region Metadata Migration APIs
        /// <summary>
        /// Forcibly change the metadata that the given identifier points to to
        /// the specified instance. This is extremely useful if the inspector
        /// graph has been modified and the editor needs to make an adjustment to
        /// the metadata so that the metadata graph remains consistent with the
        /// actual inspector graph.
        /// </summary>
        /// <remarks>
        /// You do not need to worry about removing child metadata -- they will
        /// be automatically removed.
        /// </remarks>
        public void SetChild(int identifier, fiGraphMetadata metadata) {
            _childrenInt[identifier] = metadata;
            metadata.RebuildAccessPath(identifier.ToString());
        }
        /// <summary>
        /// Forcibly change the metadata that the given identifier points to to
        /// the specified instance. This is extremely useful if the inspector
        /// graph has been modified and the editor needs to make an adjustment to
        /// the metadata so that the metadata graph remains consistent with the
        /// actual inspector graph.
        /// </summary>
        /// <remarks>
        /// You do not need to worry about removing child metadata -- they will
        /// be automatically removed.
        /// </remarks>
        public void SetChild(string identifier, fiGraphMetadata metadata) {
            _childrenString[identifier] = metadata;
            metadata.RebuildAccessPath(identifier);
        }

        public struct MetadataMigration {
            public int NewIndex, OldIndex;

            public override string ToString() {
                return "Migration [" + OldIndex + " => " + NewIndex + "]";
            }
        }

        /// <summary>
        /// Helper method that automates metadata migration for array based graph
        /// reorders.
        /// </summary>
        public static void MigrateMetadata<T>(fiGraphMetadata metadata, T[] previous, T[] updated) {
            var migrations = ComputeNeededMigrations(previous, updated);

            // migrate persistent data
            /*
            var fromKeys = new string[migrations.Count];
            var toKeys = new string[migrations.Count];
            for (int i = 0; i < migrations.Count; ++i) {
                fromKeys[i] = metadata.Enter(migrations[i].OldIndex).Metadata._accessPath;
                toKeys[i] = metadata.Enter(migrations[i].NewIndex).Metadata._accessPath;
            }*/
            // fiPersistentMetadata.Migrate(metadata.TargetObject, fromKeys,
            // toKeys);

            // migrate the graph items
            List<fiGraphMetadata> copiedGraphs = new List<fiGraphMetadata>(migrations.Count);
            for (int i = 0; i < migrations.Count; ++i) {
                copiedGraphs.Add(metadata._childrenInt[migrations[i].OldIndex]);
            }

            for (int i = 0; i < migrations.Count; ++i) {
                metadata._childrenInt[migrations[i].NewIndex] = copiedGraphs[i];
            }
        }

        /// <summary>
        /// Helper method that automates metadata migration for array based graph
        /// reorders.
        /// </summary>
        public static List<MetadataMigration> ComputeNeededMigrations<T>(T[] previous, T[] updated) {
            var migrations = new List<MetadataMigration>();

            for (int newIndex = 0; newIndex < updated.Length; ++newIndex) {
                int prevIndex = Array.IndexOf(previous, updated[newIndex]);

                if (prevIndex != -1 && prevIndex != newIndex) {
                    migrations.Add(new MetadataMigration {
                        NewIndex = newIndex,
                        OldIndex = prevIndex
                    });
                }
            }

            return migrations;
        }
        #endregion Metadata Migration APIs
        /// <summary>
        /// Get a child metadata instance for the given identifier. This is
        /// useful for collections where each item maps to a unique index.
        /// </summary>
        public fiGraphMetadataChild Enter(int childIdentifier, object context) {
            fiGraphMetadata metadata;

            if (_childrenInt.TryGetValue(childIdentifier, out metadata) == false) {
                metadata = new fiGraphMetadata(this, childIdentifier.ToString());
                _childrenInt[childIdentifier] = metadata;
            }
            metadata.Context = context;

            return new fiGraphMetadataChild { Metadata = metadata };
        }

        /// <summary>
        /// Get a child metadata instance for the given identifier. This is
        /// useful for general classes and structs where an object has a set of
        /// discrete named fields or properties.
        /// </summary>
        public fiGraphMetadataChild Enter(string childIdentifier, object context) {
            fiGraphMetadata metadata;

            if (_childrenString.TryGetValue(childIdentifier, out metadata) == false) {
                metadata = new fiGraphMetadata(this, childIdentifier);
                _childrenString[childIdentifier] = metadata;
            }
            metadata.Context = context;

            return new fiGraphMetadataChild { Metadata = metadata };
        }

        public fiGraphMetadataChild NoOp() {
            return new fiGraphMetadataChild { Metadata = this };
        }

        public T GetPersistentMetadata<T>()
            where T : IGraphMetadataItemPersistent, new() {
            bool wasCreated;
            return GetPersistentMetadata<T>(out wasCreated);
        }

        public T GetPersistentMetadata<T>(out bool wasCreated)
            where T : IGraphMetadataItemPersistent, new() {
            return GetCommonMetadata<T>(out wasCreated);
        }

        /// <summary>
        /// Get a metadata instance for an object.
        /// </summary>
        /// <typeparam name="T">The type of metadata instance.</typeparam>
        public T GetMetadata<T>() where T : IGraphMetadataItemNotPersistent, new() {
            bool wasCreated;
            return GetMetadata<T>(out wasCreated);
        }

        public T GetMetadata<T>(out bool wasCreated) where T : IGraphMetadataItemNotPersistent, new() {
            return GetCommonMetadata<T>(out wasCreated);
        }

        private T GetCommonMetadata<T>(out bool wasCreated)
            where T : new() {
            object val;
            if (_metadata.TryGetValue(typeof(T), out val) == false) {
                val = new T();
                _metadata[typeof(T)] = val;
                wasCreated = true;
            }
            else {
                wasCreated = false;
            }

            return (T)val;
        }

        /// <summary>
        /// Get a metadata instance for an object, searching up through parent
        /// chain
        /// </summary>
        /// <typeparam name="T">The type of metadata instance.</typeparam>
        public T GetInheritedMetadata<T>() where T : IGraphMetadataItemNotPersistent, new() {
            object val;

            if (_metadata.TryGetValue(typeof(T), out val)) {
                return (T)val;
            }
            else if (_parentMetadata == null) {
                return GetMetadata<T>();
            }
            else {
                return _parentMetadata.GetInheritedMetadata<T>();
            }
        }

        /// <summary>
        /// Attempts to fetch a pre-existing metadata instance for an object.
        /// </summary>
        /// <typeparam name="T">The type of metadata instance.</typeparam>
        /// <param name="metadata">The metadata instance.</param>
        /// <returns>
        /// True if a metadata instance was found, false otherwise.
        /// </returns>
        public bool TryGetMetadata<T>(out T metadata) where T : IGraphMetadataItemNotPersistent, new() {
            object item;
            bool result = _metadata.TryGetValue(typeof(T), out item);

            metadata = (T)item;
            return result;
        }

        /// <summary>
        /// Attempts to fetch a pre-existing metadata instance for an object,
        /// searching up through the parent chain
        /// </summary>
        /// <typeparam name="T">The type of metadata instance.</typeparam>
        /// <param name="metadata">
        /// The found metadata instance. Undefined if there was no metadata.
        /// </param>
        /// <returns>
        /// True if a metadata instance was found, false otherwise.
        /// </returns>
        public bool TryGetInheritedMetadata<T>(out T metadata) where T : IGraphMetadataItemNotPersistent, new() {
            object item;
            if (_metadata.TryGetValue(typeof(T), out item)) {
                metadata = (T)item;
                return true;
            }
            else if (_parentMetadata == null) {
                metadata = default(T);
                return false;
            }
            else {
                return _parentMetadata.TryGetInheritedMetadata(out metadata);
            }
        }
    }

    /// <summary>
    /// A (partial) dictionary implementation that has been optimized for fast
    /// &gt;= 0 int access.
    /// </summary>
    internal class IntDictionary<TValue> : IDictionary<int, TValue> {
        private List<fiOption<TValue>> _positives = new List<fiOption<TValue>>();
        private Dictionary<int, TValue> _negatives = new Dictionary<int, TValue>();

        public void Add(int key, TValue value) {
            if (key < 0) {
                _negatives.Add(key, value);
            }
            else {
                while (key >= _positives.Count) {
                    _positives.Add(fiOption<TValue>.Empty);
                }
                if (_positives[key].HasValue) throw new Exception("Already have a key for " + key);
                _positives[key] = fiOption.Just(value);
            }
        }

        public bool ContainsKey(int key) {
            if (key < 0) return _negatives.ContainsKey(key);
            else return key < _positives.Count && _positives[key].HasValue;
        }

        public ICollection<int> Keys {
            get { throw new NotSupportedException(); }
        }

        public bool Remove(int key) {
            if (key < 0) return _negatives.Remove(key);
            else {
                if (key >= _positives.Count) return false;
                if (_positives[key].IsEmpty) return false;

                _positives[key] = fiOption<TValue>.Empty;
                return true;
            }
        }

        public bool TryGetValue(int key, out TValue value) {
            if (key < 0) return _negatives.TryGetValue(key, out value);

            value = default(TValue);
            if (key >= _positives.Count) return false;
            if (_positives[key].IsEmpty) return false;

            value = _positives[key].Value;
            return true;
        }

        public ICollection<TValue> Values {
            get { throw new NotSupportedException(); }
        }

        public TValue this[int key] {
            get {
                if (key < 0) return _negatives[key];
                else {
                    if (key >= _positives.Count) throw new KeyNotFoundException("" + key);
                    if (_positives[key].IsEmpty) throw new KeyNotFoundException("" + key);

                    return _positives[key].Value;
                }
            }
            set {
                if (key < 0) {
                    _negatives[key] = value;
                }
                else {
                    while (key >= _positives.Count) {
                        _positives.Add(fiOption<TValue>.Empty);
                    }
                    _positives[key] = fiOption.Just(value);
                }
            }
        }

        public void Add(KeyValuePair<int, TValue> item) {
            Add(item.Key, item.Value);
        }

        public void Clear() {
            _negatives.Clear();
            _positives.Clear();
        }

        public bool Contains(KeyValuePair<int, TValue> item) {
            throw new NotSupportedException();
        }

        public void CopyTo(KeyValuePair<int, TValue>[] array, int arrayIndex) {
            foreach (var item in this) {
                if (arrayIndex >= array.Length) break;

                array[arrayIndex++] = item;
            }
        }

        public int Count {
            get {
                int count = _negatives.Count;

                for (int i = 0; i < _positives.Count; ++i) {
                    if (_positives[i].HasValue) ++count;
                }

                return count;
            }
        }

        public bool IsReadOnly {
            get { throw new NotSupportedException(); }
        }

        public bool Remove(KeyValuePair<int, TValue> item) {
            throw new NotSupportedException();
        }

        public IEnumerator<KeyValuePair<int, TValue>> GetEnumerator() {
            foreach (KeyValuePair<int, TValue> value in _negatives) {
                yield return value;
            }

            for (int i = 0; i < _positives.Count; ++i) {
                if (_positives[i].HasValue) {
                    yield return new KeyValuePair<int, TValue>(i, _positives[i].Value);
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}