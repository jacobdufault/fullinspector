using System;
using System.Collections.Generic;
using System.Reflection;
using FullInspector;
using FullInspector.Internal;
using FullInspector.Rotorz.ReorderableList;

namespace FullSerializer.Internal {
    [CustomPropertyEditor(typeof(IList<>), Inherit = true)]
    public class IListPropertyEditor<TActual, TItem> : BaseCollectionPropertyEditor<TActual, IList<TItem>, TItem, TItem> {
        public IListPropertyEditor(Type editedType, ICustomAttributeProvider attributes)
            : base(editedType, attributes) {
        }

        protected override IReorderableListAdaptor GetAdaptor(IList<TItem> collection, fiGraphMetadata metadata) {
            return new ListAdaptor<TItem>(collection, DrawItem, GetItemHeight, metadata);
        }

        protected override bool DisplayAddItemPreview {
            get { return false; }
        }
    }
}