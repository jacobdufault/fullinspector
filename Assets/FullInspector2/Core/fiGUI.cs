using System.Collections.Generic;
using UnityEngine;

namespace FullInspector.Internal {
    public static class fiGUI {
        #region Label Width Utilities
        private static readonly List<float> s_regionWidths = new List<float>();
        private static readonly Stack<float> s_savedLabelWidths = new Stack<float>();

        /// <summary>
        /// Computes the new width that labels should be.
        /// </summary>
        /// <param name="controlLabel">The label for the control. The width will be able to contain the label.</param>
        /// <param name="controlWidth">The total width available to the control.</param>
        /// <returns>The width for the controlLabel.</returns>
        public static float PushLabelWidth(GUIContent controlLabel, float controlWidth) {
            s_regionWidths.Add(controlWidth);
            s_savedLabelWidths.Push(controlWidth);

            return ComputeActualLabelWidth(s_regionWidths[0], controlLabel, controlWidth);
        }

        /// <summary>
        /// Removes a stored label width.
        /// </summary>
        /// <returns>The previous label width before it was pushed.</returns>
        public static float PopLabelWidth() {
            s_regionWidths.RemoveAt(s_regionWidths.Count - 1);
            return s_savedLabelWidths.Pop();
        }

        /// <summary>
        /// Gets the width of a label.
        /// </summary>
        /// <param name="inspectorWidth">The total width of the inspector</param>
        /// <param name="controlLabel">The label for the control. This will ensure that the returned width can contain the entire label.</param>
        /// <param name="controlWidth">The total width available to the control.</param>
        /// <returns>The width of the label, with respect to the available width in the control.</returns>
        public static float ComputeActualLabelWidth(float inspectorWidth, GUIContent controlLabel, float controlWidth) {
            float deadSpace = inspectorWidth - controlWidth;
            float targetLabelWidth = Mathf.Max(inspectorWidth * fiSettings.LabelWidthPercentage - fiSettings.LabelWidthOffset, 120);
            float labelWidth = targetLabelWidth - deadSpace;

            var minLabelWidth = Mathf.Max(fiLateBindings.EditorStyles.label.CalcSize(controlLabel).x, fiSettings.LabelWidthMin);
            labelWidth = Mathf.Clamp(labelWidth, minLabelWidth, fiSettings.LabelWidthMax);
            return labelWidth;
        }
        #endregion
    }
}