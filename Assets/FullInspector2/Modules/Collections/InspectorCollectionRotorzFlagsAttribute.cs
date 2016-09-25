using System;
using FullInspector.Rotorz.ReorderableList;

namespace FullInspector {
    /// <summary>
    /// Specify the rotorz flags for a collection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class InspectorCollectionRotorzFlagsAttribute : Attribute {
        public ReorderableListFlags Flags;

        public bool DisableReordering {
            get { return HasFlag(ReorderableListFlags.DisableReordering); }
            set { UpdateFlag(value, ReorderableListFlags.DisableReordering); }
        }

        public bool HideAddButton {
            get { return HasFlag(ReorderableListFlags.HideAddButton); }
            set { UpdateFlag(value, ReorderableListFlags.HideAddButton); }
        }

        public bool HideRemoveButtons {
            get { return HasFlag(ReorderableListFlags.HideRemoveButtons); }
            set { UpdateFlag(value, ReorderableListFlags.HideRemoveButtons); }
        }

        public bool ShowIndices {
            get { return HasFlag(ReorderableListFlags.ShowIndices); }
            set { UpdateFlag(value, ReorderableListFlags.ShowIndices); }
        }


        private void UpdateFlag(bool shouldSet, ReorderableListFlags flag) {
            if (shouldSet) Flags |= flag;
            else Flags &= ~flag;
        }

        private bool HasFlag(ReorderableListFlags flag) {
            return (Flags & flag) != 0;
        }
    }
}