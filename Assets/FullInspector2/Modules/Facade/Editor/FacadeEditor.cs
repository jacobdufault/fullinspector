using System;
using System.Collections.Generic;
using System.Linq;
using FullInspector.Internal;
using FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector {
    /// <summary>
    /// Manages the type selection drop-down for a facade.
    /// </summary>
    public class FacadeTypeManager {
        /// <summary>
        /// Creates a new type manager.
        /// </summary>
        /// <param name="baseType">The base facade type.</param>
        public FacadeTypeManager(Type baseType) {
            // TODO: Refactor this to use the types option manager/
            Types = fiReflectionUtility.GetCreatableTypesDeriving(baseType).Select(dt => dt.Type).ToArray();

            DisplayedOptions = (from type in Types
                                let name = type.CSharpName()
                                select new GUIContent(name)).ToArray();

            if (Types.Length == 0) {
                Types = new[] { baseType };
                DisplayedOptions = new[] { new GUIContent(baseType.CSharpName() + " (cannot construct)") };
            }
        }

        /// <summary>
        /// Finds the active display option index for the given type, or -1 if it isn't found.
        /// </summary>
        public int GetDisplayOptionIndex(Type type) {
            return Array.IndexOf(Types, type);
        }

        /// <summary>
        /// The available types. This will always have at least one element.
        /// </summary>
        public Type[] Types;

        /// <summary>
        /// A displayable variant of the type array. This will always have at least one element.
        /// </summary>
        public GUIContent[] DisplayedOptions;
    }

    [CustomPropertyEditor(typeof(Facade<>))]
    public class FacadeEditor<T> : PropertyEditor<Facade<T>> {
        /// <summary>
        /// Returns the default value for the given type. Notably, this will return a zeroed out
        /// value type if the type is a value type, not null.
        /// </summary>
        private static object GetDefault(Type type) {
            if (type.IsValueType) {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        /// <summary>
        /// Deserializes a property on the facade.
        /// </summary>
        private static object DeserializeProperty(BaseSerializer serializer,
            ISerializationOperator serializationOperator, InspectedProperty property,
            Facade<T> facade) {

            string data;
            if (facade.FacadeMembers.TryGetValue(property.Name, out data)) {
                try {
                    return serializer.Deserialize(property.StorageType, data, serializationOperator);
                }
                catch {
                }
            }

            return GetDefault(property.StorageType);
        }

        /// <summary>
        /// Serializes a property that will be stored on the facade.
        /// </summary>
        public static bool TrySerializeProperty(BaseSerializer serializer,
            ISerializationOperator serializationOperator, InspectedProperty property, object value,
            out string data) {

            try {
                data = serializer.Serialize(property.StorageType, value, serializationOperator);
                return true;
            }
            catch {
                data = string.Empty;
                return false;
            }
        }

        private static float LabelHeight = EditorStyles.label.CalcHeight(GUIContent.none, 100);
        private const float SplitterHeight = 2f;
        private static FacadeTypeManager TypeOptions = new FacadeTypeManager(typeof(T));

        private static void DrawHeader(ref Rect region, GUIContent label, Facade<T> element,
            out bool changedTypes) {

            changedTypes = false;

            if (string.IsNullOrEmpty(label.text) && TypeOptions.Types.Length == 1) {
                return;
            }

            Rect labelRect = region;
            labelRect.height = LabelHeight;
            region.y += LabelHeight;
            region = fiRectUtility.IndentedRect(region);

            if (TypeOptions.Types.Length == 1) {
                EditorGUI.LabelField(labelRect, label);
            }
            else {
                int currentIndex = TypeOptions.GetDisplayOptionIndex(element.InstanceType);
                int updatedIndex = EditorGUI.Popup(labelRect, label, currentIndex, TypeOptions.DisplayedOptions);

                if (currentIndex != updatedIndex &&
                    updatedIndex >= 0 && updatedIndex < TypeOptions.Types.Length) {

                    changedTypes = true;
                    element.InstanceType = TypeOptions.Types[updatedIndex];
                }
            }
        }


        public override Facade<T> Edit(Rect region, GUIContent label, Facade<T> element, fiGraphMetadata metadata) {
            if (element == null) {
                element = new Facade<T>();
            }

            if (element.InstanceType == null) {
                element.InstanceType = TypeOptions.Types[0];
            }

            bool changedTypes;
            DrawHeader(ref region, label, element, out changedTypes);

            var facadeMembers = new Dictionary<string, string>();
            var facadeReferences = new List<UnityObject>();

            InspectedType inspectedType = InspectedType.Get(element.InstanceType);

            var serializer = (BaseSerializer)fiSingletons.Get(fiInstalledSerializerManager.DefaultMetadata.SerializerType);
            var deserializationOp = new ListSerializationOperator() {
                SerializedObjects = element.ObjectReferences
            };
            var serializationOp = new ListSerializationOperator() {
                SerializedObjects = facadeReferences
            };

            float usedHeight = 0;

            var anim = metadata.GetMetadata<fiAnimationMetadata>();
            if (anim.IsAnimating) {
                fiEditorGUI.BeginFadeGroupHeight(LabelHeight, ref region, anim.AnimationHeight);
                fiEditorUtility.RepaintAllEditors();
            }

            var properties = inspectedType.GetProperties(InspectedMemberFilters.InspectableMembers);
            for (int i = 0; i < properties.Count; ++i) {
                InspectedProperty property = properties[i];

                object propertyValue = DeserializeProperty(serializer, deserializationOp, property, element);

                float height = fiEditorGUI.EditPropertyHeightDirect(property, propertyValue, metadata.Enter(property.Name));

                Rect propertyRect = region;
                propertyRect.height = height;
                region.y += height;
                region.y += SplitterHeight;

                usedHeight += height + SplitterHeight;

                object updatedValue = fiEditorGUI.EditPropertyDirect(propertyRect, property, propertyValue, metadata.Enter(property.Name));

                string data;
                if (TrySerializeProperty(serializer, serializationOp, property, updatedValue, out data)) {
                    facadeMembers[property.Name] = data;
                }
            }

            if (anim.IsAnimating) fiEditorGUI.EndFadeGroup();

            element.FacadeMembers = facadeMembers;
            element.ObjectReferences = facadeReferences;

            if (changedTypes && anim.UpdateHeight(usedHeight)) {
                fiEditorUtility.RepaintAllEditors();
            }

            return element;
        }

        public override float GetElementHeight(GUIContent label, Facade<T> element, fiGraphMetadata metadata) {
            float height = 0;
            if (string.IsNullOrEmpty(label.text) == false || TypeOptions.Types.Length > 1) {
                height = LabelHeight;
            }

            var anim = metadata.GetMetadata<fiAnimationMetadata>();
            if (anim.IsAnimating) {
                return height + anim.AnimationHeight;
            }

            if (element == null) {
                element = new Facade<T>();
            }


            if (element.InstanceType == null) {
                element.InstanceType = TypeOptions.Types[0];
            }

            InspectedType inspectedType = InspectedType.Get(element.InstanceType);

            var serializer = (BaseSerializer)fiSingletons.Get(fiInstalledSerializerManager.DefaultMetadata.SerializerType);
            var deserializationOp = new ListSerializationOperator() {
                SerializedObjects = element.ObjectReferences
            };

            var properties = inspectedType.GetProperties(InspectedMemberFilters.InspectableMembers);
            for (int i = 0; i < properties.Count; ++i) {
                InspectedProperty property = properties[i];

                object propertyValue = DeserializeProperty(serializer, deserializationOp, property, element);

                height += fiEditorGUI.EditPropertyHeightDirect(property, propertyValue, metadata.Enter(property.Name));
                height += SplitterHeight;
            }

            return height;
        }
    }
}