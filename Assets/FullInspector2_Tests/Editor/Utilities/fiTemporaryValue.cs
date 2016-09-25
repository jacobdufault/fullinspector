using System;

namespace FullInspector.Tests {
    public class fiTemporaryValue<T> : IDisposable {
        private T _previousValue;
        private Action<T> _setter;

        public fiTemporaryValue(T currentValue, T newValue, Action<T> setter) {
            _previousValue = currentValue;
            _setter = setter;

            _setter(newValue);
        }

        public void Dispose() {
            _setter(_previousValue);
        }
    }
}