using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    /// <summary>
    /// This class manages the deserialization and serialization of objects when
    /// we are in the editor. It will automatically register itself to be updated
    /// inside of the editor. If we're in a player, then calls to this class are
    /// no-ops since either serialization callbacks or Awake() will be used to
    /// serialize objects.
    /// </summary>
    // TODO: Should the SetDirty calls in fiISerializedObjectUtility be moved
    //       into this class? I think it might make sense to do it, because this
    //       is the "editor" serialization integration.
    // TODO: There appears to be a bug in Unity. If we save the state of a prefab
    //       when it is not being actively inspected, prefab changes will *not*
    //       get propagated to the prefab instances, despite the prefab having
    //       received those changes. The changes get propagated after entering
    //       play-mode, though.
    // NOTE: With the current approach of deferring serialization for the
    //       actively inspected object, if the user compiles and edits data while
    //       compiling but a serialization does *not* run after the edit and
    //       before the script reload, the edited changes will be lost.
    // TODO: Figure out if we can find a good work-around for the above note. We
    //       could just serialize constantly if we are compiling, but that causes
    //       recompiles to take a lot longer.
    public static class fiSerializationManager {
        /// <summary>
        /// Batches up a large number of serialization requests into just one.
        /// </summary>
        private class DeferredSerialization {
            /// <summary>
            /// How much time between serialization requests.
            /// </summary>
            private static readonly TimeSpan DELAY = TimeSpan.FromSeconds(0.5);

            /// <summary>
            /// The object we will serialize.
            /// </summary>
            private UnityObject _tracking;

            private DateTime _startTime;

            public void RequestSerialization(UnityObject tracking) {
                if (_tracking == tracking)
                    return;
                if (Application.isPlaying && fiLateBindings.PrefabUtility.IsPrefab(tracking))
                    return;

                // Unity Time APIs throw exceptions off the main thread.
                _startTime = DateTime.Now;

                if (_tracking != tracking) {
                    if (_tracking != null) {
                        SerializeObject(typeof(DeferredSerialization), _tracking);
                    }
                    else {
                        fiLateBindings.EditorApplication.AddUpdateFunc(Update);
                    }

                    _tracking = tracking;
                }
            }

            private void Update() {
                if ((!fiLateBindings.EditorApplication.isPlaying &&
                      fiLateBindings.EditorApplication.isCompilingOrChangingToPlayMode) ||
                    (DateTime.Now - _startTime) > DELAY) {
                    SerializeObject(typeof(DeferredSerialization), _tracking);
                    _tracking = null;

                    fiLateBindings.EditorApplication.RemUpdateFunc(Update);
                }
            }
        }

        private static DeferredSerialization s_inspectedObjectSerialization = new DeferredSerialization();

        static fiSerializationManager() {
            if (fiUtility.IsEditor) {
                fiLateBindings.EditorApplication.AddUpdateFunc(OnEditorUpdate);
            }
        }

        /// <summary>
        /// Should serialization be disabled? This is used by the serialization
        /// migration system where after migration serialization should not
        /// happen automatically.
        /// </summary>
        [NonSerialized]
        public static bool DisableAutomaticSerialization = false;

        private static readonly List<ISerializedObject> s_pendingDeserializations = new List<ISerializedObject>();
        private static readonly List<ISerializedObject> s_pendingSerializations = new List<ISerializedObject>();
        private static readonly Dictionary<ISerializedObject, fiSerializedObjectSnapshot> s_snapshots = new Dictionary<ISerializedObject, fiSerializedObjectSnapshot>();

        public static HashSet<UnityObject> DirtyForceSerialize = new HashSet<UnityObject>();

        private static bool SupportsMultithreading<TSerializer>() where TSerializer : BaseSerializer {
            return
                fiSettings.ForceDisableMultithreadedSerialization == false &&
                fiUtility.IsUnity4 == false && // Too many things break in Unity 4 off the main thread that we won't even bother attempting
                fiSingletons.Get<TSerializer>().SupportsMultithreading;
        }

        /// <summary>
        /// Common logic for Awake() or OnEnable() methods inside of behaviors.
        /// </summary>
        public static void OnUnityObjectAwake(ISerializedObject obj) {
            // No need to deserialize (possibly deserialized via
            // OnUnityObjectDeserialize)
            if (obj.IsRestored) return;

            // Otherwise do a regular deserialization
            DoDeserialize(obj);
        }

        /// <summary>
        /// Common logic for ISerializationCallbackReceiver.OnDeserialize
        /// </summary>
        public static void OnUnityObjectDeserialize<TSerializer>(ISerializedObject obj) where TSerializer : BaseSerializer {
            if (SupportsMultithreading<TSerializer>()) {
                DoDeserialize(obj);
                return;
            }

            if (fiUtility.IsEditor == false) return;

            // We are in the editor so we have to use EditorApplication.update to
            // linearize. This does not necessarily mean we will actually
            // deserialize the object, since if we are in play-mode we will
            // discard the EditorApplication.update linearization path in favor
            // of the Awake() linearization path since that is how the player
            // will do it.
            lock (s_pendingDeserializations) {
                s_pendingDeserializations.Add(obj);
            }
        }

        /// <summary>
        /// Common logic for ISerializationCallbackReceiver.OnSerialize
        /// </summary>
        public static void OnUnityObjectSerialize<TSerializer>(ISerializedObject obj) where TSerializer : BaseSerializer {
            if (SupportsMultithreading<TSerializer>()) {
                DoSerialize(obj);
                return;
            }

            // BUG/FIXME: If (in a deployed player) the serializer does not
            // support multithreaded serialization and we Instantiate an object
            // with modifications, then those modifications will not get
            // persisted. The Instantiated object will not be properly serialized
            // - the user will need to manually call SaveState() on the behavior.

            if (fiUtility.IsEditor == false) return;

            // We have to run the serialization request on the Unity thread
            lock (s_pendingSerializations) {
                s_pendingSerializations.Add(obj);
            }
        }

        // We cannot get the current selection while serializing, so we have to
        // cache it from our OnEditorUpdate callback.
        private static ISerializedObject[] s_cachedSelection = new ISerializedObject[0];

        private static void OnEditorUpdate() {
            s_cachedSelection = fiLateBindings.Selection.activeSelection.OfType<ISerializedObject>().ToArray();

            if (Application.isPlaying) {
                if (s_pendingDeserializations.Count > 0 || s_pendingSerializations.Count > 0 || s_snapshots.Count > 0) {
                    // Serialization / linearization will occur via Awake()

                    s_pendingDeserializations.Clear();
                    s_pendingSerializations.Clear();
                    s_snapshots.Clear();
                }
                //fiLateBindings.EditorApplication.RemUpdateFunc(OnEditorUpdate);
                return;
            }

            // Deserialize any inspected objects that have not been restored yet.
            // This happens when adding a new component since Unity does not call
            // ISerializationCallbackReceiver.OnDeserialized.
            for (int i = 0; i < s_cachedSelection.Length; ++i) {
                if (s_cachedSelection[i].IsRestored == false)
                    DoDeserialize(s_cachedSelection[i]);
            }

            while (s_pendingDeserializations.Count > 0) {
                ISerializedObject obj;
                lock (s_pendingDeserializations) {
                    obj = s_pendingDeserializations[s_pendingDeserializations.Count - 1];
                    s_pendingDeserializations.RemoveAt(s_pendingDeserializations.Count - 1);
                }

                // Check to make sure the object isn't destroyed.
                if (obj is UnityObject && ((UnityObject)obj) == null)
                    continue;

                DoDeserialize(obj);
            }

            while (s_pendingSerializations.Count > 0) {
                ISerializedObject obj;
                lock (s_pendingSerializations) {
                    obj = s_pendingSerializations[s_pendingSerializations.Count - 1];
                    s_pendingSerializations.RemoveAt(s_pendingSerializations.Count - 1);
                }

                // Check to make sure the object isn't destroyed.
                if (obj is UnityObject && ((UnityObject)obj) == null) continue;

                DoSerialize(obj);
            }
        }

        private static void DoDeserialize(ISerializedObject obj) {
            obj.RestoreState();
        }

        private static void DoSerialize(ISerializedObject obj) {
            // If we have serialization disabled, then we *definitely* do not
            // want to do anything.
            // Note: We put this check here for code clarity / robustness
            //       purposes. If this proves to be a perf issue, it can be
            //       hoisted outside of the top-level loop which invokes this
            //       method.
            if (DisableAutomaticSerialization) return;

            bool forceSerialize = obj is UnityObject && DirtyForceSerialize.Contains((UnityObject)obj);
            if (forceSerialize)
                DirtyForceSerialize.Remove((UnityObject)obj);

            // Do not serialize objects which have been destroyed.
            if (obj is UnityObject && ((UnityObject)obj) == null)
                return;

            // If this object is currently being inspected then we don't want to
            // serialize it. This gives a big perf boost. Note that we *do* want
            // to serialize the object if we are entering play-mode or compiling
            // - otherwise a data loss will occur.
            if (forceSerialize == false && obj is UnityObject) {
                // If the serialized object request belongs to any of the items
                // in the selection, serialize it later. We have to use
                // s_cachedSelection because Selection.activeGameObject will
                // throw an exception if we are currently on a non-Unity thread.
                var toSerialize = (UnityObject)obj;
                for (int i = 0; i < s_cachedSelection.Length; ++i) {
                    if (ReferenceEquals(toSerialize, s_cachedSelection[i])) {
                        s_inspectedObjectSerialization.RequestSerialization(toSerialize);
                        return;
                    }
                }
            }

            HandleReset(obj);
            obj.SaveState();
        }

        private static HashSet<ISerializedObject> s_seen = new HashSet<ISerializedObject>();
        private static void HandleReset(ISerializedObject obj) {
            // We don't want to send a reset notification for new objects which
            // have no data. If we've already seen an object and it has no data,
            // then it was certainly reset.
            if (s_seen.Add(obj))
                return;

            // All serialized data is wiped out, but we have already
            // seen/serialized this object. Very likely it got reset.
            if (IsNullOrEmpty(obj.SerializedObjectReferences) &&
                IsNullOrEmpty(obj.SerializedStateKeys) &&
                IsNullOrEmpty(obj.SerializedStateValues)) {
                fiLog.Log(typeof(fiSerializationManager), "Reseting object of type {0}", obj.GetType());

                // Note: we do not clear out the keys; if we did, then we would
                //       not actually deserialize "null" onto them
                // Note: we call SaveState() so we can fetch the keys we need to
                //       deserialize
                obj.SaveState();
                for (int i = 0; i < obj.SerializedStateValues.Count; ++i) {
                    obj.SerializedStateValues[i] = null;
                }
                obj.RestoreState();

                // TODO: Does Reset get invoked off the Unity thread? Will it get
                //       invoked twice, once by Unity and once by Full Inspector?
                fiRuntimeReflectionUtility.InvokeMethod(obj.GetType(), "Reset", obj, null);

                obj.SaveState();
            }
        }

        private static bool IsNullOrEmpty<T>(IList<T> list) {
            return list == null || list.Count == 0;
        }

        public static void SerializeObject(Type logContext, object obj) {
            if (obj is GameObject) {
                foreach (var component in ((GameObject)obj).GetComponents<ISerializedObject>())
                    Serialize(logContext, component);
            }
            else {
                Serialize(logContext, obj);
            }
        }

        private static void Serialize(Type logContext, object obj) {
            if (obj is ISerializedObject) {
                fiLog.Log(logContext, "Serializing object of type {0}", obj.GetType());
                ((ISerializedObject)obj).SaveState();
            }
        }
    }
}