using FullInspector.Internal;
using FullInspector.Modules;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector {
    /// <summary>
    /// The default behavior editor is used whenever there is not a user-defined one that should be
    /// used instead.
    /// </summary>
    public class DefaultBehaviorEditor : BehaviorEditor<UnityObject> {
        protected override void OnSceneGUI(UnityObject behavior) {
        }

        protected override void OnEdit(Rect rect, UnityObject behavior, fiGraphMetadata metadata) {
            fiGraphMetadataChild childMetadata = metadata.Enter("DefaultBehaviorEditor");
            childMetadata.Metadata.GetPersistentMetadata<fiDropdownMetadata>().ForceDisable();

            // We don't want to get the IObjectPropertyEditor for the given target, which extends
            // UnityObject, so that we can actually edit the property instead of getting a Unity
            // reference field. We also don't want the AbstractTypePropertyEditor, which we will get
            // if the behavior has any derived types.
            PropertyEditorChain editorChain = PropertyEditor.Get(behavior.GetType(), null);
            IPropertyEditor editor = editorChain.SkipUntilNot(
                typeof (IObjectPropertyEditor),
                typeof (AbstractTypePropertyEditor));

            // Run the editor
            editor.Edit(rect, GUIContent.none, behavior, childMetadata);
        }

        protected override float OnGetHeight(UnityObject behavior, fiGraphMetadata metadata) {
            fiGraphMetadataChild childMetadata = metadata.Enter("DefaultBehaviorEditor");
            childMetadata.Metadata.GetPersistentMetadata<fiDropdownMetadata>().ForceDisable();

            float height = 0;

            // We don't want to get the IObjectPropertyEditor for the given target, which extends
            // UnityObject, so that we can actually edit the property instead of getting a Unity
            // reference field. We also don't want the AbstractTypePropertyEditor, which we will get
            // if the behavior has any derived types.
            PropertyEditorChain editorChain = PropertyEditor.Get(behavior.GetType(), null);
            IPropertyEditor editor = editorChain.SkipUntilNot(
                typeof (IObjectPropertyEditor),
                typeof (AbstractTypePropertyEditor));

            height += editor.GetElementHeight(GUIContent.none, behavior, childMetadata);

            return height;
        }

        public static void Edit(Rect rect, UnityObject behavior, fiGraphMetadata metadata) {
            Instance.OnEdit(rect, behavior, metadata);
        }

        public static float GetHeight(UnityObject behavior, fiGraphMetadata metadata) {
            return Instance.OnGetHeight(behavior, metadata);
        }

        private DefaultBehaviorEditor() {
        }

        public static DefaultBehaviorEditor Instance = new DefaultBehaviorEditor();
    }

    /// <summary>
    /// If you wish to just extend the default behavior editor by adding a section before/after it, you
    /// can just extend this class and override the appropriate methods.
    /// </summary>
    public class DefaultBehaviorEditor<TBehavior> : BehaviorEditor<TBehavior>
        where TBehavior : UnityObject {

        protected virtual void OnBeforeEdit(Rect rect, TBehavior behavior, fiGraphMetadata metadata) {
        }

        protected virtual float OnBeforeEditHeight(TBehavior behavior, fiGraphMetadata metadata) {
            return 0;
        }

        protected virtual void OnAfterEdit(Rect rect, TBehavior behavior, fiGraphMetadata metadata) {
        }

        protected virtual float OnAfterEditHeight(TBehavior behavior, fiGraphMetadata metadata) {
            return 0;
        }

        protected override void OnEdit(Rect rect, TBehavior behavior, fiGraphMetadata metadata) {
            float beforeHeight = OnBeforeEditHeight(behavior, metadata);
            float afterHeight = OnAfterEditHeight(behavior, metadata);

            Rect beforeRect = rect;
            beforeRect.height = beforeHeight;

            Rect behaviorRect = rect;
            behaviorRect.y += beforeRect.height;
            behaviorRect.height -= beforeHeight + afterHeight;

            Rect afterRect = rect;
            afterRect.y += beforeRect.height + behaviorRect.height;
            afterRect.height = afterHeight;

            OnBeforeEdit(beforeRect, behavior, metadata);
            DefaultBehaviorEditor.Edit(behaviorRect, behavior, metadata);
            OnAfterEdit(afterRect, behavior, metadata);
        }

        protected override float OnGetHeight(TBehavior behavior, fiGraphMetadata metadata) {
            return
                OnBeforeEditHeight(behavior, metadata) +
                DefaultBehaviorEditor.GetHeight(behavior, metadata) +
                OnAfterEditHeight(behavior, metadata);
        }

        protected override void OnSceneGUI(TBehavior behavior) {
        }
    }
}