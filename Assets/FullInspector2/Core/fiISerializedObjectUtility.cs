using FullSerializer.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    /// <summary>
    /// Helper methods for actually serializing objects that extend ISerializedObject. This works via reflection.
    /// </summary>
    public static class fiISerializedObjectUtility {
        private static bool SaveStateForProperty(ISerializedObject obj, InspectedProperty property, BaseSerializer serializer, ISerializationOperator serializationOperator, out string serializedValue, ref bool success) {
            object currentValue = property.Read(obj);

            try {
                if (currentValue == null) {
                    serializedValue = null;
                }
                else {
                    serializedValue = serializer.Serialize(property.MemberInfo, currentValue, serializationOperator);
                }
                return true;
            }
            catch (Exception e) {
                success = false;
                serializedValue = null;

                Debug.LogError("Exception caught when serializing property <" +
                    property.Name + "> in <" + obj + "> with value " + currentValue + "\n" +
                    e);
                return false;
            }
        }

        /// <summary>
        /// Serializes the current state of the given object.
        /// </summary>
        /// <typeparam name="TSerializer">The type of serializer to use for the serialization
        /// process.</typeparam>
        /// <param name="obj">The object that should be serialized.</param>
        /// <returns>True if serialization was entirely successful, false if something bad happened along the way.</returns>
        public static bool SaveState<TSerializer>(ISerializedObject obj)
            where TSerializer : BaseSerializer {

            fiLog.Log(typeof(fiISerializedObjectUtility), "Serializing object of type {0}", obj.GetType());

            bool success = true;

            var callbacks = obj as ISerializationCallbacks;
            if (callbacks != null) callbacks.OnBeforeSerialize();

            // fetch the selected serializer
            var serializer = fiSingletons.Get<TSerializer>();

            // setup the serialization operator
            var serializationOperator = fiSingletons.Get<ListSerializationOperator>();
            serializationOperator.SerializedObjects = new List<UnityObject>();

            // get the properties that we will be serializing
            var serializedKeys = new List<string>();
            var serializedValues = new List<string>();

            // PERF: Calling InspectedType().GetProperties() is extremely expensive. If we're not in the editor, then we can avoid making
            //       that call because the only time we serialize an object is if it has existing FS data. If it has FS data, then that data
            //       stores the set of properties which are serialized, so we can just hijack that information when doing the serialization to
            //       determine which properties to serialize.

            if (fiUtility.IsEditor || obj.SerializedStateKeys == null || obj.SerializedStateKeys.Count == 0) {
                var properties = InspectedType.Get(obj.GetType()).GetProperties(InspectedMemberFilters.FullInspectorSerializedProperties);
                for (int i = 0; i < properties.Count; ++i) {
                    InspectedProperty property = properties[i];
                    string serializedValue;
                    if (SaveStateForProperty(obj, property, serializer, serializationOperator, out serializedValue, ref success)) {
                        serializedKeys.Add(property.Name);
                        serializedValues.Add(serializedValue);
                    }
                }
            }
            else {
                var inspectedType = InspectedType.Get(obj.GetType());
                for (int i = 0; i < obj.SerializedStateKeys.Count; ++i) {
                    InspectedProperty property = inspectedType.GetPropertyByName(obj.SerializedStateKeys[i]) ?? inspectedType.GetPropertyByFormerlySerializedName(obj.SerializedStateKeys[i]);
                    if (property == null) continue;

                    string serializedValue;
                    if (SaveStateForProperty(obj, property, serializer, serializationOperator, out serializedValue, ref success)) {
                        serializedKeys.Add(property.Name);
                        serializedValues.Add(serializedValue);
                    }
                }
            }

            // Write the updated data out to the object.

            // Note that we only write data out to the object if our serialized state has
            // changed. Unity will blindly rewrite the data on disk which will cause some
            // source control systems to check-out the files. If we are just updating
            // the content to the same content, we do not want to cause an accidental
            // checkout.

            bool changed = false;
            if (AreListsDifferent(obj.SerializedStateKeys, serializedKeys)) {
                obj.SerializedStateKeys = serializedKeys;
                changed = true;
            }
            if (AreListsDifferent(obj.SerializedStateValues, serializedValues)) {
                obj.SerializedStateValues = serializedValues;
                changed = true;
            }
            if (AreListsDifferent(obj.SerializedObjectReferences, serializationOperator.SerializedObjects)) {
                obj.SerializedObjectReferences = serializationOperator.SerializedObjects;
                changed = true;
            }

            if (changed && fiUtility.IsEditor) {
                fiLateBindings.EditorApplication.InvokeOnEditorThread(() => {
                    var unityObj = (UnityObject)obj;
                    if (unityObj != null)
                        fiLateBindings.EditorUtility.SetDirty(unityObj);
                });
            }

            if (callbacks != null) callbacks.OnAfterSerialize();

            return success;
        }

        private static bool AreListsDifferent(IList<string> a, IList<string> b) {
            // invariant: Nullable{a}, NotNullable{b}
            if (a == null) return true;
            if (a.Count != b.Count) return true;

            int len = a.Count;
            for (int i = 0; i < len; ++i) {
                if (a[i] != b[i]) {
                    return true;
                }
            }
            return false;
        }

        private static bool AreListsDifferent(IList<UnityObject> a, IList<UnityObject> b) {
            // invariant: Nullable{a}, NotNullable{b}
            if (a == null) return true;
            if (a.Count != b.Count) return true;

            int len = a.Count;
            for (int i = 0; i < len; ++i) {
                if (ReferenceEquals(a[i], b[i]) == false) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Deserializes an object that has been serialized.
        /// </summary>
        /// <typeparam name="TSerializer">The type of serializer that was used to serialize the
        /// object.</typeparam>
        /// <param name="obj">The object that will be restored from its serialized state.</param>
        /// <returns>True if restoration was completely successful, false if something bad happened
        /// at some point (the object may be in a partially deserialized state).</returns>
        public static bool RestoreState<TSerializer>(ISerializedObject obj)
            where TSerializer : BaseSerializer {

            fiLog.Log(typeof(fiISerializedObjectUtility), "Deserializing object of type {0}", obj.GetType());

            var callbacks = obj as ISerializationCallbacks;
            try {
                if (callbacks != null) callbacks.OnBeforeDeserialize();

                // ensure references are initialized
                if (obj.SerializedStateKeys == null) obj.SerializedStateKeys = new List<string>();
                if (obj.SerializedStateValues == null) obj.SerializedStateValues = new List<string>();
                if (obj.SerializedObjectReferences == null) obj.SerializedObjectReferences = new List<UnityObject>();

                // try to verify that no data corruption occurred
                if (obj.SerializedStateKeys.Count != obj.SerializedStateValues.Count) {
                    if (fiSettings.EmitWarnings) {
                        Debug.LogWarning("Serialized key count does not equal value count; possible " +
                            "data corruption / bad manual edit?", obj as UnityObject);
                    }
                }

                // there is nothing to deserialize
                if (obj.SerializedStateKeys.Count == 0) {
                    if (fiSettings.AutomaticReferenceInstantation) {
                        InstantiateReferences(obj, null);
                    }
                    obj.IsRestored = true;
                    return true;
                }

                // fetch the selected serializer
                var serializer = fiSingletons.Get<TSerializer>();

                // setup the serialization operator setup the serialization operator
                var serializationOperator = fiSingletons.Get<ListSerializationOperator>();
                serializationOperator.SerializedObjects = obj.SerializedObjectReferences;

                // get the properties that we will be serializing
                var inspectedType = InspectedType.Get(obj.GetType());

                bool success = true;

                for (int i = 0; i < obj.SerializedStateKeys.Count; ++i) {
                    var name = obj.SerializedStateKeys[i];
                    var state = obj.SerializedStateValues[i];

                    var property = inspectedType.GetPropertyByName(name) ?? inspectedType.GetPropertyByFormerlySerializedName(name);
                    if (property == null) {
                        if (fiSettings.EmitWarnings) {
                            Debug.LogWarning("Unable to find serialized property with name=" + name +
                                " on type " + obj.GetType(), obj as UnityObject);
                        }
                        continue;
                    }

                    object restoredValue = null;

                    if (string.IsNullOrEmpty(state) == false) {
                        try {
                            restoredValue = serializer.Deserialize(property.MemberInfo, state,
                                serializationOperator);
                        }
                        catch (Exception e) {
                            success = false;
                            Debug.LogError("Exception caught when deserializing property <" + name +
                                "> with type <" + obj.GetType() + ">\n" + e, obj as UnityObject);
                        }
                    }

                    // sigh... CompareBaseObjectsExternal exception is thrown when we write a null
                    // UnityObject reference in a non-primary thread using reflection
                    //
                    // This is commented out because we're not currently doing multithreaded
                    // deserialization, but this was a tricky issue to find so it will remain
                    // documented here.
                    //
                    // Please note that this breaks Reset() when there is a generic UnityObject
                    // type on the BaseBehavior, ie,
                    // class MyBehavior : BaseBehavior { public SharedInstance<int> myInt; }
                    //
                    //if (ReferenceEquals(restoredValue, null) &&
                    //    typeof(UnityObject).IsAssignableFrom(property.StorageType)) {
                    //    continue;
                    //}

                    try {
                        property.Write(obj, restoredValue);
                    }
                    catch (Exception e) {
                        success = false;
                        if (fiSettings.EmitWarnings) {
                            Debug.LogWarning(
                                "Caught exception when updating property value; see next message for the exception",
                                obj as UnityObject);
                            Debug.LogError(e);
                        }
                    }
                }

                obj.IsRestored = true;
                return success;
            }
            finally {
                if (callbacks != null) callbacks.OnAfterDeserialize();
            }
        }

        /// <summary>
        /// Instantiates all of the references in the given object.
        /// </summary>
        /// <param name="obj">The object to instantiate references in.</param>
        /// <param name="metadata">The (cached) metadata for the object.</param>
        private static void InstantiateReferences(object obj, InspectedType metadata) {
            if (metadata == null) {
                metadata = InspectedType.Get(obj.GetType());
            }

            // we don't want to do anything with collections
            if (metadata.IsCollection) {
                return;
            }

            var inspectedProperties = metadata.GetProperties(InspectedMemberFilters.InspectableMembers);
            for (int i = 0; i < inspectedProperties.Count; ++i) {
                var property = inspectedProperties[i];

                // this type is a reference, so we might need to instantiate it
                if (property.StorageType.Resolve().IsClass) {
                    // cannot allocate an instance for abstract types
                    if (property.StorageType.Resolve().IsAbstract) {
                        continue;
                    }
                    // check to see if the property already has a value; if it does, then skip it
                    object current = property.Read(obj);
                    if (current != null) {
                        continue;
                    }

                    // the property is null; we need to instantiate a new value for it
                    var propertyMetadata = InspectedType.Get(property.StorageType);

                    // the value cannot be created using the default constructor (ie, it has only
                    // one constructor that takes parameters); we cannot initialize an instance that
                    // is guaranteed to be in a valid state
                    if (propertyMetadata.HasDefaultConstructor == false) {
                        continue;
                    }

                    object instance = propertyMetadata.CreateInstance();
                    property.Write(obj, instance);

                    // recursively create instances
                    InstantiateReferences(instance, propertyMetadata);
                }
            }
        }
    }
}