using FullInspector.Internal;
using FullSerializer.Internal;
using System;
using System.Collections.Generic;

namespace FullInspector.BackupService {
    /// <summary>
    /// Marks if a property should be restored when restoring a backup. This is shared as a
    /// reference across fiSerializedProperty and fiDeserializedProperty, so modifying it in
    /// one location also modifies it in the other.
    /// </summary>
    [Serializable]
    public class fiEnableRestore {
        public bool Enabled;
    }

    /// <summary>
    /// A serialized object that has been deserialized. This is used to show the user the current
    /// deserialized value.
    /// </summary>
    public class fiDeserializedObject {
        public fiDeserializedObject(fiSerializedObject serializedState) {
            Type targetType = serializedState.Target.Target.GetType();

            var serializationOperator = new fiSerializationOperator() {
                SerializedObjects = serializedState.ObjectReferences
            };

            // Fetch the serializer that the target uses
            Type serializerType = BehaviorTypeToSerializerTypeMap.GetSerializerType(targetType);
            var serializer = (BaseSerializer)fiSingletons.Get(serializerType);

            var inspectedType = InspectedType.Get(targetType);

            Members = new List<fiDeserializedMember>();

            foreach (fiSerializedMember member in serializedState.Members) {
                InspectedProperty property = inspectedType.GetPropertyByName(member.Name);
                if (property != null) {
                    object deserialized = serializer.Deserialize(
                        fsPortableReflection.AsMemberInfo(property.StorageType), member.Value, serializationOperator);
                    Members.Add(new fiDeserializedMember() {
                        InspectedProperty = property,
                        Value = deserialized,
                        ShouldRestore = member.ShouldRestore
                    });
                }
            }
        }

        /// <summary>
        /// The deserialized values.
        /// </summary>
        public List<fiDeserializedMember> Members;
    }

    /// <summary>
    /// A deserialized item.
    /// </summary>
    public class fiDeserializedMember {
        public InspectedProperty InspectedProperty;
        public object Value;
        public fiEnableRestore ShouldRestore;
    }
}