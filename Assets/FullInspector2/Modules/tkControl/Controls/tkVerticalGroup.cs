using System;
using System.Collections;
using System.Collections.Generic;
using FullInspector.Internal;
using UnityEngine;

namespace FullInspector {
    public partial class tk<T, TContext> {
        /// <summary>
        /// Lays out items vertically, similar to GUILayout.BeginVertical.
        /// </summary>
        public class VerticalGroup : tkControl<T, TContext>, IEnumerable {
            private struct SectionItem {
                public tkControl<T, TContext> Rule;
            }

            [ShowInInspector]
            private readonly List<SectionItem> _items = new List<SectionItem>();
            private readonly float _marginBetweenItems;

            public VerticalGroup()
                : this(fiLateBindings.EditorGUIUtility.standardVerticalSpacing) {
            }

            public VerticalGroup(float marginBetweenItems) {
                _marginBetweenItems = marginBetweenItems;
            }

            protected override IEnumerable<tkIControl> NonMemberChildControls {
                get {
                    foreach (var item in _items) {
                        yield return item.Rule;
                    }
                }
            }

            public void Add(tkControl<T, TContext> rule) {
                InternalAdd(rule);
            }

            private void InternalAdd(tkControl<T, TContext> rule) {
                _items.Add(new SectionItem {
                    Rule = rule
                });
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata) {
                for (int i = 0; i < _items.Count; ++i) {
                    SectionItem item = _items[i];
                    if (item.Rule.ShouldShow(obj, context, metadata) == false) continue;

                    var height = item.Rule.GetHeight(obj, context, metadata);

                    var itemRect = rect;
                    itemRect.height = height;
                    obj = item.Rule.Edit(itemRect, obj, context, metadata);

                    rect.y += height;
                    rect.y += _marginBetweenItems;
                }

                return obj;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata) {
                float height = 0;

                for (int i = 0; i < _items.Count; ++i) {
                    var item = _items[i];
                    if (item.Rule.ShouldShow(obj, context, metadata) == false) continue;

                    height += item.Rule.GetHeight(obj, context, metadata);

                    // no margin after the last item
                    if (i != _items.Count - 1) {
                        height += _marginBetweenItems;
                    }
                }

                return height;
            }

            IEnumerator IEnumerable.GetEnumerator() {
                // We only implement this so that the inline add syntax works
                throw new NotSupportedException();
            }
        }
    }
}