using System;
using UnityEngine;

namespace FullInspector.Samples.FullSerializer {
    [AddComponentMenu("Full Inspector Samples/FullSerializer/Behavior Serialization")]
    public class SampleFullSerializerBehaviorSerialization : BaseBehavior<FullSerializerSerializer> {
        // Lets create a type that Unity cannot serialize
        public struct NotSerializableByUnity {
            public int Value;
        }

        // All of these types will be serialized.
        public NotSerializableByUnity Serialized0;
        [SerializeField]
        public NotSerializableByUnity Serialized1;
        [ShowInInspector]
        [SerializeField]
        protected internal NotSerializableByUnity Serialized2;
        [ShowInInspector]
        [SerializeField]
        internal NotSerializableByUnity Serialized3;
        [ShowInInspector]
        [SerializeField]
        protected NotSerializableByUnity Serialized4;
        [ShowInInspector]
        [SerializeField]
        private NotSerializableByUnity Serialized5;

        // None of these types will be serialized
        [NonSerialized]
        public NotSerializableByUnity NotSerialized0;
        [ShowInInspector]
        protected internal NotSerializableByUnity NotSerialized1;
        [ShowInInspector]
        internal NotSerializableByUnity NotSerialized2;
        [ShowInInspector]
        protected NotSerializableByUnity NotSerialized3;
        [ShowInInspector]
        private NotSerializableByUnity NotSerialized4;

        // The same rules apply to properties
        public NotSerializableByUnity Serialized6 { get; set; }
        [SerializeField]
        public NotSerializableByUnity Serialized7 { get; set; }
        [NotSerialized] // identical to NonSerialized, just also usable on properties
        public NotSerializableByUnity NotSerialized5 { get; set; }
        [ShowInInspector]
        private NotSerializableByUnity NotSerialized6 { get; set; }
    }
}