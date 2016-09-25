using System;
using System.Reflection;
using FullInspector.Internal;
using UnityEngine;
using tk = FullInspector.tk<UnityEngine.RectOffset>;

namespace FullInspector.Modules {
    [CustomPropertyEditor(typeof(RectOffset))]
    public class RectOffsetPropertyEditor : tkControlPropertyEditor<RectOffset> {
        public RectOffsetPropertyEditor(Type dataType, ICustomAttributeProvider attr) : base(dataType) { }

        protected override tkControlEditor GetControlEditor(GUIContent label, RectOffset element, fiGraphMetadata graphMetadata) {
            return new tkControlEditor(new tk.VerticalGroup {
                new tk.Label(label,
                    new tk.VerticalGroup {
                        new tk.PropertyEditor("bottom"),
                        new tk.PropertyEditor("left"),
                        new tk.PropertyEditor("right"),
                        new tk.PropertyEditor("top"),
                    })
                });
        }
    }
}