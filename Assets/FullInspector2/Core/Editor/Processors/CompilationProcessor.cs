using System;
using UnityEditor;

namespace FullInspector.Internal {
    /// <summary>
    /// Detects when Unity is recompiling code. Before a recompile happens, all state is saved and
    /// after the recompilation is finished all state is restored.
    /// </summary>
    [InitializeOnLoad]
    public static class CompilationDetector {
        static CompilationDetector() {
            if (fiSettings.ForceSaveAllAssetsOnRecompilation) {
                EditorApplication.update += SaveAllUpdate;
            }

            if (fiSettings.ForceRestoreAllAssetsOnRecompilation) {
                EditorApplication.update += RestoreAllUpdate;
            }
        }

        /// <summary>
        /// True if we have detected a compile but have already saved. This is set to false by Unity
        /// after a compilation has finished.
        /// </summary>
        [NonSerialized]
        private static bool _saved = false;

        private static void SaveAllUpdate() {
            if (!_saved && EditorApplication.isCompiling) {
                _saved = true;
                fiSaveManager.SaveAll();
            }
        }

        /// <summary>
        /// True if everything has been restored. This is set to false by Unity after a compilation
        /// / reload has occurred.
        /// </summary>
        [NonSerialized]
        private static bool _restored = false;

        private static void RestoreAllUpdate() {
            if (_restored == false) {
                _restored = true;
                fiSaveManager.RestoreAll();
            }
        }
    }
}