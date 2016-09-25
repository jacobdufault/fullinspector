using FullInspector.Internal;
using UnityEngine;

namespace FullInspector {
    partial class tk<T, TContext> {
        /// <summary>
        /// Disables hierarchy mode for the given subcontrol. You can use this if you need foldouts to be
        /// not move to the left of the labels and instead go inside of the controls they represent.
        /// </summary>
        public class DisableHierarchyMode : tkControl<T, TContext> {
            private tkControl<T, TContext> _childControl;

            public DisableHierarchyMode(tkControl<T, TContext> childControl) {
                _childControl = childControl;
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata) {
                fiLateBindings.fiEditorGUI.PushHierarchyMode(false);
                var result = _childControl.Edit(rect, obj, context, metadata);
                fiLateBindings.fiEditorGUI.PopHierarchyMode();
                return result;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata) {
                return _childControl.GetHeight(obj, context, metadata);
            }
        }
    }
}