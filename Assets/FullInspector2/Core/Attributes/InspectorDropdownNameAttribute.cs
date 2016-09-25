using System;

namespace FullInspector {
    /// <summary>
    /// Annotating a type with this attribute allows you to specify what name it will appear with
    /// inside of the abstract type selection dropdown.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
    public sealed class InspectorDropdownNameAttribute : Attribute {
        /// <summary>
        /// The name that the type will use in the abstract type dropdown selection
        /// wizard. The default value is the C# formatted type name for the type. You
        /// can create "folders" within the dropdown popup by using '/'; for example,
        /// "folder/my type" will appear inside "folder" as "my type".
        /// </summary>
        public string DisplayName;

        /// <summary>
        /// Sets the name of the type to use in the abstract type dropdown.
        /// </summary>
        /// <param name="displayName">
        /// The name that the type will use in the abstract type dropdown selection
        /// wizard. The default value is the C# formatted type name for the type. You
        /// can create "folders" within the dropdown popup by using '/'; for example,
        /// "folder/my type" will appear inside "folder" as "my type".
        /// </param>
        public InspectorDropdownNameAttribute(string displayName) {
            DisplayName = displayName;
        }
    }
}