using System;
using System.IO;
using FullSerializer.Internal;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Internal.Versioning {
    [InitializeOnLoad]
    public class fiVersionManager {
        // NOTE:
        // When adding a new version marker, here's what needs to be done:
        // 1. Add a new type, fiImportVersionMarkter *that has a unique new .meta file*
        // 2. Up the CurrentVersion string below
        // 3. Add the previous type name to OldVersionMarkers
        // 4. Delete the old version type file

        /// <summary>
        /// The current version string.
        /// </summary>
        public static string CurrentVersion = "2.6.2";

        /// <summary>
        /// Full Inspector will effectively force the user to do a clean import if any of the following types are found in
        /// the assembly.
        /// </summary>
        private static string[] OldVersionMarkers =
            {
                "FullInspector.Internal.Versioning.fiImportVersionMarker_24",
                "FullInspector.Internal.Versioning.fiImportVersionMarker_25",
                "FullInspector.Internal.Versioning.fiImportVersionMarker_26",
                "FullInspector.Internal.Versioning.fiImportVersionMarker_2_6_1",
            };

        static fiVersionManager() {
            foreach (var oldVersion in OldVersionMarkers) {
                Type oldType = fsTypeCache.GetType(oldVersion);
                if (oldType != null) {

                    if (EditorUtility.DisplayDialog("Clean Import Needed", "Full Inspector has detected that you recently upgraded versions but did not do a " +
                        "clean import. This will lead to subtle errors." + Environment.NewLine + Environment.NewLine + "Please delete the \"" +
                        fiSettings.RootDirectory + "\" folder and reimport Full Inspector.", "Delete folder?", "I'll do it later")) {

                        Debug.Log("Deleting \"" + fiSettings.RootDirectory + "\"");
                        Directory.Delete(fiSettings.RootDirectory, /*recursive:*/ true);

                        string metapath = fiSettings.RootDirectory.TrimEnd('/') + ".meta";
                        Debug.Log("Deleting \"" + metapath + "\"");
                        File.Delete(metapath);
                    }
                }
            }
        }
    }
}