using System;
using FullInspector.Internal;
using UnityEngine;

namespace FullInspector.Modules {
    [CustomPropertyEditor(typeof(Gradient))]
    public class GradientPropertyEditor : fiGenericPropertyDrawerPropertyEditor<GradientMonoBehaviourStorage, Gradient> {
        public override bool CanEdit(Type type) {
            return typeof(Gradient).IsAssignableFrom(type);
        }
    }
}
