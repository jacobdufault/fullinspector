#if !UNITY_4_3

using System.Collections.Generic;
using FullInspector.Internal;
using UnityEngine;

namespace FullInspector {
    // TODO: Move this into Core.

    /// <summary>
    /// BaseObject is similar to BaseBehavior and BaseScriptableObject except that it's base class is Object. You have to
    /// mark the derived type as [Serializable] if you're using BaseObject within a MonoBehaviour context.
    /// </summary>
    /// <remarks>Because BaseObject requires Unity's ISerializationCallbackReceiver for serialization support, only Full Serializer
    /// is supported for the serialization engine (it is the only serializer to support serialization off of the main thread).</remarks>
    public abstract class BaseObject : fiValueProxyEditor, fiIValueProxyAPI, ISerializedObject, ISerializationCallbackReceiver {
        /// <summary>
        /// Serializing references derived from UnityObject is tricky for a number of reasons, so we
        /// just let Unity handle it. The object can be modified in the inspector and be deleted, or
        /// it can become a prefab. Further, there is no good way to uniquely identify components
        /// and game objects that handle prefabs and instantiation well. We therefore just let Unity
        /// serialize our references for us.
        /// </summary>
        /// <remarks>
        /// We add a NotSerialized annotation to this item so that FI will not serialize it
        /// </remarks>
        [SerializeField, NotSerialized, HideInInspector]
        private List<Object> _objectReferences;

        List<Object> ISerializedObject.SerializedObjectReferences {
            get {
                return _objectReferences;
            }
            set {
                _objectReferences = value;
            }
        }

        /// <summary>
        /// The key fields that are serialized. These map to the properties/fields that Full
        /// Inspector has discovered in the behavior type that need to be serialized. This value
        /// needs to be serialized by Unity and therefore cannot be auto-implemented by a property.
        /// </summary>
        /// <remarks>
        /// We add a NotSerialized annotation to this item so that FI will not serialize it
        /// </remarks>
        [SerializeField, NotSerialized, HideInInspector]
        private List<string> _serializedStateKeys;
        List<string> ISerializedObject.SerializedStateKeys {
            get {
                return _serializedStateKeys;
            }
            set {
                _serializedStateKeys = value;
            }
        }

        /// <summary>
        /// The value fields that are serialized. These correspond to the key fields that Full
        /// Inspector has discovered in the behavior type that need to be serialized. This value
        /// needs to be serialized by Unity and therefore cannot be auto-implemented by a property.
        /// </summary>
        /// <remarks>
        /// We add a NotSerialized annotation to this item so that FI will not serialize it
        /// </remarks>
        [SerializeField, NotSerialized, HideInInspector]
        private List<string> _serializedStateValues;

        List<string> ISerializedObject.SerializedStateValues {
            get {
                return _serializedStateValues;
            }
            set {
                _serializedStateValues = value;
            }
        }

        bool ISerializedObject.IsRestored { get; set; }

        void ISerializedObject.RestoreState() {
            fiISerializedObjectUtility.RestoreState<FullSerializerSerializer>(this);
        }

        void ISerializedObject.SaveState() {
            fiISerializedObjectUtility.SaveState<FullSerializerSerializer>(this);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            fiISerializedObjectUtility.RestoreState<FullSerializerSerializer>(this);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            fiISerializedObjectUtility.SaveState<FullSerializerSerializer>(this);
        }

        object fiIValueProxyAPI.Value {
            get { return this; }
            set { }
        }

        void fiIValueProxyAPI.SaveState() {
        }

        void fiIValueProxyAPI.LoadState() {
        }
    }
}

#endif