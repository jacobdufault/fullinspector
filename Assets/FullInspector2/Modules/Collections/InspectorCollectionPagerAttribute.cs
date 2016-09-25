using System;

namespace FullInspector {
    /// <summary>
    /// Enables customization of how the pager interface on collections is activated. The pager
    /// is used to show a subset of the current collection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class InspectorCollectionPagerAttribute : Attribute {
        /// <summary>
        /// The minimum collection length before the pager is displayed. A value of 0 means that the pager
        /// will always be displayed, and a negative value means that the pager will never be displayed. Use
        /// AlwaysHide or AlwaysShow as utility methods for setting PageMinimumCollectionLength to those
        /// special values.
        /// </summary>
        public int PageMinimumCollectionLength;

        /// <summary>
        /// If true, then the pager will always be hidden. This is a proxy for setting PageMinimumCollectionLength to -1.
        /// </summary>
        public bool AlwaysHide {
            set {
                if (value) PageMinimumCollectionLength = -1;
                else PageMinimumCollectionLength = fiSettings.DefaultPageMinimumCollectionLength;
            }
            get { return PageMinimumCollectionLength < 0; }
        }

        /// <summary>
        /// If true, then the pager will always be shown. This is a proxy for setting PageMinimumCollectionLength to 0.
        /// </summary>
        public bool AlwaysShow {
            set {
                if (value) PageMinimumCollectionLength = 0;
                else PageMinimumCollectionLength = fiSettings.DefaultPageMinimumCollectionLength;
            }
            get { return PageMinimumCollectionLength == 0; }
        }

        public InspectorCollectionPagerAttribute() {
            PageMinimumCollectionLength = fiSettings.DefaultPageMinimumCollectionLength;
        }

        public InspectorCollectionPagerAttribute(int pageMinimumCollectionLength) {
            PageMinimumCollectionLength = pageMinimumCollectionLength;
        }
    }
}
