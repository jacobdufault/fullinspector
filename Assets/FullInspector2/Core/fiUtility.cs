using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    public static class fiUtility {
        public static string CombinePaths(string a, string b) {
            try {
                return Path.Combine(a, b).Replace('\\', '/');
            }
            catch (Exception) {
                Debug.Log("Caught exception combining " + a + " and " + b);
                throw;
            }
        }
        public static string CombinePaths(string a, string b, string c) {
            return Path.Combine(Path.Combine(a, b), c).Replace('\\', '/');
        }
        public static string CombinePaths(string a, string b, string c, string d) {
            return Path.Combine(Path.Combine(Path.Combine(a, b), c), d).Replace('\\', '/');
        }

        /// <summary>
        /// Compares two floating point values and determines if they are equal.
        /// </summary>
        public static bool NearlyEqual(float a, float b) {
            return NearlyEqual(a, b, float.Epsilon);
        }

        /// <summary>
        /// Compares to floating point values and determines if they are equal.
        /// </summary>
        public static bool NearlyEqual(float a, float b, float epsilon) {
            var absA = Math.Abs(a);
            var absB = Math.Abs(b);
            var diff = Math.Abs(a - b);

            if (a == b) { // shortcut, handles infinities
                return true;
            }
            else if (a == 0 || b == 0 || diff < float.MinValue) {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < (epsilon * Double.MinValue);
            }
            else { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }

        /// <summary>
        /// Destroys the given object using the proper destroy function. If the game is in edit
        /// mode, then DestroyImmedate is used. Otherwise, Destroy is used.
        /// </summary>
        public static void DestroyObject(UnityObject obj) {
            if (Application.isPlaying) {
                UnityObject.Destroy(obj);
            }
            else {
                UnityObject.DestroyImmediate(obj, true);
            }
        }

        /// <summary>
        /// Destroys the object and sets it to null.
        /// </summary>
        public static void DestroyObject<T>(ref T obj)
            where T : UnityObject {

            DestroyObject(obj);
            obj = null;
        }

        /// <summary>
        /// Removes leading whitespace after newlines from a string. This is extremely useful when
        /// using the C# multiline @ string.
        /// </summary>
        public static string StripLeadingWhitespace(this string s) {
            // source: http://stackoverflow.com/a/7178336
            Regex r = new Regex(@"^\s+", RegexOptions.Multiline);
            return r.Replace(s, string.Empty);
        }

        /// <summary>
        /// This is equivalent to Application.isEditor except that it can be
        /// called off of the main thread.
        /// </summary>
        public static bool IsEditor {
            get {
                if (_cachedIsEditor.HasValue == false) {
                    _cachedIsEditor = Type.GetType("UnityEditor.Editor, UnityEditor", /*throwOnError:*/false) != null;
                }

                return _cachedIsEditor.Value;
            }
        }
        private static bool? _cachedIsEditor;

        /// <summary>
        /// Returns true if the current thread is the main Unity thread - ie, Unity API calls will not throw exceptions.
        /// </summary>
        public static bool IsMainThread {
            get {
                if (IsEditor == false) throw new InvalidOperationException("Only available in the editor");

#if UNITY_METRO
                throw new System.NotImplementedException();
#else
                return Thread.CurrentThread.ManagedThreadId == 1;
#endif
            }
        }

        /// <summary>
        /// Returns true if this is a Unity 4 environment.
        /// </summary>
        public static bool IsUnity4 {
            get {
                if (_isUnity4.HasValue == false) {
                    // UnityEngine.RuntimeInitializeOnLoadMethod was introduced in Unity 5. If it isn't available, then we are
                    // in a version below Unity 5. We don't support Unity 3, so we will just assume Unity 4.
                    _isUnity4 = Type.GetType("UnityEngine.RuntimeInitializeOnLoadMethodAttribute, UnityEngine", /*throwOnError:*/false) == null;
                }

                return _isUnity4.Value;
            }
        }
        private static bool? _isUnity4;

        /// <summary>
        /// Creates a dictionary from the given keys and given values.
        /// </summary>
        /// <typeparam name="TKey">The key type of the dictionary.</typeparam>
        /// <typeparam name="TValue">The value type of the dictionary.</typeparam>
        /// <param name="keys">The keys in the dictionary. A null key will be skipped.</param>
        /// <param name="values">The values in the dictionary.</param>
        /// <returns>A dictionary that contains the given key to value mappings.</returns>
        public static Dictionary<TKey, TValue> CreateDictionary<TKey, TValue>(IList<TKey> keys, IList<TValue> values) {
            var dict = new Dictionary<TKey, TValue>();

            if (keys != null && values != null) {
                for (int i = 0; i < Mathf.Min(keys.Count, values.Count); ++i) {
                    if (ReferenceEquals(keys[i], null)) continue;

                    dict[keys[i]] = values[i];
                }
            }

            return dict;
        }

        /// <summary>
        /// Swaps two items.
        /// </summary>
        public static void Swap<T>(ref T a, ref T b) {
            T tmp = a;
            a = b;
            b = tmp;
        }
    }
}