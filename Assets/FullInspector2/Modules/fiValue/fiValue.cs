#if !UNITY_4_3
using FullInspector.Internal;
using FullSerializer.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    /// <summary>
    /// Used by the fiValue property drawer to get a common editing API for all of
    /// the fiValue values.
    /// </summary>
    public interface fiIValueProxyAPI {
        object Value { get; set; }
        void SaveState();
        void LoadState();
    }

    /// <summary>
    /// A proxy type so that we can get the PropertyDrawer to activate on all
    /// fiValue derived types.
    /// </summary>
    public class fiValueProxyEditor { }
}

namespace FullInspector {
    /// <summary>
    /// An fiValue type that does not do any serialization. Use this if you're just
    /// interested in getting the inspector.
    /// </summary>
    public abstract class fiValueNullSerializer<T> : fiValueProxyEditor, fiIValueProxyAPI {
        public T Value;

        #region fiIValueProxyAPI
        object fiIValueProxyAPI.Value {
            get {
                return Value;
            }
            set {
                Value = (T)value;
            }
        }

        void fiIValueProxyAPI.SaveState() {
        }

        void fiIValueProxyAPI.LoadState() {
        }
        #endregion
    }

    /// <summary>
    /// fiValue allows you to use the Full Inspector inspecting and serialization engine on a MonoBehaviour derived type,
    /// which allows for seamless compability with other assets. Usage of this type is easy; simply derive a serializable
    /// custom class with the generic parameter instantiated.
    ///
    /// **IMPORTANT**: Due to limitations/a bug within Unity's serializedProperty API, the derived fiValue type cannot be
    /// in a namespace.  Sorry :(
    /// </summary>
    /// <remarks>
    /// Because fiValue is deserialized using ISerializationCallbackReceiver which operates off the main thread, only
    /// Full Serializer can be used as the active serialization engine -- none of the other ones are thread-safe w.r.t.
    /// Unity's requirements (for example, you cannot call operator== off the main thread).
    /// </remarks>
    /// <typeparam name="T">The type of value to serialize.</typeparam>
    public abstract class fiValue<T> : fiValueProxyEditor, fiIValueProxyAPI, ISerializationCallbackReceiver {

        /// <summary>
        /// The value that can be manipulated.
        /// </summary>
        public T Value;

        [SerializeField, HideInInspector]
        private string SerializedState;
        [SerializeField, HideInInspector]
        private List<UnityObject> SerializedObjectReferences;

        #region ISerializationCallbackReceiver
        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            Serialize();
        }


        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            Deserialize();
        }
        #endregion

        #region fiIValueProxyAPI
        object fiIValueProxyAPI.Value {
            get {
                return Value;
            }
            set {
                Value = (T)value;
            }
        }


        void fiIValueProxyAPI.SaveState() {
            Serialize();
        }

        void fiIValueProxyAPI.LoadState() {
            Deserialize();
        }
        #endregion

        #region Serialization
        private void Serialize() {
            // fetch the selected serializer
            var serializer = fiSingletons.Get<FullSerializerSerializer>();

            // setup the serialization operator
            var serializationOperator = fiSingletons.Get<ListSerializationOperator>();
            serializationOperator.SerializedObjects = new List<UnityObject>();

            try {
                SerializedState = serializer.Serialize(typeof(T).Resolve(), Value, serializationOperator);
            }
            catch (Exception e) {
                Debug.LogError("Exception caught when serializing " + this + " (with type " + GetType() + ")\n" + e);
            }

            SerializedObjectReferences = serializationOperator.SerializedObjects;
        }

        private void Deserialize() {
            if (SerializedObjectReferences == null) {
                SerializedObjectReferences = new List<UnityObject>();
            }

            // fetch the selected serializer
            var serializer = fiSingletons.Get<FullSerializerSerializer>();
            var serializationOperator = fiSingletons.Get<ListSerializationOperator>();
            serializationOperator.SerializedObjects = SerializedObjectReferences;

            if (string.IsNullOrEmpty(SerializedState) == false) {
                try {
                    Value = (T)serializer.Deserialize(typeof(T).Resolve(), SerializedState, serializationOperator);
                }
                catch (Exception e) {
                    Debug.LogError("Exception caught when deserializing " + this + " (with type " + GetType() + ");\n" + e);
                }
            }
        }
        #endregion
    }
}
#endif
