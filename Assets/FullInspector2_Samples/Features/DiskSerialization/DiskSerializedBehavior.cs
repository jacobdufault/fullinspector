#if !UNITY_WEBPLAYER

using System.IO;
using UnityEngine;

// NOTE: Because we're using NotSupportedSerializationOperator(), if we try to serialize a
//       UnityEngine.Object reference a NotSupportedException will be thrown. If you know of a good
//       way to serialize UnityEngine.Object references to disk, please submit a bug report
//       detailing the process and I'll be happy to add a DiskSerializationOperator to Full
//       Inspector.

namespace FullInspector.Samples.Other.DiskSerialization {
    /// <summary>
    /// The object that we will serialize.
    /// </summary>
    public struct SerializedStruct {
        public bool BoolValue;
        public int IntValue;
    }

    [AddComponentMenu("Full Inspector Samples/Other/Disk Serialized Behavior")]
    public class DiskSerializedBehavior : BaseBehavior<FullSerializerSerializer> {
        [InspectorComment("This is the value that will be serialized. Use the buttons below for " +
            "serialization operations.")]
        public SerializedStruct Value;

        [InspectorComment("The path that the value will be serialized to")]
        [InspectorMargin(10)]
        public string Path;

        [InspectorMargin(10)]
        [InspectorHidePrimary]
        [ShowInInspector]
        private int __inspectorMethodDivider;

        [InspectorButton]
        private void DeserializeFromPath() {
#if (!UNITY_EDITOR && UNITY_WINRT) == false
            // Read in the serialized state
            string content = File.ReadAllText(Path);

            // Restore the value
            Value = SerializationHelpers.DeserializeFromContent<SerializedStruct, FullSerializerSerializer>(content);
            Debug.Log("Object state has been restored from " + Path);
#endif
        }

        [InspectorButton]
        private void SerializeToPath() {
#if (!UNITY_EDITOR && UNITY_WINRT) == false
            // Get the serialized state of the object
            string content = SerializationHelpers.SerializeToContent<SerializedStruct, FullSerializerSerializer>(Value);

            // Write it out to disk
            File.WriteAllText(Path, content);
            Debug.Log("Object state has been saved to " + Path);
#endif
        }

        [InspectorButton]
        private void SerializeToConsole() {
            // Get the serialized state of the object
            string content = SerializationHelpers.SerializeToContent<SerializedStruct, FullSerializerSerializer>(Value);

            // Write it out to the console
            Debug.Log(content);
        }
    }
}

#endif