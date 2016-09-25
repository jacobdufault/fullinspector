using System.Collections.Generic;

namespace FullInspector.Internal {
    public static class fiDirectory {
        public static bool Exists(string path) {
#if UNITY_EDITOR || !UNITY_WINRT
            return System.IO.Directory.Exists(path);
#else
            throw new System.NotSupportedException();
#endif
        }

        public static void CreateDirectory(string path) {
#if UNITY_EDITOR || !UNITY_WINRT
            System.IO.Directory.CreateDirectory(path);
#else
            throw new System.NotSupportedException();
#endif
        }

        public static IEnumerable<string> GetDirectories(string path) {
#if UNITY_EDITOR || !UNITY_WINRT
            return System.IO.Directory.GetDirectories(path);
#else      
            throw new System.NotSupportedException();
#endif
        }
    }
}