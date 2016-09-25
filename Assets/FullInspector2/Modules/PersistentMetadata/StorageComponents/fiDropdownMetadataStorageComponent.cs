using System;
using FullInspector.Internal;
using UnityEngine;

namespace FullInspector {
    [Serializable]
    public class fiDropdownMetadata : IGraphMetadataItemPersistent
#if !UNITY_4_3
        , ISerializationCallbackReceiver
#endif
        {
        /// <summary>
        /// Is the foldout currently active, ie, is the rendered item being displayed or is the
        /// short-form foldout being displayed?
        /// </summary>
        public bool IsActive {
            get {
                return _isActive.value;
            }
            set {
                if (value != _isActive.target) {
                    if (fiSettings.EnableAnimation) {
                        _isActive.target = value;
                    }
                    else {
                        _isActive = new fiAnimBool(value);
                    }
                }
            }
        }
        private fiAnimBool _isActive = new fiAnimBool(true);

        /// <summary>
        /// What percentage are we at in the animation between active states?
        /// </summary>
        public float AnimPercentage {
            get {
                return _isActive.faded;
            }
        }

        /// <summary>
        /// Are we currently animating between different states?
        /// </summary>
        public bool IsAnimating {
            get {
                return _isActive.isAnimating;
            }
        }

        /// <summary>
        /// Should we render a dropdown? This will be false if the override has been set *or* if
        /// the element is not above a certain minimum height.
        /// </summary>
        public bool ShouldDisplayDropdownArrow {
            get {
                return _forceDisable == false && _showDropdown;
            }
            set {
                if (_forceDisable && value) {
                    return;
                }
                _showDropdown = value;
            }
        }
        [SerializeField]
        private bool _showDropdown;

        /// <summary>
        /// Inverts the default state of the dropdown metadata, ie, collapsed is default. This
        /// is useful for serialization.
        /// </summary>
        public void InvertDefaultState() {
            _invertedDefaultState = true;
        }
        private bool _invertedDefaultState;

        public void ForceHideWithoutAnimation() {
            _forceDisable = false;
            _showDropdown = true;
            _isActive = new fiAnimBool(false);
        }

        /// <summary>
        /// Should rendering of the dropdown be completely skipped?
        /// </summary>
        public void ForceDisable() {
            _forceDisable = true;
        }
        private bool _forceDisable;


#if !UNITY_4_3
        [SerializeField]
        private bool _serializedIsActive;
        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            _serializedIsActive = IsActive;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            _isActive = new fiAnimBool(_serializedIsActive);
        }
#endif

        bool IGraphMetadataItemPersistent.ShouldSerialize() {
            if (_invertedDefaultState) return IsActive;
            return IsActive == false;
        }
    }

}

namespace FullInspector.Internal {
    // boilerplate to integrate with Unity

    // A component for Unity to store the data within
    [AddComponentMenu("")]
    public class fiDropdownMetadataStorageComponent : fiBaseStorageComponent<fiDropdownGraphMetadataSerializer> { }

    // To serialize the graph metadata
    [Serializable] public class fiDropdownGraphMetadataSerializer : fiGraphMetadataSerializer<fiDropdownMetadata> { }

    // To provide the presistent metadata system information about our types
    public class fiDropdownMetadataProvider : fiPersistentEditorStorageMetadataProvider<fiDropdownMetadata, fiDropdownGraphMetadataSerializer> { }
}