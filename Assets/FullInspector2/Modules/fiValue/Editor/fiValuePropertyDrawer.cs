#if !UNITY_4_3
using System;
using System.Collections.Generic;
using FullInspector.Internal;
using FullSerializer.Internal;
using UnityEditor;
using UnityEngine;

namespace FullInspector {
    [CustomPropertyDrawer(typeof(fiValueProxyEditor), /*useForChildren:*/ true)]
    public class fiValuePropertyDrawer : PropertyDrawer {
        #region Reflection
        public Type GetPropertyType(SerializedProperty property) {
            Type holderType = fsTypeCache.GetType(property.type);

            while (holderType != null &&
                (holderType.IsGenericType == false || holderType.GetGenericTypeDefinition() != typeof(fiValue<>))) {
                holderType = holderType.BaseType;
            }

            if (holderType == null) {
                var fieldType = fieldInfo.FieldType;

                // Unity isn't terribly consistent. The field type can be an array/list even though the
                // element it sends to us is an element inside of the list.
                if (fieldType.IsArray)
                    return fieldType.GetElementType();
                if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
                    return fieldType.GetGenericArguments()[0];

                return fieldInfo.FieldType;
            }
            return holderType.GetGenericArguments()[0];
        }
        #endregion

        #region GUI
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var propertyType = GetPropertyType(property);
            if (propertyType == null) return;

            var target = (fiIValueProxyAPI)fiSerializedPropertyUtility.GetTarget(property);
            var metadata = fiSerializedPropertyUtility.GetMetadata(property);
            var editor = PropertyEditor.Get(propertyType, fieldInfo).FirstEditor;

            if (property.prefabOverride) fiUnityInternalReflection.SetBoldDefaultFont(true);

            var savedHierarchyMode = EditorGUIUtility.hierarchyMode;
            EditorGUIUtility.hierarchyMode = true;

            EditorGUI.BeginChangeCheck();
            target.Value = editor.Edit(position, label, target.Value, metadata);
            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }

            EditorGUIUtility.hierarchyMode = savedHierarchyMode;

            if (property.prefabOverride) fiUnityInternalReflection.SetBoldDefaultFont(false);

            fiSerializedPropertyUtility.RevertPrefabContextMenu(position, property);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var propertyType = GetPropertyType(property);
            if (propertyType == null) return 0;

            var target = (fiIValueProxyAPI)fiSerializedPropertyUtility.GetTarget(property);
            var metadata = fiSerializedPropertyUtility.GetMetadata(property);
            var editor = PropertyEditor.Get(propertyType, fieldInfo).FirstEditor;

            return editor.GetElementHeight(label, target.Value, metadata);
        }
        #endregion
    }
}
#endif
