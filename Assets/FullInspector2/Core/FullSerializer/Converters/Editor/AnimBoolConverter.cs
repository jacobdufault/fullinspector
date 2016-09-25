using System;
using FullSerializer;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace FullInspector.Serializers.FullSerializer {
    [InitializeOnLoad]
    public class AnimBoolConverter : fsConverter {
        public static void Register() {
            FullSerializerSerializer.AddConverter<AnimBoolConverter>();
        }

        public override bool CanProcess(Type type) {
            return type == typeof(AnimBool);
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType) {
            var anim = (AnimBool)instance;
            serialized = new fsData(anim.target);
            return fsResult.Success;
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType) {
            instance = new AnimBool(data.AsBool);
            return fsResult.Success;
        }

        public override bool RequestCycleSupport(Type storageType) {
            return false;
        }

        public override bool RequestInheritanceSupport(Type storageType) {
            return false;
        }

        public override object CreateInstance(fsData data, Type storageType) {
            return storageType;
        }
    }
}