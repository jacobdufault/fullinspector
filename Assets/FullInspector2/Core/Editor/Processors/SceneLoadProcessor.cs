namespace FullInspector.Internal {
    /*
     * This file can be used to detect when a new scene has been loaded. For the moment, that logic
     * is unnecessary. The approach has been saved, though.
     */

    /*
    [InitializeOnLoad]
    public static class SceneLoadProcessor {
        private static string _lastScene;

        static SceneLoadProcessor() {
            _lastScene = EditorApplication.currentScene;
            EditorApplication.update += Update;
        }

        private static void Update() {
            if (_lastScene != EditorApplication.currentScene) {
                //a scene change has happened
                Debug.Log("Last scene=" + _lastScene + "; new scene=" +
                    EditorApplication.currentScene + "; restoring saved content");
                _lastScene = EditorApplication.currentScene;

                //SaveManager.Restore();
            }
        }
    }
    */
}