using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FullSerializer.Internal;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Internal {
    /// <summary>
    /// The general property editor that takes over when there is no specific override. This uses
    /// reflection to discover what values to edit.
    /// </summary>
    internal class ReflectedPropertyEditor : IPropertyEditor, IPropertyEditorEditAPI {
        private class SelectedCategoryMetadata : IGraphMetadataItemNotPersistent {
            public int SelectedCategoryIndex;
        }

        public bool DisplaysStandardLabel {
            get { return true; }
        }

        /// <summary>
        /// The maximum depth that the reflected editor will go to for automatic object reference
        /// instantiation. Beyond this depth, the user will have the manually instantiate
        /// references. We have a depth limit so that we don't end up in an infinite object
        /// construction cycle.
        /// </summary>
        private const int MaximumNestingDepth = 5;
        private static fiCycleDetector _cycleEdit;
        private static fiCycleDetector _cycleHeight;
        private static fiCycleDetector _cycleScene;

        /// <summary>
        /// This returns true if automatic instantiation should be enabled. Automatic instantiation
        /// gets disabled after the reflected editor has gone a x calls deep into itself in an
        /// attempt to prevent infinite recursion.
        /// </summary>
        private bool ShouldAutoInstantiate() {
            if (_cycleEdit != null && _cycleEdit.Depth >= MaximumNestingDepth) return false;
            if (_cycleHeight != null && _cycleHeight.Depth >= MaximumNestingDepth) return false;
            if (_cycleScene != null && _cycleScene.Depth >= MaximumNestingDepth) return false;
            return true;
        }

        private InspectedType _metadata;

        public PropertyEditorChain EditorChain {
            get;
            set;
        }

        public ReflectedPropertyEditor(InspectedType metadata) {
            _metadata = metadata;
        }

        /// <summary>
        /// How tall buttons should be.
        /// </summary>
        private static float ButtonHeight = 18;

        /// <summary>
        /// How tall the label element should be.
        /// </summary>
        private const float TitleHeight = 17;

        /// <summary>
        /// How much space is between each element.
        /// </summary>
        private const float DividerHeight = 2f;

        /// <summary>
        /// The height of the category toolbar.
        /// </summary>
        private const float CategoryToolbarHeight = 18f;

        /// <summary>
        /// Returns true if the given GUIContent element contains any content.
        /// </summary>
        private static bool HasLabel(GUIContent label) {
            return label.text != GUIContent.none.text ||
                label.image != GUIContent.none.image ||
                label.tooltip != GUIContent.none.tooltip;
        }

        /// <summary>
        /// Draws a label at the given region. Returns an indented rectangle that can be used for
        /// drawing properties directly under the label.
        /// </summary>
        private static Rect DrawLabel(Rect region, GUIContent label) {
            Rect titleRect = new Rect(region);
            titleRect.height = TitleHeight;
            region.y += TitleHeight;
            region.height -= TitleHeight;

            EditorGUI.LabelField(titleRect, label);
            return fiRectUtility.IndentedRect(region);
        }

        public object OnSceneGUI(object element) {
            try {
                if (_cycleScene == null) {
                    _cycleScene = new fiCycleDetector(_cycleEdit, _cycleHeight);
                }
                _cycleScene.Enter();

                // cycle; don't do anything
                if (_cycleScene.TryMark(element) == false) {
                    return element;
                }

                // Not showing a scene GUI for the object for this frame should be fine
                if (element == null) {
                    return element;
                }

                var inspectedProperties = _metadata.GetProperties(InspectedMemberFilters.InspectableMembers);
                for (int i = 0; i < inspectedProperties.Count; ++i) {
                    var property = inspectedProperties[i];

                    var editorChain = PropertyEditor.Get(property.StorageType, property.MemberInfo);
                    IPropertyEditor editor = editorChain.FirstEditor;

                    object currentValue = property.Read(element);
                    object updatedValue = editor.OnSceneGUI(currentValue);

                    // We use EqualityComparer instead of == because EqualityComparer will properly unbox structs
                    if (EqualityComparer<object>.Default.Equals(currentValue, updatedValue) == false) {
                        property.Write(element, updatedValue);
                    }
                }

                return element;
            }
            finally {
                _cycleScene.Exit();
                if (_cycleScene.Depth == 0) {
                    _cycleScene = null;
                }
            }
        }

        public bool CanEdit(Type dataType) {
            throw new NotSupportedException();
        }

        /// <summary>
        /// A helper method that draws the inspector for a field/property at the given location.
        /// </summary>
        private void EditProperty(ref Rect region, object element, InspectedProperty property, fiGraphMetadata metadata) {
            bool hasPrefabDiff = fiPrefabTools.HasPrefabDiff(element, property);
            if (hasPrefabDiff) fiUnityInternalReflection.SetBoldDefaultFont(true);

            // edit the property
            {
                var childMetadata = metadata.Enter(property.Name, element);
                fiGraphMetadataCallbacks.PropertyMetadataCallback(childMetadata.Metadata, property);

                Rect propertyRect = region;
                float propertyHeight = fiEditorGUI.EditPropertyHeight(element, property, childMetadata);
                propertyRect.height = propertyHeight;

                fiEditorGUI.EditProperty(propertyRect, element, property, childMetadata);

                region.y += propertyHeight;
            }

            if (hasPrefabDiff) fiUnityInternalReflection.SetBoldDefaultFont(false);
        }

        /// <summary>
        /// A helper method that draws a button at the given region.
        /// </summary>
        private void EditButton(ref Rect region, object element, InspectedMethod method) {
            Rect buttonRect = region;
            buttonRect.height = ButtonHeight;

            GUIContent buttonLabel = method.DisplayLabel;

            // Disable the button if invoking it will cause an exception
            if (method.HasArguments) {
                buttonLabel = new GUIContent(buttonLabel);
                buttonLabel.text += " (Remove method parameters to enable this button)";
            }

            EditorGUI.BeginDisabledGroup(method.HasArguments);
            if (GUI.Button(buttonRect, buttonLabel)) {
                method.Invoke(element);
                GUI.changed = true; // anything could have happened
            }
            EditorGUI.EndDisabledGroup();

            region.y += ButtonHeight;
        }

        private static bool ShouldShowMemberDynamic(object element, MemberInfo member) {
            var showIf = fsPortableReflection.GetAttribute<InspectorShowIfAttribute>(member);
            var hideIf = fsPortableReflection.GetAttribute<InspectorHideIfAttribute>(member);
            if (showIf != null && hideIf != null) {
                Debug.LogError("Only one of [InspectorShowIf] or [InspectorHideIf] can be on " + member.DeclaringType + "." + member.Name);
            }

            if (showIf != null) {
                return fiLogicalOperatorSupport.ComputeValue(showIf.Operator, showIf.ConditionalMemberNames, element);
            }

            if (hideIf != null) {
                return !fiLogicalOperatorSupport.ComputeValue(hideIf.Operator, hideIf.ConditionalMemberNames, element);
            }

            return true;
        }

        private void EditInspectedMember(ref Rect region, object element, InspectedMember member, fiGraphMetadata metadata) {
            // requested skip
            if (ShouldShowMemberDynamic(element, member.MemberInfo) == false) {
                return;
            }

            if (member.IsMethod) {
                EditButton(ref region, element, member.Method);
            }
            else {
                EditProperty(ref region, element, member.Property, metadata);
            }

            region.y += DividerHeight;
        }

        private static bool UseCategories(Dictionary<string, List<InspectedMember>> categories) {
            if (!fiSettings.DisplaySingleCategory && categories.Count <= 1)
                return false;

            return categories.Count > 0;
        }

        /// <summary>
        /// Draws the actual property editors.
        /// </summary>
        private object EditPropertiesButtons(GUIContent label, Rect region, object element, fiGraphMetadata metadata) {
            // If we have a label, then our properties block is indented and we should use hierarchy mode. Otherwise
            // we do not have a label so our properties block is *not* indented so we should *not* use hierarchy mode.
            //
            // HACK: We also check the nesting depth - if we're the top-level editor, then we want to enable hierarchy
            //       mode
            fiEditorGUI.PushHierarchyMode(_cycleEdit.Depth == 1 || string.IsNullOrEmpty(label.text) == false);

            var categories = _metadata.GetCategories(InspectedMemberFilters.InspectableMembers);
            if (UseCategories(categories)) {
                var selectedCategoryMetadata = metadata.GetMetadata<SelectedCategoryMetadata>();

                Rect toolbarRect = region;
                toolbarRect.height = CategoryToolbarHeight;
                region.y += CategoryToolbarHeight + fiLateBindings.EditorGUIUtility.standardVerticalSpacing;
                region.height -= CategoryToolbarHeight + fiLateBindings.EditorGUIUtility.standardVerticalSpacing;

                int index = selectedCategoryMetadata.SelectedCategoryIndex;
                selectedCategoryMetadata.SelectedCategoryIndex = GUI.Toolbar(toolbarRect, index, categories.Keys.ToArray());

                foreach (var member in categories.Values.ElementAt(index)) {
                    EditInspectedMember(ref region, element, member, metadata);
                }

                // Make sure we don't prune metadata
                foreach (var member in _metadata.GetMembers(InspectedMemberFilters.InspectableMembers)) {
                    metadata.Enter(member.Name, element);
                }
            }

            else {
                var orderedMembers = _metadata.GetMembers(InspectedMemberFilters.InspectableMembers);
                for (int i = 0; i < orderedMembers.Count; ++i) {
                    EditInspectedMember(ref region, element, orderedMembers[i], metadata);
                }
            }

            fiEditorGUI.PopHierarchyMode();

            return element;
        }

        public object Edit(Rect region, GUIContent label, object element, fiGraphMetadata metadata) {
            try {
                if (_cycleEdit == null) {
                    _cycleEdit = new fiCycleDetector(_cycleHeight, _cycleScene);
                }
                _cycleEdit.Enter();

                if (_cycleEdit.TryMark(element) == false) {
                    EditorGUI.LabelField(region, label, new GUIContent("<cycle>"));
                    return element;
                }

                if (HasLabel(label)) {
                    region = DrawLabel(region, label);
                }

                if (element == null) {
                    // if the user want's an instance, we'll create one right away We also check to
                    // make sure we should automatically instantiate references, as if we're pretty
                    // far down in the nesting level there may be an infinite recursion going on
                    if (fiSettings.InspectorAutomaticReferenceInstantation &&
                        _metadata.HasDefaultConstructor &&
                        ShouldAutoInstantiate()) {

                        element = _metadata.CreateInstance();
                        GUI.changed = true;
                    }

                    // otherwise we show a button to create an instance
                    else {
                        string buttonMessage = "null - create instance (ctor)?";
                        if (_metadata.HasDefaultConstructor == false) {
                            buttonMessage = "null - create instance (unformatted)?";
                        }
                        if (fiEditorGUI.LabeledButton(region, _metadata.ReflectedType.Name, buttonMessage)) {
                            element = _metadata.CreateInstance();
                            GUI.changed = true;
                        }

                        return element;
                    }
                }

                return EditPropertiesButtons(label, region, element, metadata);

            }
            finally {
                _cycleEdit.Exit();
                if (_cycleEdit.Depth == 0) {
                    _cycleEdit = null;
                }
            }
        }

        public float GetElementHeight(GUIContent label, object element, fiGraphMetadata metadata) {
            try {
                if (_cycleHeight == null) {
                    _cycleHeight = new fiCycleDetector(_cycleEdit, _cycleScene);
                }
                _cycleHeight.Enter();

                if (_cycleHeight.TryMark(element) == false) {
                    return EditorStyles.label.CalcHeight(GUIContent.none, 100);
                }

                float height = HasLabel(label) ? TitleHeight + fiRectUtility.IndentVertical : 0;

                if (element == null) {
                    // if the user want's an instance, we'll create one right away. We also check to
                    // make sure we should automatically instantiate references, as if we're pretty
                    // far down in the nesting level there may be an infinite recursion going on
                    if (fiSettings.InspectorAutomaticReferenceInstantation &&
                        _metadata.HasDefaultConstructor &&
                        ShouldAutoInstantiate()) {

                        element = _metadata.CreateInstance();
                        GUI.changed = true;
                    }

                    // otherwise we show a button to create an instance
                    else {
                        height += ButtonHeight;
                    }
                }

                if (element != null) {
                    // figure out which members we should display
                    List<InspectedMember> displayableMembers;
                    var categories = _metadata.GetCategories(InspectedMemberFilters.InspectableMembers);
                    if (UseCategories(categories)) {
                        var selectedCategoryMetadata = metadata.GetMetadata<SelectedCategoryMetadata>();
                        height += CategoryToolbarHeight + fiLateBindings.EditorGUIUtility.standardVerticalSpacing;
                        displayableMembers = categories.Values.ElementAt(selectedCategoryMetadata.SelectedCategoryIndex);
                    }
                    else {
                        displayableMembers = _metadata.GetMembers(InspectedMemberFilters.InspectableMembers);
                    }

                    // compute the height of the members we will display
                    for (int i = 0; i < displayableMembers.Count; ++i) {
                        var member = displayableMembers[i];

                        // requested skip
                        if (ShouldShowMemberDynamic(element, member.MemberInfo) == false) {
                            continue;
                        }

                        var childMetadata = metadata.Enter(member.Name, element);

                        if (member.IsMethod) {
                            height += ButtonHeight;
                        }
                        else {
                            fiGraphMetadataCallbacks.PropertyMetadataCallback(childMetadata.Metadata, member.Property);
                            height += fiEditorGUI.EditPropertyHeight(element, member.Property, childMetadata);
                        }

                        height += DividerHeight;
                    }

                    // Remove the last divider
                    if (displayableMembers.Count > 0) height -= DividerHeight;
                }

                return height;
            }
            finally {
                _cycleHeight.Exit();
                if (_cycleHeight.Depth == 0) {
                    _cycleHeight = null;
                }
            }
        }

        public GUIContent GetFoldoutHeader(GUIContent label, object element) {
            return label;
        }

        public static IPropertyEditor TryCreate(Type dataType, ICustomAttributeProvider attributes) {
            // The reflected property editor is applicable to *every* type except collections, where
            // it is expected that the ICollectionPropertyEditor will take over (or something more
            // specific than that, such as the IListPropertyEditor).

            var metadata = InspectedType.Get(dataType);
            if (metadata.IsCollection) {
                return null;
            }

            return new ReflectedPropertyEditor(metadata);
        }
    }
}