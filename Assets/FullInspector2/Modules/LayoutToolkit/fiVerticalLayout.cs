using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullInspector.LayoutToolkit {
    /// <summary>
    /// Lays out items vertically, similar to GUILayout.BeginVertical.
    /// </summary>
    public class fiVerticalLayout : fiLayout, IEnumerable {
        private struct SectionItem {
            public string Id;
            public fiLayout Rule;
        }

        private List<SectionItem> _items = new List<SectionItem>();

        public void Add(fiLayout rule) {
            Add(string.Empty, rule);
        }

        public void Add(string sectionId, fiLayout rule) {
            _items.Add(new SectionItem {
                Id = sectionId,
                Rule = rule
            });
        }

        public void Add(string sectionId, float height) {
            Add(sectionId, new fiLayoutHeight(sectionId, height));
        }

        public void Add(float height) {
            Add(string.Empty, height);
        }

        public override Rect GetSectionRect(string sectionId, Rect initial) {
            for (int i = 0; i < _items.Count; ++i) {
                SectionItem item = _items[i];

                if (item.Id == sectionId || item.Rule.RespondsTo(sectionId)) {
                    if (item.Rule.RespondsTo(sectionId)) {
                        initial = item.Rule.GetSectionRect(sectionId, initial);
                    }
                    else {
                        initial.height = item.Rule.Height;
                    }
                    break;
                }

                initial.y += item.Rule.Height;
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
                    height += _items[i].Rule.Height;
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