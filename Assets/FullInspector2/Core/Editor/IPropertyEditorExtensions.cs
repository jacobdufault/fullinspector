using FullInspector.Internal;
using System;
using UnityEditor;
using UnityEngine;

namespace FullInspector {
    /// <summary>
    /// The cached height metadata is used to optimize CPU/execution time for some memory so that
    /// we don't have to recompute heights multiple times over. In essence, if we call
    /// GetElementHeight from Edit, then GetElementHeight will short-circuit and just return a
    /// cached height.
    /// </summary>
    public class CachedHeightMedatadata : IGraphMetadataItemNotPersistent {
        public float CachedHeight;
    }

    public static class PropertyEditorExtensions {
        /// <summary>
        /// We store the current
        /// </summary>
        private enum BaseMethodCall {
            None,
            Edit,
            GetElementHeight,
            OnSceneGUI
        }
        /// <summary>
        /// The current method that we are invoking.
        /// </summary>
        private static BaseMethodCall BaseMethod;


        private static void BeginMethodSet(BaseMethodCall method, out bool set) {
            set = BaseMethod == BaseMethodCall.None;

            if (!set && BaseMethod != method) {
                throw new InvalidOperationException(method + " cannot be called from " + BaseMethod);
            }

            if (set) {
                BaseMethod = method;
            }
        }
        private static void EndMethodSet(bool set) {
            if (set) {
                BaseMethod = BaseMethodCall.None;
            }
        }

        private static float FoldoutHeight = EditorStyles.foldout.CalcHeight(GUIContent.none, 100);

        /// <summary>
        /// Helper method to fetch the editing API for an IPropertyEditor.
        /// </summary>
        private static IPropertyEditorEditAPI GetEditingAPI(IPropertyEditor editor) {
            var api = editor as IPropertyEditorEditAPI;

            if (api == null) {
                throw new InvalidOperationException(string.Format("Type {0} needs to extend " +
                    "IPropertyEditorEditAPI", editor.GetType()));
            }

            return api;
        }

        /// <summary>
        /// Display a Unity inspector GUI that provides an editing interface for the given object.
        /// </summary>
        /// <param name="region">The rect on the screen to draw the GUI controls.</param>
        /// <param name="label">The label to label the controls with.</param>
        /// <param name="element">The element itself to edit. This can be mutated directly. For
        /// values which cannot be mutated, such as structs, the return value is used to update the
        /// stored value.</param>
        /// <returns>An updated instance of the element.</returns>
        public static T Edit<T>(this IPropertyEditor editor, Rect region, GUIContent label, T element, fiGraphMetadataChild metadata) {
            var api = GetEditingAPI(editor);

            bool setBaseMethod;
            BeginMethodSet(BaseMethodCall.Edit, out setBaseMethod);

            try {
                EditorGUIUtility.labelWidth = fiGUI.PushLabelWidth(label, region.width);

                // TODO: introduce a fast-path, we are burning lots of time in this function
                T result;
                if (typeof(T).IsPrimitive) result = DoEditFast(api, region, label, element, metadata.Metadata);
                else result = DoEditSlow(api, region, label, element, metadata.Metadata);

                EditorGUIUtility.labelWidth = fiGUI.PopLabelWidth();

                return result;
            }
            finally {
                EndMethodSet(setBaseMethod);
            }

        }

        private static float FoldoutEmptyWidth {
            get {
                if (foldoutEmptyWidth == null)
                    foldoutEmptyWidth = EditorStyles.foldout.CalcSize(GUIContent.none).x;
                return foldoutEmptyWidth.Value;
            }
        }
        private static float? foldoutEmptyWidth;

        private static T DoEditFast<T>(IPropertyEditorEditAPI api, Rect region, GUIContent label, T element, fiGraphMetadata metadata) {
            return (T)api.Edit(region, label, element, metadata);
        }

