using System.Collections;
using System.Collections.Generic;
using FullInspector.Internal;
using UnityEngine;
using tk = FullInspector.tk<System.Collections.IList, FullInspector.Modules.tkDatabaseContext>;

namespace FullInspector.Modules {
    class tkDatabaseContext {
        public GUIContent label;
        public IList editedList;
    }

    /// <summary>
    /// Provides a relatively simple editor for IList{T} types that only views
    /// one element at a time. This is useful if the list is massive, or perhaps
    /// to just reduce information overload when editing.
    /// </summary>
    [CustomAttributePropertyEditor(typeof(InspectorDatabaseEditorAttribute), ReplaceOthers = true)]
    public class InspectorDatabaseEditorAttributeEditor<TDerived, T> :
        AttributePropertyEditor<IList<T>, InspectorDatabaseEditorAttribute>
        where TDerived : IList<T> {
        public override bool DisplaysStandardLabel {
            get { return false; }
        }

        /// <summary>
        /// Returns true if there is currently an item that is being edited.
        /// </summary>
        private static bool HasEditableItem(tkDatabaseContext context) {
            int index = GetCurrentIndex(context);
            return index >= 0 && index < context.editedList.Count;
        }

        /// <summary>
        /// Attempts to ensure that the current editing index is not out of
        /// range. However, if the edited list is empty, then the index will
        /// always be out of range.
        /// </summary>
        private static void TryEnsureValidIndex(tkDatabaseContext context) {
            var list = context.editedList;
            var metadata = fiGlobalMetadata.Get<InspectorDatabaseEditorMetadata>(list);

            if (list.Count == 0) {
                metadata.CurrentIndex = -1;
            }
            else if (metadata.CurrentIndex < 0) {
                metadata.CurrentIndex = 0;
            }
            else if (metadata.CurrentIndex >= list.Count) {
                metadata.CurrentIndex = list.Count - 1;
            }
        }

        private static bool CanDeleteEditedItem(tkDatabaseContext context) {
            return HasEditableItem(context);
        }
        private static void DeleteEditedItem(tkDatabaseContext context) {
            fiListUtility.RemoveAt<T>(ref context.editedList, GetCurrentIndex(context));
            TryEnsureValidIndex(context);
        }

        private static bool CanInsertItem(tkDatabaseContext context) {
            var list = context.editedList;
            return list.Count > 0 && GetCurrentIndex(context) < list.Count - 1;
        }
        private static void InsertItem(tkDatabaseContext context) {
            fiListUtility.InsertAt<T>(ref context.editedList, GetCurrentIndex(context));
        }

        private static int GetCurrentIndex(tkDatabaseContext context) {
            var metadata = fiGlobalMetadata.Get<InspectorDatabaseEditorMetadata>(context.editedList);
            return metadata.CurrentIndex;
        }

        private static void ChangeIndexTo(tkDatabaseContext context, int index) {
            var metadata = fiGlobalMetadata.Get<InspectorDatabaseEditorMetadata>(context.editedList);
            metadata.CurrentIndex = index;
            TryEnsureValidIndex(context);
        }

        private static bool CanMoveIndexByOffset(tkDatabaseContext context, int offset) {
            int newIndex = GetCurrentIndex(context) + offset;
            return newIndex >= 0;
        }
        private static void MoveIndexByOffset(tkDatabaseContext context, int offset) {
            var metadata = fiGlobalMetadata.Get<InspectorDatabaseEditorMetadata>(context.editedList);
            metadata.CurrentIndex += offset;

            if (metadata.CurrentIndex < 0) {
                metadata.CurrentIndex = 0;
            }

            // If we don't have enough elements inside of the list, then we'll
            // just add to the list until we have a valid index
            while (metadata.CurrentIndex >= context.editedList.Count) {
                fiListUtility.Add<T>(ref context.editedList);
                metadata.CurrentIndex = context.editedList.Count - 1;
            }
        }

        private static bool CanSwapItemByOffset(tkDatabaseContext context, int offset) {
            int a = GetCurrentIndex(context);
            int b = a + offset;

            var list = context.editedList;
            return list.Count > 0 && a >= 0 && a < list.Count && b >= 0 && b < list.Count;
        }
        private static void SwapItemByOffset(tkDatabaseContext context, int offset) {
            int a = GetCurrentIndex(context);
            int b = a + offset;

            // Make sure we can do the swap - we *should* be able to remove this
            // code
            var list = context.editedList;
            if (list.Count > 0 && a >= 0 && a < list.Count && b >= 0 && b < list.Count) {
                var temp = list[a];
                list[a] = list[b];
                list[b] = temp;
            }
        }

        private static bool ShouldAddNextItem(tkDatabaseContext context) {
            var list = context.editedList;
            return list.Count == 0 || GetCurrentIndex(context) == list.Count - 1;
        }
        private static fiGUIContent GetForwardButtonText(tkDatabaseContext context) {
            if (ShouldAddNextItem(context)) return Label_Forward_Add;
            return Label_Forward_Forward;
        }

        private static fiGUIContent GetLabelText(tkDatabaseContext context) {
            var label = new GUIContent(context.label);

            if (context.editedList.Count == 0) {
                label.text += " (empty)";
            }
            else {
                label.text += " (element " + (GetCurrentIndex(context) + 1) + " of " + context.editedList.Count + ")";
            }

            return label;
        }

        private static readonly fiGUIContent Label_SwapBack = new fiGUIContent("<Swap", "Swaps this item with the previous item");
        private static readonly fiGUIContent Label_SwapForward = new fiGUIContent("Swap>", "Swaps this item with the next item");
        private static readonly fiGUIContent Label_Back = new fiGUIContent("<<", "View the previous item");
        private static readonly fiGUIContent Label_Forward_Forward = new fiGUIContent(">>", "View the next item");
        private static readonly fiGUIContent Label_Forward_Add = new fiGUIContent("Add", "Add a new item");
        private static readonly fiGUIContent Label_Insert = new fiGUIContent("Ins", "Insert a new item into current position");
        private static readonly fiGUIContent Label_Delete = new fiGUIContent("X", "Delete the current item");

        private static readonly tkControlEditor ControlEditor = new tkControlEditor(
            new tk.VerticalGroup {
                new tk.Box(new tk.Margin(12,
                    new tk.VerticalGroup {
                        new tk.IntSlider(tk.Val((o, c) => GetLabelText(c)),
                            /*min, max*/ 1, tk.Val(l => l.Count),
                            /*get*/ (l, c) => l.Count == 0 ? 0 : GetCurrentIndex(c) + 1,
                            /*set*/ (l, c, i) => ChangeIndexTo(c, i-1)) {
                            Style = new tk.EnabledIf(l => l.Count > 0)
                        },

                        new tk.HorizontalGroup {
                            {
                                60,
                                new tk.Button(Label_SwapBack, (o, c) => SwapItemByOffset(c, -1)) {
                                    Style = new tk.EnabledIf((o, c) => CanSwapItemByOffset(c, -1))
                                }
                            },

                            2,

                            new tk.Button(Label_Back, (o, c) => MoveIndexByOffset(c, -1)) {
                                Style = new tk.EnabledIf((o, c) => CanMoveIndexByOffset(c, -1))
                            },

                            2,

                            new tk.Button(tk.Val((o, c) => GetForwardButtonText(c)), (o, c) => MoveIndexByOffset(c, 1)) {
                                Style = new tk.ColorIf((o, c) => ShouldAddNextItem(c), Color.green)
                            },

                            2,

                            {
                                60,
                                new tk.Button(Label_SwapForward, (o, c) => SwapItemByOffset(c, 1)) {
                                    Style = new tk.EnabledIf((o, c) => CanSwapItemByOffset(c, 1))
                                }
                            },

                            2,

                            {
                                40,
                                new tk.Button(Label_Insert, (o, c) => InsertItem(c)) {
                                    Styles = {
                                           new tk.Color(Color.green),
                                           new tk.EnabledIf((o, c) => CanInsertItem(c))
                                     }
                                }
                            },

                            2,

                            {
                                25,
                                new tk.Button(Label_Delete, (o, c) => DeleteEditedItem(c)) {
                                    Styles = {
                                        new tk.Color(Color.red),
                                        new tk.EnabledIf((o, c) => CanDeleteEditedItem(c))
                                    }
                                }
                            }
                        }
                    }
                )),

                new tk.ShowIf((o,c) => HasEditableItem(c),
                    tk.PropertyEditor.Create(fiGUIContent.Empty, null,
                        (o, c) => (T)c.editedList[GetCurrentIndex(c)],
                        (o, c, v) => c.editedList[GetCurrentIndex(c)] = v))
            });

        private static fiGraphMetadata EnsureInitialState(tkDatabaseContext context, fiGraphMetadata metadata) {
            if (context.editedList == null) {
                context.editedList = (IList)InspectedType.Get(typeof(TDerived)).CreateInstance();
            }

            InspectorDatabaseEditorMetadata databaseMetadata = metadata.GetMetadata<InspectorDatabaseEditorMetadata>();

            // Set the global metadata to the graph metadata, as the graph
            // metadata is persistent but users still may want to access the
            // global metadata.
            fiGlobalMetadata.Set(context.editedList, databaseMetadata);

            // Disable the dropdown
            metadata.GetPersistentMetadata<fiDropdownMetadata>().ForceDisable();

            TryEnsureValidIndex(context);

            return metadata.Enter(databaseMetadata.CurrentIndex, context.editedList).Metadata;
        }

        protected override IList<T> Edit(Rect region, GUIContent label, IList<T> list, InspectorDatabaseEditorAttribute attribute, fiGraphMetadata metadata) {
            var context = new tkDatabaseContext {
                editedList = (IList)list,
                label = label
            };
            metadata = EnsureInitialState(context, metadata);

            ControlEditor.Context = context;
            fiEditorGUI.tkControl(region, label, context.editedList, metadata, ControlEditor);

            return (IList<T>)context.editedList;
        }

        protected override float GetElementHeight(GUIContent label, IList<T> list, InspectorDatabaseEditorAttribute attribute, fiGraphMetadata metadata) {
            var context = new tkDatabaseContext {
                editedList = (IList)list,
                label = label
            };
            metadata = EnsureInitialState(context, metadata);

            ControlEditor.Context = context;
            var height = fiEditorGUI.tkControlHeight(label, context.editedList, metadata, ControlEditor);

            return height;
        }

        /// <summary>
        /// The metadata we store on each item that we edit so that we know what
        /// the active editing item is.
        /// </summary>
        // TODO: make this persistent
        public class InspectorDatabaseEditorMetadata : IGraphMetadataItemNotPersistent {
            public int CurrentIndex;
        }
    }
}