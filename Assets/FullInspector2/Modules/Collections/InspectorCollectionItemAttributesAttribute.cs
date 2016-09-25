using System;
using System.Reflection;
using System.Linq;
using FullInspector.Internal;
using FullSerializer.Internal;

namespace FullInspector {
    /// <summary>
    /// <para>
    /// You can use this interface to customize how rendering of items inside of an collection is done. Usage is slightly unintuitive
    /// because C# annotations are not very expressive.
    /// </para>
    /// <para>
    /// Let's say we want to display a comment above every field inside of the list. Here's how we can do it:
    /// 
    /// <![CDATA[
    /// class ObjectsItemAttrs : fiICollectionAttributeProvider {
    ///     public IEnumerable<object> GetAttributes() {
    ///         yield return new InspectorCommentAttribute("Hi!");
    ///     }
    /// }
    /// [InspectorCollectionItemAttributes(typeof(ObjectsItemAttrs))]
    /// public List<object> Objects;
    /// ]]>
    /// 
    /// Whereas if we were displaying the same thing normally (without the comment) it would be a simple:
    /// <![CDATA[
    /// public List<object> Objects;
    /// ]]>
    /// 
    /// There's quite a bit of boilerplate, but it enables this powerful customization.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class InspectorCollectionItemAttributesAttribute : Attribute {
        // ICustomAttributeProvider not necessarily available, so we just use MemberInfo instead
        public MemberInfo AttributeProvider;

        public InspectorCollectionItemAttributesAttribute(Type attributes) {
            if (typeof(fiICollectionAttributeProvider).Resolve().IsAssignableFrom(attributes.Resolve()) == false) {
                throw new ArgumentException("Must be an instance of FullInspector.fiICollectionAttributeProvider", "attributes");
            }

            var instance = (fiICollectionAttributeProvider)Activator.CreateInstance(attributes);
            AttributeProvider = fiAttributeProvider.Create(instance.GetAttributes().ToArray());
        }
    }
}
