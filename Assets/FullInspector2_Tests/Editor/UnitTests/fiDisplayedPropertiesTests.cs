using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace FullInspector.Tests {
    public class fiDisplayedPropertiesTests {
        private struct ModelType {
            public int AutoPropertyPublicGetPublicSet { get; set; }
            public int AutoPropertyPublicGetPrivateSet { get; private set; }

            public int ManualPropertyPublicGetPublicSet { get { return 0; } set { } }
            public int ManualPropertyPublicGetPrivateSet { get { return 0; } private set { } }
            public int ManualPropertyPublicGet { get { return 0; } }

            [HideInInspector]
            public int ForceHide_AutoPropertyPublicGetPublicSet { get; set; }
            [HideInInspector]
            public int ForceHide_AutoPropertyPublicGetPrivateSet { get; private set; }

            [ShowInInspector]
            public int ForceShow_ManualPropertyPublicGetPublicSet { get { return 0; } set { } }
            [ShowInInspector]
            public int ForceShow_ManualPropertyPublicGetPrivateSet { get { return 0; } private set { } }
            [ShowInInspector]
            public int ForceShow_ManualPropertyPublicGet { get { return 0; } }

            [SerializeField]
            public int ForceSerialized_AutoPropertyPublicGetPublicSet { get; set; }
            [SerializeField]
            public int ForceSerialized_AutoPropertyPublicGetPrivateSet { get; private set; }
            [SerializeField]
            public int ForceSerialized_ManualPropertyPublicGetPrivateSet { get { return 0; } set { } }
            [NotSerialized]
            public int ForceNotSerialized_AutoPropertyPublicGetPublicSet { get; set; }
            [NotSerialized]
            public int ForceNotSerialized_AutoPropertyPublicGetPrivateSet { get; private set; }
        }

        [Test]
        public void VerifyPropertySerializationAndDisplay() {
            // SerializeAutoProperties = true
            using (new fiTemporaryValue<bool>(fiSettings.SerializeAutoProperties, newValue: true, setter: val => {
                fiSettings.SerializeAutoProperties = val;
                InspectedType.ResetCacheForTesting();
            })) {
                var displayedProperties = InspectedType.Get(typeof(ModelType)).GetProperties(
                        InspectedMemberFilters.InspectableMembers).Select(t => t.Name).ToArray();
                var serializedProperties = InspectedType.Get(typeof(ModelType)).GetProperties(
                        InspectedMemberFilters.FullInspectorSerializedProperties).Select(t => t.Name).ToArray();

                CollectionAssert.AreEqual(new[] {
                    "AutoPropertyPublicGetPublicSet",
                    "AutoPropertyPublicGetPrivateSet",
                    "ForceShow_ManualPropertyPublicGetPublicSet",
                    "ForceShow_ManualPropertyPublicGetPrivateSet",
                    "ForceShow_ManualPropertyPublicGet",
                    "ForceSerialized_AutoPropertyPublicGetPublicSet",
                    "ForceSerialized_AutoPropertyPublicGetPrivateSet",
                    "ForceSerialized_ManualPropertyPublicGetPrivateSet"
                }, displayedProperties);
                CollectionAssert.AreEqual(new[] {
                    "AutoPropertyPublicGetPublicSet",
                    "AutoPropertyPublicGetPrivateSet",
                    "ForceHide_AutoPropertyPublicGetPublicSet",
                    "ForceHide_AutoPropertyPublicGetPrivateSet",
                    "ForceSerialized_AutoPropertyPublicGetPublicSet",
                    "ForceSerialized_AutoPropertyPublicGetPrivateSet",
                    "ForceSerialized_ManualPropertyPublicGetPrivateSet"
                }, serializedProperties);
            }

            // SerializeAutoProperties = false
            using (new fiTemporaryValue<bool>(fiSettings.SerializeAutoProperties, newValue: false, setter: val => {
                fiSettings.SerializeAutoProperties = val;
                InspectedType.ResetCacheForTesting();
            })) {
                var displayedProperties = InspectedType.Get(typeof(ModelType)).GetProperties(
                        InspectedMemberFilters.InspectableMembers).Select(t => t.Name).ToArray();
                var serializedProperties = InspectedType.Get(typeof(ModelType)).GetProperties(
                        InspectedMemberFilters.FullInspectorSerializedProperties).Select(t => t.Name).ToArray();

                CollectionAssert.AreEqual(new[] {
                    "ForceShow_ManualPropertyPublicGetPublicSet",
                    "ForceShow_ManualPropertyPublicGetPrivateSet",
                    "ForceShow_ManualPropertyPublicGet",
                    "ForceSerialized_AutoPropertyPublicGetPublicSet",
                    "ForceSerialized_AutoPropertyPublicGetPrivateSet",
                    "ForceSerialized_ManualPropertyPublicGetPrivateSet"
                }, displayedProperties);
                CollectionAssert.AreEqual(new[] {
                    "ForceSerialized_AutoPropertyPublicGetPublicSet",
                    "ForceSerialized_AutoPropertyPublicGetPrivateSet",
                    "ForceSerialized_ManualPropertyPublicGetPrivateSet"
                }, serializedProperties);
            }
        }
    }
}