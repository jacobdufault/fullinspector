using System;
using UnityEngine;

namespace FullInspector {
    /// <summary>
    /// An IAttributePropertyEditor is identical to an IPropertyEditor, except that it also
    /// contains an Attribute parameter.
    /// </summary>
    public interface IAttributePropertyEditor : IPropertyEditor {
        /// <summary>
        /// The attribute that was used to create this editor.
        /// </summary>
        Attribute Attribute { get; set; }
    }

    /// <summary>
    /// A property editor is the core editing abstraction used within FI. This allows for
    /// overriding of the default inspector logic for elements of a custom type. PropertyEditors
    /// are extremely similar to Unity's PropertyDrawers, except that they support generics and
    /// are fully type-safe. The AttributePropertyEditor is identical to the PropertyEditor, except
    /// that it is activated when a user adds an attribute to a type.
    /// </summary>
    /// <remarks>
    /// Recall that to get FI to actually use the property editor, you need to add the
    /// [CustomAttributePropertyEditor] attribute to the type with appropriate parameters.
    /// </remarks>
    /// <typeparam name="TElement">The element type that the editor will edit</typeparam>
    public abstract class AttributePropertyEditor<TElement, TAttribute> :
        IAttributePropertyEditor, IPropertyEditorEditAPI
        where TAttribute : Attribute {

        // Use the magic of explicit interfaces to provide an Edit function that both accepts and
        // returns the proper type.

        // See parent interface types for comments.

        public PropertyEditorChain EditorChain {
            get;
            set;
        }


        // note: We don't expose the attribute to child types here, but rather in the actual
        //       API callbacks. User-defined editors are more resistant to change this way, as
        //       they can be oblivious to instance semantics w.r.t. the attribute
        private TAttribute _attribute;
        Attribute IAttributePropertyEditor.Attribute {
            get {
                return _attribute;
            }
            set {
                _attribute = (TAttribute)value;
            }
        }

        #region IPropertyEditorEditAPI
        object IPropertyEditorEditAPI.Edit(Rect region, GUIContent label, object element, fiGraphMetadata metadata) {
            if (element != null && element is TElement == false) {
                Debug.LogError("Property editor " + GetType().FullName +
                    " cannot be used on an object of type " + element.GetType());
                return 0;
            }

            return Edit(region, label, (TElement)element, _attribute, metadata);
        }

        float IPropertyEditorEditAPI.GetElementHeight(GUIContent label, object element, fiGraphMetadata metadata) {
            if (element != null && element is TElement == false) {
                Debug.LogError("Property editor " + GetType().FullName +
                    " cannot be used on an object of type " + element.GetType());
                return 0;
            }

            return GetElementHeight(label, (TElement)element, _attribute, metadata);
        }
        object IPropertyEditorEditAPI.OnSceneGUI(object element) {
            return OnSceneGUI((TElement)element, _attribute);
        }
        public virtual bool DisplaysStandardLabel {
            get { return true; }
        }
        #endregion

        #region Virtual Methods
        protected virtual TElement Edit(Rect region, GUIContent label, TElement element, TAttribute attribute, fiGraphMetadata metadata) {
            var nextEditor = EditorChain.GetNextEditor(this);
            return PropertyEditorExtensions.Edit(nextEditor, region, label, element, metadata.Enter("IAttributePropertyEditor"));
        }
        protected virtual float GetElementHeight(GUIContent label, TElement element, TAttribute attribute, fiGraphMetadata metadata) {
            var nextEditor = EditorChain.GetNextEditor(this);
            return PropertyEditorExtensions.GetElementHeight(nextEditor, label, element, metadata.Enter("IAttributePropertyEditor"));
        }
        protected virtual TElement OnSceneGUI(TElement element, TAttribute attribute) {
            return element;
        }
        public virtual GUIContent GetFoldoutHeader(GUIContent label, object element) {
            return label;
        }
        public virtual bool CanEdit(Type type) {
            return typeof(TElement).IsAssignableFrom(type);
        }
        #endregion
    }
}