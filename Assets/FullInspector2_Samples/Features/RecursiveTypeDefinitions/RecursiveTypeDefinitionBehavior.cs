using UnityEngine;

namespace FullInspector.Samples.Other.RecursiveTypeDefinitions {
    public class RecursiveType {
        public int PrefixValue;
        public RecursiveType Recursive;
        public int PostfixValue;
    }

    [AddComponentMenu("Full Inspector Samples/Other/Recursive Type Definition")]
    public class RecursiveTypeDefinitionBehavior : BaseBehavior<FullSerializerSerializer> {
        [InspectorComment(CommentType.Info, "Full Inspector automatically instantiates null " +
            "references for you, but for recursive types this will cause an infinite loop. Full " +
            "Inspector detects this and automatically stops instantiation. You can continue " +
            "allocation, however, by just pressing the \"create instance\" button.")]
        [ShowInInspector]
        [InspectorHidePrimary]
        private int _comment;

        public RecursiveType Root;
    }
}

