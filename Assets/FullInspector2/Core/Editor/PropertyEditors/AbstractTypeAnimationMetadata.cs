using UnityEngine;

namespace FullInspector.Internal {
    public class AbstractTypeAnimationMetadata : IGraphMetadataItemNotPersistent {
        /// <summary>
        /// We only animate the abstract type height when we have changed types ourselves. If we
        /// always animate when a height change occurs and the inside is also animating, then we
        /// will create an "animation lag" which looks awful.
        /// </summary>
        [SerializeField]
        public bool ChangedTypes = true;
    }
}