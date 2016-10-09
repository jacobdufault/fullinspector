using System.Collections;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Tests {
    public class UnityEventSerializationTest : fiBaseEditorTest {
        public override IEnumerable ExecuteTest(MonoBehaviour target) {
            var go = new GameObject();
            go.hideFlags = HideFlags.DontSave;
            OnCleanup += () => UnityObject.DestroyImmediate(go);
            var model = go.AddComponent<UnityEventContainer>();

            UnityEventTools.AddPersistentListener(model.unityEvent, model.EventAction);
            yield return fiTestUtilities.Serialize(target);

            for (int i = 0; i < 50; ++i)
                yield return null;
        }
    }
}