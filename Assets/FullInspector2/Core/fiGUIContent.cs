using UnityEngine;

namespace FullInspector {
    /// <summary>
    /// This is a wrapper that will generate Unity GUIContent instances. This
    /// class has an implicit conversion from a string so that it is easy to
    /// easily construct GUIContents that just have a label.
    /// </summary>
    public class fiGUIContent {
        public static fiGUIContent Empty = new fiGUIContent();

        private string _text;
        private string _tooltip;
        private Texture _image;

        public fiGUIContent()
            : this("", "", null) {
        }

        public fiGUIContent(string text)
            : this(text, "", null) {
        }

        public fiGUIContent(string text, string tooltip)
            : this(text, tooltip, null) {
        }

        public fiGUIContent(string text, string tooltip, Texture image) {
            _text = text;
            _tooltip = tooltip;
            _image = image;
        }

        public fiGUIContent(Texture image)
            : this("", "", image) {
        }

        public fiGUIContent(Texture image, string tooltip)
            : this("", tooltip, image) {
        }

        public GUIContent AsGUIContent {
            get {
                return new GUIContent(_text, _image, _tooltip);
            }
        }

        public bool IsEmpty {
            get {
                if (string.IsNullOrEmpty(_text) == false) return false;
                if (string.IsNullOrEmpty(_tooltip) == false) return false;
                if (_image != null) return false;

                return true;
            }
        }

        public static implicit operator GUIContent(fiGUIContent label) {
            if (label == null) return GUIContent.none;
            return label.AsGUIContent;
        }

        public static implicit operator fiGUIContent(string text) {
            return new fiGUIContent {
                _text = text
            };
        }

        public static implicit operator fiGUIContent(GUIContent label) {
            return new fiGUIContent {
                _text = label.text,
                _tooltip = label.tooltip,
                _image = label.image
            };
        }
    }
}