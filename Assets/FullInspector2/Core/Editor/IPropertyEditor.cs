using System;
using FullSerializer;
using UnityEngine;

namespace FullInspector {
    /// <summary>
    /// Marks an object as a property editor.
    /// </summary>
    /// <remarks>
    /// This interface is split up into two parts, the core one that user's typically access,
    /// IPropertyEditor, and one that implements the actual editing API, IPropertyEditorEditAPI.
    /// <br />
    /// You almost certainly want to extend PropertyEditor{T} instead of this
    /// interface. It provides an automatic implementation for all methods and gives type-safety to
    /// your editor.
    /// </remarks>
    public interface IPropertyEditor {
        /// <summary>
        /// The editing chain that this editor is within. This can be used to fetch the next editor
        /// to use and the like.
        /// </summary>
        PropertyEditorChain EditorChain {
            get;
            set;
        }

        /// <summary>
        /// Can the editor edit the given type?
        /// </summary>
        bool CanEdit(Type dataType);
    }

    /// <summary>
    /// An optional annotation for a property editor to specify its default foldout state.
    /// </summary>
    public interface IPropertyEditorDefaultFoldoutState {
        /// <summary>
        /// The default foldout state for the property editor. If set to false, then the editor
        /// will be collapsed immediately.
        /// </summary>
        bool DefaultFoldoutState { get; }
    }

    /// <summary>
    /// A property editor is the core editing abstraction used within FI. This allows for overriding
    /// of the default inspector logic for elements of a custom type. PropertyEditors are extremely
    /// similar to Unity's PropertyDrawers, except that they support generics and are fully
    /// type-safe.
    /// </summary>
    /// <remarks>
    /// Recall that to get FI to actually use the property editor, you need to add the
    /// [CustomPropertyEditor] attribute to the type with appropriate parameters.
    /// </remarks>
    /// <typeparam name="TElement">The element type that the editor will edit</typeparam>
    public abstract class PropertyEditor<TElement> : IPropertyEditor, IPropertyEditorEditAPI {

        // Use the magic of explicit interfaces to provide an Edit function that both accepts and
        // returns the proper type.

        // See parent interface types for comments.

        public PropertyEditorChain EditorChain {
            get;
            set;
        }

        #region IPropertyEditorEditAPI
        object IPropertyEditorEditAPI.Edit(Rect region, GUIContent label, object element, fiGraphMetadata metadata) {
            if (element != null && element is TElement == false) {
                Debug.LogError("Property editor " + GetType().CSharpName() +
                    " cannot be used on an object of type " + element.GetType());
                return 0;
            }

            return Edit(region, label, (TElement)element, metadata);
        }
        float IPropertyEditorEditAPI.GetElementHeight(GUIContent label, object element, fiGraphMetadata metadata) {
            if (element != null && element is TElement == false) {
                Debug.LogError("Property editor " + GetType().CSharpName() +
                    " cannot be used on an object of type " + element.GetType());
                return 0;
            }

            return GetElementHeight(label, (TElement)element, metadata);
        }
        object IPropertyEditorEditAPI.OnSceneGUI(object element) {
            if (element != null && element is TElement == false) {
                Debug.LogError("Property editor " + GetType().CSharpName() +
                    " cannot be used on an object of type " + element.GetType());
                return 0;
            }

            return OnSceneGUI((TElement)element);
        }
        public virtual bool DisplaysStandardLabel {
            get { return true; }
        }
        #endregion

        #region Virtual Methods
        public virtual TElement Edit(Rect region, GUIContent label, TElement element, fiGraphMetadata metadata) {
            var nextEditor = EditorChain.GetNextEditor(this);
            return PropertyEditorExtensions.Edit(nextEditor, region, label, element, metadata.Enter("IPropertyEditor"));
        }
        public virtual float GetElementHeight(GUIContent label, TElement element, fiGraphMetadata metadata) {
            var nextEditor = EditorChain.GetNextEditor(this);
            return PropertyEditorExtensions.GetElementHeight(nextEditor, label, element, metadata.Enter("IPropertyEditor"));
        }
        public virtual TElement OnSceneGUI(TElement element) {
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