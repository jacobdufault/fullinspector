using System;
using System.Reflection;

namespace FullInspector {
    /// <summary>
    /// The NullSerializer simply serializes everything directly to null. This means that FI will
    /// *not* serialize anything.
    /// </summary>
    [Obsolete("Please use [fiInspectorOnly]")]
    public class NullSerializer : BaseSerializer {
        public override string Serialize(MemberInfo storageType, object value,
            ISerializationOperator serializationOperator) {

            return null;
        }

        public override object Deserialize(MemberInfo storageType, string serializedState,
            ISerializationOperator serializationOperator) {

            return null;
        }
    }
}