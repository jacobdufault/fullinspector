using System;

namespace FullInspector {
    /// <summary>
    /// An annotation that signals that a class is a property editor for a given property type.
    /// </summary>
    /// <remarks>
    /// If the property editor is editing a generic type, then PropertyType should just be an open
    /// generic type reflecting the edited type. For example, for a List property editor,
    /// PropertyType should be typeof(List{}) (where {} are angle brackets).
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class CustomPropertyEditorAttribute : Attribute {
        /// <summary>
        /// The type of property that this class is a property editor for. This can be either a
        /// non-generic type or an open generic type (List{} is an open generic type, but List[int]
        /// is not).
        /// </summary>
        public Type PropertyType;

        /// <summary>
        /// Should this editor type be used for inherited PropertyTypes? If Inherit is true, then
        /// the first generic parameter of your property editor type will be automatically populated
        /// with the derived type that the property editor is being used to edit.
        /// </summary>
        public bool Inherit;

        /// <summary>
        /// Internal bool to prevent a warning emission if this property editor targets UnityObject
        /// types.
        /// </summary>
        public bool DisableErrorOnUnityObject;

        /// <summary>
        /// Mark this type as an IPropertyEditor. It will be instantiated automatically.
        /// </summary>
        /// <param name="propertyType">The type of property that this PropertyEditor is
        /// editing.</param>
        public CustomPropertyEditorAttribute(Type propertyType)
            : this(propertyType, false) {
        }

        /// <summary>
        /// Mark this type as an IPropertyEditor. It will be instantiated automatically.
        /// </summary>
        /// <param name="propertyType">The type of property that this PropertyEditor is
        /// editing.</param>
        /// <param name="inherit">If true, then this PropertyEditor will also be used for types that
        /// derive from propertyType. If true, then this also has implications on the generic
        /// parameter list. See the documentation on the member variable for more
        /// information.</param>
        public CustomPropertyEditorAttribute(Type propertyType, bool inherit) {
            PropertyType = propertyType;
            Inherit = inherit;
        }
    }
}