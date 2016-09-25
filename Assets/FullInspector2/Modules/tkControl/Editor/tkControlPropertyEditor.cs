using System;
using System.Reflection;
using FullSerializer;
using FullSerializer.Internal;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    /// <summary>
    /// A standard tkControlPropertyEditor except with some more appropriate values popualted.
    /// </summary>
    public abstract class tkControlPropertyEditor<TEdited> : tkControlPropertyEditor {
        public tkControlPropertyEditor(Type dataType) : base(dataType) { }

        public override bool CanEdit(Type dataType) {
            return typeof(TEdited).IsAssignableFrom(dataType);
        }

        protected override object CreateInstance() {
            return InspectedType.Get(typeof(TEdited)).CreateInstance();
        }

        protected sealed override tkControlEditor GetControlEditor(GUIContent label, object element, fiGraphMetadata graphMetadata) {
            return GetControlEditor(label, (TEdited)element, graphMetadata);
        }

        protected abstract tkControlEditor GetControlEditor(GUIContent label, TEdited element, fiGraphMetadata graphMetadata);
    }

    /// <summary>
    /// Derive from this class if you wish to write a custom property editor that is rendered
    /// from a tkControl.
    /// </summary>
    /// <remarks>You probably want to derive from tkControlPropertyEditor{TEdited}</remarks>
    public class tkControlPropertyEditor : IPropertyEditor, IPropertyEditorEditAPI {
        public class fiLayoutPropertyEditorMetadata : IGraphMetadataItemNotPersistent {
            [fsIgnore]
            public tkControlEditor Layout;
        }

        private Type _dataType;
        public tkControlPropertyEditor(Type dataType) {
            _dataType = dataType;
        }

        private static T InvokeInstanceOrStaticMethod<T>(string[] methodNames, Type type, object instance) {
            foreach (var methodName in methodNames) {
                foreach (var method in type.GetFlattenedMethods(methodName)) {
                    if (instance == null && method.IsStatic) return (T)method.Invoke(instance, null);
                    if (instance != null && method.IsStatic == false) return (T)method.Invoke(instance, null);
                }
            }

            return default(T);
        }

        protected virtual tkControlEditor GetControlEditor(GUIContent label, object element, fiGraphMetadata graphMetadata) {
            fiLayoutPropertyEditorMetadata metadata;
            if (graphMetadata.TryGetMetadata(out metadata) == false) {
                metadata = graphMetadata.GetMetadata<fiLayoutPropertyEditorMetadata>();
                metadata.Layout = InvokeInstanceOrStaticMethod<tkControlEditor>(new[] { "GetEditor", "FullInspector.tkCustomEditor.GetEditor" }, _dataType, element);
                metadata.Layout.Context = Activator.CreateInstance(metadata.Layout.Control.ContextType);
            }

            return metadata.Layout;
        }

        public PropertyEditorChain EditorChain {
            get;
            set;
        }

        public virtual bool CanEdit(Type dataType) {
            // TODO: this doesn't need to be overridable; do what the default control does
            throw new NotSupportedException();
        }

        protected virtual object CreateInstance() {
            if (_dataType.IsAbstract || _dataType.IsInterface || _dataType.IsGenericTypeDefinition) return null;

            try {
                return Activator.CreateInstance(_dataType);
            }
            catch (Exception) {
            }

            return null;
        }

        public object Edit(Rect region, GUIContent label, object element, fiGraphMetadata metadata) {
            if (element == null) element = CreateInstance();
            var editor = GetControlEditor(label, element, metadata);
            return fiEditorGUI.tkControl(region, label, element, metadata, editor);
        }

        public float GetElementHeight(GUIContent label, object element, fiGraphMetadata metadata) {
            if (element == null) element = CreateInstance();
            var editor = GetControlEditor(label, element, metadata);
            return fiEditorGUI.tkControlHeight(label, element, metadata, editor);
        }

        public GUIContent GetFoldoutHeader(GUIContent label, object element) {
            return label;
        }

        public object OnSceneGUI(object element) {
            return element;
        }

        public bool DisplaysStandardLabel {
            get { return false; }
        }

        public static IPropertyEditor TryCreate(Type dataType, ICustomAttributeProvider attributes) {
            if (typeof(UnityObject).IsAssignableFrom(dataType) ||
                typeof(tkCustomEditor).IsAssignableFrom(dataType) == false) {

                return null;
            }

            return new tkControlPropertyEditor(dataType);
        }
    }
}
