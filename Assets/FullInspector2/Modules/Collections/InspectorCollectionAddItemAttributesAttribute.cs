using System;
using System.Reflection;
using System.Linq;
using FullInspector.Internal;
using FullSerializer.Internal;

namespace FullInspector {
    /// <summary>
    /// Allows you to customize the `Add` item in, say, a Dictionary or a HashSet. This is analogous to
    /// InspectorCollectionItemAttributes so please see the documentation on that class for usage instructions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class InspectorCollectionAddItemAttributesAttribute : Attribute {
        // ICustomAttributeProvider not necessarily available, so we just use MemberInfo instead
        public MemberInfo AttributeProvider;

        public InspectorCollectionAddItemAttributesAttribute(Type attributes) {
            if (typeof(fiICollectionAttributeProvider).Resolve().IsAssignableFrom(attributes.Resolve()) == false) {
                throw new ArgumentException("Must be an instance of FullInspector.fiICollectionAttributeProvider", "attributes");
            }

            var instance = (fiICollectionAttributeProvider)Activator.CreateInstance(attributes);
            AttributeProvider = fiAttributeProvider.Create(instance.GetAttributes().ToArray());
        }
    }
}
