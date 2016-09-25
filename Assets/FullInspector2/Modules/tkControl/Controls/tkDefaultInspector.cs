using System;
using FullInspector.Internal;
using FullSerializer.Internal;
using UnityEngine;

namespace FullInspector {
    public partial class tk<T, TContext> {
        /// <summary>
        /// Draws the default inspector for the given type.
        /// </summary>
        public class DefaultInspector : tkControl<T, TContext> {
            private readonly Type type_fitkControlPropertyEditor = fsTypeCache.GetType("FullInspector.Internal.tkControlPropertyEditor");
            private readonly Type type_IObjectPropertyEditor = fsTypeCache.GetType("FullInspector.Modules.Common.IObjectPropertyEditor");

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata) {
                return (T)fiLateBindings.PropertyEditor.EditSkipUntilNot(new [] {
                    type_fitkControlPropertyEditor, type_IObjectPropertyEditor
                }, typeof(T), typeof(T).Resolve(), rect, GUIContent.none, obj, new fiGraphMetadataChild { Metadata = metadata });
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata) {
                return fiLateBindings.PropertyEditor.GetElementHeightSkipUntilNot(new[] {
                    type_fitkControlPropertyEditor, type_IObjectPropertyEditor
                }, typeof(T), typeof(T).Resolve(), GUIContent.none, obj, new fiGraphMetadataChild { Metadata = metadata });
            }
        }
    }
}