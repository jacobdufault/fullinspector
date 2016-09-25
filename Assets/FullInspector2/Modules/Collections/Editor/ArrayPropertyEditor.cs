using FullInspector.Rotorz.ReorderableList;
using System;
using System.Reflection;

namespace FullInspector.Internal {
    public class ArrayPropertyEditor<T> : BaseCollectionPropertyEditor<T[], T[], T, T> {
        public ArrayPropertyEditor(Type editedType, ICustomAttributeProvider attributes)
            : base(editedType, attributes) {
        }

        protected override bool DisplayAddItemPreview {
            get { return false; }
        }

        protected override IReorderableListAdaptor GetAdaptor(T[] collection, fiGraphMetadata metadata) {
            return new ArrayAdaptor<T>(collection, DrawItem, GetItemHeight, metadata);
        }

        protected override void OnPostEdit(ref T[] collection, IReorderableListAdaptor adaptor) {
            collection = ((ArrayAdaptor<T>)adaptor).StoredArray;
        }
    }
}