using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FullInspector.Internal {
    public class StackCollection<T> : ICollection<T> {
        public Stack<T> Stack;

        public StackCollection(Stack<T> stack) {
            Stack = stack;
        }

        public int Count {
            get {
                return Stack.Count;
            }
        }

        public bool IsReadOnly {
            get {
                throw new NotSupportedException();
            }
        }

        public void Add(T item) {
            Stack.Push(item);
        }

        public void Clear() {
            Stack.Clear();
        }

        public bool Contains(T item) {
            return Stack.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            Stack.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item) {
            var removed = new Stack<T>();
            bool found = false;

            while (Stack.Count > 0) {
                var removedItem = Stack.Pop();

                if (EqualityComparer<T>.Default.Equals(removedItem, item)) {
                    found = true;
                    break;
                }

                removed.Push(removedItem);
            }

            foreach (var i in removed) {
                Stack.Push(i);
            }
            return found;
        }

        public IEnumerator<T> GetEnumerator() {
            return Stack.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return Stack.GetEnumerator();
        }
    }

    [CustomPropertyEditor(typeof(Stack<>), Inherit = true)]
    public class StackPropertyEditor<TStack, T> : PropertyEditor<TStack>
        where TStack : Stack<T> {

        private ICollectionPropertyEditor<TStack, T> ActualEditor;

        public StackPropertyEditor(Type editedType, ICustomAttributeProvider attribute) {
            ActualEditor = new ICollectionPropertyEditor<TStack, T>(editedType, attribute);
        }

        public override TStack Edit(Rect region, GUIContent label, TStack element, fiGraphMetadata metadata) {
            var collection = new StackCollection<T>(element);
            collection = (StackCollection<T>)ActualEditor.Edit(region, label, collection, metadata);
            return (TStack)collection.Stack;
        }

        public override float GetElementHeight(GUIContent label, TStack element, fiGraphMetadata metadata) {
            var collection = new StackCollection<T>(element);
            return ActualEditor.GetElementHeight(label, collection, metadata);
        }

        public override GUIContent GetFoldoutHeader(GUIContent label, object element) {
            return ActualEditor.GetFoldoutHeader(label, new StackCollection<T>((Stack<T>)element));
        }

        public override bool CanEdit(Type type) {
            return ActualEditor.CanEdit(typeof(StackCollection<>).MakeGenericType(typeof(T)));
        }

        public override bool DisplaysStandardLabel {
            get {
                return ActualEditor.DisplaysStandardLabel;
            }
        }

        public override TStack OnSceneGUI(TStack element) {
            var collection = new StackCollection<T>(element);
            collection = (StackCollection<T>)ActualEditor.OnSceneGUI(collection);
            return (TStack)collection.Stack;
        }
    }
}