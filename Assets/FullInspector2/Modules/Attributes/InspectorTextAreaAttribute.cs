using System;

namespace FullInspector {
    /// <summary>
    /// Show a text-area instead of a text-field for a string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class InspectorTextAreaAttribute : Attribute {
        /// <summary>
        /// The height of the text area.
        /// </summary>
        public float Height;

        public int Lines {
            set { Height = value * 17; }
        }

        public InspectorTextAreaAttribute() : this(250) {
        }

        public InspectorTextAreaAttribute(float height) {
            Height = height;
        }
    }
}