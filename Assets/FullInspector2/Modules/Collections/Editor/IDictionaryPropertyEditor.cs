using System;
using System.Collections.Generic;
using System.Reflection;
using FullInspector;
using FullInspector.Internal;
using FullInspector.Rotorz.ReorderableList;
using UnityEngine;

namespace FullSerializer.Internal {
    // special one for dictionaries so we only edit the key in the add region
    [CustomPropertyEditor(typeof(IDictionary<,>), Inherit = true)]
    public class IDictionaryPropertyEditor<TActual, TKey, TValue> : BaseCollectionPropertyEditor<TActual, IDictionary<TKey, TValue>, KeyValuePair<TKey, TValue>, TKey> {
        public IDictionaryPropertyEditor(Type editedType, ICustomAttributeProvider attributes)
            : base(editedType, attributes) {
        }

        protected override IReorderableListAdaptor GetAdaptor(IDictionary<TKey, TValue> collection, fiGraphMetadata metadata) {
            return new CollectionAdaptor<KeyValuePair<TKey, TValue>>(collection, DrawItem, GetItemHeight, metadata);
        }

        protected override void AddItemToCollection(TKey item, ref IDictionary<TKey, TValue> collection, IReorderableListAdaptor adaptor0) {
            try {
                if (!collection.ContainsKey(item)) {
                    var adaptor = adaptor0 as CollectionAdaptor<KeyValuePair<TKey, TValue>>;
                    if (adaptor == null && adaptor0 is PageAdaptor) {
                        PageAdaptor pageAdaptor = (PageAdaptor)adaptor0;
                        adaptor = (CollectionAdaptor<KeyValuePair<TKey, TValue>>)pageAdaptor.BackingAdaptor;
                    }

                    adaptor.Add(new KeyValuePair<TKey, TValue>(item, default(TValue)));
                }
            }
            catch (Exception) {
                if (ReferenceEquals(item, null)) {
                    Debug.LogError("Unable to add null keys to dictionaries; please select an instance first.");
                    return;
                }

                throw;
            }
        }

        protected override bool AllowReordering {
            get { return false; }
        }
    }
}