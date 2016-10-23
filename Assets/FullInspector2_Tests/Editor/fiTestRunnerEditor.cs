using System;
using FullInspector.Internal;
using FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace FullInspector.Tests {
    [CustomEditor(typeof(fiTestRunner))]
    public class fiTestRunnerEditor : Editor {
        private enum TestStatus {
            InProgress,
            Done
        }

        private static TestStatus RunTest(fiTestRunner.RunningTest test) {
            // Skip test for now if it isn't interested in the current event
            // type.
            if (!test.Test.WantsEvent(Event.current.type))
                return TestStatus.InProgress;

            try {
                if (test.Progress.MoveNext() == false)
                    return TestStatus.Done;
                return TestStatus.InProgress;
            }
            catch (Exception e) {
                Debug.LogError("Failed test " + test.GetType().CSharpName());
                Debug.LogException(e);
                return TestStatus.Done;
            }
        }

        public override void OnInspectorGUI() {
            using (new fiTemporaryValue<bool>(Assert.raiseExceptions, true, v => Assert.raiseExceptions = v)) {
                fiTestRunner target = (fiTestRunner)this.target;

                if (GUILayout.Button("Run tests")) {
                    foreach (fiBaseEditorTest test in fiRuntimeReflectionUtility.GetAssemblyInstances<fiBaseEditorTest>()) {
                        target.RunningTests.Add(new fiTestRunner.RunningTest {
                            Test = test,
                            Progress = test.ExecuteTest(target).GetEnumerator()
                        });
                    }
                }

                if (target.RunningTests.Count > 0) {
                    if (RunTest(target.RunningTests[0]) == TestStatus.Done) {
                        target.RunningTests[0].Test.Cleanup();
                        target.RunningTests.RemoveAt(0);
                    }

                    // Repaint while we still have tests in progress.
                    if (target.RunningTests.Count > 0)
                        Repaint();
                }
            }
        }
    }
}