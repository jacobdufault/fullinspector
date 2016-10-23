using System;

namespace FullInspector {
    /// <summary>
    /// Display the given method as a button in the inspector. You can customize
    /// the order of that the button is displayed in w.r.t. fields or properties
    /// by using [InspectorOrder].
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class InspectorButtonAttribute : Attribute {
        /// <summary>
        /// Creates a button with a default name generated based off of the
        /// method name.
        /// </summary>
        public InspectorButtonAttribute() { }
    }
}