using System;
using System.Collections.Generic;
using FullSerializer.Internal;

namespace FullInspector.Internal {
    /// <summary>
    /// Manages communication with the installed serialization engines.
    /// </summary>
    // TODO: Redesign this to work better.
    public static class fiInstalledSerializerManager {
        public const string GeneratedTypeName = "fiLoadedSerializers";

        private static fiISerializerMetadata GetProvider(Type type) {
            return (fiISerializerMetadata)Activator.CreateInstance(type);
        }

        public static bool TryGetLoadedSerializerType(out fiILoadedSerializers serializers) {
            string typeName = "FullInspector.Internal." + GeneratedTypeName;

            fsTypeCache.Reset();
            Type type = fsTypeCache.GetType(typeName);
            if (type == null) {
                serializers = null;
                return false;
            }

            serializers = (fiILoadedSerializers)Activator.CreateInstance(type);
            return true;
        }


        static fiInstalledSerializerManager() {
            var optIn = new List<Type>();
            var optOut = new List<Type>();

            LoadedMetadata = new List<fiISerializerMetadata>();

            fiILoadedSerializers serializers;
            if (TryGetLoadedSerializerType(out serializers)) {
                _defaultMetadata = GetProvider(serializers.DefaultSerializerProvider);

                foreach (var providerType in serializers.AllLoadedSerializerProviders) {
                    fiISerializerMetadata metadata = GetProvider(providerType);

                    LoadedMetadata.Add(metadata);
                    optIn.AddRange(metadata.SerializationOptInAnnotationTypes);
                    optOut.AddRange(metadata.SerializationOptOutAnnotationTypes);
                }

            }

            foreach (var providerType in fiRuntimeReflectionUtility.AllSimpleTypesDerivingFrom(typeof(fiISerializerMetadata))) {
                fiISerializerMetadata metadata = GetProvider(providerType);

                LoadedMetadata.Add(metadata);
                optIn.AddRange(metadata.SerializationOptInAnnotationTypes);
                optOut.AddRange(metadata.SerializationOptOutAnnotationTypes);
            }

            SerializationOptInAnnotations = optIn.ToArray();
            SerializationOptOutAnnotations = optOut.ToArray();
        }

        public static List<fiISerializerMetadata> LoadedMetadata {
            get;
            private set;
        }


        private static fiISerializerMetadata _defaultMetadata;
        public static fiISerializerMetadata DefaultMetadata {
            get {
                if (_defaultMetadata == null) {
                    throw new InvalidOperationException("Please register a default serializer. " +
                        "You should see a popup window on the next serialization reload.");
                }

                return _defaultMetadata;
            }
        }

        public static bool IsLoaded(Guid serializerGuid) {
            if (LoadedMetadata == null) {
                return false;
            }

            for (int i = 0; i < LoadedMetadata.Count; ++i) {
                if (LoadedMetadata[i].SerializerGuid == serializerGuid) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Is there a default serializer?
        /// </summary>
        public static bool HasDefault {
            get {
                return _defaultMetadata != null;
            }
        }

        /// <summary>
        /// Annotations that signify a field or property inside a type should *definitely* be
        /// serialized.
        /// </summary>
        public static Type[] SerializationOptInAnnotations {
            get;
            private set;
        }

        /// <summary>
        /// Annotations that signify a field or property inside a type should definitely *not* be
        /// serialized.
        /// </summary>
        public static Type[] SerializationOptOutAnnotations {
            get;
            private set;
        }
    }
}