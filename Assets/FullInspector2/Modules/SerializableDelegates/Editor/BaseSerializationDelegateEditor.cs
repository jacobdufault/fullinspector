using FullInspector.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Modules {
    public abstract class BaseSerialiationInvokableEditor<TSerializationDelegate> : PropertyEditor<TSerializationDelegate>
        where TSerializationDelegate : BaseSerializationDelegate, new() {

        /// <summary>
        /// The amount of space between the object selector and the method selection popup.
        /// </summary>
        private const float DividerHeight = 1f;

        /// <summary>
        /// Amount of space after the method selection popup that will separate this element from
        /// the next one.
        /// </summary>
        private const float EndSeparatorHeight = 2f;

        /// <summary>
        /// Adaptor method to determine if the given method should be shown in the method dropdown.
        /// </summary>
        protected abstract bool IsValidMethod(MethodInfo method);

        /// <summary>
        /// Ensures that action is not null.
        /// </summary>
        private static void EnsureInstance(ref TSerializationDelegate action) {
            if (action == null) {
                action = new TSerializationDelegate();
            }
        }

        /// <summary>
        /// Splits the general region rect into the two object and popup rects.
        /// </summary>
        private static void SplitRects(Rect region, out Rect objectRect, out Rect popupRect) {
            objectRect = region;
            objectRect.height = EditorStyles.objectField.CalcHeight(GUIContent.none, 100);

            popupRect = region;
            popupRect.y += objectRect.height + DividerHeight;
            popupRect.height = EditorStyles.popup.CalcHeight(GUIContent.none, 100);
            popupRect.x += fiRectUtility.IndentHorizontal;
            popupRect.width -= fiRectUtility.IndentHorizontal;
        }

        /// <summary>
        /// Returns the methods that should be shown in the dropdown, and returns the active method
        /// in that list.
        /// </summary>
        private void GetMethodOptions(TSerializationDelegate action, out int selectedIndex, out string[] displayedOptions) {
            selectedIndex = -1;
            var options = new List<string>();

            var containerType = InspectedType.Get(action.MethodContainer.GetType());
            var currentMethodName = action.MethodName;

            var methods = containerType.GetMethods(InspectedMemberFilters.All);
            for (int i = 0; i < methods.Count; ++i) {
                var method = methods[i];

                if (method.Method.IsGenericMethodDefinition || IsValidMethod(method.Method) == false) {
                    continue;
                }

                if (currentMethodName == method.Method.Name) {
                    selectedIndex = options.Count;
                }
                options.Add(method.Method.Name);
            }

            displayedOptions = options.ToArray();
        }

        /// <summary>
        /// Returns either the container type, or if the contaier is null, then typeof(UnityObject).
        /// </summary>
        private Type GetContainerTypeOrUnityObject(TSerializationDelegate element) {
            if (element.MethodContainer == null) {
                return typeof(UnityObject);
            }
            return element.MethodContainer.GetType();
        }

        public override TSerializationDelegate Edit(Rect region, GUIContent label, TSerializationDelegate element, fiGraphMetadata metadata) {
            EnsureInstance(ref element);

            Rect objectRect, popupRect;
            SplitRects(region, out objectRect, out popupRect);

            element.MethodContainer = fiEditorGUI.ObjectField(objectRect, label, element.MethodContainer,
                GetContainerTypeOrUnityObject(element), /*allowSceneObjects:*/true);

            if (element.MethodContainer != null) {
                string[] displayedOptions;
                int selectedIndex;
                GetMethodOptions(element, out selectedIndex, out displayedOptions);

                int updatedIndex = EditorGUI.Popup(popupRect, "Method", selectedIndex, displayedOptions);

                if (updatedIndex >= 0 && updatedIndex < displayedOptions.Length) {
                    element.MethodName = displayedOptions[updatedIndex];
                }
            }

            return element;
        }

        public override float GetElementHeight(GUIContent label, TSerializationDelegate element, fiGraphMetadata metadata) {
            EnsureInstance(ref element);

            if (element.MethodContainer == null) {
                return
                    EditorStyles.objectField.CalcHeight(label, 100);
            }

            return
                EditorStyles.objectField.CalcHeight(label, 100) +
                DividerHeight +
                EditorStyles.popup.CalcHeight(GUIContent.none, 100) +
                EndSeparatorHeight;
        }
    }
}