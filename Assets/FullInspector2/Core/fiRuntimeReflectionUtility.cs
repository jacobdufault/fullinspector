using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FullSerializer;
using FullSerializer.Internal;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Internal {
    /// <summary>
    /// Some reflection utilities that can be AOT compiled (and are therefore available at runtime).
    /// </summary>
    public class fiRuntimeReflectionUtility {
        /// <summary>
        /// Invokes the given static method on the given type.
        /// </summary>
        /// <param name="type">The type to search for the method.</param>
        /// <param name="methodName">The name of the method.</param>
        /// <param name="parameters">The parameters to invoke the method with.</param>
        public static object InvokeStaticMethod(Type type, string methodName, object[] parameters) {
            try {
                return type.GetFlattenedMethod(methodName).Invoke(null, parameters);
            }
            catch { }
            return null;
        }
        public static object InvokeStaticMethod(string typeName, string methodName, object[] parameters) {
            return InvokeStaticMethod(fsTypeCache.GetType(typeName), methodName, parameters);
        }

        /// <summary>
        /// Invokes the given method on the given type.
        /// </summary>
        /// <param name="type">The type to find the method to invoke from.</param>
        /// <param name="methodName">The name of the method to invoke.</param>
        /// <param name="thisInstance">The "this" object in the method.</param>
        /// <param name="parameters">The parameters to invoke with.</param>
        public static void InvokeMethod(Type type, string methodName, object thisInstance, object[] parameters) {
            try {
                type.GetFlattenedMethod(methodName).Invoke(thisInstance, parameters);
            }
            catch { }
        }

        /// <summary>
        /// Reads the given field from the value. Note that only field reads are supported.
        /// </summary>
        /// <typeparam name="T">The type of value stored within the field that we are reading.</typeparam>
        /// <typeparam name="TContext">The type of context we are reading the field from.</typeparam>
        /// <param name="context">The value where we are reading the field from.</param>
        /// <param name="fieldName">The name of the field we are reading.</param>
        /// <returns>The read value, or a thrown exception on error.</returns>
        public static T ReadField<TContext, T>(TContext context, string fieldName) {
            var members = typeof(TContext).GetFlattenedMember(fieldName);
            if (members == null || members.Length == 0)
                throw new ArgumentException(typeof(TContext).CSharpName() + " does not contain a field named \"" + fieldName + "\"");
            if (members.Length > 1)
                throw new ArgumentException(typeof(TContext).CSharpName() + " has more than one field named \"" + fieldName + "\"");

            var fieldInfo = members[0] as FieldInfo;
            if (fieldInfo == null)
                throw new ArgumentException(typeof(TContext).CSharpName() + "." + fieldName + " is not a field");

            if (fieldInfo.FieldType != typeof(T))
                throw new ArgumentException(typeof(TContext).CSharpName() + "." + fieldName + " type is not compatable with " + typeof(T).CSharpName());

            return (T)fieldInfo.GetValue(context);
        }

        public static T ReadFields<TContext, T>(TContext context, params string[] fieldNames) {
            for (int i = 0; i < fieldNames.Length; ++i) {
                string fieldName = fieldNames[i];
                var members = typeof(TContext).GetFlattenedMember(fieldName);
                if (members == null || members.Length == 0) continue;
                if (members.Length > 1) continue;

                var fieldInfo = members[0] as FieldInfo;
                if (fieldInfo == null) continue;
                if (fieldInfo.FieldType != typeof(T)) continue;

                return (T)fieldInfo.GetValue(context);
            }

            throw new ArgumentException("Unable to read any of the following fields " + string.Join(", ", fieldNames) + " on " + context);
        }

        /// <summary>
        /// Returns a list of object instances from types in the assembly that implement the given
        /// type. This only constructs objects which have default constructors.
        /// </summary>
        public static IEnumerable<TInterface> GetAssemblyInstances<TInterface>() {
            return from assembly in GetUserDefinedEditorAssemblies()
                   from type in assembly.GetTypesWithoutException()

                   where type.Resolve().IsGenericTypeDefinition == false
                   where type.Resolve().IsAbstract == false
                   where type.Resolve().IsInterface == false

                   where typeof(TInterface).IsAssignableFrom(type)
                   where type.GetDeclaredConstructor(fsPortableReflection.EmptyTypes) != null

                   select (TInterface)Activator.CreateInstance(type);
        }

        /// <summary>
        /// Returns all types that derive from UnityEngine.Object that are usable during runtime.
        /// </summary>
        public static IEnumerable<Type> GetUnityObjectTypes() {
            return from assembly in GetRuntimeAssemblies()

                   // GetExportedTypes() doesn't work for dynamic modules, so we jut use GetTypes()
                   // instead and manually filter for public
                   from type in assembly.GetTypesWithoutException()
                   where type.Resolve().IsVisible

                   // Cannot be an open generic type
                   where type.Resolve().IsGenericTypeDefinition == false

                   where typeof(UnityObject).IsAssignableFrom(type)

                   select type;
        }

        /// <summary>
        /// Returns the equivalent of assembly.GetName().Name, which does not work on WebPlayer.
        /// </summary>
        private static string GetName(Assembly assembly) {
            int index = assembly.FullName.IndexOf(",");
            if (index >= 0) {
                return assembly.FullName.Substring(0, index);
            }
            return assembly.FullName;
        }

        /// <summary>
        /// Return a guess of all assemblies that can be used at runtime.
        /// </summary>
        public static IEnumerable<Assembly> GetRuntimeAssemblies() {
#if !UNITY_EDITOR && UNITY_METRO && !ENABLE_IL2CPP
            yield return typeof(Assembly).GetTypeInfo().Assembly;
#else

            if (_cachedRuntimeAssemblies == null) {
                _cachedRuntimeAssemblies =
                    (from assembly in AppDomain.CurrentDomain.GetAssemblies()

                     // Unity exposes lots of assemblies that won't contain any behaviors that will
                     // contain a MonoBehaviour or UnityObject reference... so we ignore them to speed
                     // up reflection processing

                     where IsBannedAssembly(assembly) == false
                     where IsUnityEditorAssembly(assembly) == false

                     // In the editor, even Assembly-CSharp, etc contain a reference to UnityEditor,
                     // so we can't strip the assembly that way
                     where GetName(assembly).Contains("-Editor") == false

                     select assembly).ToList();

                fiLog.Blank();
                foreach (var assembly in _cachedRuntimeAssemblies) {
                    fiLog.Log(typeof(fiRuntimeReflectionUtility), "GetRuntimeAssemblies - " + GetName(assembly));
                }
                fiLog.Blank();
            }
            return _cachedRuntimeAssemblies;
#endif
        }
        private static List<Assembly> _cachedRuntimeAssemblies;

        /// <summary>
        /// Returns a guess of all user-defined assemblies that are available in the editor, but not
        /// necessarily in the runtime. This is a superset over GetRuntimeAssemblies().
        /// </summary>
        public static IEnumerable<Assembly> GetUserDefinedEditorAssemblies() {
#if !UNITY_EDITOR && UNITY_METRO && !ENABLE_IL2CPP
            yield return typeof(Assembly).GetTypeInfo().Assembly;
#else
            if (_cachedUserDefinedEditorAssemblies == null) {
                _cachedUserDefinedEditorAssemblies =
                    (from assembly in AppDomain.CurrentDomain.GetAssemblies()

                     where IsBannedAssembly(assembly) == false
                     where IsUnityEditorAssembly(assembly) == false

                     // bugfix: This does not necessarily work because Assembly-CSharp will is not guaranteed
                     //         to have a reference to UnityEditor if there are no conditional references to
                     //         UnityEditor
                     //where ContainsAssemblyReference(assembly, typeof(EditorWindow).Assembly)

                     select assembly).ToList();

                fiLog.Blank();
                foreach (var assembly in _cachedUserDefinedEditorAssemblies) {
                    fiLog.Log(typeof(fiRuntimeReflectionUtility), "GetUserDefinedEditorAssemblies - " + GetName(assembly));
                }
                fiLog.Blank();
            }

            return _cachedUserDefinedEditorAssemblies;
#endif
        }
        private static List<Assembly> _cachedUserDefinedEditorAssemblies;

        /// <summary>
        /// Gets all possible editor assemblies, including those defined by Unity. This is a superset over
        /// GetUserDefinedEditorAssemblies().
        /// </summary>
        public static IEnumerable<Assembly> GetAllEditorAssemblies() {
#if !UNITY_EDITOR && UNITY_METRO && !ENABLE_IL2CPP
            yield return typeof(Assembly).GetTypeInfo().Assembly;
#else

            if (_cachedAllEditorAssembles == null) {
                _cachedAllEditorAssembles =
                    (from assembly in AppDomain.CurrentDomain.GetAssemblies()

                     where IsBannedAssembly(assembly) == false

                     // bugfix: This does not necessarily work because Assembly-CSharp will is not guaranteed
                     //         to have a reference to UnityEditor if there are no conditional references to
                     //         UnityEditor
                     //where ContainsAssemblyReference(assembly, typeof(EditorWindow).Assembly)

                     select assembly).ToList();
                fiLog.Blank();
                foreach (var assembly in _cachedAllEditorAssembles) {
                    fiLog.Log(typeof(fiRuntimeReflectionUtility), "GetAllEditorAssemblies - " + GetName(assembly));
                }
                fiLog.Blank();
            }

            return _cachedAllEditorAssembles;
#endif
        }
        private static List<Assembly> _cachedAllEditorAssembles;


        private static bool IsUnityEditorAssembly(Assembly assembly) {
            var allowableScripts = new string[] {
                "UnityEditor",
                "UnityEditor.UI",
            };

            return allowableScripts.Contains(GetName(assembly));
        }

        /// <summary>
        /// Returns true if the given assembly is likely to contain user-scripts or it is a core
        /// runtime assembly (ie, UnityEngine).
        /// </summary>
        /// <param name="name">The unqualified name of the assembly.</param>
        private static bool IsBannedAssembly(Assembly assembly) {
            var bannedScripts = new string[] {
                "AssetStoreTools",
                "AssetStoreToolsExtra",

                "UnityScript",
                "UnityScript.Lang",

                "Boo.Lang.Parser",
                "Boo.Lang",
                "Boo.Lang.Compiler",

                "mscorlib",
                "System.ComponentModel.DataAnnotations",
                "System.Xml.Linq",

                "ICSharpCode.NRefactory",
                "Mono.Cecil",
                "Mono.Cecil.Mdb",

                "Unity.DataContract",
                "Unity.IvyParser",
                "Unity.Locator",
                "Unity.PackageManager",
                "Unity.SerializationLogic",

                //"UnityEngine",
                "UnityEngine.Analytics",
                "UnityEngine.Networking",
                "UnityEngine.UI",

                "UnityEditor.Advertisements",
                "UnityEditor.Android.Extensions",
                "UnityEditor.AppleTV.Extensions",
                "UnityEditor.BB10.Extensions",
                "UnityEditor.EditorTestsRunner",
                "UnityEditor.Graphs",
                "UnityEditor.iOS.Extensions",
                "UnityEditor.iOS.Extensions.Common",
                "UnityEditor.iOS.Extensions.Xcode",
                "UnityEditor.LinuxStandalone.Extensions",
                "UnityEditor.Metro.Extensions",
                "UnityEditor.Networking",
                "UnityEditor.OSXStandalone.Extensions",
                "UnityEditor.SamsungTV.Extensions",
                "UnityEditor.Tizen.Extensions",
                "UnityEditor.TreeEditor",
                "UnityEditor.WebGL.Extensions",
                "UnityEditor.WindowsStandalone.Extensions",
                "UnityEditor.WP8.Extensions",

                "protobuf-net",

                "Newtonsoft.Json",

                "System",
                "System.Configuration",
                "System.Xml",
                "System.Core",

                "Mono.Security",

                "I18N",
                "I18N.West",

                "nunit.core",
                "nunit.core.interfaces",
                "nunit.framework",
                "NSubstitute",

                "UnityVS.VersionSpecific",
                "SyntaxTree.VisualStudio.Unity.Bridge",
                "SyntaxTree.VisualStudio.Unity.Messaging",
            };
            return bannedScripts.Contains(GetName(assembly));
        }

        /// <summary>
        /// Returns all types in the current AppDomain that derive from the given baseType and are a
        /// class that is not an open generic type.
        /// </summary>
        public static IEnumerable<Type> AllSimpleTypesDerivingFrom(Type baseType) {
            return from assembly in GetRuntimeAssemblies()
                   from type in assembly.GetTypesWithoutException()
                   where baseType.IsAssignableFrom(type)
                   where type.Resolve().IsClass
                   where type.Resolve().IsGenericTypeDefinition == false
                   select type;
        }

        /// <summary>
        /// Returns all types in the current AppDomain that derive from the given baseType, are classes,
        /// are not generic, have a default constuctor, and are not abstract.
        /// </summary>
        public static IEnumerable<Type> AllSimpleCreatableTypesDerivingFrom(Type baseType) {
            return from type in AllSimpleTypesDerivingFrom(baseType)
                   where type.Resolve().IsAbstract == false
                   where type.Resolve().IsGenericType == false
                   where type.GetDeclaredConstructor(fsPortableReflection.EmptyTypes) != null
                   select type;
        }
    }
}