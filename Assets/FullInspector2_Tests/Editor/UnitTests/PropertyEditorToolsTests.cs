//#define enabled
#if enabled

using FullInspector.Internal;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FullInspector.Tests {
    public class PropertyEditorToolsTests {
        public class BaseEditor : IPropertyEditor {
            public object Edit(Rect region, GUIContent label, object element) {
                throw new NotImplementedException();
            }

            public float GetElementHeight(GUIContent label, object element) {
                throw new NotImplementedException();
            }

            public GUIContent GetFoldoutHeader(GUIContent label, object element) {
                throw new NotImplementedException();
            }

            public bool IsIgnored(MemberInfo type) {
                throw new NotImplementedException();
            }
        }

        public class Data : Iface2 { }
        public class Data<T1> : Iface2<T1> { }
        public class Data<T1, T2> : Iface2<T1, T2> { }
        public class Data<T1, T2, T3> : Iface2<T1, T2, T3> { }

        public class Iface1 { }
        public class Iface1<T1> { }
        public class Iface1<T1, T2> { }
        public class Iface1<T1, T2, T3> { }

        public class Iface2 : Iface1 { }
        public class Iface2<T1> : Iface1<T1> { }
        public class Iface2<T1, T2> : Iface1<T1, T2> { }
        public class Iface2<T1, T2, T3> : Iface1<T1, T2, T3> { }

        [CustomPropertyEditor(typeof(Data))]
        public class DataEditor : BaseEditor { }
        [CustomPropertyEditor(typeof(Data<>))]
        public class DataEditor<T1> : BaseEditor { }
        [CustomPropertyEditor(typeof(Data<,>))]
        public class DataEditor<T1, T2> : BaseEditor { }
        [CustomPropertyEditor(typeof(Data<,,>))]
        public class DataEditor<T1, T2, T3> : BaseEditor { }

        [CustomPropertyEditor(typeof(Data))]
        public class DataEditorIncludeType<TData> : BaseEditor { }
        [CustomPropertyEditor(typeof(Data<>))]
        public class DataEditorIncludeType<TData, T1> : BaseEditor { }
        [CustomPropertyEditor(typeof(Data<,>))]
        public class DataEditorIncludeType<TData, T1, T2> : BaseEditor { }
        [CustomPropertyEditor(typeof(Data<,,>))]
        public class DataEditorIncludeType<TData, T1, T2, T3> : BaseEditor { }

        [CustomPropertyEditor(typeof(Iface1), Inherit = true)]
        public class Iface1Editor : BaseEditor { }
        [CustomPropertyEditor(typeof(Iface1<>), Inherit = true)]
        public class Iface1Editor<T1> : BaseEditor { }
        [CustomPropertyEditor(typeof(Iface1<,>), Inherit = true)]
        public class Iface1Editor<T1, T2> : BaseEditor { }
        [CustomPropertyEditor(typeof(Iface1<,,>), Inherit = true)]
        public class Iface1Editor<T1, T2, T3> : BaseEditor { }

        [CustomPropertyEditor(typeof(Iface1), Inherit = true)]
        public class Iface1EditorIncludeType<TData> : BaseEditor { }
        [CustomPropertyEditor(typeof(Iface1<>), Inherit = true)]
        public class Iface1EditorIncludeType<TData, T1> : BaseEditor { }
        [CustomPropertyEditor(typeof(Iface1<,>), Inherit = true)]
        public class Iface1EditorIncludeType<TData, T1, T2> : BaseEditor { }
        [CustomPropertyEditor(typeof(Iface1<,,>), Inherit = true)]
        public class Iface1EditorIncludeType<TData, T1, T2, T3> : BaseEditor { }

        [CustomPropertyEditor(typeof(Iface2), Inherit = true)]
        public class Iface2Editor : BaseEditor { }
        [CustomPropertyEditor(typeof(Iface2<>), Inherit = true)]
        public class Iface2Editor<T1> : BaseEditor { }
        [CustomPropertyEditor(typeof(Iface2<,>), Inherit = true)]
        public class Iface2Editor<T1, T2> : BaseEditor { }
        [CustomPropertyEditor(typeof(Iface2<,,>), Inherit = true)]
        public class Iface2Editor<T1, T2, T3> : BaseEditor { }

        [CustomPropertyEditor(typeof(Iface2), Inherit = true)]
        public class Iface2EditorIncludeType<TData> : BaseEditor { }
        [CustomPropertyEditor(typeof(Iface2<>), Inherit = true)]
        public class Iface2EditorIncludeType<TData, T1> : BaseEditor { }
        [CustomPropertyEditor(typeof(Iface2<,>), Inherit = true)]
        public class Iface2EditorIncludeType<TData, T1, T2> : BaseEditor { }
        [CustomPropertyEditor(typeof(Iface2<,,>), Inherit = true)]
        public class Iface2EditorIncludeType<TData, T1, T2, T3> : BaseEditor { }

        [Test]
        public void TryCreateEditorTest() {
            Assert.IsInstanceOf<DataEditor>(PropertyEditorTools.TryCreateEditor(typeof(Data), typeof(DataEditor)));
            Assert.IsInstanceOf<DataEditor<int>>(PropertyEditorTools.TryCreateEditor(typeof(Data<int>), typeof(DataEditor<>)));
            Assert.IsInstanceOf<DataEditor<int, object>>(PropertyEditorTools.TryCreateEditor(typeof(Data<int, object>), typeof(DataEditor<,>)));
            Assert.IsInstanceOf<DataEditor<int, object, string>>(PropertyEditorTools.TryCreateEditor(typeof(Data<int, object, string>), typeof(DataEditor<,,>)));

            Assert.IsInstanceOf<DataEditorIncludeType<Data>>(PropertyEditorTools.TryCreateEditor(typeof(Data), typeof(DataEditorIncludeType<>)));
            Assert.IsInstanceOf<DataEditorIncludeType<Data<int>, int>>(PropertyEditorTools.TryCreateEditor(typeof(Data<int>), typeof(DataEditorIncludeType<,>)));
            Assert.IsInstanceOf<DataEditorIncludeType<Data<int, object>, int, object>>(PropertyEditorTools.TryCreateEditor(typeof(Data<int, object>), typeof(DataEditorIncludeType<,,>)));
            Assert.IsInstanceOf<DataEditorIncludeType<Data<int, object, string>, int, object, string>>(PropertyEditorTools.TryCreateEditor(typeof(Data<int, object, string>), typeof(DataEditorIncludeType<,,,>)));
        }

        [Test]
        public void TryCreateInheritedEditor() {
            Assert.IsInstanceOf<Iface1Editor>(PropertyEditorTools.TryCreateEditor(typeof(Data), typeof(Iface1Editor)));
            Assert.IsInstanceOf<Iface1Editor<int>>(PropertyEditorTools.TryCreateEditor(typeof(Data<int>), typeof(Iface1Editor<>)));
            Assert.IsInstanceOf<Iface1Editor<int, object>>(PropertyEditorTools.TryCreateEditor(typeof(Data<int, object>), typeof(Iface1Editor<,>)));
            Assert.IsInstanceOf<Iface1Editor<int, object, string>>(PropertyEditorTools.TryCreateEditor(typeof(Data<int, object, string>), typeof(Iface1Editor<,,>)));

            Assert.IsNull(PropertyEditorTools.TryCreateEditor(typeof(Data), typeof(Iface1Editor<>)));
            Assert.IsNotNull(PropertyEditorTools.TryCreateEditor(typeof(Data), typeof(Iface1EditorIncludeType<>)));

            Assert.IsInstanceOf<Iface1EditorIncludeType<Data>>(PropertyEditorTools.TryCreateEditor(typeof(Data), typeof(Iface1EditorIncludeType<>)));
            Assert.IsInstanceOf<Iface1EditorIncludeType<Data<int>, int>>(PropertyEditorTools.TryCreateEditor(typeof(Data<int>), typeof(Iface1EditorIncludeType<,>)));
            Assert.IsInstanceOf<Iface1EditorIncludeType<Data<int, object>, int, object>>(PropertyEditorTools.TryCreateEditor(typeof(Data<int, object>), typeof(Iface1EditorIncludeType<,,>)));
            Assert.IsInstanceOf<Iface1EditorIncludeType<Data<int, object, string>, int, object, string>>(PropertyEditorTools.TryCreateEditor(typeof(Data<int, object, string>), typeof(Iface1EditorIncludeType<,,,>)));

            Assert.IsInstanceOf<Iface2Editor>(PropertyEditorTools.TryCreateEditor(typeof(Data), typeof(Iface2Editor)));
            Assert.IsInstanceOf<Iface2Editor<int>>(PropertyEditorTools.TryCreateEditor(typeof(Data<int>), typeof(Iface2Editor<>)));
            Assert.IsInstanceOf<Iface2Editor<int, object>>(PropertyEditorTools.TryCreateEditor(typeof(Data<int, object>), typeof(Iface2Editor<,>)));
            Assert.IsInstanceOf<Iface2Editor<int, object, string>>(PropertyEditorTools.TryCreateEditor(typeof(Data<int, object, string>), typeof(Iface2Editor<,,>)));

            Assert.IsInstanceOf<Iface2EditorIncludeType<Data>>(PropertyEditorTools.TryCreateEditor(typeof(Data), typeof(Iface2EditorIncludeType<>)));
            Assert.IsInstanceOf<Iface2EditorIncludeType<Data<int>, int>>(PropertyEditorTools.TryCreateEditor(typeof(Data<int>), typeof(Iface2EditorIncludeType<,>)));
            Assert.IsInstanceOf<Iface2EditorIncludeType<Data<int, object>, int, object>>(PropertyEditorTools.TryCreateEditor(typeof(Data<int, object>), typeof(Iface2EditorIncludeType<,,>)));
            Assert.IsInstanceOf<Iface2EditorIncludeType<Data<int, object, string>, int, object, string>>(PropertyEditorTools.TryCreateEditor(typeof(Data<int, object, string>), typeof(Iface2EditorIncludeType<,,,>)));
        }

        [CustomPropertyEditor(typeof(object))]
        public class ObjectEditor : BaseEditor { }

        [CustomPropertyEditor(typeof(IList<>), Inherit = true)]
        public class IListEditor<TList, TData> : BaseEditor { }

        [Test]
        public void ListEditorTest() {
            Assert.IsInstanceOf<IListEditor<CustomList, int>>(PropertyEditorTools.TryCreateEditor(typeof(CustomList), typeof(IListPropertyEditor<,>)));
            Assert.IsInstanceOf<IListEditor<List<int>, int>>(PropertyEditorTools.TryCreateEditor(typeof(List<int>), typeof(IListEditor<,>)));
            Assert.IsNull(PropertyEditorTools.TryCreateEditor(typeof(int), typeof(IListEditor<,>)));

            Assert.IsInstanceOf<IListEditor<List<CustomList>, CustomList>>(PropertyEditorTools.TryCreateEditor(typeof(List<CustomList>), typeof(IListEditor<,>)));
            Assert.IsInstanceOf<IListEditor<List<List<int>>, List<int>>>(PropertyEditorTools.TryCreateEditor(typeof(List<List<int>>), typeof(IListEditor<,>)));
        }

        [Test]
        public void ObjectEditorTest() {
            Assert.IsInstanceOf<ObjectEditor>(PropertyEditorTools.TryCreateEditor(typeof(object), typeof(ObjectEditor)));
            Assert.IsNull(PropertyEditorTools.TryCreateEditor(typeof(int), typeof(ObjectEditor)));
        }

        internal class CustomList : IList<int> {
            public int IndexOf(int item) {
                throw new System.NotImplementedException();
            }

            public void Insert(int index, int item) {
                throw new System.NotImplementedException();
            }

            public void RemoveAt(int index) {
                throw new System.NotImplementedException();
            }

            public int this[int index] {
                get {
                    throw new System.NotImplementedException();
                }
                set {
                    throw new System.NotImplementedException();
                }
            }

            public void Add(int item) {
                throw new System.NotImplementedException();
            }

            public void Clear() {
                throw new System.NotImplementedException();
            }

            public bool Contains(int item) {
                throw new System.NotImplementedException();
            }

            public void CopyTo(int[] array, int arrayIndex) {
                throw new System.NotImplementedException();
            }

            public int Count {
                get { throw new System.NotImplementedException(); }
            }

            public bool IsReadOnly {
                get { throw new System.NotImplementedException(); }
            }

            public bool Remove(int item) {
                throw new System.NotImplementedException();
            }

            public IEnumerator<int> GetEnumerator() {
                throw new System.NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                throw new System.NotImplementedException();
            }
        }
    }
}

#endif