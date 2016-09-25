using System;
using System.Reflection;
using FullInspector.Internal;
using UnityEngine;
using tk = FullInspector.tk<UnityEngine.GUIStyleState>;

namespace FullInspector.Modules {
    [CustomPropertyEditor(typeof(GUIStyleState))]
    public class GUIStyleStatePropertyEditor : tkControlPropertyEditor<GUIStyleState> {
        public GUIStyleStatePropertyEditor(Type dataType, ICustomAttributeProvider attr) : base(dataType) { }

        protected override tkControlEditor GetControlEditor(GUIContent label, GUIStyleState element, fiGraphMetadata graphMetadata) {
            return new tkControlEditor(new tk.VerticalGroup {
                new tk.Foldout(label, FontStyle.Normal, /*defaultToExpanded:*/false,
                    new tk.VerticalGroup {
                        new tk.PropertyEditor("background"),
                        new tk.PropertyEditor("textColor")
                    })
            });
        }
    }
}