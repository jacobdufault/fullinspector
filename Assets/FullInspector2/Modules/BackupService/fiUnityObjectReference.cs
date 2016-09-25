using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    /// <summary>
    /// A reference to a Component that tries really hard to not go away, even if it's stored
    /// inside of a ScriptableObject.
    /// </summary>
    [Serializable]
    public class fiUnityObjectReference {
        /// <summary>
        /// Our referenced object. Sometimes set to null by Unity -- if that
        /// happens, we use the instance id to refetch the non-null instance.
        /// </summary>
        [SerializeField]
        private UnityObject _target;

        public fiUnityObjectReference() {
        }

        public fiUnityObjectReference(UnityObject target, bool tryRestore) {
            Target = target;
            if (tryRestore)
                TryRestoreFromInstanceId();
        }

        /// <summary>
        /// Returns true if this reference points to a valid object.
        /// </summary>
        public bool IsValid {
            get {
                return Target != null;
            }
        }

        /// <summary>
        /// The actual component reference.
        /// </summary>
        public UnityObject Target;

        /// <summary>
        /// Restores the object (if fake null) to an actual object instance via its instance id.
        /// </summary>
        private void TryRestoreFromInstanceId() {
            if (_target == null && ReferenceEquals(_target, null) == false) {
                int instanceId = _target.GetInstanceID();
                _target = fiLateBindings.EditorUtility.InstanceIDToObject(instanceId);
            }
        }

        public override int GetHashCode() {
            if (IsValid == false)
                return 0;

            return Target.GetHashCode();
        }

        public override bool Equals(object obj) {
            var r = obj as fiUnityObjectReference;
            return r != null && r.Target == Target;
        }
    }
}