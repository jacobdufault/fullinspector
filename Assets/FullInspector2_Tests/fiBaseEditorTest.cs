using System;
using System.Collections;
using UnityEngine;

namespace FullInspector.Tests {
    /// <summary>
    /// Tests which verify EditorGUI should extend this class instead of a unit test.
    /// </summary>
    public abstract class fiBaseEditorTest {
        /// <summary>
        /// Execute a test. Throw an exception to fail. Success is assumed if there is no exception.
        /// If a test needs to run across more than one update, then this can yield any value. The
        /// yielded value is ignored.
        /// </summary>
        public abstract IEnumerable ExecuteTest(MonoBehaviour target);

        public event Action OnCleanup;

        /// <summary>
        /// Run any logic to cleanup any state. Run even if the test failed.
        /// </summary>
        public void Cleanup() {
            if (OnCleanup != null)
                OnCleanup();
        }

        /// <summary>
        /// Return true if the test was ExecuteTest to run for the given EventType.
        /// </summary>
        public virtual bool WantsEvent(EventType eventType) {
            return eventType == EventType.Repaint;
        }
    }
}