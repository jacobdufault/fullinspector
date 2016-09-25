using System.Collections.Generic;
using FullInspector.Rotorz.ReorderableList;
using UnityEngine;

namespace FullInspector.Internal {
    /// <summary>
    /// Reorderable list adapter for generic list.
    /// </summary>
    /// <remarks>
    /// <para>This adapter can be subclassed to add special logic to item height calculation. You
    /// may want to implement a custom adapter class where specialized functionality is
    /// needed.</para>
    /// </remarks>
    public class ListAdaptor<T> : IReorderableListAdaptor {
        public delegate float ItemHeight(T item, fiGraphMetadataChild metadata);
        public delegate T ItemDrawer(Rect position, T item, fiGraphMetadataChild metadata);

        private ItemHeight _itemHeight;
        private ItemDrawer _itemDrawer;
        private fiGraphMetadata _metadata;
        public IList<T> List;

        private static T DefaultItemGenerator() {
            return default(T);
        }

        /// <param name="list">The list which can be reordered.</param>
        /// <param name="itemDrawer">Callback to draw list item.</param>
        /// <param name="itemHeight">Height of list item in pixels.</param>
        public ListAdaptor(IList<T> list, ItemDrawer itemDrawer, ItemHeight itemHeight, fiGraphMetadata metadata) {
            _metadata = metadata;
            List = list;
            _itemDrawer = itemDrawer;
            _itemHeight = itemHeight;
        }

        public int Count {
            get { return List.Count; }
        }
        public virtual bool CanDrag(int index) {
            return true;
        }
        public virtual bool CanRemove(int index) {
            return true;
        }
        public void Add() {
            T item = DefaultItemGenerator();
            List.Add(item);
        }
        public void Insert(int index) {
            Add();

            // shift metadata forwards
            for (int i = List.Count - 1; i > index; --i) {
                List[i] = List[i - 1];
                _metadata.SetChild(i, _metadata.Enter(i - 1).Metadata);
            }

            // update the reference at index
            List[index] = default(T);
            _metadata.SetChild(index, new fiGraphMetadata());
        }

        public void Duplicate(int index) {
            T current = List[index];
            Insert(index);
            List[index] = current;
        }
        public void Remove(int index) {
            // shift elements back
            for (int i = index; i < List.Count - 1; ++i) {
                _metadata.SetChild(i, _metadata.Enter(i + 1).Metadata);
            }
            List.RemoveAt(index);
        }
        public void Move(int srcIndex, int destIndex) {
            if (destIndex > srcIndex)
                --destIndex;

            // Swap the metadata.
            var srcMetadata = _metadata.Enter(srcIndex).Metadata;
            var destMetadata = _metadata.Enter(destIndex).Metadata;
            _metadata.SetChild(srcIndex, destMetadata);
            _metadata.SetChild(destIndex, srcMetadata);

            // Swap the items.
            T srcValue = List[srcIndex];
            T destValue = List[destIndex];
            List[srcIndex] = destValue;
            List[destIndex] = srcValue;
        }

        public void Clear() {
            List.Clear();
        }

        public virtual void DrawItem(Rect position, int index) {
            // Rotorz seems to sometimes give an index of -1, not sure why.
            if (index < 0) {
                return;
            }

            var metadata = _metadata.Enter(index);
            fiGraphMetadataCallbacks.ListMetadataCallback(metadata.Metadata, fiGraphMetadataCallbacks.Cast(List), index);

            var updatedItem = _itemDrawer(position, List[index], metadata);
            var existingItem = List[index];

            if (existingItem == null ||
                existingItem.Equals(updatedItem) == false) {
                List[index] = updatedItem;
            }
        }
        public virtual float GetItemHeight(int index) {
            var metadata = _metadata.Enter(index);
            fiGraphMetadataCallbacks.ListMetadataCallback(metadata.Metadata, fiGraphMetadataCallbacks.Cast(List), index);
            return _itemHeight(List[index], metadata);
        }
    }
}