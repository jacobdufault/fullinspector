using System;
using System.Reflection;
using FullInspector.Internal;
using FullSerializer;
using UnityEngine;

namespace FullInspector {
    public partial class tk<T, TContext> {
        public class PropertyEditor : tkControl<T, TContext> {
            private MemberInfo _attributes;
            private Func<T, TContext, object> _getValue;
            private Action<T, TContext, object> _setValue;
            private Value<fiGUIContent> _label;
            private Type _fieldType;
            private string _errorMessage;

            private void InitializeFromMemberName(string memberName) {
                var property = InspectedType.Get(typeof(T)).GetPropertyByName(memberName);

                if (property == null) {
                    _errorMessage = "Unable to locate member `" + memberName + "` on type `" + typeof(T).CSharpName() + "`";
                    _fieldType = typeof(T);
                    _attributes = null;
                    _getValue = (o, c) => default(T);
                    _setValue = (o, c, v) => { };
                    _label = new fiGUIContent(memberName + " (unable to locate)");
                    return;
                }

                _fieldType = property.StorageType;
                _attributes = property.MemberInfo;
                _getValue = (o, c) => property.Read(o);
                _setValue = (o, c, v) => property.Write(o, v);
                _label = new fiGUIContent(property.DisplayName);
            }

            public PropertyEditor(string memberName) {
                InitializeFromMemberName(memberName);
            }

            public PropertyEditor(fiGUIContent label, string memberName)
                : this(memberName) {
                _label = label;
            }

            public PropertyEditor(Value<fiGUIContent> label, string memberName)
                : this(memberName) {
                _label = label;
            }

            public PropertyEditor(
                fiGUIContent label,
                Type fieldType, MemberInfo attributes,
                Func<T, TContext, object> getValue, Action<T, TContext, object> setValue) {

                _label = label;
                _fieldType = fieldType;
                _attributes = attributes;
                _getValue = getValue;
                _setValue = setValue;
            }

            public static PropertyEditor Create<TEdited>(fiGUIContent label, MemberInfo attributes,
                Func<T, TContext, TEdited> getValue, Action<T, TContext, TEdited> setValue) {

                return new PropertyEditor(label, typeof(TEdited), attributes, (o, c) => getValue(o, c), (o, c, v) => setValue(o, c, (TEdited)v));
            }

            public static PropertyEditor Create<TEdited>(fiGUIContent label, Func<T, TContext, TEdited> getValue) {
                return new PropertyEditor(label, typeof(TEdited), null, (o, c) => getValue(o, c), null);
            }

            public static PropertyEditor Create<TEdited>(fiGUIContent label, Func<T, TContext, TEdited> getValue, Action<T, TContext, TEdited> setValue) {
                return new PropertyEditor(label, typeof(TEdited), null, (o, c) => getValue(o, c), (o, c, v) => setValue(o, c, (TEdited)v));
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata) {
                if (_errorMessage != null) {
                    fiLateBindings.EditorGUI.HelpBox(rect, _errorMessage, CommentType.Error);
                    return obj;
                }


                fiLateBindings.EditorGUI.BeginChangeCheck();

                fiLateBindings.EditorGUI.BeginDisabledGroup(_setValue == null);

                var val = _getValue(obj, context);
                var childMetadata = new fiGraphMetadataChild { Metadata = GetInstanceMetadata(metadata) };
                var label = _label.GetCurrentValue(obj, context);
                var updated = fiLateBindings.PropertyEditor.Edit(_fieldType, _attributes, rect, label, val, childMetadata);

                fiLateBindings.EditorGUI.EndDisabledGroup();

                if (fiLateBindings.EditorGUI.EndChangeCheck()) {
                    if (_setValue != null) _setValue(obj, context, updated);
                }

                return obj;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata) {
                if (_errorMessage != null) {
                    return fiCommentUtility.GetCommentHeight(_errorMessage, CommentType.Error);
                }

                var val = _getValue(obj, context);
                var childMetadata = new fiGraphMetadataChild { Metadata = GetInstanceMetadata(metadata) };
                var label = _label.GetCurrentValue(obj, context);
                return fiLateBindings.PropertyEditor.GetElementHeight(_fieldType, _attributes, label, val, childMetadata);
            }
        }
    }
}