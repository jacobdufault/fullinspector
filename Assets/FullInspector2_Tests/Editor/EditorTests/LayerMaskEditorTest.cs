using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace FullInspector.Tests {
    public class LayerMaskEditorTest : fiBaseEditorTest {
        public class Model {
            public LayerMask mask;
        }

        public override IEnumerable ExecuteTest(MonoBehaviour target) {
            var metadata = new fiGraphMetadata();

            LayerMask expectedValue = ~(1 << LayerMask.NameToLayer("Water"));

            Model model = new Model {
                mask = 0
            };

            yield return fiTestUtilities.DrawPropertyEditor(model, metadata);
            model.mask = expectedValue;
            yield return fiTestUtilities.DrawPropertyEditor(model, metadata);

            Assert.AreEqual(expectedValue, model.mask);
        }
    }
}