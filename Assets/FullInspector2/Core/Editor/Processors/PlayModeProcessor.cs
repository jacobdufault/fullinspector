using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    /// <summary>
    /// Detects when the editor has entered or left play mode or has paused.
    /// </summary>
    /// <remarks>
    /// This class is currently disabled / not used, but it remains here because the code is tricky
    /// to get right and may be useful in the future.
    /// </remarks>
    [InitializeOnLoad]
    public class PlayModeDetector : ScriptableObject {
        static PlayModeDetector() {
            //EditorApplication.playmodeStateChanged += PlaymodeStateChange;
            //EditorApplication.update += Update;
        }

        private static void Update() {
            // entering play mode
            if (EditorApplication.isPlayingOrWillChangePlaymode && EditorApplication.isPlaying == false) {
                OnTransition(EditorTransition.PreIntoPlay);
            }
        }

        /// <summary>
        /// A transition that the editor can go through.
        /// </summary>
        private enum EditorTransition {
            /// <summary>
            /// The editor is about to enter play mode but has yet to go through Unity
            /// serialization.
            /// </summary>
            PreIntoPlay,

            /// <summary>
            /// The editor is about to enter play mode and has just gone through Unity
            /// serialization.
            /// </summary>
            PostIntoPlay,

            /// <summary>
            /// The editor is leaving play mode and has just gone through Unity serialization.
            /// </summary>
            PostOutPlay,

            /// <summary>
            /// The editor is about to enter pause mode
            /// </summary>
            IntoPause,

            /// <summary>
            /// The editor is about to leave pause mode
            /// </summary>
            OutPause
        }

        /// <summary>
        /// The last playing state
        /// </summary>
        private static bool _lastPlaying;

        /// <summary>
        /// The last paused state
        /// </summary>
        private static bool _lastPaused;

        private static void PlaymodeStateChange() {
            // The play mode life cycle works like this: Currently in editor and you press play:
            // - PlaymodeStateChange isPlaying=false
            // - PlaymodeStateChange isPlaying=true
            // - ...playing the game...
            // - PlayModeStateChange isPlaying=true isPaused=true when paused
            // - ..game paused...
            // - PlayModeStateChange isPlaying=true isPaused=false when unpaused
            // - ...playing the game...
            // - PlaymodeStateChange isPlaying=true
            // - PlaymodeStateChange isPlaying=false

            bool isPlaying = EditorApplication.isPlaying;
            bool isPaused = EditorApplication.isPaused;

            // we were not paused before but are now
            if (!_lastPaused && isPaused) {
                OnTransition(EditorTransition.IntoPause);
            }

            // we were paused before but are not now
            if (_lastPaused && !isPaused) {
                OnTransition(EditorTransition.OutPause);
            }

            _lastPaused = isPaused;

            // we were playing before but are not now
            if (_lastPlaying && !isPlaying) {
                OnTransition(EditorTransition.PostOutPlay);
            }

            _lastPlaying = isPlaying;
        }

        /// <summary>
        /// This function is called when the editor is undergoing a play mode related transition.
        /// </summary>
        private static void OnTransition(EditorTransition transition) {
            fiLog.Log(typeof(PlayModeDetector), "Got transition " + transition);

            switch (transition) {
                case EditorTransition.PreIntoPlay:
                    fiSerializationManager.SerializeObject(typeof(PlayModeDetector), Selection.activeObject);
                    break;

                case EditorTransition.PostIntoPlay:
                    // we don't have to restore here, as BaseBehavior will automatically restore
                    // itself
                    break;
                case EditorTransition.PostOutPlay:
                    // we don't have to restore here, as BaseBehaviorEditor will automatically
                    // restore the necessary BaseBehaviors
                    break;

                // don't do anything for pausing in this release
                case EditorTransition.IntoPause:
                case EditorTransition.OutPause:
                    break;
            }
        }
    }
}