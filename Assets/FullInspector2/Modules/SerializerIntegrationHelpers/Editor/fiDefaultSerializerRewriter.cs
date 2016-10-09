using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Internal {
    /// <summary>
    /// Helper to write the C# file containing the selected serializer information.
    /// </summary>
    public static class fiDefaultSerializerRewriter {
        private static string OutputFilePath = fiUtility.CombinePaths(fiSettings.RootGeneratedDirectory, fiInstalledSerializerManager.GeneratedTypeName + ".cs");
        private static string OutputBehaviorPath = fiUtility.CombinePaths(fiSettings.RootGeneratedDirectory, "BaseBehavior.cs");
        private static string OutputScriptableObjectPath = fiUtility.CombinePaths(fiSettings.RootGeneratedDirectory, "BaseScriptableObject.cs");
        private static string OutputSharedInstancePath = fiUtility.CombinePaths(fiSettings.RootGeneratedDirectory, "SharedInstance.cs");

        private static void Append(StringBuilder builder) {
            builder.Append(Environment.NewLine);
        }
        private static void Append(StringBuilder builder, string line) {
            builder.Append(line);
            builder.Append(Environment.NewLine);
        }

        private static string GetTypeName(Type type) {
            if (type.Namespace == null) {
                return type.Name;
            }

            return type.Namespace + "." + type.Name;
        }

        private static List<string> GetLoadedProviders() {
            return (from type in fiRuntimeReflectionUtility.AllSimpleTypesDerivingFrom(typeof(fiISerializerMetadata))
                    select GetTypeName(type)).ToList();
        }

        public static void Generate() {
            if (fiInstalledSerializerManager.HasDefault == false) {
                Debug.LogWarning("Cannot regenerate serialization metadata without a registered default serializer");
            }

            List<string> allProviders = GetLoadedProviders();

            // fetch a default provider that is NOT the providerType
            string defaultProvider = null;
            string defaultSerializer = null;

            if (fiInstalledSerializerManager.HasDefault) {
                defaultProvider = GetTypeName(fiInstalledSerializerManager.DefaultMetadata.GetType());
                defaultSerializer = GetTypeName(fiInstalledSerializerManager.DefaultMetadata.SerializerType);
            }
            else {
                defaultProvider = null;
                defaultSerializer = null;

                foreach (var metadata in fiInstalledSerializerManager.LoadedMetadata) {
                    defaultProvider = GetTypeName(metadata.GetType());
                    defaultSerializer = GetTypeName(metadata.SerializerType);
                    break;
                }
            }

            // rebuild the files
            GenerateDefaultBehaviors(defaultSerializer);
            GenerateFileInternal(defaultProvider, allProviders);
            AssetDatabase.Refresh();
        }

        // import
        public static void GenerateFileIncluding(string providerType, string serializerType) {
            List<string> allProviders = GetLoadedProviders();
            if (allProviders.Contains(providerType) == false) {
                allProviders.Add(providerType);
            }

            string defaultProvider = providerType;
            string defaultSerializer = serializerType;

            if (fiInstalledSerializerManager.HasDefault) {
                defaultProvider = GetTypeName(fiInstalledSerializerManager.DefaultMetadata.GetType());
                defaultSerializer = GetTypeName(fiInstalledSerializerManager.DefaultMetadata.SerializerType);
            }

            GenerateDefaultBehaviors(defaultSerializer);
            GenerateFileInternal(defaultProvider, allProviders);
            AssetDatabase.Refresh();
        }

        // delete
        public static void GenerateFileExcluding(string providerType, string serializerType) {
            List<string> allProviders = GetLoadedProviders();
            if (allProviders.Contains(providerType)) {
                allProviders.Remove(providerType);
            }

            // fetch a default provider that is NOT the providerType
            string defaultProvider = null;
            string defaultSerializer = null;

            if (fiInstalledSerializerManager.HasDefault) {
                defaultProvider = GetTypeName(fiInstalledSerializerManager.DefaultMetadata.GetType());
                defaultSerializer = GetTypeName(fiInstalledSerializerManager.DefaultMetadata.SerializerType);
            }

            if (defaultProvider == providerType) {
                defaultProvider = null;
                defaultSerializer = null;

                foreach (var metadata in fiInstalledSerializerManager.LoadedMetadata) {
                    string metadataType = GetTypeName(metadata.GetType());
                    if (metadataType != providerType) {
                        defaultProvider = metadataType;
                        defaultSerializer = GetTypeName(metadata.SerializerType);
                        break;
                    }
                }
            }

            // no default provider could be found -- just delete the file info
            if (string.IsNullOrEmpty(defaultProvider)) {
                try {
                    File.Delete(OutputFilePath);
                    File.Delete(OutputFilePath + ".meta");
                }
                catch { }
                try {
                    File.Delete(OutputBehaviorPath);
                    File.Delete(OutputBehaviorPath + ".meta");
                }
                catch { }
                try {
                    File.Delete(OutputScriptableObjectPath);
                    File.Delete(OutputScriptableObjectPath + ".meta");
                }
                catch { }

                AssetDatabase.Refresh();
                return;
            }

            GenerateDefaultBehaviors(defaultSerializer);
            GenerateFileInternal(defaultProvider, allProviders);
            AssetDatabase.Refresh();
        }

        // change default
        public static void GenerateFileChangeDefault(string providerType, string serializerType) {
            List<string> allProviders = GetLoadedProviders();
            if (allProviders.Contains(providerType) == false) {
                allProviders.Add(providerType);
            }

            string defaultProvider = providerType;
            string defaultSerializier = serializerType;

            GenerateDefaultBehaviors(defaultSerializier);
            GenerateFileInternal(defaultProvider, allProviders);
            AssetDatabase.Refresh();
        }

        private static void GenerateDefaultBehaviors(string serializerType) {
            var builder = new StringBuilder();
            Append(builder, "// WARNING: This file has been automatically generated by Full Inspector, as part of the serializer");
            Append(builder, "//          selection wizard. It will be overwritten if you change your selected serializers.");
            Append(builder);
            Append(builder, "namespace FullInspector {");
            Append(builder, "    public abstract class BaseBehavior : BaseBehavior<" + serializerType + "> {}");
            Append(builder, "}");
            File.WriteAllText(OutputBehaviorPath, builder.ToString());

            builder = new StringBuilder();
            Append(builder, "// WARNING: This file has been automatically generated by Full Inspector, as part of the serializer");
            Append(builder, "//          selection wizard. It will be overwritten if you change your selected serializers.");
            Append(builder);
            Append(builder, "namespace FullInspector {");
            Append(builder, "    public abstract class BaseScriptableObject : BaseScriptableObject<" + serializerType + "> {}");
            Append(builder, "}");
            File.WriteAllText(OutputScriptableObjectPath, builder.ToString());

            builder = new StringBuilder();
            Append(builder, "// WARNING: This file has been automatically generated by Full Inspector, as part of the serializer");
            Append(builder, "//          selection wizard. It will be overwritten if you change your selected serializers.");
            Append(builder);
            Append(builder, "namespace FullInspector {");
            Append(builder, "    public abstract class SharedInstance<T> : SharedInstance<T, " + serializerType + "> {}");
            Append(builder, "}");
            File.WriteAllText(OutputSharedInstancePath, builder.ToString());
        }


        private static void GenerateFileInternal(string defaultProvider, List<string> allProviders) {
            string param0 = defaultProvider;
            string param1 = string.Join("," + Environment.NewLine, allProviders.Select(n => string.Format("                    typeof({0})", n)).ToArray());

            var builder = new StringBuilder();

            Append(builder, "// WARNING: This file has been automatically generated by Full Inspector, as part of the serializer");
            Append(builder, "//          selection wizard. It will be overwritten if you change your selected serializers.");
            Append(builder);
            Append(builder, "using System;");
            Append(builder);
            Append(builder, "namespace FullInspector.Internal {");
            Append(builder, "    public class " + fiInstalledSerializerManager.GeneratedTypeName + " : fiILoadedSerializers {");
            Append(builder, "        public Type DefaultSerializerProvider {");
            Append(builder, "           get { return typeof(" + param0 + "); }");
            Append(builder, "        }");
            Append(builder);
            Append(builder, "        public Type[] AllLoadedSerializerProviders {");
            Append(builder, "           get {");
            Append(builder, "               return new Type[] {");
            Append(builder, param1);
            Append(builder, "               };");
            Append(builder, "           }");
            Append(builder, "        }");
            Append(builder, "    }");
            Append(builder, "}");

            File.WriteAllText(OutputFilePath, builder.ToString());
        }
    }
}
