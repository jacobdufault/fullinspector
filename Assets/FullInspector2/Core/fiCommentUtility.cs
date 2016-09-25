using System;
using UnityEngine;

namespace FullInspector.Internal {
    /// <summary>
    /// Contains some utility functions that are useful when drawing the GUI for CommentAttributes.
    /// </summary>
    public static class fiCommentUtility {
        /// <summary>
        /// Returns the height of the given comment.
        /// </summary>
        public static int GetCommentHeight(string comment, CommentType commentType) {
            int minHeight = 38;
            if (commentType == CommentType.None) minHeight = 17;

            GUIStyle style = "HelpBox";
            return Math.Max(
                (int)style.CalcHeight(new GUIContent(comment), Screen.width),
                minHeight);
        }
    }
}