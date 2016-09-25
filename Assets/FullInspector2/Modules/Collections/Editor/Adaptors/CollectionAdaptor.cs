using FullInspector.Rotorz.ReorderableList;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FullInspector.Internal {
    /// <summary>
    /// Reorderable list adapter for ICollection types
    /// </summary>
    public class CollectionAdaptor<T> : IReorderableListAdaptor {
        public delegate float ItemHeight(T item, fiGraphMetadataChild metadata);
        public delegate T ItemDrawer(Rect position, T item, fiGraphMetadataChild metadata);

        /// <summary>
        /// Returns the height of the given element.
        /// </summary>
        private ItemHeight _height;

        /// <summary>
        /// Provides an editor for the given element.
        /// </summary>
        private ItemDrawer _drawer;

        /// <summary>
        /// Metadata we use for the callbacks.
        /// </summary>
        private fiGraphMetadata _metadata;

        /// <summary>
        /// Stores all of the elements
        /// </summary>
        private ICollection<T> _collection;

        /// <summary>
        /// A cached version of the collection optimized for item lookup.
        /// </summary>
        private T[] _collectionCache;

        /// <summary>
        /// For performance reasons, the CollectionAdaptor stores an array version of the
        /// collection. If the adapted collection has been structurally modified, for example, an
        /// item has been added, then the local cache is invalid. Calling this method updates the
        /// cache, which will restore proper adapter semantics.
        /// </summary>
        public void InvalidateCache(bool migrateMetadata) {
            T[] oldCache = _collectionCache;
            T[] newCache = _collection.ToArray();
            _collectionCache = newCache;

            // migrate metadata
            if (oldCache != null && migrateMetadata) {
                fiGraphMetadata.MigrateMetadata(_metadata, oldCache, newCache);
            }
        }

        public CollectionAdaptor(ICollection<T> collection, ItemDrawer drawer, ItemHeight height,
            fiGraphMetadata metadata) {

            _metadata = metadata;
            _collection = collection;
            _drawer = drawer;
            _height = height;

            InvalidateCache(/*migrateMetadata:*/ false);
        }

        public int Count {
            get { return _collectionCache.Length; }
        }

        public virtual bool CanDrag(int index) {
            return true;
        }

        public virtual bool CanRemove(int index) {
            return true;
        }

        public void Add(T item) {
            _collection.Add(item);
            InvalidateCache(/*migrateMetadata:*/ true);
        }

        public void Add() {
            Add(default(T));
        }

        public void Insert(int index) {
            throw new NotSupportedException();
        }

        public void Duplicate(int index) {
            T element = _collectionCache[index];
            _collection.Add(element);

            InvalidateCache(/*migrateMetadata:*/ true);
        }

        public void Remove(int index) {
            T element = _collection.ElementAt(index);
            _collection.Remove(element);
            InvalidateCache(/*migrateMetadata:*/ true);
        }

        public void Move(int sourceIndex, int destIndex) {
            throw new NotSupportedException();
        }

        public void Clear() {
            _collection.Clear();
            InvalidateCache(/*migrateMetadata:*/ true);
        }

        public virtual void DrawItem(Rect position, int index) {
            // Rotorz seems to sometimes give an index of -1, not sure why.
            if (index < 0) {
                return;
            }

            T element = _collectionCache[index];
            T updated = _drawer(position, element, _metadata.Enter(index));

            // If the modified item is equal to the updated item, then we don't have to replace it
            // in the collection.
            if (EqualityComparer<T>.Default.Equals(element, updated) == false) {
                fiLog.Log(GetType(), "Removing old element " + element + " (at index " + index + ") and adding new element " + updated);
                
                // Removing/adding the item is considered an atomic operation; if any
                // part of it fails then we do not want to modify the collection (ie, if
                // adding the updated element fails in a dictionary because the key already
                // exists).

                bool didRemove = false;
                try {
                    _collection.Remove(element);
                    didRemove = true;
                    _collection.Add(updated);
                }
                catch (Exception) {
                    // Swallow the exception - usually it will be stating that an existing entry
                    // already exists in, say, the dictionary

                    // Treat the entire operation as atomic; undo previous work if anything failed
                    if (didRemove) _collection.Add(element);
                }

                InvalidateCache(/*migrateMetadata:*/ false);
            }
        }

        public virtual float GetItemHeight(int index) {
            T element = _collectionCache[index];

            return _height(element, _metadata.Enter(index));
        }
    }
}