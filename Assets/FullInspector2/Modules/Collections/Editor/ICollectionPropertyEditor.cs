using System;
using System.Collections.Generic;
using System.Reflection;
using FullInspector.Rotorz.ReorderableList;

namespace FullInspector.Internal {
    [CustomPropertyEditor(typeof(ICollection<>), Inherit = true)]
    public class ICollectionPropertyEditor<TActual, TItem> : BaseCollectionPropertyEditor<TActual, ICollection<TItem>, TItem, TItem> {
        public ICollectionPropertyEditor(Type editedType, ICustomAttributeProvider attributes)
            : base(editedType, attributes) {
        }

        protected override IReorderableListAdaptor GetAdaptor(ICollection<TItem> collection, fiGraphMetadata metadata) {
            return new CollectionAdaptor<TItem>(collection, DrawItem, GetItemHeight, metadata);
        }

        protected override bool AllowReordering {
            get { return false; }
        }
    }
}