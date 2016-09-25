using Newtonsoft.Json;
using System;
using UnityEngine;

namespace FullInspector.Serializers.JsonNet {
    /// <summary>
    /// Converts UnityEngine.Color types
    /// </summary>
    public class ColorConverter : JsonConverter {
        [JsonObject(MemberSerialization.OptIn)]
        private struct WritableColor {
            [JsonProperty]
            public float r;
            [JsonProperty]
            public float g;
            [JsonProperty]
            public float b;
            [JsonProperty]
            public float a;
        }

        public override bool CanConvert(Type objectType) {
            return objectType == typeof(Color) || objectType == typeof(Color?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer) {

            WritableColor writable = serializer.Deserialize<WritableColor>(reader);
            return new Color(writable.r, writable.g, writable.b, writable.a);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var color = (Color)value;
            WritableColor writable = new WritableColor() {
                r = color.r,
                g = color.g,
                b = color.b,
                a = color.a
            };

            serializer.Serialize(writer, writable);
        }
    }
}