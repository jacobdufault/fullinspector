using System;
using FullInspector.Internal;
using FullSerializer;
using UnityEngine;

namespace FullInspector {
    public partial class tk<T, TContext> {
        /// <summary>
        /// This will draw a button inside of the given rectangle.
        /// </summary>
        public class Button : tkControl<T, TContext> {
            [ShowInInspector]
            private readonly Value<fiGUIContent> _label;
            private readonly bool _enabled;
            private readonly Action<T, TContext> _onClick;

            public Button(string methodName) {
                InspectedMethod foundMethod = null;
                foreach (var method in InspectedType.Get(typeof(T)).GetMethods(InspectedMemberFilters.All)) {
                    if (method.Method.Name == methodName) {
                        foundMethod = method;
                    }
                }

                if (foundMethod != null) {
                    _label = (fiGUIContent)foundMethod.DisplayLabel;
                    _enabled = true;
                    _onClick = (o, c) => foundMethod.Invoke(o);
                }
                else {
                    Debug.LogError("Unable to find method " + methodName + " on " + typeof(T).CSharpName());
                    _label = new fiGUIContent(methodName + " (unable to find on " + typeof(T).CSharpName() + ")");
                    _enabled = false;
                    _onClick = (o, c) => { };
                }
            }

            public Button(Value<fiGUIContent> label, Action<T, TContext> onClick) {
                _enabled = true;
                _label = label;
                _onClick = onClick;
            }
            public Button(fiGUIContent label, Action<T, TContext> onClick)
                : this(Val(label), onClick) {
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata) {
                fiLateBindings.EditorGUI.BeginDisabledGroup(!_enabled);
                if (GUI.Button(rect, _label.GetCurrentValue(obj, context))) {
                    _onClick(obj, context);
                }
                fiLateBindings.EditorGUI.EndDisabledGroup();
                return obj;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata) {
                return 18;
            }
        }
    }
}