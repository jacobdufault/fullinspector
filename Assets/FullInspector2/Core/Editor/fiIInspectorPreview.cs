using UnityEngine;

namespace FullInspector {
    public interface fiIInspectorPreview {
        void OnPreviewGUI(Rect r, GUIStyle background);
        void OnPreviewSettings();
    }
}