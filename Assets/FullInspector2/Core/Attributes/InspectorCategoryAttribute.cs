using System;

namespace FullInspector {
    /// <summary>
    /// Display this field, property, or method inside of the given tab group / category within
    /// the inspector. Each member can be part of multiple categories - simply apply this attribute
    /// multiple times. If you apply this attribute to a class, then the non-annotated members will
    /// fall into that category.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public sealed class InspectorCategoryAttribute : Attribute {
        /// <summary>
        /// The category to display this member in.
        /// </summary>
        public string Category;

        public InspectorCategoryAttribute(string category) {
            Category = category;
        }
    }
}