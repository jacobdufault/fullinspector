using FullInspector.Internal;
using FullSerializer;
using System;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules {
    [CustomBehaviorEditor(typeof(SharedInstance<,>), Inherit = true)]
    public class fiSharedInstanceBehaviorEditor<TActual, T, TSerializer> : BehaviorEditor<SharedInstance<T, TSerializer>>
        where TSerializer : BaseSerializer {

        protected override void OnEdit(Rect rect, SharedInstance<T, TSerializer> behavior, fiGraphMetadata metadata) {
            behavior.Instance = PropertyEditor.Get(typeof(T), null).FirstEditor.Edit(rect, GUIContent.none, behavior.Instance, metadata.Enter("Instance"));
        }

        protected override float OnGetHeight(SharedInstance<T, TSerializer> behavior, fiGraphMetadata metadata) {
            return PropertyEditor.Get(typeof(T), null).FirstEditor.GetElementHeight(GUIContent.none, behavior.Instance, metadata.Enter("Instance"));
        }

        protected override void OnSceneGUI(SharedInstance<T, TSerializer> behavior) {
        }
    }

    [CustomPropertyEditor(typeof(SharedInstance<,>), DisableErrorOnUnityObject = true, Inherit = true)]
    public class fiSharedInstancePropertyEditor<TActual, T, TSerializer> : PropertyEditor<SharedInstance<T, TSerializer>>
        where TSerializer : BaseSerializer {

        // This property editor is rather strange. It works in two different modes based on the type of TSerializer. Because it is inherited (so that it
        // properly supports SharedInstance<>) it will be invoked by the parent editor (as expected) *but also* by the object property editor when an
        // inline view is requested (because the inline view is requested for the actual object type, but that editor is *also* this one). We can detect
        // the two scenarios by looking at TActual; when TActual is generic, we are being invoked for the standard view; when TActual is not generic,
        // we are being invoked as an inline editor.

        public class SharedInstanceMetadata : IGraphMetadataItemNotPersistent {
            [fsIgnore]
            public fiOption<SharedInstance<T, TSerializer>> UpdatedInstance;
        }

        private static void TryEnsureScript() {
            Type actualScriptableObjectType = fiSharedInstanceUtility.GetSerializableType(typeof(SharedInstance<T, TSerializer>));
            if (actualScriptableObjectType == null) {
                if (typeof(TActual).GetGenericTypeDefinition() == typeof(SharedInstance<,>)) {
                    fiSharedInstanceScriptGenerator.GenerateScript(typeof(T), typeof(TSerializer));
                }
                // assumed to be SharedInstance<>
                else if (typeof(TActual).GetGenericArguments().Length == 1) {
                    fiSharedInstanceScriptGenerator.GenerateScript(typeof(T), null);
                }
                else {
                    Debug.LogError("SharedInstance generator does not know how to generate a derived type for " + typeof(TActual));
                }
                return;
            }
        }

        public override SharedInstance<T, TSerializer> Edit(Rect region, GUIContent label, SharedInstance<T, TSerializer> element, fiGraphMetadata metadata) {
            TryEnsureScript();

            if (typeof(TActual).IsGenericType) {
                region = EditorGUI.PrefixLabel(region, label);


                float ButtonRectWidth = 23;
                Rect buttonRect = region, objectRect = region;

                buttonRect.x += buttonRect.width - ButtonRectWidth;
                buttonRect.width = ButtonRectWidth;
                buttonRect.height = EditorGUIUtility.singleLineHeight;

                objectRect.width -= buttonRect.width;

                if (GUI.Button(buttonRect, new GUIContent("\u2261"))) {
                    fiSharedInstanceSelectorWindow.Show(typeof(T), typeof(SharedInstance<T, TSerializer>),
                        instance => {
                            metadata.GetMetadata<SharedInstanceMetadata>().UpdatedInstance = fiOption.Just((SharedInstance<T, TSerializer>)instance);
                        });
                }

                fiEditorGUI.PushHierarchyMode(false);
                // Use the standard object property editor
                element = EditorChain.GetNextEditor(this).Edit(objectRect, GUIContent.none, element, metadata.Enter("ObjectReference"));
                fiEditorGUI.PopHierarchyMode();
            }
            else {
                if (element != null) {
                    fiEditorGUI.PushHierarchyMode(false);
                    element.Instance = PropertyEditor.Get(typeof(T), null).FirstEditor.Edit(region, new GUIContent("Instance"), element.Instance, new fiGraphMetadataChild { Metadata = metadata });
                    fiEditorGUI.PopHierarchyMode();
                }
            }

            var sharedInstanceMetadata = metadata.GetMetadata<SharedInstanceMetadata>();
            if (sharedInstanceMetadata.UpdatedInstance.HasValue) {
                element = sharedInstanceMetadata.UpdatedInstance.Value;
                sharedInstanceMetadata.UpdatedInstance = fiOption<SharedInstance<T, TSerializer>>.Empty;
                GUI.changed = true;
            }

            return element;
        }

        public override float GetElementHeight(GUIContent label, SharedInstance<T, TSerializer> element, fiGraphMetadata metadata) {
            if (typeof(TActual).IsGenericType == false) {
                return PropertyEditor.Get(typeof(T), null).FirstEditor.GetElementHeight(new GUIContent("Instance"), element.Instance, new fiGraphMetadataChild { Metadata = metadata });
            }


            TryEnsureScript();
            return EditorChain.GetNextEditor(this).GetElementHeight(label, element, metadata.Enter("ObjectReference"));
        }
    }
}