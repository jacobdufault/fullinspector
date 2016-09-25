using System;
using FullInspector.Rotorz.ReorderableList;
using UnityEngine;

namespace FullInspector.Internal {
    public class PageAdaptor : IReorderableListAdaptor {
        public readonly IReorderableListAdaptor BackingAdaptor;
        private int _startIndex;
        private int _endIndex;

        public PageAdaptor(IReorderableListAdaptor backingAdaptor, int startIndex, int endIndex) {
            BackingAdaptor = backingAdaptor;
            _startIndex = startIndex;
            _endIndex = endIndex;
        }

        public int Count {
            get {
                return Math.Min(BackingAdaptor.Count, _endIndex - _startIndex + 1);
            }
        }

        private int MapIndex(int index) {
            return _startIndex + index;
        }

        public bool CanDrag(int index) {
            return BackingAdaptor.CanDrag(MapIndex(index));
        }

        public bool CanRemove(int index) {
            return BackingAdaptor.CanRemove(MapIndex(index));
        }

        public void Add() {
            BackingAdaptor.Add();
        }

        public void Insert(int index) {
            BackingAdaptor.Insert(MapIndex(index));
        }

        public void Duplicate(int index) {
            BackingAdaptor.Duplicate(MapIndex(index));
        }

        public void Remove(int index) {
            BackingAdaptor.Remove(MapIndex(index));
            _endIndex = Math.Min(_endIndex, BackingAdaptor.Count - 1);
        }

        public void Move(int sourceIndex, int destIndex) {
            BackingAdaptor.Move(MapIndex(sourceIndex), MapIndex(destIndex));
        }

        public void Clear() {
            _startIndex = 0;
            _endIndex = 0;
            BackingAdaptor.Clear();
        }

        public void DrawItem(Rect position, int index) {
            BackingAdaptor.DrawItem(position, MapIndex(index));
        }

        public float GetItemHeight(int index) {
            return BackingAdaptor.GetItemHeight(MapIndex(index));
        }
    }
}