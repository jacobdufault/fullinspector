using UnityEditor.AnimatedValues;
using UnityEngine;

namespace FullInspector.Internal {
    /// <summary>
    /// A wrapper around AnimFloat that makes it easier to use.
    /// </summary>
    public class fiAnimationMetadata : IGraphMetadataItemNotPersistent {
        [SerializeField]
        private AnimFloat _animator;

        /// <summary>
        /// Update the height of the editor.
        /// </summary>
        /// <param name="height">The new height.</param>
        /// <param name="allowAnimation">Should animation be allowed? Sometimes animation is not
        /// desired if a sub-item is animation. If this item animations when a sub-item is
        /// animating, then there will be visible UX jerk.</param>
        /// <returns>If true, then a transition is occurring and the animation is rendering.</returns>
        public bool UpdateHeight(float height) {
            if (_animator == null) {
                _animator = new AnimFloat(height);
                return false;
            }

            if (_animator.target != height) {
                if (fiSettings.EnableAnimation) {
                    _animator.target = height;
                }
                else {
                    _animator = new AnimFloat(height);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns true if animation is occurring.
        /// </summary>
        public bool IsAnimating {
            get {
                return _animator != null && _animator.isAnimating;
            }
        }

        /// <summary>
        /// The current height of the animation.
        /// </summary>
        public float AnimationHeight {
            get {
                if (_animator == null) {
                    return 0;
                }

                return _animator.value;
            }
        }
    }
}