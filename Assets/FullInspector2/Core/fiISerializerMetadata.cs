using System;

namespace FullInspector {
    /// <summary>
    /// An interface that a serializer provides that gives Full Inspector some important metadata.
    /// </summary>
    public interface fiISerializerMetadata {
        /// <summary>
        /// A unique guid that identifies this serializer.
        /// </summary>
        Guid SerializerGuid { get; }

        /// <summary>
        /// The type that derives from BaseSerializer in this package, used for serialization needs.
        /// </summary>
        Type SerializerType { get; }

        /// <summary>
        /// Annotation types that mark a field or property as "opt-in" for serialization.
        /// </summary>
        Type[] SerializationOptInAnnotationTypes { get; }

        /// <summary>
        /// Annotation types that specify a field or property should be ignored by serialization.
        /// </summary>
        Type[] SerializationOptOutAnnotationTypes { get; }
    }
}