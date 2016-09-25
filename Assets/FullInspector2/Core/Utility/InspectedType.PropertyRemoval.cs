using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector {
    public partial class InspectedType {
        private static void InitializePropertyRemoval() {
            // We need to remove some properties that when viewed using reflection either
            // a) are not pretty/necessary
            // b) hard-crash Unity

            // NOTE: Because of the way the property resolution system works, we have to make sure
            //       that we remove properties from the highest level items in the inheritance
            //       hierarchy first. Otherwise, the property will show up in derived types that
            //       have already had their properties resolved.

            RemoveProperty<IntPtr>("m_value");

            RemoveProperty<UnityObject>("m_UnityRuntimeReferenceData");
            RemoveProperty<UnityObject>("m_UnityRuntimeErrorString");
            RemoveProperty<UnityObject>("name");
            RemoveProperty<UnityObject>("hideFlags");

            RemoveProperty<Component>("active");
            RemoveProperty<Component>("tag");

            RemoveProperty<Behaviour>("enabled");

            RemoveProperty<MonoBehaviour>("useGUILayout");
        }

        /// <summary>
        /// Attempts to remove the property with the given name.
        /// </summary>
        /// <param name="propertyName">The name of the property to remove.</param>
        public static void RemoveProperty<T>(string propertyName) {
            var type = InspectedType.Get(typeof(T));

            type._nameToProperty.Remove(propertyName);

            // reset all filter caches
            type._cachedMembers = new Dictionary<IInspectedMemberFilter, List<InspectedMember>>();
            type._cachedMethods = new Dictionary<IInspectedMemberFilter, List<InspectedMethod>>();
            type._cachedProperties = new Dictionary<IInspectedMemberFilter, List<InspectedProperty>>();

            // remove it from _allmembers
            for (int i = 0; i < type._allMembers.Count; ++i) {
                var member = type._allMembers[i];
                if (propertyName == member.Name) {
                    type._allMembers.RemoveAt(i);
                    break;
                }
            }
        }
    }
}