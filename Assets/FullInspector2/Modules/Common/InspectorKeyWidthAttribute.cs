using System;

namespace FullInspector {
    /// <summary>
    /// Allows the width of a KeyValuePair to be modified. If you wish to use this inside of a
    /// collection/dictionary, please see InspectorCollectionItemAttributes to activate it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InspectorKeyWidthAttribute : Attribute {
        /// <summary>
        /// The percentage of available width that the key will use in the KeyValuePair.
        /// </summary>
        public float WidthPercentage;

        public InspectorKeyWidthAttribute(float widthPercentage) {
            if (widthPercentage < 0 || widthPercentage >= 1)
                throw new ArgumentException("widthPercentage must be between [0,1]");
            WidthPercentage = widthPercentage;
        }
    }
}