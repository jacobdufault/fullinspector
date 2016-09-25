using System;
using FullSerializer;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace FullInspector.Serializers.FullSerializer {
    [InitializeOnLoad]
    public class AnimFloatConverter : fsConverter {
        static AnimFloatConverter() {
            FullSerializerSerializer.AddConverter<AnimFloatConverter>();
        }

        public override bool CanProcess(Type type) {
            return type == typeof(AnimFloat);
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType) {
            var anim = (AnimFloat)instance;
            serialized = new fsData(anim.target);
            return fsResult.Success;
        }

        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType) {
            instance = new AnimFloat((float)data.AsDouble);
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