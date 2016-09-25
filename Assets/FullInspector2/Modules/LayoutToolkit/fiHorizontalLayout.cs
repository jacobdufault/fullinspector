using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullInspector.LayoutToolkit {
    /// <summary>
    /// Specifies how a layout item in the horizontal layout will be expanded.
    /// </summary>
    public enum fiExpandMode {
        /// <summary>
        /// The item does not have a fixed with and should expand to fill extra space.
        /// </summary>
        Expand,

        /// <summary>
        /// The item has a fixed width and should not be expanded.
        /// </summary>
        Fixed
    }

    /// <summary>
    /// Does a horizontal layout similar to GUILayout.Horizontal.
    /// </summary>
    public class fiHorizontalLayout : fiLayout, IEnumerable {
        private struct SectionItem {
            public string Id;

            public float MinWidth;
            public fiExpandMode ExpandMode;
            public fiLayout Rule;
        }

        private List<SectionItem> _items = new List<SectionItem>();
        private fiLayout _defaultRule = new fiVerticalLayout();

        public fiHorizontalLayout() { }
        public fiHorizontalLayout(fiLayout defaultRule) {
            _defaultRule = defaultRule;
        }

        /// <summary>
        /// Create an rule with auto width.
        /// </summary>
        public void Add(fiLayout rule) {
            ActualAdd(string.Empty, 0, fiExpandMode.Expand, rule);
        }

        /// <summary>
        /// Create a divider.
        /// </summary>
        public void Add(float width) {
            ActualAdd(string.Empty, width, fiExpandMode.Fixed, _defaultRule);
        }

        /// <summary>
        /// Create a label with auto width and the default height.
        /// </summary>
        public void Add(string id) {
            ActualAdd(id, 0, fiExpandMode.Expand, _defaultRule);
        }

        /// <summary>
        /// Create a label with a specific width.
        /// </summary>
        public void Add(string id, float width) {
            ActualAdd(id, width, fiExpandMode.Fixed, _defaultRule);
        }

        /// <summary>
        /// Create a labeled rule that has auto width.
        /// </summary>
        public void Add(string id, fiLayout rule) {
            ActualAdd(id, 0, fiExpandMode.Expand, rule);
        }

        /// <summary>
        /// Create a rule with the specific width.
        /// </summary>
        public void Add(float width, fiLayout rule) {
            ActualAdd(string.Empty, width, fiExpandMode.Fixed, rule);
        }

        /// <summary>
        /// Create a rule with the specified width and label.
        /// </summary>
        public void Add(string id, float width, fiLayout rule) {
            ActualAdd(id, width, fiExpandMode.Fixed, rule);
        }

        private void ActualAdd(string id, float width, fiExpandMode expandMode, fiLayout rule) {
            _items.Add(new SectionItem {
                Id = id,
                MinWidth = width,
                ExpandMode = expandMode,
                Rule = rule
            });
        }

        /// <summary>
        /// Finds the number of items in the layout which want to expand.
        /// </summary>
        private int ExpandCount {
            get {
                int expandCount = 0;
                for (int i = 0; i < _items.Count; ++i) {
                    if (_items[i].ExpandMode == fiExpandMode.Expand) {
                        expandCount += 1;
                    }
                }
                if (expandCount == 0) {
                    expandCount = 1;
                }
                return expandCount;
            }
        }

        /// <summary>
        /// Finds the minimum width required by the layout.
        /// </summary>
        private float MinimumWidth {
            get {
                float minimumWidth = 0;
                for (int i = 0; i < _items.Count; ++i) {
                    minimumWidth += _items[i].MinWidth;
                }
                return minimumWidth;
            }
        }

        public override Rect GetSectionRect(string sectionId, Rect initial) {
            float emptyWidth = initial.width - MinimumWidth;
            if (emptyWidth < 0) {
                emptyWidth = 0;
            }

            float expandRatio = 1.0f / ExpandCount;

            for (int i = 0; i < _items.Count; ++i) {
                SectionItem item = _items[i];

                float width = item.MinWidth;
                if (item.ExpandMode == fiExpandMode.Expand) {
                    width += emptyWidth * expandRatio;
                }

                if (item.Id == sectionId || item.Rule.RespondsTo(sectionId)) {
                    initial.width = width;
                    initial = item.Rule.GetSectionRect(sectionId, initial);

                    break;
                }

                initial.x += width;
            }

            return initial;
        }

        public override bool RespondsTo(string sectionId) {
            for (int i = 0; i < _items.Count; ++i) {
                if (_items[i].Id == sectionId || _items[i].Rule.RespondsTo(sectionId)) {
                    return true;
                }
            }

            return false;
        }

        public override float Height {
            get {
                float height = 0;

                for (int i = 0; i < _items.Count; ++i) {
                    height = Math.Max(height, _items[i].Rule.Height);
                }

                return height;
            }
        }

        // We only implement this so that the inline add syntax works
        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotSupportedException();
        }
    }
}