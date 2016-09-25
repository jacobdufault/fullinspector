using FullInspector.Internal;
using UnityEngine;

namespace FullInspector {
    public partial class tk<T, TContext> {
        /// <summary>
        /// This will draw the child control with a dropdown arrow next to it.
        /// </summary>
        public class Foldout : tkControl<T, TContext> {
            private readonly GUIStyle _foldoutStyle;

            [ShowInInspector]
            private readonly fiGUIContent _label;

            [ShowInInspector]
            private readonly tkControl<T, TContext> _control;

            [ShowInInspector]
            private readonly bool _defaultToExpanded;

            /// <summary>
            /// Should the child control be indented? This defaults to true.
            /// </summary>
            [ShowInInspector]
            public bool IndentChildControl {
                get { return !_doNotIndentChildControl; }
                set { _doNotIndentChildControl = !value; }
            }
            private bool _doNotIndentChildControl;

            /// <summary>
            /// Should we force a setting for hierarchy mode? If this is not set, then the existing
            /// value for hierarchy mode will be used.
            /// </summary>
            /// <remarks>The hierarchy mode determines if the foldout is indented or not. If hierarchy
            /// mode is true, then we will *not* indent the label next to the foldout dropdown button.</remarks>
            public bool? HierarchyMode;

            public Foldout(fiGUIContent label, tkControl<T, TContext> control)
                : this(label, FontStyle.Normal, control) {
            }

            public Foldout(fiGUIContent label, FontStyle fontStyle, tkControl<T, TContext> control)
                : this(label, fontStyle, true, control) {
            }

            public Foldout(fiGUIContent label, FontStyle fontStyle, bool defaultToExpanded, tkControl<T, TContext> control) {
                _label = label;
                _foldoutStyle = new GUIStyle(fiLateBindings.EditorStyles.foldout)
                {
                    fontStyle = fontStyle
                };
                _defaultToExpanded = defaultToExpanded;
                _control = control;
            }

            private tkFoldoutMetadata GetMetadata(fiGraphMetadata metadata) {
                bool wasCreated;
                var foldout = GetInstanceMetadata(metadata).GetPersistentMetadata<tkFoldoutMetadata>(out wasCreated);

                if (wasCreated) foldout.IsExpanded = _defaultToExpanded;

                return foldout;
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata) {
                var foldout = GetMetadata(metadata);

                if (HierarchyMode.HasValue) {
                    fiLateBindings.fiEditorGUI.PushHierarchyMode(HierarchyMode.Value);
                }

                Rect foldoutRect = rect;
                foldoutRect.height = fiLateBindings.EditorGUIUtility.singleLineHeight;
                foldout.IsExpanded = fiLateBindings.EditorGUI.Foldout(foldoutRect, foldout.IsExpanded, _label, /*toggleOnLabelClick:*/true, _foldoutStyle);

                if (foldout.IsExpanded) {
                    var delta = fiLateBindings.EditorGUIUtility.singleLineHeight + fiLateBindings.EditorGUIUtility.standardVerticalSpacing;

                    Rect controlRect = rect;
                    if (IndentChildControl) {
                        controlRect.x += fiRectUtility.IndentHorizontal;
                        controlRect.width -= fiRectUtility.IndentHorizontal;
                    }
                    controlRect.y += delta;
                    controlRect.height -= delta;

                    obj = _control.Edit(controlRect, obj, context, metadata);
                }

                if (HierarchyMode.HasValue) {
                    fiLateBindings.fiEditorGUI.PopHierarchyMode();
                }

                return obj;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata) {
                var foldout = GetMetadata(metadata);

                float height = fiLateBindings.EditorGUIUtility.singleLineHeight;

                if (foldout.IsExpanded) {
                    height += fiLateBindings.EditorGUIUtility.standardVerticalSpacing;
                    height += _control.GetHeight(obj, context, metadata);
                }

                return height;
            }
        }
    }
}