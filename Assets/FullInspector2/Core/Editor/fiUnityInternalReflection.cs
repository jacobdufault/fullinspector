using UnityEditor;

namespace FullInspector.Internal {
    /// <summary>
    /// This class contains methods that do not use public Unity API. These are subject to
    /// break/change per update.
    /// </summary>
    public static class fiUnityInternalReflection {
        /// <summary>
        /// Attempts to enable/disable the bold font that is used by Unity when an object has a
        /// value different from its prefab.
        /// </summary>
        /// <param name="enabled">True if the bold font is set, false if it is not.</param>
        public static void SetBoldDefaultFont(bool enabled) {
            fiRuntimeReflectionUtility.InvokeStaticMethod(
                typeof(EditorGUIUtility), "SetBoldDefaultFont", new object[] { enabled });
        }
    }
}