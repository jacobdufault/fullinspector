using UnityEngine;

namespace FullInspector.LayoutToolkit {
    /// <summary>
    /// Forces the laid out rectangle to have a specific height.
    /// </summary>
    public class fiLayoutHeight : fiLayout {
        private string _id;
        private float _height;

        public fiLayoutHeight(float height) {
            _id = string.Empty;
            _height = height;
        }

        public fiLayoutHeight(string sectionId, float height) {
            _id = sectionId;
            _height = height;
        }

        public override bool RespondsTo(string sectionId) {
            return _id == sectionId;
        }

        public override Rect GetSectionRect(string sectionId, Rect initial) {
            initial.height = _height;
            return initial;
        }

        public void SetHeight(float height) {
            _height = height;
        }

        public override float Height {
            get { return _height; }
        }
    }
}