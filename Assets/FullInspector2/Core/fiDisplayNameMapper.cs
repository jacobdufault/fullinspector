using System.Collections.Generic;
using System.Text;

namespace FullInspector.Internal {
    /// <summary>
    /// Helpers for mapping a property name to a display name that should be shown in the inspector.
    /// </summary>
    /// <remarks>This is essentially a reimplementation for
    /// UnityEditor.ObjectNames.NicifyVariableName, but DisplayNameMapper allocates less
    /// frequently.</remarks>
    public static class fiDisplayNameMapper {
        /// <summary>
        /// A cache of mapped names, so we don't have to constantly reallocate string names.
        /// </summary>
        private static readonly Dictionary<string, string> _mappedNames = new Dictionary<string, string>();

        /// <summary>
        /// Convert the given property name into a name that will be used for the Unity inspector.
        /// For example, Unity by default converts "fieldValue" into "Field Value".
        /// </summary>
        public static string Map(string propertyName) {
            if (string.IsNullOrEmpty(propertyName)) {
                return "";
            }

            string mappedName;
            if (_mappedNames.TryGetValue(propertyName, out mappedName) == false) {
                mappedName = MapInternal(propertyName);
                _mappedNames[propertyName] = mappedName;
            }
            return mappedName;
        }

        /// <summary>
        /// Computes the actual mapped name. We try to not call this function as it allocates a
        /// fair amount.
        /// </summary>
        private static string MapInternal(string propertyName) {
            // remove leading m_ (only if the name is not actually m_, though)
            if (propertyName.StartsWith("m_") && propertyName != "m_") {
                propertyName = propertyName.Substring(2);
            }

            int start = 0;
            while (start < propertyName.Length && propertyName[start] == '_') {
                ++start;
            }

            // the string is just "___"; don't modify it
            if (start >= propertyName.Length) {
                return propertyName;
            }


            var result = new StringBuilder();

            bool forceCaptial = true;
            
            // insert spaces before capitals or _
            for (int i = start; i < propertyName.Length; ++i) {
                char c = propertyName[i];

                if (c == '_') {
                    forceCaptial = true;
                    continue;
                }

                if (forceCaptial) {
                    forceCaptial = false;
                    c = char.ToUpper(c);
                }

                if (i != start && ShouldInsertSpace(i, propertyName)) {
                    result.Append(' ');
                }
                result.Append(c);
            }

            return result.ToString();
        }

        private static bool ShouldInsertSpace(int currentIndex, string str) {
            if (char.IsUpper(str[currentIndex])) {

                if ((currentIndex + 1) >= str.Length || char.IsUpper(str[currentIndex + 1])) {
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}