using UnityEngine;

namespace FullInspector {
    // TODO: Remove this API. Let's just cleanup the IPropertyEditor API.
    // TODO: Introduce a more natural split of things. I think we should have a serialization and an editor split.

    /// <summary>
    /// This is the core editing API that property editors use. We split the editing API and the
    /// IPropertyEditor interfaces into two as we define extension methods on IPropertyEditor that
    /// allows FI to run arbitrary code before/after the actual edit method executes.
    /// </summary>
    public interface IPropertyEditorEditAPI {
        /// <summary>
        /// Display a Unity inspector GUI that provides an editing interface for the given object.
        /// </summary>
        /// <param name="region">The rect on the screen to draw the GUI controls.</param>
        /// <param name="label">The label to label the controls with.</param>
        /// <param name="element">The element itself to edit. This can be mutated directly. For
        /// values which cannot be mutated, such as structs, the return value is used to update the
        /// stored value.</param>
        /// <returns>An updated instance of the element.</returns>
        object Edit(Rect region, GUIContent label, object element, fiGraphMetadata metadata);

        /// <summary>
        /// Returns the height of the region that needs editing.
        /// </summary>
        /// <param name="label">The label that will be used when editing.</param>
        /// <param name="element">The element that will be edited.</param>
        /// <returns>The height of the region that needs editing.</returns>
        float GetElementHeight(GUIContent label, object element, fiGraphMetadata metadata);

        /// <summary>
        /// Returns a header that should be used for the foldout. An item is displayed within a
        /// foldout when this property editor reaches a certain height.
        /// </summary>
        /// <param name="label">The current foldout label.</param>
        /// <param name="element">The current object element.</param>
        /// <returns>An updated label.</returns>
        GUIContent GetFoldoutHeader(GUIContent label, object element);

        /// <summary>
        /// Draw an optional scene GUI.
        /// </summary>
        /// <param name="element">The object instance to edit using the scene GUI.</param>
        /// <returns>An updated object instance.</returns>
        object OnSceneGUI(object element);

        /// <summary>
        /// Does this editor display a standard label that can instead be rendered with EditorGUI.Foldout?
        /// </summary>
        bool DisplaysStandardLabel {
            get;
        }
    }
}