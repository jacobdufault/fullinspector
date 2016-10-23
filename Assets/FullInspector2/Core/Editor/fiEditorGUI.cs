using System;
using System.Collections.Generic;
using FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    public class fiEditorGUI {
        #region Hierarchy Mode Utilities
        private static readonly Stack<bool> _hierarchyMode = new Stack<bool>();

        public static void PushHierarchyMode(bool state) {
            _hierarchyMode.Push(EditorGUIUtility.hierarchyMode);
            EditorGUIUtility.hierarchyMode = state;
        }

        public static void PopHierarchyMode() {
            EditorGUIUtility.hierarchyMode = _hierarchyMode.Pop();
        }
        #endregion Hierarchy Mode Utilities

        #region Splitters
        // see http://answers.unity3d.com/questions/216584/horizontal-line.html

        private static readonly GUIStyle splitter;

        static fiEditorGUI() {
            splitter = new GUIStyle();
            splitter.normal.background = EditorGUIUtility.whiteTexture;
            splitter.stretchWidth = true;
            splitter.margin = new RectOffset(0, 0, 7, 7);
        }

        private static readonly Color splitterColor = EditorGUIUtility.isProSkin ?
            new Color(0.157f, 0.157f, 0.157f) : new Color(0.5f, 0.5f, 0.5f);

        // GUI Style
        public static void Splitter(Rect position) {
            if (Event.current.type == EventType.Repaint) {
                Color restoreColor = GUI.color;
                GUI.color = splitterColor;
                splitter.Draw(position, false, false, false, false);
                GUI.color = restoreColor;
            }
        }
        #endregion Splitters

        /// <summary>
        /// Draws a normal EditorGUI.ObjectField, except it includes the
        /// component type.
        /// </summary>
        public static UnityObject ObjectField(Rect rect, GUIContent label, UnityObject element, Type objectType, bool allowSceneObjects) {
            if (label != null && !string.IsNullOrEmpty(label.text))
                rect = EditorGUI.PrefixLabel(rect, label);
            element = EditorGUI.ObjectField(rect, GUIContent.none, element, objectType, allowSceneObjects);

            // Draw the component type inside of the object field.
            if (element != null) {
                Vector2 size = EditorStyles.objectField.CalcSize(new GUIContent(element.name));
                Rect objectInnerLabelRect = rect;
                objectInnerLabelRect.x += size.x - 8;
                objectInnerLabelRect.width -= size.x + 10;
                GUI.Label(objectInnerLabelRect, " (" + element.GetType().CSharpName() + ")");
            }

            return element;
        }

        #region Fade Groups
        public static bool WillShowFadeGroup(float fade) {
            return fade > 0f;
        }

        public static void UpdateFadeGroupHeight(ref float height, float labelHeight, float fade) {
            height -= labelHeight;
            height *= fade;
            height += labelHeight;
        }

        public static void BeginFadeGroupHeight(float labelHeight, ref Rect group, float fadeHeight) {
            Rect beginArea = group;
            beginArea.height = fadeHeight;

            GUI.BeginGroup(beginArea);
            group.x = 0;
            group.y = 0;
        }

        public static void BeginFadeGroup(float labelHeight, ref Rect group, float fade) {
            float height = group.height;
            UpdateFadeGroupHeight(ref height, labelHeight, fade);
            //group.height = height;

            Rect beginArea = group;
            beginArea.height = height;

            GUI.BeginGroup(beginArea);
            group.x = 0;
            group.y = 0;
        }

        public static void EndFadeGroup() {
            GUI.EndGroup();
        }
        #endregion Fade Groups

        #region Drag and Drop
        public static bool TryDragAndDropArea(Rect dropArea, Predicate<UnityObject> filter, out UnityObject[] droppedObjects) {
            switch (Event.current.type) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (dropArea.Contains(Event.current.mousePosition)) {
                        bool allow = true;

                        foreach (UnityObject obj in DragAndDrop.objectReferences) {
                            if (filter(obj) == false) {
                                allow = false;
                                break;
                            }
                        }

                        if (allow) {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                            if (Event.current.type == EventType.DragPerform) {
                                DragAndDrop.AcceptDrag();
                                droppedObjects = DragAndDrop.objectReferences;
                                return true;
                            }
                        }
                        else {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                        }
                    }
                    break;
            }

            droppedObjects = null;
            return false;
        }
        #endregion Drag and Drop

        #region Fade Groups - Automatic Helpers
        public static void AnimatedBegin(ref Rect rect, fiGraphMetadata metadata) {
            var anim = metadata.GetMetadata<fiAnimationMetadata>();
            if (anim.IsAnimating) {
                BeginFadeGroupHeight(0, ref rect, anim.AnimationHeight);
            }
        }

        public static void AnimatedEnd(fiGraphMetadata metadata) {
            var anim = metadata.GetMetadata<fiAnimationMetadata>();
            if (anim.IsAnimating) {
                EndFadeGroup();
            }
        }

        public static float AnimatedHeight(float currentHeight, bool updateHeight, fiGraphMetadata metadata) {
            var anim = metadata.GetMetadata<fiAnimationMetadata>();
            if (updateHeight) anim.UpdateHeight(currentHeight);

            if (anim.IsAnimating) {
                fiEditorUtility.RepaintAllEditors();
                return anim.AnimationHeight;
            }

            return currentHeight;
        }

        #endregion Fade Groups - Automatic Helpers

        #region Labeled Buttons
        /// <summary>
        /// Draws a button with a label in front of the button.
        /// </summary>
        public static bool LabeledButton(Rect rect, string label, string button) {
            return LabeledButton(rect, new GUIContent(label), new GUIContent(button));
        }

        /// <summary>
        /// Draws a button with a label in front of the button.
        /// </summary>
        public static bool LabeledButton(Rect rect, GUIContent label, GUIContent button) {
            Rect buttonRect = EditorGUI.PrefixLabel(rect, label);
            return GUI.Button(buttonRect, button);
        }

        private static bool IsWhiteSpaceOnly(string str) {
            for (int i = 0; i < str.Length; ++i) {
                if (char.IsWhiteSpace(str[i]) == false) {
                    return false;
                }
            }

            return true;
        }
        #endregion Labeled Buttons

        #region Toolkit Editing
        private static readonly GUIContent tkControl_DebugControl_Label = new GUIContent("Control");
        private static readonly PropertyEditorChain tkControl_PropertyEditor = PropertyEditor.Get(typeof(tkIControl), null);
        private const string tkControl_Metadata_Layout = "Layout";
        private const string tkControl_Metadata_DebugControl = "DebugControl";

        private const float tkControl_MarginBeforeHelp = 15f;
        private const float tkControl_HelpRectHeight = 38f;

        /// <summary>
        /// Draws an editor for the given control at the given rect.
        /// </summary>
        /// <param name="rect">The rect to draw the editor within.</param>
        /// <param name="label">The label for the edited control.</param>
        /// <param name="element">The element to edit.</param>
        /// <param name="metadata">The metadata to use when editing.</param>
        /// <param name="control">
        /// The actual control that will be used for the editor.
        /// </param>
        /// <returns>The updated element instance.</returns>
        public static object tkControl(Rect rect, GUIContent label, object element, fiGraphMetadata metadata, tkControlEditor control) {
            fiLateBindingsBinder.EnsureLoaded();

            Rect layoutRect = rect;
            Rect helpRect = rect;
            Rect tweakerRect = rect;

            if (control.Debug) {
                float tweakerRectHeight = tkControl_PropertyEditor.FirstEditor.GetElementHeight(tkControl_DebugControl_Label, control, metadata.Enter(tkControl_Metadata_DebugControl, null));

                layoutRect.height -= tkControl_MarginBeforeHelp + tkControl_HelpRectHeight +
                                     fiLateBindings.EditorGUIUtility.standardVerticalSpacing + tweakerRectHeight;

                helpRect = layoutRect;
                helpRect.y += layoutRect.height + tkControl_MarginBeforeHelp;
                helpRect.height = tkControl_HelpRectHeight;

                tweakerRect = helpRect;
                tweakerRect.y += tweakerRect.height + fiLateBindings.EditorGUIUtility.standardVerticalSpacing;
                tweakerRect.height = tweakerRectHeight;
            }

            SetupContext(control, label);
            element = control.Control.Edit(layoutRect, element, control.Context, metadata.Enter(tkControl_Metadata_Layout, metadata.Context).Metadata);

            if (control.Debug) {
                EditorGUI.HelpBox(helpRect, "The layout below should be used for visualizing the runtime layout structure or for tweaking layout values like minimum width. Changes will *not* persist - you need to modify the code itself.", MessageType.Info);
                tkControl_PropertyEditor.FirstEditor.Edit(tweakerRect, tkControl_DebugControl_Label, control, metadata.Enter(tkControl_Metadata_DebugControl, null));
            }

            return element;
        }

        /// <summary>
        /// Draws an editor for the given control at the given rect.
        /// </summary>
        /// <param name="element">The element to edit.</param>
        /// <param name="label">The label for the edited control.</param>
        /// <param name="metadata">The metadata to use when editing.</param>
        /// <param name="control">
        /// The actual control that will be used for the editor.
        /// </param>
        /// <returns>
        /// The height that is needed to fully display this control.
        /// </returns>
        public static float tkControlHeight(GUIContent label, object element, fiGraphMetadata metadata, tkControlEditor control) {
            fiLateBindingsBinder.EnsureLoaded();

            SetupContext(control, label);
            var height = control.Control.GetHeight(element, control.Context, metadata.Enter(tkControl_Metadata_Layout, metadata.Context).Metadata);

            if (control.Debug) {
                height += tkControl_MarginBeforeHelp;
                height += tkControl_HelpRectHeight;
                height += fiLateBindings.EditorGUIUtility.standardVerticalSpacing;
                height += tkControl_PropertyEditor.FirstEditor.GetElementHeight(tkControl_DebugControl_Label, control, metadata.Enter(tkControl_Metadata_DebugControl, null));
            }

            return height;
        }

        private static void SetupContext(tkControlEditor control, GUIContent label) {
            if (control.Context is tkContextLabelRequest) {
                ((tkContextLabelRequest)control.Context).Label = label;
            }
        }
        #endregion Toolkit Editing

        #region Property Editing
        private static void RevertPrefabContextMenu(Rect region, object context, InspectedProperty property) {
            if (Event.current.type == EventType.ContextClick &&
                region.Contains(Event.current.mousePosition) &&

                // This can be a relatively heavy function call, so we check it
                // last. If the rect bounds check ends up consuming lots of time,
                // then HasPrefabDiff has a small fast-path section that can
                // short-circuit the bounds check.
                fiPrefabTools.HasPrefabDiff(context, property)) {
                Event.current.Use();

                var content = new GUIContent("Revert " + property.DisplayName + " to Prefab Value");

                GenericMenu menu = new GenericMenu();
                menu.AddItem(content, /*on:*/false, () => {
                    fiPrefabTools.RevertValue(context, property);
                });
                menu.ShowAsContext();
            }
        }

        /// <summary>
        /// Draws a GUI for editing the given property and returns the updated
        /// value. This does
        /// *not* write the updated value to a container.
        /// </summary>
        public static object EditPropertyDirect(Rect region, InspectedProperty property, object propertyValue, fiGraphMetadataChild metadataChild) {
            return EditPropertyDirect(region, property, propertyValue, metadataChild, null);
        }

        /// <summary>
        /// Draws a GUI for editing the given property and returns the updated
        /// value. This does
        /// *not* write the updated value to a container.
        /// </summary>
        /// <param name="context">
        /// An optional context that the property value came from. If this is not
        /// given, then a prefab context menu will not be displayable.
        /// </param>
        public static object EditPropertyDirect(Rect region, InspectedProperty property, object propertyValue, fiGraphMetadataChild metadataChild, object context) {
            fiGraphMetadata metadata = metadataChild.Metadata;

            // Show a "revert to prefab" value context-menu if possible
            if (context != null) {
                RevertPrefabContextMenu(region, context, property);
            }

            // get the label / tooltip
            GUIContent label = new GUIContent(property.DisplayName,
                                              InspectorTooltipAttribute.GetTooltip(property.MemberInfo));

            var editorChain = PropertyEditor.Get(property.StorageType, property.MemberInfo);
            IPropertyEditor editor = editorChain.FirstEditor;

            EditorGUI.BeginDisabledGroup(property.CanWrite == false);
            propertyValue = editor.Edit(region, label, propertyValue, metadata.Enter("EditProperty", metadata.Context));
            EditorGUI.EndDisabledGroup();

            return propertyValue;
        }

        public static void EditProperty(Rect region, object container, InspectedProperty property, fiGraphMetadataChild metadata) {
            EditorGUI.BeginChangeCheck();

            object propertyValue = property.Read(container);
            object updatedValue = EditPropertyDirect(region, property, propertyValue, metadata, container);

            if (EditorGUI.EndChangeCheck()) {
                property.Write(container, updatedValue);

                // Make sure we propagate the changes up the edit stack. For
                // example, if this property is on a struct on a struct, then the
                // top-level struct will not get modified without propagation of
                // the change check.
                GUI.changed = true;
            }
        }

        public static float EditPropertyHeightDirect(InspectedProperty property, object propertyValue, fiGraphMetadataChild metadataChild) {
            fiGraphMetadata metadata = metadataChild.Metadata;

            var editor = PropertyEditor.Get(property.StorageType, property.MemberInfo).FirstEditor;

            GUIContent propertyLabel = new GUIContent(property.DisplayName);

            // Either the foldout is active or we are not displaying a foldout.
            // Either way, we want to report the full height of the property.
            return editor.GetElementHeight(propertyLabel, propertyValue, metadata.Enter("EditProperty", metadata.Context));
        }

        public static float EditPropertyHeight(object container, InspectedProperty property, fiGraphMetadataChild metadata) {
            object propertyValue = property.Read(container);
            return EditPropertyHeightDirect(property, propertyValue, metadata);
        }
        #endregion Property Editing
    }
}