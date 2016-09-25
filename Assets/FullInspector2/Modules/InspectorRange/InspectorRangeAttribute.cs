using System;

namespace FullInspector {
    /// <summary>
    /// Keep a numeric value within the given min/max range, with an optional step.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class InspectorRangeAttribute : Attribute {
        /// <summary>
        /// The minimum value.
        /// </summary>
        public float Min;

        /// <summary>
        /// The maximum value.
        /// </summary>
        public float Max;

        /// <summary>
        /// The step to use. This is optional.
        /// </summary>
        public float Step = float.NaN;

        public InspectorRangeAttribute(float min, float max) {
            Min = min;
            Max = max;
        }
    }
}