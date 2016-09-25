using System;

namespace FullInspector.Internal {
    /// <summary>
    /// Static class that contains helpers for fiOption.
    /// </summary>
    public static class fiOption {
        /// <summary>
        /// Create an option containing the value. Generic arguments can be omitted.
        /// </summary>
        public static fiOption<T> Just<T>(T value) {
            return new fiOption<T>(value);
        }
    }

    /// <summary>
    /// A simple monad that can either contain or not contain a value.
    /// </summary>
    public struct fiOption<T> {
        /// <summary>
        /// If true, then the option contains a value.
        /// </summary>
        /// <remarks>
        /// We ensure that the zero state of fiOption ensures that it is empty.
        /// </remarks>
        private bool _hasValue;

        /// <summary>
        /// The stored value. Only contains data is _hasValue is true.
        /// </summary>
        private T _value;

        /// <summary>
        /// Create a new option instance containing the given value. An option constructed using
        /// this method will *never* be empty. If you want an empty option, make sure to use
        /// fiOption{T}.Empty.
        /// </summary>
        /// <param name="value">The value to store.</param>
        public fiOption(T value) {
            _hasValue = true;
            _value = value;
        }

        /// <summary>
        /// An empty option instance.
        /// </summary>
        public static fiOption<T> Empty = new fiOption<T>() {
            _hasValue = false,
            _value = default(T)
        };

        /// <summary>
        /// True if the option contains a value.
        /// </summary>
        public bool HasValue {
            get {
                return _hasValue;
            }
        }

        /// <summary>
        /// True if the option is empty.
        /// </summary>
        public bool IsEmpty {
            get {
                return _hasValue == false;
            }
        }

        /// <summary>
        /// Fetch the value stored in the option. This will throw an exception if the option is
        /// empty.
        /// </summary>
        public T Value {
            get {
                if (HasValue == false) {
                    throw new InvalidOperationException("There is no value inside the option");
                }

                return _value;
            }
        }
    }
}