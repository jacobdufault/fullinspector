using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FullInspector.Internal.Preserve;
using FullInspector.Serializers.FullSerializer;
using FullSerializer;
using FullSerializer.Internal;
using UnityEngine;

namespace FullInspector {
    /// <summary>
    /// Implements Full Inspector integration with Full Serializer, a .NET serializer that just
    /// works. Use Unity style annotations (such as [SerializeField]) to serialize your types.
    /// </summary>
    [Preserve]
    public class FullSerializerSerializer : BaseSerializer {
        // Since we have a _serializer.Context object that we use during serialization, each thread needs its own fsSerializer
        // instance. This makes adding custom converters/processors a bit tricky, as we have to readd them for each thread.
        [ThreadStatic]
        private static fsSerializer _serializer;
        private static readonly List<fsSerializer> _serializers = new List<fsSerializer>();
        private static readonly List<Type> _converters = new List<Type>();
        private static readonly List<Type> _processors = new List<Type>();

        private static fsSerializer Serializer {
            get {
                if (_serializer == null) {
                    lock (typeof(FullSerializerSerializer)) {
                        _serializer = new fsSerializer();
                        _serializers.Add(_serializer);

                        _serializer.RemoveProcessor<fsSerializationCallbackReceiverProcessor>();

                        foreach (var converter in _converters) {
                            _serializer.AddConverter((fsConverter)Activator.CreateInstance(converter));
                        }
                        foreach (var processor in _processors) {
                            _serializer.AddProcessor((fsObjectProcessor)Activator.CreateInstance(processor));
                        }
                    }
                }

                return _serializer;
            }
        }

        /// <summary>
        /// Register the given converter for usage in the serializer.
        /// </summary>
        public static void AddConverter<TConverter>() where TConverter : fsConverter, new() {
            lock (typeof(FullSerializerSerializer)) {
                _converters.Add(typeof(TConverter));
                foreach (var serializer in _serializers) serializer.AddConverter(new TConverter());
            }
        }

        /// <summary>
        /// Register the given processor for usage in the serializer.
        /// </summary>
        public static void AddProcessor<TProcessor>() where TProcessor : fsObjectProcessor, new() {
            lock (typeof(FullSerializerSerializer)) {
                _processors.Add(typeof(TProcessor));
                foreach (var serializer in _serializers) serializer.AddProcessor(new TProcessor());
            }
        }

        static FullSerializerSerializer() {
            AddConverter<UnityObjectConverter>();
#if !UNITY_4_3
            AddProcessor<SerializationCallbackReceiverObjectProcessor>();
#endif
        }

        public override string Serialize(MemberInfo storageType, object value,
            ISerializationOperator serializationOperator) {

            Serializer.Context.Set(serializationOperator);

            fsData data;
            var fail = Serializer.TrySerialize(GetStorageType(storageType), value, out data);
            if (EmitFailWarning(fail)) return null;

            if (fiSettings.PrettyPrintSerializedJson) return fsJsonPrinter.PrettyJson(data);
            return fsJsonPrinter.CompressedJson(data);
        }

        public override object Deserialize(MemberInfo storageType, string serializedState,
            ISerializationOperator serializationOperator) {

            fsData data;
            var result = fsJsonParser.Parse(serializedState, out data);
            if (EmitFailWarning(result)) return null;

            Serializer.Context.Set(serializationOperator);

            object deserialized = null;
            result = Serializer.TryDeserialize(data, GetStorageType(storageType), ref deserialized);
            if (EmitFailWarning(result)) return null;

            return deserialized;
        }

        public override bool SupportsMultithreading {
            get { return true; }
        }

        private static bool EmitFailWarning(fsResult result) {
            if (fiSettings.EmitWarnings && result.RawMessages.Any()) {
                Debug.LogWarning(result.FormattedMessages);
            }

            return result.Failed;
        }
    }
}