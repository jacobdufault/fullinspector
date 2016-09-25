using UnityEngine;

namespace FullInspector.LayoutToolkit {
    /// <summary>
    /// A rule in the layout toolkit. A rule defines how some layout operation will occur. The
    /// primary rules are the horizontal and vertical rules, which function similar to Unity's
    /// GUILayout.BeginHorizontal and BeginVertical. However, rules are much more flexible;
    /// there is also a rule for centering items vertically, among other things.
    /// </summary>
    public abstract class fiLayout {
        /// <summary>
        /// Does this layout rule respond to the given section identifier?
        /// </summary>
        public abstract bool RespondsTo(string id);

        /// <summary>
        /// Return the rect for the given section identifier.
        /// </summary>
        public abstract Rect GetSectionRect(string id, Rect initial);

        /// <summary>
        /// Return the height of this layout.
        /// </summary>
        public abstract float Height {
            get;
        }
    }
}