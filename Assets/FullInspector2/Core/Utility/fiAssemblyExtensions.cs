using System;
using System.Reflection;
using System.Collections.Generic;

namespace FullInspector.Internal {
    public static class fiAssemblyExtensions {
        private static readonly Type[] s_EmptyArray = { };
        private static readonly Dictionary<Assembly, Type[]> s_assemblyTotypeCache = new Dictionary<Assembly, Type[]>();

        public static Type[] GetTypesWithoutException(this Assembly assembly) {
            try {
                Type[] types;
                if (!s_assemblyTotypeCache.TryGetValue(assembly, out types)) {
                    s_assemblyTotypeCache[assembly] = types = assembly.GetTypes();
                }

                return types;
            }
            catch {
                return s_EmptyArray;
            }
        }
    }
}