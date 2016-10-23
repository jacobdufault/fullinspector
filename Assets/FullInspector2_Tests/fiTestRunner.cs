using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FullInspector.Tests {
    // TODO: Move to an editor window approach? That way we can remove this
    //       MonoBehaviour.

    /// <summary>
    /// Hosts FI editor tests. Has a custom editor.
    /// </summary>
    public class fiTestRunner : MonoBehaviour {
        public class RunningTest {
            public fiBaseEditorTest Test;
            public IEnumerator Progress;
        }

        public List<RunningTest> RunningTests = new List<RunningTest>();
    }
}