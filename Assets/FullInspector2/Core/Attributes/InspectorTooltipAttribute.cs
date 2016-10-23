using System;
using System.Reflection;
using FullSerializer.Internal;

namespace FullInspector {
    /// <summary>
    /// Adds a tooltip to an field or property that is viewable in the inspector.
    /// If you add this to a MonoBehaviour derived class, the tooltip text will
    /// appear in the "Name" field that appears at the start of every inspector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class InspectorTooltipAttribute : Attribute {
        public string Tooltip;

        public InspectorTooltipAttribute(string tooltip) {
            Tooltip = tooltip ?? "";
        }

        public static string GetTooltip(MemberInfo memberInfo) {
            var tooltip = fsPortableReflection.GetAttribute<InspectorTooltipAttribute>(memberInfo);
            if (tooltip == null)
                return "";
            return tooltip.Tooltip;
        }
    }
}