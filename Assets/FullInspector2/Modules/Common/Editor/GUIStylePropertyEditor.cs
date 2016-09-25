using System;
using System.Reflection;
using FullInspector.Internal;
using UnityEngine;
using tk = FullInspector.tk<UnityEngine.GUIStyle>;

namespace FullInspector.Modules {
    [CustomPropertyEditor(typeof(GUIStyle))]
    public class GUIStylePropertyEditor : tkControlPropertyEditor<GUIStyle> {
        public GUIStylePropertyEditor(Type dataType, ICustomAttributeProvider attr) : base(dataType) { }

        protected override tkControlEditor GetControlEditor(GUIContent label, GUIStyle element, fiGraphMetadata graphMetadata) {
            return new tkControlEditor(new tk.VerticalGroup {
                new tk.Label(label),

                new tk.Indent(new tk.VerticalGroup {
                    new tk.PropertyEditor("active"),
                    new tk.PropertyEditor("alignment"),
                    new tk.PropertyEditor("border"),
                    new tk.PropertyEditor("clipping"),
                    new tk.PropertyEditor("contentOffset"),
                    new tk.PropertyEditor("fixedHeight"),
                    new tk.PropertyEditor("fixedWidth"),
                    new tk.PropertyEditor("focused"),
                    new tk.PropertyEditor("font"),
                    new tk.PropertyEditor("fontSize"),
                    new tk.PropertyEditor("fontStyle"),
                    new tk.PropertyEditor("hover"),
                    new tk.PropertyEditor("imagePosition"),
                    new tk.PropertyEditor("margin"),
                    new tk.PropertyEditor("name"),
                    new tk.PropertyEditor("normal"),
                    new tk.PropertyEditor("onActive"),
                    new tk.PropertyEditor("onFocused"),
                    new tk.PropertyEditor("onHover"),
                    new tk.PropertyEditor("onNormal"),
                    new tk.PropertyEditor("overflow"),
                    new tk.PropertyEditor("padding"),
                    new tk.PropertyEditor("richText"),
                    new tk.PropertyEditor("stretchHeight"),
                    new tk.PropertyEditor("stretchWidth"),
                    new tk.PropertyEditor("wordWrap")
                })
                /*
                public GUIStyleState active { get; set; }
                public TextAnchor alignment { get; set; }
                public RectOffset border { get; set; }
                [Obsolete("warning Don't use clipOffset - put things inside BeginGroup instead. This functionality will be removed in a later version.")]
                public Vector2 clipOffset { get; set; }
                public TextClipping clipping { get; set; }
                public Vector2 contentOffset { get; set; }
                public float fixedHeight { get; set; }
                public float fixedWidth { get; set; }
                public GUIStyleState focused { get; set; }
                public Font font { get; set; }
                public int fontSize { get; set; }
                public FontStyle fontStyle { get; set; }
                public GUIStyleState hover { get; set; }
                public ImagePosition imagePosition { get; set; }
                public bool isHeightDependantOnWidth { get; }
                public float lineHeight { get; }
                public RectOffset margin { get; set; }
                public string name { get; set; }
                public GUIStyleState normal { get; set; }
                public GUIStyleState onActive { get; set; }
                public GUIStyleState onFocused { get; set; }
                public GUIStyleState onHover { get; set; }
                public GUIStyleState onNormal { get; set; }
                public RectOffset overflow { get; set; }
                public RectOffset padding { get; set; }
                public bool richText { get; set; }
                public bool stretchHeight { get; set; }
                public bool stretchWidth { get; set; }
                public bool wordWrap { get; set; }
                */
            });
        }
    }
}