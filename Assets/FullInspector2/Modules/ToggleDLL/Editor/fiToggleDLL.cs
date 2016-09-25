using System;
using System.IO;
using UnityEngine;

namespace FullInspector.Modules {
    // TODO: Finish this module.

    /// <summary>
    /// This class enables a menu item that allows the developer to toggle between DLL or non-dll versions of Full Inspector. Toggling
    /// is done by switching file extensions.
    /// </summary>
    public class fiToggleDLL {
        private static void RecursiveFileNameChange(string directory, string bannedSubstring, string fromExtension, string toExtension) {
            string fromExtensionMeta = fromExtension + ".meta";
            string toExtensionMeta = toExtension + ".meta";

            Debug.Log("Changing the extension of all files inside of \"" + directory + "\" from \"" + fromExtension + "\" to \"" + toExtension + "\"");

            foreach (string originalName in Directory.GetFiles(directory, "*", SearchOption.AllDirectories)) {
                if (originalName.Contains(bannedSubstring)) continue;

                TryMoveFile(originalName, fromExtension, toExtension);
                TryMoveFile(originalName, fromExtensionMeta, toExtensionMeta);
            }
        }

        private static void TryMoveFile(string originalName, string originalExtension, string newExtension) {
            if (originalName.EndsWith(originalExtension)) {
                string newName = originalName.Substring(0, originalName.Length - originalExtension.Length) + newExtension;

                try {
                    //Debug.Log("Moving " + originalName + " to " + newName);
                    File.Move(originalName, newName);
                }
                catch (Exception e) {
                    Debug.LogException(e);
                }
            }
        }

        /* TODO: disabled until DLL support is fully complete / out of testing
        [MenuItem("Window/Full Inspector/Developer/Enable DLLs", false, 1)]
        public static void EnableDLLs() {
            RecursiveFileNameChange(fiSettings.RootDirectory, "Serializers", ".dll_deactivated", ".dll");
            RecursiveFileNameChange(fiSettings.RootDirectory, "Serializers", ".cs", ".cs_deactivated");
            UnityEditor.AssetDatabase.Refresh();
        }

        [MenuItem("Window/Full Inspector/Developer/Disable DLLs", false, 1)]
        public static void DisableDLLs() {
            RecursiveFileNameChange(fiSettings.RootDirectory, "Serializers", ".dll", ".dll_deactivated");
            RecursiveFileNameChange(fiSettings.RootDirectory, "Serializers", ".cs_deactivated", ".cs");
            UnityEditor.AssetDatabase.Refresh();
        }*/
    }
}