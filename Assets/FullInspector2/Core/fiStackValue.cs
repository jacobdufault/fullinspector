using System.Collections.Generic;

namespace FullInspector.Internal {
    /// <summary>
    /// This is a simple wrapper around a stack. It's designed for storing a global value
    /// within recursive object graphs where the same object instance is used higher
    /// up in the graph (ie, the property editor system).
    /// </summary>
    public class fiStackValue<T> {
        private readonly Stack<T> _stack = new Stack<T>();

        public void Push(T value) {
            _stack.Push(value);
        }

        public T Pop() {
            if (_stack.Count > 0) {
                return _stack.Pop();
            }
            return default(T);
        }

        public T Value {
            get { return _stack.Peek(); }
            set { Pop(); Push(value); }
        }
    }
}