using System;

namespace FullInspector {
    /// <summary>
    /// Mark a type as a custom behavior editor. That editor needs to derive from
    /// BehaviorEditor{TBehavior} and will be used as the editor for that behavior type.
    /// </summary>
    public class CustomBehaviorEditorAttribute : Attribute {
        /// <summary>
        /// The behavior type to edit.
        /// </summary>
        public Type BehaviorType;

        /// <summary>
        /// True if this should editor should apply to derived types. Defaults to true.
        /// </summary>
        public bool Inherit = true;

        public CustomBehaviorEditorAttribute(Type behaviorType) {
            BehaviorType = behaviorType;
        }
    }
}