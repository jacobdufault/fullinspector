using UnityEngine;

namespace FullInspector.LayoutToolkit {
    /// <summary>
    /// Centers the layout rule within it vertically.
    /// </summary>
    public class fiCenterVertical : fiLayout {
        private string _id;
        private fiLayout _centered;

        public fiCenterVertical(string id, fiLayout centered) {
            _id = id;
            _centered = centered;
        }

        public fiCenterVertical(fiLayout centered)
            : this(string.Empty, centered) {
        }

        public override bool RespondsTo(string sectionId) {
            return _id == sectionId || _centered.RespondsTo(sectionId);
        }

        public override Rect GetSectionRect(string sectionId, Rect initial) {
            float padding = initial.height - _centered.Height;
            initial.y += padding / 2;
            initial.height -= padding;

            initial = _centered.GetSectionRect(sectionId, initial);
            return initial;
        }

        public override float Height {
            get { return _centered.Height; }
        }
    }
}