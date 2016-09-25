using FullInspector.Serializers.JsonNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

namespace FullInspector {
    public class JsonNetSerializer : BaseSerializer {
        /// <summary>
        /// The JsonConverters that we need to use for serialization to happen correctly.
        /// </summary>
        private static JsonConverter[] RequiredConverters = {
            new UnityObjectConverter(),
            new Vector2Converter(),
            new Vector3Converter(),
            new Vector4Converter(),
            new ColorConverter(),
        };

        /// <summary>
        /// Every converter that will be used during (de)serialization.
        /// </summary>
        private static JsonConverter[] AllConverters;

        static JsonNetSerializer() {
            // create the list of all of the JsonConverters that we will be using
            List<JsonConverter> allConverters = new List<JsonConverter>();
            allConverters.AddRange(RequiredConverters);
            allConverters.AddRange(JsonNetSettings.CustomConverters);
            AllConverters = allConverters.ToArray();

            // the settings we use for serialization
            Settings = new JsonSerializerSettings() {
                Converters = AllConverters,

                // ensure that we recreate containers and don't just append to them if they are
                // already allocated (we want to replace whatever Unity deserialized into the
                // list)
                ObjectCreationHandling = ObjectCreationHandling.Replace,

                // handle inheritance correctly
                TypeNameHandling = TypeNameHandling.Auto,

                // don't be so strict about types
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,

                // we want to serialize loops, otherwise self-referential Components won't work
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,

                Error = HandleError
            };
        }

        /// <summary>
        /// Formats an ErrorContext for pretty printing.
        /// </summary>
        private static string FormatError(ErrorContext error) {
            return "Attempting to recover from serialization error\n" +
                error.Error.Message + "\n" +
                "Member: " + error.Member + "\n" +
                "OriginalObject: " + error.OriginalObject + "\n" +
                "Exception: " + error.Error;
        }

        /// <summary>
        /// Json.NET callback for when an error occurs. We simply print out the error and continue
        /// with the deserialization process.
        /// </summary>
        private static void HandleError(object sender, ErrorEventArgs e) {
            if (fiSettings.EmitWarnings) {
                Debug.LogWarning(FormatError(e.ErrorContext));
            }
            e.ErrorContext.Handled = true;
        }

        /// <summary>
        /// The serialization settings that are used
        /// </summary>
        private static JsonSerializerSettings Settings;

        public override string Serialize(MemberInfo storageType, object value,
            ISerializationOperator serializationOperator) {

            if (value == null) {
                return "null";
            }

            try {
                JsonNetOperatorHack.ActivateOperator = serializationOperator;

                var formatting = fiSettings.PrettyPrintSerializedJson ? Formatting.Indented : Formatting.None;

                // Json.NET for Unity does not support specifying the initial serialized type with
                // JsonConvert.SerializeObject, so we work around that by forcing Json.NET to emit
                // type information for all types when the initial type does not match
                if (GetStorageType(storageType) != value.GetType()) {
                    var saved = Settings.TypeNameHandling;

                    Settings.TypeNameHandling = TypeNameHandling.All;
                    string serialized = JsonConvert.SerializeObject(value, formatting, Settings);
                    Settings.TypeNameHandling = saved;

                    return serialized;
                }

                return JsonConvert.SerializeObject(value, formatting, Settings);
            }
            finally {
                JsonNetOperatorHack.ActivateOperator = null;
            }
        }

        public override object Deserialize(MemberInfo storageType, string serializedState,
            ISerializationOperator serializationOperator) {

            try {
                JsonNetOperatorHack.ActivateOperator = serializationOperator;
                return JsonConvert.DeserializeObject(serializedState, GetStorageType(storageType), Settings);
            }
            finally {
                JsonNetOperatorHack.ActivateOperator = null;
            }
        }
    }
}