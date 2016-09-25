using System;
using System.Collections.Generic;

namespace FullInspector {
    /// <summary>
    /// In Full Inspector, there are typically a large number of property editors that can be used
    /// for each type, for example, a user defined editor, then the abstract editor, then the
    /// reflected editor. PropertyEditorChain encapsulates this idea and makes it easy to retrieve
    /// the next editor that will be used.
    /// </summary>
    public class PropertyEditorChain {
        /// <summary>
        /// The editing chain. The most applicable editor is at index 0 (followed by the next most
        /// applicable at index 1, ...).
        /// </summary>
        private List<IPropertyEditor> _editors = new List<IPropertyEditor>();

        /// <summary>
        /// Adds an editor to the end of this chain.
        /// </summary>
        internal void AddEditor(IPropertyEditor editor) {
            if (editor.EditorChain != null) {
                throw new InvalidOperationException("Editor " + editor + " is already part of " +
                    "another PropertyEditorChain");
            }

            _editors.Add(editor);
            editor.EditorChain = this;
        }

        /// <summary>
        /// Returns true if there is another editor after the given one.
        /// </summary>
        public bool HasNextEditor(IPropertyEditor editor) {
            return GetNextEditor(editor) != null;
        }

        /// <summary>
        /// Returns the next editor that will be used, or null if the given editor is either the
        /// last one or was not found in the chain.
        /// </summary>
        /// <param name="editor">The editor that is currently being used.</param>
        /// <returns>The next editor, or null if there is no next one.</returns>
        public IPropertyEditor GetNextEditor(IPropertyEditor editor) {
            for (int i = 0; i < _editors.Count; ++i) {
                if (_editors[i] == editor) {
                    if ((i + 1) >= _editors.Count) {
                        return null;
                    }

                    return _editors[i + 1];
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the first property editor in this chain that is not an instance of any of the
        /// given types.
        /// </summary>
        public IPropertyEditor SkipUntilNot(params Type[] skipTypes) {
            for (int i = 0; i < _editors.Count; ++i) {
                IPropertyEditor editor = _editors[i];

                bool skip = false;

                for (int j = 0; j < skipTypes.Length; ++j) {
                    Type skipType = skipTypes[j];
                    if (skipType.IsInstanceOfType(editor)) {
                        skip = true;
                        break;
                    }
                }

                if (skip == false) {
                    return editor;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the first property editor in this chain.
        /// </summary>
        public IPropertyEditor FirstEditor {
            get {
                if (_editors.Count == 0) {
                    throw new InvalidOperationException("The chain contains no editors");
                }

                return _editors[0];
            }
        }
    }
}