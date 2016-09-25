using FullSerializer.Internal;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Internal {
    [CustomPropertyDrawer(typeof(fiInspectorOnly), true)]
    [CustomPropertyDrawer(typeof(fiInspectorOnlyAttribute))]
    public class fiInspectorOnly_PropertyDrawer : PropertyDrawer {
        private bool? _isEnabled;
        private bool IsEnabled {
            get {
                if (_isEnabled == null) {
                    _isEnabled = fsPortableReflection.HasAttribute<fiInspectorOnlyAttribute>(fieldInfo.FieldType) ||
                                 fsPortableReflection.HasAttribute<fiInspectorOnlyAttribute>(fieldInfo) ||
                                 typeof(fiInspectorOnly).IsAssignableFrom(fieldInfo.FieldType);

                    if (_isEnabled.Value && typeof(IList).IsAssignableFrom(fieldInfo.FieldType)) {
                        Debug.LogWarning("[fiInspectorOnly] does not currently support lists (on " + fieldInfo + ")");
                        _isEnabled = false;
                    }
                }

                return _isEnabled.Value;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if (IsEnabled) {
                var propertyType = fieldInfo.FieldType;
                var target = fiSerializedPropertyUtility.GetTarget(property);
                var metadata = fiSerializedPropertyUtility.GetMetadata(property);
                var editor = PropertyEditor.Get(propertyType, fieldInfo).FirstEditor;

                return editor.GetElementHeight(label, target, metadata);
            }

            return EditorGUI.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (IsEnabled) {
                var propertyType = fieldInfo.FieldType;
                var target = fiSerializedPropertyUtility.GetTarget(property);
                var metadata = fiSerializedPropertyUtility.GetMetadata(property);
                var editor = PropertyEditor.Get(propertyType, fieldInfo).FirstEditor;

                if (property.prefabOverride) fiUnityInternalReflection.SetBoldDefaultFont(true);

                var savedHierarchyMode = EditorGUIUtility.hierarchyMode;
                EditorGUIUtility.hierarchyMode = true;

                EditorGUI.BeginChangeCheck();
                target = editor.Edit(position, label, target, metadata);

                if (EditorGUI.EndChangeCheck()) {
                    fiSerializedPropertyUtility.WriteTarget(property, target);
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                }

                EditorGUIUtility.hierarchyMode = savedHierarchyMode;

                if (property.prefabOverride) fiUnityInternalReflection.SetBoldDefaultFont(false);

                fiSerializedPropertyUtility.RevertPrefabContextMenu(position, property);
                return;
            }

            EditorGUI.PropertyField(position, property, label);
        }
    }
}