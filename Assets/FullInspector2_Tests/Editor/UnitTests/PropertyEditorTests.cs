using System;
using System.Collections.Generic;
using FullInspector.Internal;
using FullInspector.Modules;
using NUnit.Framework;
using UnityEngine;

namespace FullInspector.Tests {
    public class PropertyEditorTests {
        [Test]
        public void ArrayTest() {
            Assert.IsInstanceOf<ArrayPropertyEditor<int>>(PropertyEditor.Get(typeof(int[]), null).FirstEditor);
            Assert.IsInstanceOf<ArrayPropertyEditor<object>>(PropertyEditor.Get(typeof(object[]), null).FirstEditor);
            Assert.IsInstanceOf<ArrayPropertyEditor<List<int>>>(PropertyEditor.Get(typeof(List<int>[]), null).FirstEditor);
        }

        [Test]
        public void EnumTest() {
            Assert.IsInstanceOf<EnumPropertyEditor>(PropertyEditor.Get(typeof(CustomEnum), null).FirstEditor);
        }

        [Test]
        public void ComponentTest() {
            ObjectPropertyEditor<Transform> oo = new ObjectPropertyEditor<Transform>();
            Assert.IsTrue(oo.CanEdit(typeof(Transform)));

            Assert.IsInstanceOf<ObjectPropertyEditor<Transform>>(PropertyEditor.Get(typeof(Transform), null).FirstEditor);
        }

#if false // TODO: Renable
        [Test]
        public void InheritedPropertyEditorTest() {
            Assert.IsInstanceOf<IListPropertyEditor<List<int>, int>>(PropertyEditor.Get(typeof(List<int>), null).FirstEditor);
            Assert.IsInstanceOf<IListPropertyEditor<CustomList, int>>(PropertyEditor.Get(typeof(CustomList), null).FirstEditor);
            Assert.IsInstanceOf<IListPropertyEditor<CustomList<int>, int>>(PropertyEditor.Get(typeof(CustomList<int>), null).FirstEditor);
            Assert.IsInstanceOf<IListPropertyEditor<CustomList<string>, string>>(PropertyEditor.Get(typeof(CustomList<string>), null).FirstEditor);
        }
#endif

        [Test]
        public void OverrideInheritedPropertyEditor() {
            Assert.IsInstanceOf<OverridenListPropertyEditor>(PropertyEditor.Get(typeof(OverriddenList), null).FirstEditor);
            Assert.IsInstanceOf<OverridenListPropertyEditor<string>>(PropertyEditor.Get(typeof(OverriddenList<string>), null).FirstEditor);
        }

        #region Types for Testing
        private enum CustomEnum { }
        private class CustomList : IList<int> {
            public int IndexOf(int item) {
                throw new NotImplementedException();
            }

            public void Insert(int index, int item) {
                throw new NotImplementedException();
            }

            public void RemoveAt(int index) {
                throw new NotImplementedException();
            }

            public int this[int index] {
                get {
                    throw new NotImplementedException();
                }
                set {
                    throw new NotImplementedException();
                }
            }

            public void Add(int item) {
                throw new NotImplementedException();
            }

            public void Clear() {
                throw new NotImplementedException();
            }

            public bool Contains(int item) {
                throw new NotImplementedException();
            }

            public void CopyTo(int[] array, int arrayIndex) {
                throw new NotImplementedException();
            }

            public int Count {
                get { throw new NotImplementedException(); }
            }

            public bool IsReadOnly {
                get { throw new NotImplementedException(); }
            }

            public bool Remove(int item) {
                throw new NotImplementedException();
            }

            public IEnumerator<int> GetEnumerator() {
                throw new NotImplementedException();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
                throw new NotImplementedException();
            }
        }
        private class CustomList<T> : IList<T> {

            public int IndexOf(T item) {
                throw new NotImplementedException();
            }

            public void Insert(int index, T item) {
                throw new NotImplementedException();
            }

            public void RemoveAt(int index) {
                throw new NotImplementedException();
            }

            public T this[int index] {
                get {
                    throw new NotImplementedException();
                }
                set {
                    throw new NotImplementedException();
                }
            }

            public void Add(T item) {
                throw new NotImplementedException();
            }

            public void Clear() {
                throw new NotImplementedException();
            }

            public bool Contains(T item) {
                throw new NotImplementedException();
            }

            public void CopyTo(T[] array, int arrayIndex) {
                throw new NotImplementedException();
            }

            public int Count {
                get { throw new NotImplementedException(); }
            }

            public bool IsReadOnly {
                get { throw new NotImplementedException(); }
            }

            public bool Remove(T item) {
                throw new NotImplementedException();
            }

            public IEnumerator<T> GetEnumerator() {
                throw new NotImplementedException();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
                throw new NotImplementedException();
            }
        }

        private class OverriddenList : CustomList { }
        private class OverriddenList<T> : CustomList<T> { }

        [CustomPropertyEditor(typeof(OverriddenList))]
        private class OverridenListPropertyEditor : PropertyEditor<OverriddenList> {
            public override OverriddenList Edit(Rect region, GUIContent label, OverriddenList element, fiGraphMetadata metadata) {
                throw new NotImplementedException();
            }

            public override float GetElementHeight(GUIContent label, OverriddenList element, fiGraphMetadata metadata) {
                throw new NotImplementedException();
            }
        }

        [CustomPropertyEditor(typeof(OverriddenList<>))]
        private class OverridenListPropertyEditor<T> : PropertyEditor<OverriddenList<T>> {
            public override OverriddenList<T> Edit(Rect region, GUIContent label, OverriddenList<T> element, fiGraphMetadata metadata) {
                throw new NotImplementedException();
            }

            public override float GetElementHeight(GUIContent label, OverriddenList<T> element, fiGraphMetadata metadata) {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}