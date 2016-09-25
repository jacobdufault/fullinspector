using System;

namespace FullInspector {
    /// <summary>
    /// Display the given method as a button in the inspector. You  can customize the order of
    /// that the button is displayed in w.r.t. fields or properties by using [InspectorOrder].
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class InspectorButtonAttribute : Attribute {
        /// <summary>
        /// The name of the button. If this is null or the empty string, then a default name
        /// generated off of the method name that this attribute targets should be used instead.
        /// </summary>
        [Obsolete("Please use InspectorName to get the custom name of the button")]
        public string DisplayName;

        /// <summary>
        /// Creates a button with a default name generated based off of the method name.
        /// </summary>
        public InspectorButtonAttribute() {}

        /// <summary>
        /// Set the name of the button.
        /// </summary>
        [Obsolete("Please use InspectorName to set the name of the button")]
        public InspectorButtonAttribute(string displayName) {}
    }
}