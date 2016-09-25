// Copyright (c) 2012-2013 Rotorz Limited. All rights reserved. Use of this source code is governed
// by a BSD-style license that can be found in the LICENSE file.

using FullInspector.Rotorz.ReorderableList.Internal;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Rotorz.ReorderableList {

    /// <summary>
    /// Utility class for drawing reorderable lists.
    /// </summary>
    public static class ReorderableListGUI {

        /// <summary>
        /// Default list item height.
        /// </summary>
        public const float DefaultItemHeight = 18;

        /// <summary>
        /// Gets or sets zero-based index of last item which was changed. A value of -1 indicates
        /// that no item was changed by list.
        /// </summary>
        /// <remarks>
        /// <para>This property should not be set when items are added or removed.</para>
        /// </remarks>
        public static int indexOfChangedItem { get; internal set; }

        /// <summary>
        /// Gets zero-based index of list item which is currently being drawn; or a value of -1 if
        /// no item is currently being drawn.
        /// </summary>
        public static int currentItemIndex {
            get { return ReorderableListControl.currentItemIndex; }
        }

        /// <summary>
        /// Gets the default list control implementation.
        /// </summary>
        private static ReorderableListControl defaultListControl { get; set; }

        static ReorderableListGUI() {
            InitStyles();

            defaultListControl = new ReorderableListControl();

            // Duplicate default styles to prevent user scripts from interferring with the default
            // list control instance.
            defaultListControl.containerStyle = new GUIStyle(defaultContainerStyle);
            defaultListControl.addButtonStyle = new GUIStyle(defaultAddButtonStyle);
            defaultListControl.removeButtonStyle = new GUIStyle(defaultRemoveButtonStyle);

            indexOfChangedItem = -1;
        }

        #region Custom Styles

        /// <summary>
        /// Gets default style for title header.
        /// </summary>
        public static GUIStyle defaultTitleStyle { get; private set; }

        /// <summary>
        /// Gets default style for background of list control.
        /// </summary>
        public static GUIStyle defaultContainerStyle { get; private set; }
        /// <summary>
        /// Gets default style for add item button.
        /// </summary>
        public static GUIStyle defaultAddButtonStyle { get; private set; }

        public static GUIStyle defaultAddButtonStyleFlipped { get; private set; }
        public static GUIStyle defaultAddButtonStyleIndependent { get; private set; }

        /// <summary>
        /// Gets default style for remove item button.
        /// </summary>
        public static GUIStyle defaultRemoveButtonStyle { get; private set; }

        private static void InitStyles() {
            defaultTitleStyle = new GUIStyle {
                border = new RectOffset(2, 2, 2, 1),
                margin = new RectOffset(5, 5, 5, 0),
                padding = new RectOffset(5, 5, 0, 0),
                alignment = TextAnchor.MiddleLeft,
                normal = {
                    //background = ReorderableListResources.texTitleBackground,
                    //textColor = EditorGUIUtility.isProSkin
                    //    ? new Color(0.8f, 0.8f, 0.8f)
                    //    : new Color(0.2f, 0.2f, 0.2f)
                }
            };

            defaultContainerStyle = new GUIStyle {
                border = new RectOffset(2, 2, 1, 2),
                margin = new RectOffset(5, 5, 5, 5),
                padding = new RectOffset(1, 1, 3, 2),
                normal = { background = ReorderableListResources.texContainerBackground }
            };

            defaultAddButtonStyle = new GUIStyle {
                fixedWidth = 30,
                fixedHeight = 16,
                normal = { background = ReorderableListResources.texAddButtonFlipped },
                active = { background = ReorderableListResources.texAddButtonActiveFlipped }
            };

            defaultAddButtonStyleFlipped = new GUIStyle {
                fixedWidth = 30,
                fixedHeight = 16,
                normal = { background = ReorderableListResources.texAddButton },
                active = { background = ReorderableListResources.texAddButtonActive }
            };

            defaultAddButtonStyleIndependent = new GUIStyle {
                fixedWidth = 30,
                fixedHeight = 16,
                normal = { background = ReorderableListResources.texAddButtonIndependent },
                active = { background = ReorderableListResources.texAddButtonActive }
            };


            defaultRemoveButtonStyle = new GUIStyle {
                fixedWidth = 27,
                active = {
                    background =
                        ReorderableListResources.CreatePixelTexture("Dark Pixel (List GUI)",
                            new Color32(18, 18, 18, 255))
                },
                imagePosition = ImagePosition.ImageOnly,
                alignment = TextAnchor.MiddleCenter
            };

        }
        #endregion

        private static readonly GUIContent s_Temp = new GUIContent();

        #region Title Control

        /// <summary>
        /// Draw title control for list field.
        /// </summary>
        /// <remarks>
        /// <para>When needed, should be shown immediately before list field.</para>
        /// </remarks>
        /// <example>
        /// <code language="csharp"><![CDATA[ ReorderableListGUI.Title(titleContent); ReorderableListGUI.ListField(list, DynamicListGU.TextFieldItemDrawer); ]]></code> <code language="unityscript"><![CDATA[ ReorderableListGUI.Title(titleContent); ReorderableListGUI.ListField(list, DynamicListGU.TextFieldItemDrawer); ]]></code>
        /// </example>
        /// <param name="title">Content for title control.</param>
        public static void Title(GUIContent title) {
            Rect position = GUILayoutUtility.GetRect(title, defaultTitleStyle);
            position.height += 6;
            Title(position, title);
        }

        public static float CalculateTitleHeight() {
            return defaultTitleStyle.CalcHeight(GUIContent.none, 100) + 6;
        }

        /// <summary>
        /// Draw title control for list field.
        /// </summary>
        /// <remarks>
        /// <para>When needed, should be shown immediately before list field.</para>
        /// </remarks>
        /// <example>
        /// <code language="csharp"><![CDATA[ ReorderableListGUI.Title("Your Title"); ReorderableListGUI.ListField(list, DynamicListGU.TextFieldItemDrawer); ]]></code> <code language="unityscript"><![CDATA[ ReorderableListGUI.Title('Your Title'); ReorderableListGUI.ListField(list, DynamicListGU.TextFieldItemDrawer); ]]></code>
        /// </example>
        /// <param name="title">Text for title control.</param>
        public static void Title(string title) {
            s_Temp.text = title;
            Title(s_Temp);
        }

        /// <summary>
        /// Draw title control for list field with absolute positioning.
        /// </summary>
        /// <param name="position">Position of control.</param>
        /// <param name="title">Content for title control.</param>
        public static void Title(Rect position, GUIContent title) {
            EditorGUI.LabelField(position, title);
        }

        /// <summary>
        /// Draw title control for list field with absolute positioning.
        /// </summary>
        /// <param name="position">Position of control.</param>
        /// <param name="text">Text for title control.</param>
        public static void Title(Rect position, string text) {
            s_Temp.text = text;
            Title(position, s_Temp);
        }

        #endregion

        #region Adaptor Control

        /// <summary>
        /// Draw list field control for adapted collection.
        /// </summary>
        /// <param name="adaptor">Reorderable list adaptor.</param>
        /// <param name="drawEmpty">Callback to draw custom content for empty list
        /// (optional).</param>
        /// <param name="flags">Optional flags to pass into list field.</param>
        private static void DoListField(IReorderableListAdaptor adaptor, ReorderableListControl.DrawEmpty drawEmpty, ReorderableListFlags flags) {
            ReorderableListControl.DrawControlFromState(adaptor, drawEmpty, flags);
        }
        /// <summary>
        /// Draw list field control for adapted collection.
        /// </summary>
        /// <param name="position">Position of control.</param>
        /// <param name="adaptor">Reorderable list adaptor.</param>
        /// <param name="drawEmpty">Callback to draw custom content for empty list
        /// (optional).</param>
        /// <param name="flags">Optional flags to pass into list field.</param>
        private static void DoListFieldAbsolute(Rect position, IReorderableListAdaptor adaptor, ReorderableListControl.DrawEmptyAbsolute drawEmpty, ReorderableListFlags flags) {
            ReorderableListControl.DrawControlFromState(position, adaptor, drawEmpty, flags);
        }

        /// <inheritdoc cref="DoListField(IReorderableListAdaptor, ReorderableListControl.DrawEmpty, ReorderableListFlags)"/>
        public static void ListField(IReorderableListAdaptor adaptor, ReorderableListControl.DrawEmpty drawEmpty, ReorderableListFlags flags) {
            DoListField(adaptor, drawEmpty, flags);
        }
        /// <inheritdoc cref="DoListFieldAbsolute(Rect, IReorderableListAdaptor, ReorderableListControl.DrawEmptyAbsolute, ReorderableListFlags)"/>
        public static void ListFieldAbsolute(Rect position, IReorderableListAdaptor adaptor, ReorderableListControl.DrawEmptyAbsolute drawEmpty, ReorderableListFlags flags) {
            DoListFieldAbsolute(position, adaptor, drawEmpty, flags);
        }

        /// <inheritdoc cref="DoListField(IReorderableListAdaptor, ReorderableListControl.DrawEmpty, ReorderableListFlags)"/>
        public static void ListField(IReorderableListAdaptor adaptor, ReorderableListControl.DrawEmpty drawEmpty) {
            DoListField(adaptor, drawEmpty, 0);
        }
        /// <inheritdoc cref="DoListFieldAbsolute(Rect, IReorderableListAdaptor, ReorderableListControl.DrawEmptyAbsolute, ReorderableListFlags)"/>
        public static void ListFieldAbsolute(Rect position, IReorderableListAdaptor adaptor, ReorderableListControl.DrawEmptyAbsolute drawEmpty) {
            DoListFieldAbsolute(position, adaptor, drawEmpty, 0);
        }

        /// <inheritdoc cref="DoListField(IReorderableListAdaptor, ReorderableListControl.DrawEmpty, ReorderableListFlags)"/>
        public static void ListField(IReorderableListAdaptor adaptor, ReorderableListFlags flags) {
            DoListField(adaptor, null, flags);
        }
        /// <inheritdoc cref="DoListFieldAbsolute(Rect, IReorderableListAdaptor, ReorderableListControl.DrawEmptyAbsolute, ReorderableListFlags)"/>
        public static void ListFieldAbsolute(Rect position, IReorderableListAdaptor adaptor, ReorderableListFlags flags) {
            DoListFieldAbsolute(position, adaptor, null, flags);
        }

        /// <inheritdoc cref="DoListField(IReorderableListAdaptor, ReorderableListControl.DrawEmpty, ReorderableListFlags)"/>
        public static void ListField(IReorderableListAdaptor adaptor) {
            DoListField(adaptor, null, 0);
        }
        /// <inheritdoc cref="DoListFieldAbsolute(Rect, IReorderableListAdaptor, ReorderableListControl.DrawEmptyAbsolute, ReorderableListFlags)"/>
        public static void ListFieldAbsolute(Rect position, IReorderableListAdaptor adaptor) {
            DoListFieldAbsolute(position, adaptor, null, 0);
        }

        /// <summary>
        /// Calculate height of list field for adapted collection.
        /// </summary>
        /// <param name="adaptor">Reorderable list adaptor.</param>
        /// <param name="flags">Optional flags to pass into list field.</param>
        /// <returns>Required list height in pixels.</returns>
        public static float CalculateListFieldHeight(IReorderableListAdaptor adaptor, ReorderableListFlags flags) {
            // We need to push/pop flags so that nested controls are properly calculated.
            var restoreFlags = defaultListControl.flags;
            try {
                defaultListControl.flags = flags;
                return defaultListControl.CalculateListHeight(adaptor);
            }
            finally {
                defaultListControl.flags = restoreFlags;
            }
        }

        ///
        ///
        /// <inheritdoc cref="CalculateListFieldHeight(IReorderableListAdaptor, ReorderableListFlags)"/>
        public static float CalculateListFieldHeight(IReorderableListAdaptor adaptor) {
            return CalculateListFieldHeight(adaptor, 0);
        }

        #endregion

    }

}