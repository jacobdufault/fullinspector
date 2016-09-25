using Newtonsoft.Json;
using System;

namespace FullInspector.Serializers.JsonNet {
    public class JsonNetMetadata : fiISerializerMetadata {
        public Guid SerializerGuid {
            get { return new Guid("330ef139-4bcf-4d72-99e3-ef9ed34b6baf"); }
        }

        public Type SerializerType {
            get { return typeof(JsonNetSerializer); }
        }

        public Type[] SerializationOptInAnnotationTypes {
            get {
                return new Type[] { 
                    typeof(JsonPropertyAttribute),
                    typeof(JsonConverterAttribute)
                };
            }
        }

        public Type[] SerializationOptOutAnnotationTypes {
            get {
                return new Type[] { 
                    typeof(JsonIgnoreAttribute)
                };
            }
        }
    }
}