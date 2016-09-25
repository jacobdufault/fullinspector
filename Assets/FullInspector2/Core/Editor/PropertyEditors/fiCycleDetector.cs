using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FullInspector.Internal {
    /// <summary>
    /// This class assists with cycle detection in an object graph.
    /// </summary>
    public class fiCycleDetector {
        // From http://stackoverflow.com/questions/4901320/is-there-any-kind-of-referencecomparer-in-net
        class ReferenceComparer : IEqualityComparer<object> {
            private static ReferenceComparer _instance;
            public static ReferenceComparer Instance {
                get {
                    return _instance ?? (_instance = new ReferenceComparer());
                }
            }

            bool IEqualityComparer<object>.Equals(object x, object y) {
                return ReferenceEquals(x, y);
            }
            int IEqualityComparer<object>.GetHashCode(object obj) {
                return RuntimeHelpers.GetHashCode(obj);
            }
        }

        /// <summary>
        /// Factory we use for allocation HashSets (to minimize GC pressure).
        /// </summary>
        private static fiFactory<HashSet<object>> Factory =
            new fiFactory<HashSet<object>>(set => set.Clear(), ReferenceComparer.Instance);

        /// <summary>
        /// Cycle detectors that we are depending on / derived from.
        /// </summary>
        private fiCycleDetector[] _parents;

        /// <summary>
        /// The objects we have encountered so far.
        /// </summary>
        private fiOption<HashSet<object>> _objects;

        /// <summary>
        /// The recursion depth level that we're at.
        /// </summary>
        private int _depth;

        public fiCycleDetector(params fiCycleDetector[] parents) {
            _parents = parents;
            _objects = fiOption<HashSet<object>>.Empty;
        }

        /// <summary>
        /// The current recursion / nesting depth.
        /// </summary>
        public int Depth {
            get {
                int depth = _depth;

                for (int i = 0; i < _parents.Length; ++i) {
                    if (_parents[i] != null) {
                        depth += _parents[i].Depth;
                    }
                }

                return depth;
            }
        }

        /// <summary>
        /// Increase the recursion / nesting depth.
        /// </summary>
        public void Enter() {
            ++_depth;
        }

        /// <summary>
        /// Decrease the recursion / nesting depth.
        /// </summary>
        public void Exit() {
            if (_depth == 0) {
                throw new InvalidOperationException("Mismatched Enter/Exit");
            }

            --_depth;
            if (_depth == 0 && _objects.HasValue) {
                Factory.ReuseInstance(_objects.Value);
                _objects = fiOption<HashSet<object>>.Empty;
            }
        }

        /// <summary>
        /// Tries to mark an object. Returns false if the object is already in the cycle detector,
        /// ie, it has already been encountered (which implies that it's part of a cycle).
        /// </summary>
        public bool TryMark(object obj) {
            if (obj == null) return true;
            if (obj.GetType().IsPrimitive) return true;
            if (obj.GetType().IsValueType) return true;

            if (_objects.IsEmpty) {
                _objects = fiOption.Just(Factory.GetInstance());
            }

            if (IsCycle(obj)) {
                return false;
            }

            _objects.Value.Add(obj);
            return true;
        }

        /// <summary>
        /// Returns true if the object is in the cycle detector, ie, the object is part of a cycle.
        /// </summary>
        public bool IsCycle(object obj) {
            if (_objects.IsEmpty) {
                return false;
            }

            if (_objects.Value.Contains(obj)) {
                return true;
            }

            for (int i = 0; i < _parents.Length; ++i) {
                if (_parents[i] != null) {
                    if (_parents[i].IsCycle(obj)) {
                        return true;
                    }
                }
            }

            return false;
        }
    }

}