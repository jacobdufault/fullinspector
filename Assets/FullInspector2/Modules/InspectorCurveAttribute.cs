using System;

namespace FullInspector {
    /// <summary>
    /// Keep a numeric value within the given min/max range, with an optional
    /// step.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class InspectorCurveAttribute : Attribute {
        public float xMin;
        public float xMax;

        public float yMin;
        public float yMax;

        public InspectorCurveAttribute(float xMin = 0, float yMin = 0, float xMax = 1, float yMax = 1) {
            this.xMin = xMin;
            this.xMax = xMax;
            this.yMin = yMin;
            this.xMax = xMax;
        }
    }
}