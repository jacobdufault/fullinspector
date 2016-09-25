namespace FullInspector {
    public class tkControlEditor {
        /// <summary>
        /// Determines if the debugger will be displayed beneath the control. The debugger
        /// will allow you to easily tweak values within the control so that you can get
        /// the perfect GUI.
        /// </summary>
        public bool Debug;

        /// <summary>
        /// The control that will render the GUI.
        /// </summary>
        public tkIControl Control;

        /// <summary>
        /// The context object.
        /// </summary>
        public object Context;

        public tkControlEditor(tkIControl control)
            : this(false, control) {
        }

        public tkControlEditor(bool debug, tkIControl control) {
            Debug = debug;
            Control = control;

            int nextId = 0;
            control.InitializeId(ref nextId);
        }
    }
}