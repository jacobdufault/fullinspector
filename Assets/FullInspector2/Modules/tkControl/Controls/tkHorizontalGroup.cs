using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullInspector {
    public partial class tk<T, TContext> {
        /// <summary>
        /// Does a horizontal layout similar to GUILayout.Horizontal.
        /// </summary>
        public class HorizontalGroup : tkControl<T, TContext>, IEnumerable {
            private struct SectionItem
#if UNITY_EDITOR
            : tkCustomEditor
#endif
				{
                // We wrap _minWidth and _fillStrength into properties so that
                // we can verify their values when the user modifies them in the
                // inspector

                private float _minWidth;
                [ShowInInspector]
                public float MinWidth {
                    get { return _minWidth; }
                    set { _minWidth = Math.Max(value, 0); }
                }

                private float _fillStrength;
                [ShowInInspector]
                public float FillStrength {
                    get { return _fillStrength; }
                    set { _fillStrength = Math.Max(value, 0); }
                }

                /// <summary>
                /// Should this layout item match the height of the rect passed into the horizontal group?
                /// If this is true, then the subrect will not have its height trimmed. Note that for the
                /// total group height calculation, this is not used -- this means that the minimum height
                /// passed to a control will always be >= its requested height, just when MatchParentHeight
                /// is true the height passed into Edit may be > than the height requested from GetHeight.
                /// </summary>
                public bool MatchParentHeight;

                public tkControl<T, TContext> Rule;

                // The following variables are used inside of the layout logic.
                public bool Layout_IsFlexible;
                public float Layout_FlexibleWidth;
                public float Layout_Width {
                    get {
                        if (Layout_IsFlexible) return Layout_FlexibleWidth;
                        return MinWidth;
                    }
                }

                // TODO: Support DLL-based builds for this (small) feature
#if UNITY_EDITOR
                public tkControlEditor GetEditor() {
                    return new tkControlEditor(
                        new tk<SectionItem, tkDefaultContext>.VerticalGroup {
                        new tk<SectionItem, tkDefaultContext>.PropertyEditor("MinWidth"),
                        new tk<SectionItem, tkDefaultContext>.PropertyEditor("FillStrength"),
                        new tk<SectionItem, tkDefaultContext>.PropertyEditor("MatchParentHeight"),

                        new tk<SectionItem, tkDefaultContext>.StyleProxy {
                            Style = new tk<SectionItem, tkDefaultContext>.ReadOnly(),
                            Control =
                                new tk<SectionItem, tkDefaultContext>.VerticalGroup {
                                    new tk<SectionItem, tkDefaultContext>.Label("Runtime Layout Information", FontStyle.Bold,
                                        new tk<SectionItem, tkDefaultContext>.VerticalGroup {
                                            new tk<SectionItem, tkDefaultContext>.HorizontalGroup {
                                                { 130, new tk<SectionItem, tkDefaultContext>.PropertyEditor("Flexible?", "Layout_IsFlexible") },
                                                new tk<SectionItem, tkDefaultContext>.PropertyEditor("FlexWidth", "Layout_FlexibleWidth")
                                            },
                                            new tk<SectionItem, tkDefaultContext>.PropertyEditor("Used Width", "Layout_Width")
                                        })
                            }
                        },

                        new tk<SectionItem, tkDefaultContext>.PropertyEditor("Rule")
                    });
                }
#endif
            }

            [ShowInInspector]
            private readonly List<SectionItem> _items = new List<SectionItem>();

            private static readonly tkControl<T, TContext> DefaultRule = new VerticalGroup();

            protected override IEnumerable<tkIControl> NonMemberChildControls {
                get {
                    foreach (var item in _items) {
                        yield return item.Rule;
                    }
                }
            }

            /// <summary>
            /// Create an rule with auto width.
            /// </summary>
            public void Add(tkControl<T, TContext> rule) {
                InternalAdd(false, 0, 1, rule);
            }

            /// <summary>
            /// Create a rule with auto width that can control if it matches the parent height.
            /// </summary>
            /// <param name="matchParentHeight">If true, then the height of the rect passed to the
            /// rule will be equal to the height of the overall rect passed to this horizontal group.</param>
            public void Add(bool matchParentHeight, tkControl<T, TContext> rule) {
                InternalAdd(matchParentHeight, 0, 1, rule);
            }

            /// <summary>
            /// Create a divider.
            /// </summary>
            public void Add(float width) {
                InternalAdd(false, width, 0, DefaultRule);
            }

            /// <summary>
            /// Create a rule with the specific width.
            /// </summary>
            public void Add(float width, tkControl<T, TContext> rule) {
                InternalAdd(false, width, 0, rule);
            }

            private void InternalAdd(bool matchParentHeight, float width, float fillStrength, tkControl<T, TContext> rule) {
                if (width < 0) throw new ArgumentException("width must be >= 0");
                if (fillStrength < 0) throw new ArgumentException("fillStrength must be >= 0");

                _items.Add(new SectionItem {
                    MatchParentHeight = matchParentHeight,
                    MinWidth = width,
                    FillStrength = fillStrength,
                    Rule = rule
                });
            }

            private void DoLayout(Rect rect, T obj, TContext context, fiGraphMetadata metadata) {
                // The layout algorithm is relatively simple once you understand what is going on.
                // It can run up to N times where N is the number of items that have a fill strength > 0.
                //
                // The idea is simple: we initially treat every item that wants to be flexible as flexible. If
                // that item has a minimum width and it will not be met using the flexible layout logic, then
                // we rerun the layout except that item is treated as a fixed-width item instead.
                //
                // We complete the algorithm when every item has met its minimum width requirement.

                // step1: Try to treat every item that wants to be flexible as flexible.
                for (int i = 0; i < _items.Count; ++i) {
                    SectionItem item = _items[i];
                    item.Layout_IsFlexible = item.FillStrength > 0;
                    _items[i] = item;
                }

                // step2..n: Iterate until each item has a width greater than its minimum width.
                while (true) {
                tryAgain:

                    float requiredMinSpace = 0;
                    float totalFillStrength = 0;

                    for (int i = 0; i < _items.Count; ++i) {
                        var item = _items[i];
                        if (item.Rule.ShouldShow(obj, context, metadata) == false) continue;

                        if (item.Layout_IsFlexible) {
                            totalFillStrength += item.FillStrength;
                        }
                        else {
                            requiredMinSpace += item.MinWidth;
                        }
                    }


                    float growableSpace = rect.width - requiredMinSpace;


                    for (int i = 0; i < _items.Count; ++i) {
                        var item = _items[i];
                        if (item.Rule.ShouldShow(obj, context, metadata) == false) continue;

                        if (item.Layout_IsFlexible) {
                            item.Layout_FlexibleWidth = growableSpace * item.FillStrength / totalFillStrength;
                            _items[i] = item;

                            // There is not enough flexible room for this item; try again but with this item at
                            // its fixed width. We have to rerun the entire algorithm because setting an item
                            // to fixed width can alter how wide flexible items before us are.
                            if (item.Layout_FlexibleWidth < item.MinWidth) {
                                item.Layout_IsFlexible = false;
                                _items[i] = item;
                                goto tryAgain;
                            }
                        }
                    }

                    break;
                }
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata) {
                DoLayout(rect, obj, context, metadata);

                for (int i = 0; i < _items.Count; ++i) {
                    var item = _items[i];
                    if (item.Rule.ShouldShow(obj, context, metadata) == false) continue;

                    float width = item.Layout_Width;

                    Rect itemRect = rect;
                    itemRect.width = width;

                    // If we're not matching the parent height then manually trim the rects height
                    // so the layout gets a rect equal to the height it requests.
                    if (item.MatchParentHeight == false) {
                        itemRect.height = item.Rule.GetHeight(obj, context, metadata);
                    }

                    obj = item.Rule.Edit(itemRect, obj, context, metadata);

                    rect.x += width;
                }

                return obj;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata) {
                float height = 0;

                for (int i = 0; i < _items.Count; ++i) {
                    var item = _items[i];
                    if (item.Rule.ShouldShow(obj, context, metadata) == false) continue;

                    height = Math.Max(height, item.Rule.GetHeight(obj, context, metadata));
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