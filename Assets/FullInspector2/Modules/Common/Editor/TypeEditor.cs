using System;
using FullInspector.Internal;
using FullSerializer;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules {
    [CustomPropertyEditor(typeof(Type))]
    public class TypePropertyEditor : PropertyEditor<Type> {
        public class StateObject : IGraphMetadataItemNotPersistent {
            public fiOption<Type> Type;
        }

        public override Type Edit(Rect region, GUIContent label, Type element, fiGraphMetadata metadata) {
            Rect labelRect, buttonRect = region;

            if (string.IsNullOrEmpty(label.text) == false) {
                fiRectUtility.SplitHorizontalPercentage(region, .3f, 2, out labelRect, out buttonRect);
                GUI.Label(labelRect, label);
            }

            string displayed = "<no type>";
            if (element != null) {
                displayed = element.CSharpName();
            }

            StateObject stateObj = metadata.GetMetadata<StateObject>();

            if (GUI.Button(buttonRect, displayed)) {
                TypeSelectionPopupWindow.CreateSelectionWindow(element, type => stateObj.Type = fiOption.Just(type));
            }

            if (stateObj.Type.HasValue) {
                GUI.changed = true;
                var type = stateObj.Type.Value;
                stateObj.Type = fiOption<Type>.Empty;
                return type;
            }

            return element;
        }

        public override float GetElementHeight(GUIContent label, Type element, fiGraphMetadata metadata) {
            return EditorStyles.toolbarButton.CalcHeight(label, Screen.width);
        }
    }
}
