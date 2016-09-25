using FullInspector.LayoutToolkit;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules {
    [CustomPropertyEditor(typeof(Rect))]
    public class RectPropertyEditor : PropertyEditor<Rect> {
        private static fiLayout LayoutWithLabel;
        private static fiLayout LayoutWithoutLabel;

        static RectPropertyEditor() {
            var labelHeight = EditorStyles.label.CalcHeight(GUIContent.none, 0);

            LayoutWithLabel = new fiVerticalLayout {
                { "Label", labelHeight },

                new fiHorizontalLayout {
                    15,

                    new fiVerticalLayout {
                        { "Position", labelHeight },
                        2,
                        { "Size", labelHeight },
                    }
                }
            };

            LayoutWithoutLabel = new fiVerticalLayout {
                new fiVerticalLayout {
                    { "Position", labelHeight },
                    2,
                    { "Size", labelHeight },
                }
            };
        }

        public override Rect Edit(Rect region, GUIContent label, Rect element, fiGraphMetadata metadata) {
            var layout = string.IsNullOrEmpty(label.text) ? LayoutWithoutLabel : LayoutWithLabel;

            EditorGUI.LabelField(layout.GetSectionRect("Label", region), label);

            var position = new Vector2(element.xMin, element.yMin);
            var size = new Vector2(element.xMax - element.xMin, element.yMax - element.yMin);

            position = EditorGUI.Vector2Field(layout.GetSectionRect("Position", region), "Position", position);
            size = EditorGUI.Vector2Field(layout.GetSectionRect("Size", region), "Size", size);

            element.xMin = position.x;
            element.yMin = position.y;
            element.xMax = position.x + size.x;
            element.yMax = position.y + size.y;

            return element;
        }

        public override float GetElementHeight(GUIContent label, Rect element, fiGraphMetadata metadata) {
            var layout = string.IsNullOrEmpty(label.text) ? LayoutWithoutLabel : LayoutWithLabel;
            return layout.Height;
        }
    }
}