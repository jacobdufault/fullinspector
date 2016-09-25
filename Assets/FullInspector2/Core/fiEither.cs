using System;

namespace FullInspector.Internal {
    /// <summary>
    /// A object that can contain one value between two different types.
    /// </summary>
    /// <typeparam name="TA">A candidate type for the value stored.</typeparam>
    /// <typeparam name="TB">A candidate type for the value stored.</typeparam>
    public struct fiEither<TA, TB> {
        private TA _valueA;
        private TB _valueB;
        private bool _hasA;

        /// <summary>
        /// Returns the A value. Throws an exception if IsA returns false.
        /// </summary>
        public TA ValueA {
            get {
                if (IsA == false) {
                    throw new InvalidOperationException("Either does not contain value A");
                }

                return _valueA;
            }
        }

        /// <summary>
        /// Returns the B value. Throws an exception if IsB returns false.
        /// </summary>
        public TB ValueB {
            get {
                if (IsB == false) {
                    throw new InvalidOperationException("Either does not contain value B");
                }

                return _valueB;
            }
        }

        /// <summary>
        /// Does this either contain an A value?
        /// </summary>
        public bool IsA { get { return _hasA; } }

        /// <summary>
        /// Does this either contain a B value?
        /// </summary>
        public bool IsB { get { return !_hasA; } }

        /// <summary>
        /// Construct an either containing an A value.
        /// </summary>
        public fiEither(TA valueA) {
            _hasA = true;
            _valueA = valueA;
            _valueB = default(TB);
        }

        /// <summary>
        /// Construct an either containing a B value.
        /// </summary>
        public fiEither(TB valueB) {
            _hasA = false;
            _valueA = default(TA);
            _valueB = valueB;
        }
    }
}