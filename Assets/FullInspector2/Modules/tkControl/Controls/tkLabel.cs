using FullInspector.Internal;
using UnityEngine;

namespace FullInspector {
    public partial class tk<T, TContext> {
        /// <summary>
        /// Draws a label
        /// </summary>
        public class Label : tkControl<T, TContext> {
            public Value<fiGUIContent> GUIContent;
            [ShowInInspector]
            private readonly FontStyle _fontStyle;
            [ShowInInspector]
            private readonly tkControl<T, TContext> _control;

            /// <summary>
            /// If this label has an associated control, should it be rendered immediately
            /// next to the control? If this is the default value (false), then the control
            /// will be rendered *below* the existing label with an indent.
            /// </summary>
            public bool InlineControl;

            public Label(fiGUIContent label) :
                this(label, FontStyle.Normal, null) {
            }
            public Label(Value<fiGUIContent> label) :
                this(label, FontStyle.Normal, null) {
            }
            public Label(Value<fiGUIContent>.Generator label) :
                this(label, FontStyle.Normal, null) {
            }

            public Label(fiGUIContent label, FontStyle fontStyle) :
                this(label, fontStyle, null) {
            }
            public Label(Value<fiGUIContent> label, FontStyle fontStyle) :
                this(label, fontStyle, null) {
            }
            public Label(Value<fiGUIContent>.Generator label, FontStyle fontStyle) :
                this(label, fontStyle, null) {
            }

            public Label(fiGUIContent label, tkControl<T, TContext> control) :
                this(label, FontStyle.Normal, control) {
            }
            public Label(Value<fiGUIContent> label, tkControl<T, TContext> control) :
                this(label, FontStyle.Normal, control) {
            }
            public Label(Value<fiGUIContent>.Generator label, tkControl<T, TContext> control) :
                this(label, FontStyle.Normal, control) {
            }

            public Label(fiGUIContent label, FontStyle fontStyle, tkControl<T, TContext> control)
                : this(Val(label), fontStyle, control) {
            }
            public Label(Value<fiGUIContent> label, FontStyle fontStyle, tkControl<T, TContext> control) {
                GUIContent = label;
                _fontStyle = fontStyle;
                _control = control;
            }
            public Label(Value<fiGUIContent>.Generator label, FontStyle fontStyle, tkControl<T, TContext> control)
                : this(Val(label), fontStyle, control) {
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata) {
                var guiContent = GUIContent.GetCurrentValue(obj, context);

                Rect labelRect = rect;
                Rect controlRect = rect;
                bool pushedLabelWidth = false;

                if (_control != null && guiContent.IsEmpty == false) {
                    labelRect.height = fiLateBindings.EditorGUIUtility.singleLineHeight;

                    if (InlineControl) {
                        labelRect.width = fiGUI.PushLabelWidth(guiContent, labelRect.width);
                        pushedLabelWidth = true;
                        controlRect.x += labelRect.width;
                        controlRect.width -= labelRect.width;
                    }
                    else {
                        var deltaY = labelRect.height + fiLateBindings.EditorGUIUtility.standardVerticalSpacing;
                        controlRect.x += fiRectUtility.IndentHorizontal;
                        controlRect.width -= fiRectUtility.IndentHorizontal;
                        controlRect.y += deltaY;
                        controlRect.height -= deltaY;
                    }
                }

                if (guiContent.IsEmpty == false) {
                    var style = fiLateBindings.EditorStyles.label;
                    var savedFontStyle = style.fontStyle;
                    style.fontStyle = _fontStyle;
                    GUI.Label(labelRect, guiContent, style);
                    style.fontStyle = savedFontStyle;
                }

                if (_control != null) {
                    _control.Edit(controlRect, obj, context, metadata);
                }

                if (pushedLabelWidth) fiGUI.PopLabelWidth();

                return obj;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata) {
                float height = 0;

                if (GUIContent.GetCurrentValue(obj, context).IsEmpty == false) {
                    height += fiLateBindings.EditorGUIUtility.singleLineHeight;
                }

                if (_control != null) {
                    var controlHeight = _control.GetHeight(obj, context, metadata);
                    if (InlineControl == false) {
                        height += fiLateBindings.EditorGUIUtility.standardVerticalSpacing + controlHeight;
                    }
                }

                return height;
            }
        }
    }
}