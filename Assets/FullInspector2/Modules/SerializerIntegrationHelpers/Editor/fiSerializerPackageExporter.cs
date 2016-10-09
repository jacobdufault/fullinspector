using System.IO;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Internal {
    public class fiSerializerPackageExporter {
        private static string BasePath = fiUtility.CombinePaths(fiSettings.RootDirectory, "Serializers/");

        [MenuItem("Window/Full Inspector/Developer/Create Serializer Packages")]
        public static void DoExport() {
            Export(
                fiUtility.CombinePaths(BasePath, "BinaryFormatterPackage.unitypackage"),
                fiUtility.CombinePaths(BasePath, "Formatter"));

            Export(
                fiUtility.CombinePaths(BasePath, "JsonNetPackage.unitypackage"),
                fiUtility.CombinePaths(BasePath, "JsonNet"));

            Export(
                fiUtility.CombinePaths(BasePath, "ProtoBufNetPackage.unitypackage"),
                fiUtility.CombinePaths(BasePath, "protobuf-net"));
        }

        private static void Export(string to, string directory) {
            if (Directory.Exists(directory)) {
                AssetDatabase.ExportPackage(directory, to, ExportPackageOptions.Recurse);
                Debug.Log("Wrote new package to " + to + " from " + directory);
            }
        }
    }
}