        private static T DoEditSlow<T>(IPropertyEditorEditAPI api, Rect region, GUIContent label, T element, fiGraphMetadata metadata) {

            var dropdown = GetDropdownMetadata((IPropertyEditor)api, metadata);

            // Activate the foldout if we're above the minimum foldout height
            if (dropdown.ShouldDisplayDropdownArrow == false) {
                dropdown.ShouldDisplayDropdownArrow = region.height > fiSettings.MinimumFoldoutHeight;
            }

            // We're rendering dropdown logic
            if (dropdown.ShouldDisplayDropdownArrow) {

                // The foldout is hidden. Just draw the collapsed arrow and return.
                if (dropdown.IsActive == false) {

                    // Some editors don't always supply a label (for example, the collection editors),
                    // so instead of showing nothing we'll show the current ToString() value
                    if (string.IsNullOrEmpty(label.text)) {
                        label = new GUIContent(element != null ? element.ToString() : "Collapsed value");
                    }
                    label = api.GetFoldoutHeader(label, element);

                    region.width = EditorStyles.foldout.CalcSize(label).x;
                    region.height = FoldoutHeight;

                    // note: we don't use dropdown.IsActive = ... because we can be animating, and doing
                    // that direct-assign toggle will cause the animation to flip-flop infinitely
                    if (EditorGUI.Foldout(region, false, label, /*toggleOnLabelClick:*/ true) == true) {
                        dropdown.IsActive = true;
                    }

                    return element;
                }


                // Okay, we're going to be rendering the content of the body. We want to figure out how to
                // actually render the dropdown arrow. We do it in one of two ways:
                //  1. Draw using EditorGUI.Foldout with the full label
                //  2. Draw just the foldout and indent the rect if we're not in hierarchy mode

                // We have a couple of special cases to handle:
                //  1. We don't have a label
                //  2. The editor has non-standard label rendering
                // In these scenarioes, we will just draw the foldout arrow by itself and indent the contents of
                // the region.
                if (string.IsNullOrEmpty(label.text) || api.DisplaysStandardLabel == false) {

                    Rect foldoutRegion = region;
                    foldoutRegion.width = FoldoutEmptyWidth;
                    foldoutRegion.height = FoldoutHeight;

                    // note: We don't use dropdown.IsActive = ... because we can be animating, and doing
                    // that direct-assign toggle will cause the animation to flip-flop infinitely
                    if (EditorGUI.Foldout(foldoutRegion, true, GUIContent.none, /*toggleOnLabelClick:*/ true) == false) {
                        dropdown.IsActive = false;
                    }

                    // Indent the body if we're not in hierarchy mode
                    if (EditorGUIUtility.hierarchyMode == false) {
                        region.x += foldoutRegion.width;
                        region.width -= foldoutRegion.width;
                    }
                }

                // We will render the foldout including the label
                else {
                    Rect foldoutRegion = region;
                    foldoutRegion.width = EditorStyles.foldout.CalcSize(label).x;
                    foldoutRegion.height = FoldoutHeight;

                    // note: We don't use dropdown.IsActive = ... because we can be animating, and doing
                    // that direct-assign toggle will cause the animation to flip-flop infinitely
                    if (EditorGUI.Foldout(foldoutRegion, true, label, /*toggleOnLabelClick:*/ true) == false) {
                        dropdown.IsActive = false;
                    }
                    label = new GUIContent(" ");
                }
            }

            // Draw the actual edited content
            if (dropdown.IsAnimating) fiEditorGUI.BeginFadeGroup(FoldoutHeight, ref region, dropdown.AnimPercentage);
            var result = (T)api.Edit(region, label, element, metadata);
            if (dropdown.IsAnimating) fiEditorGUI.EndFadeGroup();

            // End the cull zone. This should have been started inside of GetElementHeight, but if
            // for some reason GetElementHeight was never called, this will be harmless if not
            // currently in a cull-zone.
            metadata.EndCullZone();

            return result;
        }

        /// <summary>
        /// Fetches the dropdown metadata instance that should be used. This performs any necessary initialization.
        /// </summary>
        private static fiDropdownMetadata GetDropdownMetadata(IPropertyEditor editor, fiGraphMetadata metadata) {
            bool wasCreated;
            var dropdown = metadata.GetPersistentMetadata<fiDropdownMetadata>(out wasCreated);
            if (wasCreated && editor is IPropertyEditorDefaultFoldoutState) {
                dropdown.InvertDefaultState();

                if (((IPropertyEditorDefaultFoldoutState)editor).DefaultFoldoutState == false) {
                    dropdown.ForceHideWithoutAnimation();
                }
            }

            return dropdown;
        }

