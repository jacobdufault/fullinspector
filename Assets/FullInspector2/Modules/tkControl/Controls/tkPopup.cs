using FullInspector.Internal;
using UnityEngine;

namespace FullInspector {
    public partial class tk<T, TContext> {
        public class Popup : tkControl<T, TContext> {
            private readonly Value<fiGUIContent> _label;
            private readonly Value<GUIContent[]> _options;
            private readonly Value<int> _currentSelection;
            private readonly OnSelectionChanged _onSelectionChanged;

            public delegate T OnSelectionChanged(T obj, TContext context, int selected);

            public Popup(Value<fiGUIContent> label, Value<GUIContent[]> options, Value<int> currentSelection, OnSelectionChanged onSelectionChanged) {
                _label = label;
                _options = options;
                _currentSelection = currentSelection;
                _onSelectionChanged = onSelectionChanged;
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata) {
                var label = _label.GetCurrentValue(obj, context);
                int selected = _currentSelection.GetCurrentValue(obj, context);
                var options = _options.GetCurrentValue(obj, context);
                int updated = fiLateBindings.EditorGUI.Popup(rect, label.AsGUIContent, selected, options);

                if (selected != updated) {
                    obj = _onSelectionChanged(obj, context, updated);
                }

                return obj;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata) {
                return fiLateBindings.EditorGUIUtility.singleLineHeight;
            }
        }
    }
}