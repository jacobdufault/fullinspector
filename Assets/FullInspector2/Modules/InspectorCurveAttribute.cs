using System;

namespace FullInspector {
    /// <summary>
    /// Keep a numeric value within the given min/max range, with an optional
    /// step.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class InspectorCurveAttribute : Attribute {
        public float TimeStart;
        public float TimeEnd;

        public float ValueStart;
        public float ValueEnd;

        public InspectorCurveAttribute(float timeStart = 0, float valueStart = 0, float timeEnd = 1, float valueEnd = 1) {
            TimeStart = timeStart;
            TimeEnd = timeEnd;
            ValueStart = valueStart;
            ValueEnd = valueEnd;
        }
    }
}