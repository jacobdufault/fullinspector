using System.Collections.Generic;

namespace FullInspector {
    public interface fiICollectionAttributeProvider {
        IEnumerable<object> GetAttributes();
    }
}