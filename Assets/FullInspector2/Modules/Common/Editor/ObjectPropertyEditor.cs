using FullInspector.Internal;
using FullInspector.LayoutToolkit;
using FullSerializer;
using FullSerializer.Internal;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Modules {
    /// <summary>
    /// Used to remove the generic arguments from ObjectPropertyEditor so that it can be used as a
    /// "banned" argument for PropertyEditor.Get
    /// </summary>
    public interface IObjectPropertyEditor {
    }

    /// <summary>
    /// Provides an ObjectField for every type which derives from Object.
    /// </summary>
    /// <typeparam name="ObjectType">The actual type of the derived parameter</typeparam>
    [CustomPropertyEditor(typeof(UnityObject), Inherit = true, DisableErrorOnUnityObject = true)]
    public class ObjectPropertyEditor<ObjectType> : PropertyEditor<UnityObject>, IObjectPropertyEditor
        where ObjectType : UnityObject {

        private static float FoldoutHeight = EditorStyles.foldout.CalcHeight(GUIContent.none, 100);

        private static void DisableFoldoutByDefault(object obj, fiGraphMetadata metadata) {
            metadata.GetPersistentMetadata<fiDropdownMetadata>().ForceDisable();
        }

        private static bool CanDisplayDropdown(UnityObject obj) {
            if (obj == null) return false;
            if (fiSettings.ForceDisplayInlineObjectEditor) return true;

            return
                obj is CommonBaseBehavior ||
                obj is CommonBaseScriptableObject ||
                obj is tkCustomEditor ||
                fsPortableReflection.HasAttribute<fiInspectorOnlyAttribute>(obj.GetType()) ||
                BehaviorEditor.Get(obj.GetType()) is DefaultBehaviorEditor == false;
        }

        private static fiLayout DisplayedItemLayout;
        private static fiLayoutHeight DynamicItemHeight;

        static ObjectPropertyEditor() {
            DynamicItemHeight = new fiLayoutHeight(0);

            DisplayedItemLayout =
                fiLayoutUtility.Margin(4, new fiVerticalLayout {
                    {
                        "Box", fiLayoutUtility.Margin(4, new fiHorizontalLayout {
                            { "Item", DynamicItemHeight },
                            3
                        })
                    }
                });
        }

        // Should ObjectFields display scene objects?
        private bool AllowSceneObjects(UnityObject element) {
            return true;
        }

        public override UnityObject Edit(Rect region, GUIContent label, UnityObject element, fiGraphMetadata metadata) {
            if (CanDisplayDropdown(element) == false) {
                region.height = FoldoutHeight;
                return fiEditorGUI.ObjectField(region, label, element, typeof(ObjectType), AllowSceneObjects(element));
            }

            // We have to show a foldout.

            DisableFoldoutByDefault(element, metadata);

            // The rect for the foldout
            Rect foldoutRect = region;
            foldoutRect.height = FoldoutHeight;
            if (string.IsNullOrEmpty(label.text) == false) {
                foldoutRect.width = EditorGUIUtility.labelWidth;
            }
            else {
                foldoutRect.width = 15;
            }

            Rect objectRect = region;
            objectRect.x += foldoutRect.width;
            objectRect.width -= foldoutRect.width;
            objectRect.height = FoldoutHeight;

            var foldoutState = metadata.GetMetadata<ObjectFoldoutStateGraphMetadata>();
            bool updatedActive = EditorGUI.Foldout(foldoutRect, foldoutState.IsActive, label, /*toggleOnLabelClick:*/true);
            if (updatedActive != foldoutState.IsActive && foldoutState.IsAnimating == false)
                foldoutState.IsActive = updatedActive;

            element = fiEditorGUI.ObjectField(objectRect, GUIContent.none, element, typeof(ObjectType), AllowSceneObjects(element));

            if (element != null && (foldoutState.IsActive || foldoutState.IsAnimating)) {
                Rect subRect = new Rect(region);
                subRect.y += FoldoutHeight;
                subRect.height -= FoldoutHeight;

                fiEditorGUI.BeginFadeGroup(0, ref subRect, foldoutState.AnimPercentage);

                // Reuse the height calculation from GetHeight from the BehaviorEditor by
                // calculating the base height of the layout when the dynamic item does not
                // contribute.
                DynamicItemHeight.SetHeight(0);
                DynamicItemHeight.SetHeight(subRect.height - DisplayedItemLayout.Height);

                Rect boxRect = DisplayedItemLayout.GetSectionRect("Box", subRect);
                Rect propertyRect = DisplayedItemLayout.GetSectionRect("Item", subRect);

                GUI.Box(boxRect, GUIContent.none);

                fiEditorGUI.PushHierarchyMode(false);

                var editor = BehaviorEditor.Get(element.GetType());
                editor.Edit(propertyRect, element);

                fiEditorGUI.PopHierarchyMode();

                fiEditorGUI.EndFadeGroup();

                if (foldoutState.IsAnimating) {
                    fiEditorUtility.RepaintAllEditors();
                }
            }

            return element;
        }

        public override float GetElementHeight(GUIContent label, UnityObject element, fiGraphMetadata metadata) {
            float height = FoldoutHeight;

            if (CanDisplayDropdown(element)) {
                DisableFoldoutByDefault(element, metadata);

                var foldoutState = metadata.GetMetadata<ObjectFoldoutStateGraphMetadata>();
                if (foldoutState.IsActive || foldoutState.IsAnimating) {
                    var faded = foldoutState.AnimPercentage;
                    var editor = BehaviorEditor.Get(element.GetType());

                    DynamicItemHeight.SetHeight(editor.GetHeight(element));

                    float itemHeight = DisplayedItemLayout.Height;
                    fiEditorGUI.UpdateFadeGroupHeight(ref itemHeight, 0, faded);
                    height += itemHeight;

                    DynamicItemHeight.SetHeight(0);
                }
            }

            return height;
        }
    }
}