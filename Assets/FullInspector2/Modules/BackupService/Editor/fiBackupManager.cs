using FullInspector.Internal;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FullInspector.BackupService {
    /// <summary>
    /// The central API that end-users might be interested in. Provides key functions such as
    /// creating a new backup and restoring an old one.
    /// </summary>
    public static class fiBackupManager {
        /// <summary>
        /// Returns all backups for the given object.
        /// </summary>
        public static IEnumerable<fiSerializedObject> GetBackupsFor(CommonBaseBehavior behavior) {
            foreach (var backup in fiStorageManager.SerializedObjects) {
                if (backup.Target.Target == behavior) {
                    yield return backup;
                }
            }
        }

        /// <summary>
        /// Creates a new backup of the given component. Only guaranteed to work for types that
        /// derive from CommonBaseBehavior, but there is a good chance it'll work for most/all
        /// types derived from Component.
        /// </summary>
        public static void CreateBackup(Component behavior) {
            fiSerializedObject serialized = Serialize(behavior);

            // serialization failed
            if (serialized == null) {
                return;
            }

            if (PrefabUtility.GetPrefabType(behavior) == PrefabType.Prefab) {
                fiPrefabManager.Storage.Objects.Add(serialized);
                EditorGUIUtility.PingObject(fiPrefabManager.Storage);
            }
            else {
                fiStorageManager.PersistentStorage.Objects.Add(serialized);
                EditorGUIUtility.PingObject(fiStorageManager.PersistentStorage);
            }

            fiPrefabManager.SetDirty();
        }

        /// <summary>
        /// Helper function that just ignores a few FI internal types for serialization since the
        /// backup solution serializes all inspected properties, not just those that are serialized
        /// </summary>
        private static bool ShouldIgnoreForPersist(InspectedProperty property) {
            string name = property.Name;

            return
                name.Contains("ISerializedObject.") ||
                name == "_objectReferences" ||
                name == "_serializedStateKeys" ||
                name == "_serializedStateValues" ||
                name == "_restored";
        }

        /// <summary>
        /// Restores a backup that was previously created.
        /// </summary>
        public static void RestoreBackup(fiSerializedObject serializedState) {
            Type targetType = serializedState.Target.Target.GetType();
            var inspectedType = InspectedType.Get(targetType);

            var serializationOperator = new fiSerializationOperator() {
                SerializedObjects = serializedState.ObjectReferences
            };

            // Fetch the serializer that the target uses
            Type serializerType = BehaviorTypeToSerializerTypeMap.GetSerializerType(targetType);
            var serializer = (BaseSerializer)fiSingletons.Get(serializerType);

            foreach (fiSerializedMember member in serializedState.Members) {
                // user requested a skip for restoring this property
                if (member.ShouldRestore.Enabled == false) {
                    continue;
                }

                InspectedProperty property = inspectedType.GetPropertyByName(member.Name);
                if (property != null) {
                    Type storageType = property.StorageType;
                    object restoredValue = serializer.Deserialize(storageType, member.Value, serializationOperator);
                    property.Write(serializedState.Target.Target, restoredValue);
                }
            }

            if (serializedState.Target.Target is ISerializedObject) {
                var serializedObj = ((ISerializedObject)serializedState.Target.Target);
                serializedObj.SaveState();
                serializedObj.RestoreState();
            }
        }

        /// <summary>
        /// Creates a serialized object from the given component.
        /// </summary>
        private static fiSerializedObject Serialize(Component target) {
            // Fetch the serializer that the target uses
            Type serializerType = BehaviorTypeToSerializerTypeMap.GetSerializerType(target.GetType());
            var serializer = (BaseSerializer)fiSingletons.Get(serializerType);

            // Save the current state
            if (target is ISerializedObject) {
                ((ISerializedObject)target).SaveState();
            }

            // Create our backup storage
            fiSerializedObject state = new fiSerializedObject() {
                Target = new fiUnityObjectReference(target, /*tryRestore:*/true),
                SavedAt = DateTime.Now.ToString()
            };

            var serializationOperator = new fiSerializationOperator() {
                SerializedObjects = state.ObjectReferences
            };

            // note: we use InspectedProperties, *not* SerializedProperties, because we want to
            //       backup every field
            var properties = InspectedType.Get(target.GetType()).GetProperties(InspectedMemberFilters.InspectableMembers);
            foreach (InspectedProperty property in properties) {
                if (ShouldIgnoreForPersist(property)) continue;

                Type storageType = property.StorageType;
                object currentValue = property.Read(target);

                string serialized = serializer.Serialize(storageType, currentValue, serializationOperator);

                state.Members.Add(new fiSerializedMember() {
                    Name = property.Name,
                    Value = serialized
                });
            }

            return state;
        }
    }
}