        /// <summary>
        /// Returns the height of the region that needs editing.
        /// </summary>
        /// <param name="label">The label that will be used when editing.</param>
        /// <param name="element">The element that will be edited.</param>
        /// <returns>The height of the region that needs editing.</returns>
        public static float GetElementHeight<T>(this IPropertyEditor editor, GUIContent label, T element, fiGraphMetadataChild metadata) {
            bool hasCachedHeight;
            CachedHeightMedatadata cachedHeight = metadata.Metadata.GetMetadata<CachedHeightMedatadata>(out hasCachedHeight);
            hasCachedHeight = !hasCachedHeight;

            // If we're calling from an Edit method, just reuse the last height computation (if we have a previous height computation).
            if (hasCachedHeight && BaseMethod == BaseMethodCall.Edit) {
                return cachedHeight.CachedHeight;
            }

            // If we're a dropdown that is not active, show the general foldout height
            var dropdown = GetDropdownMetadata(editor, metadata.Metadata);
            if (dropdown.ShouldDisplayDropdownArrow && dropdown.IsAnimating == false && dropdown.IsActive == false) {
                cachedHeight.CachedHeight = FoldoutHeight;
                return FoldoutHeight;
            }

            bool setBaseMethod = false;
            if (hasCachedHeight) {
                BeginMethodSet(BaseMethodCall.GetElementHeight, out setBaseMethod);
            }

            try {
                // We begin (but do not end) the cull zone here. The cull zone is terminated inside
                // of Edit(). It is safe to call BeginCullZone() multiple times -- it has no
                // effect past the first call.
                metadata.Metadata.BeginCullZone();

                var api = GetEditingAPI(editor);
                float height = api.GetElementHeight(label, element, metadata.Metadata);
                if (dropdown.IsAnimating) {
                    fiEditorUtility.RepaintAllEditors();
                    fiEditorGUI.UpdateFadeGroupHeight(ref height, FoldoutHeight, dropdown.AnimPercentage);
                }
                return metadata.Metadata.GetMetadata<CachedHeightMedatadata>().CachedHeight = height;
            }
            finally {
                if (hasCachedHeight) {
                    EndMethodSet(setBaseMethod);
                }
            }
        }

        /// <summary>
        /// Returns a header that should be used for the foldout. An item is displayed within a
        /// foldout when this property editor reaches a certain height.
        /// </summary>
        /// <param name="label">The current foldout label.</param>
        /// <param name="element">The current object element.</param>
        /// <returns>An updated label.</returns>
        public static GUIContent GetFoldoutHeader<T>(this IPropertyEditor editor, GUIContent label,
            T element) {

            var api = GetEditingAPI(editor);

            return api.GetFoldoutHeader(label, element);
        }

        /// <summary>
        /// Draw an optional scene GUI.
        /// </summary>
        /// <param name="element">The object instance to edit using the scene GUI.</param>
        /// <returns>An updated object instance.</returns>
        public static T OnSceneGUI<T>(this IPropertyEditor editor, T element) {
            var api = GetEditingAPI(editor);

            return (T)api.OnSceneGUI(element);
        }

        /// <summary>
        /// This method makes it easy to use a typical property editor as a GUILayout style method,
        /// where the rect is taken care of.
        /// </summary>
        /// <param name="editor">The editor that is being used.</param>
        /// <param name="label">The label to edit the region with.</param>
        /// <param name="element">The element that is being edited.</param>
        public static T EditWithGUILayout<T>(this IPropertyEditor editor, GUIContent label,
            T element, fiGraphMetadataChild metadata) {

            float height = editor.GetElementHeight(label, element, metadata);
            Rect region = EditorGUILayout.GetControlRect(false, height);
            if (Event.current.type != EventType.Layout) {
                return editor.Edit(region, label, element, metadata);
            }
            return element;
        }
    }
}