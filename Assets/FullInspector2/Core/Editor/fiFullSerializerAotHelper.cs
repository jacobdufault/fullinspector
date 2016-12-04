using System.IO;
using System.Linq;
using FullSerializer;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Internal {
    public static class fiFullSerializerAotHelper {
        public struct A<X, Y> {}

        [MenuItem("Window/Full Inspector/Aot Serialization Manager", priority=3)]
        private static void SelectAotSerializationManager() {
            Debug.Log(typeof(A<int, double>).CSharpName(true, true));

            fsAotConfiguration[] configs = Resources.FindObjectsOfTypeAll<fsAotConfiguration>();
            fsAotConfiguration aotConfig = configs.FirstOrDefault();
            if (ReferenceEquals(aotConfig, null)) {
                aotConfig = ScriptableObject.CreateInstance<fsAotConfiguration>();
                AssetDatabase.CreateAsset(aotConfig, Path.Combine(fiSettings.RootGeneratedDirectory, "fsAotConfiguration.asset"));
            }

            Selection.activeObject = aotConfig;
        }
    }
}