using UnityEngine;

namespace FullInspector.Samples {
    [AddComponentMenu("Full Inspector Samples/Other/Extended Default Editor")]
    public class ExtendedDefaultEditor : BaseBehavior<FullSerializerSerializer> {
        public int fromDefaultEditor1, fromDefaultEditor2, fromDefaultEditor3;
    }
}