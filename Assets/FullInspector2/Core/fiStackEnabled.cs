namespace FullInspector.Internal {
    /// <summary>
    /// Utility class that is enabled when it has been pushed to.
    /// </summary>
    public class fiStackEnabled {
        private int _count;
        public void Push() {
            ++_count;
        }
        public void Pop() {
            --_count;
            if (_count < 0) _count = 0;
        }
        public bool Enabled {
            get {
                return _count > 0;
            }
        }
    }
}