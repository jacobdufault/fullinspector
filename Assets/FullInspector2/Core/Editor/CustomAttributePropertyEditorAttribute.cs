using System;

namespace FullInspector {
    /// <summary>
    /// Allows an IPropertyEditor to be used as an attribute property editor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class CustomAttributePropertyEditorAttribute : Attribute {
        /// <summary>
        /// The attribute type that activates this property editor.
        /// </summary>
        public Type AttributeActivator;

        /// <summary>
        /// If true, then this attribute property editor will replace other property editors beneath
        /// it.
        /// </summary>
        public bool ReplaceOthers;

        /// <summary>
        /// Construct a new attribute instance.
        /// </summary>
        /// <param name="attributeActivator">The attribute type that activates this property
        /// editor.</param>
        public CustomAttributePropertyEditorAttribute(Type attributeActivator)
            : this(attributeActivator, true) {
        }

        /// <summary>
        /// Construct a new attribute instance.
        /// </summary>
        /// <param name="attributeActivator">The attribute type that activates this property
        /// editor.</param>
        /// <param name="replaceOthers">If true, then this attribute property editor will replace
        /// other property editors beneath it.</param>
        public CustomAttributePropertyEditorAttribute(Type attributeActivator, bool replaceOthers) {
            AttributeActivator = attributeActivator;
            ReplaceOthers = replaceOthers;
        }
    }
}