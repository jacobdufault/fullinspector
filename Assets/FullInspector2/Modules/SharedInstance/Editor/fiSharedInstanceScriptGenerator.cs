using System;
using System.IO;
using System.Linq;
using FullInspector.Internal;
using FullSerializer;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Modules {
    /// <summary>
    /// Generates derived types for SharedInstance{T}.
    /// </summary>
    public static class fiSharedInstanceScriptGenerator {
        public static void GenerateScript(Type instanceType, Type serializerType) {
            // The name of the file, without the .cs at the end.
            string fileName = instanceType.CSharpName(/*includeNamespace:*/true, /*ensureSafeDeclarationName:*/true);

            // The name of the class, ie, SharedInstance_SystemInt32
            string className = instanceType.CSharpName(/*includeNamespace:*/ true, /*ensureSafeDeclarationName:*/ true);
            if (instanceType.Namespace != null && instanceType.Namespace != "System") {
                className = RemoveAll(instanceType.Namespace, '.') + className;
            }
            className = "SharedInstance_" + className;

            // The value we will use inside of the generic parameter, ie, System.Int32
            string genericType = instanceType.CSharpName();
            if (instanceType.Namespace != null && instanceType.Namespace != "System") {
                genericType = instanceType.Namespace + "." + genericType;
            }

            // The name of the serializer. If null, then no serializer will be emitted. This
            // is used inside the angle brackets.
            string serializerName = null;
            if (serializerType != null) {
                serializerName = serializerType.CSharpName(includeNamespace: true);
            }

            Emit(fileName, className, genericType, serializerName);
        }

        /// <param name="fileName">The name of the file to emit. This should be the normalized class name.</param>
        /// <param name="className">The name of the class in the file, ie, class {className} {}</param>
        /// <param name="genericType">The value for the generic type, ie, class foo : parent{genericType} {}</param>
        /// <param name="serializerName">The value for the serializer type, optional. It will also go in a generic type argument.</param>
        private static void Emit(string fileName, string className, string genericType, string serializerName) {
            // Get the file path we will generate. If there is already a file there, it is assumed that
            // we are the ones who generated it and so we don't need to do anything.
            String directory = fiUtility.CombinePaths(fiSettings.RootGeneratedDirectory, "SharedInstance");
            Directory.CreateDirectory(directory);
            String path = fiUtility.CombinePaths(directory, fileName + ".cs");
            if (File.Exists(path)) return;

            string script = "";
            script += "// This is an automatically generated script that is used to remove the generic " + Environment.NewLine;
            script += "// parameter from SharedInstance<T, TSerializer> so that Unity can properly serialize it." + Environment.NewLine;
            script += Environment.NewLine;
            script += "using System;" + Environment.NewLine;
            script += Environment.NewLine;
            script += "namespace FullInspector.Generated.SharedInstance {" + Environment.NewLine;
            if (serializerName != null) {
                script += "    public class " + className + " : SharedInstance<" + genericType + ", " + serializerName + "> {}" + Environment.NewLine;
            }
            else {
                script += "    public class " + className + " : SharedInstance<" + genericType + "> {}" + Environment.NewLine;
            }
            script += "}" + Environment.NewLine;

            Debug.Log("Writing derived SharedInstance<" + genericType + ", " + serializerName + "> type (" + className + ") to " + path + "; click to see script below." +
                Environment.NewLine + Environment.NewLine + script);
            File.WriteAllText(path, script);
            AssetDatabase.Refresh();
        }

        private static string RemoveAll(string str, char c) {
            return str.Split(c).Aggregate((a, b) => a + b);
        }
    }
}