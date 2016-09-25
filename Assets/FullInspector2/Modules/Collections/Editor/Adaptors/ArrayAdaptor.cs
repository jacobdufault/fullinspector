using FullInspector.Rotorz.ReorderableList;
using System;
using UnityEngine;

namespace FullInspector.Internal {
    /// <summary>
    /// Reorderable list adapter for arrays.
    /// </summary>
    public class ArrayAdaptor<T> : IReorderableListAdaptor {
        public delegate float ItemHeight(T item, fiGraphMetadataChild metadata);
        public delegate T ItemDrawer(Rect position, T item, fiGraphMetadataChild metadata);

        private ItemHeight _itemHeight;
        private ItemDrawer _itemDrawer;

        private fiGraphMetadata _metadata;
        private T[] _array;

        public T[] StoredArray {
            get {
                return _array;
            }
        }

        public ArrayAdaptor(T[] array, ItemDrawer itemDrawer, ItemHeight itemHeight, fiGraphMetadata metadata) {
            _metadata = metadata;
            _array = array;
            _itemDrawer = itemDrawer;
            _itemHeight = itemHeight;
        }

        public int Count {
            get { return _array.Length; }
        }

        public virtual bool CanDrag(int index) {
            return true;
        }

        public virtual bool CanRemove(int index) {
            return true;
        }

        public void Add() {
            Array.Resize(ref _array, _array.Length + 1);
        }

        public void Insert(int index) {
            Add();

            // shift elements forwards
            for (int i = _array.Length - 1; i > index; --i) {
                _array[i] = _array[i - 1];
                _metadata.SetChild(i, _metadata.Enter(i - 1).Metadata);
            }

            // update the reference at index
            _array[index] = default(T);
            _metadata.SetChild(index, new fiGraphMetadata());
        }

        public void Duplicate(int index) {
            T current = _array[index];
            Insert(index);
            _array[index] = current;
        }

        public void Remove(int index) {
            for (int i = index; i < _array.Length - 1; ++i) {
                _array[i] = _array[i + 1];
                _metadata.SetChild(i, _metadata.Enter(i + 1).Metadata);
            }
            Array.Resize(ref _array, _array.Length - 1);
        }

        public void Move(int sourceIndex, int destIndex) {
            if (destIndex > sourceIndex)
                --destIndex;

            T item = _array[sourceIndex];
            fiGraphMetadata itemMetadata = _metadata.Enter(sourceIndex).Metadata;

            Remove(sourceIndex);
            Insert(destIndex);

            _array[destIndex] = item;
            _metadata.SetChild(destIndex, itemMetadata);
        }

        public void Clear() {
            _array = new T[0];
        }

        public virtual void DrawItem(Rect position, int index) {
            // Rotorz seems to sometimes give an index of -1, not sure why.
            if (index < 0) {
                return;
            }

            var metadata = _metadata.Enter(index);
            fiGraphMetadataCallbacks.ListMetadataCallback(metadata.Metadata, _array, index);
            _array[index] = _itemDrawer(position, _array[index], metadata);
        }

        public virtual float GetItemHeight(int index) {
            var metadata = _metadata.Enter(index);
            fiGraphMetadataCallbacks.ListMetadataCallback(metadata.Metadata, _array, index);
            return _itemHeight(_array[index], metadata);
        }
    }
}