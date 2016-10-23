using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace FullInspector.Internal {
    public struct fiSerializationPackage {
        public string Title;
        public string Description;

        public string ProviderType;
        public string SerializerType;

        public string PackagePath;
        public Guid SerializerGuid;
        public string PackageDirectory;

        public bool CanRemove;
    }

    [InitializeOnLoad]
    public class fiSerializerManagerEditorWindow : EditorWindow {
        private List<fiSerializationPackage> Packages = new List<fiSerializationPackage>() {
            new fiSerializationPackage {
                Title = "Full Serializer (recommended)",
                Description = "Full Serializer is an free and open-source JSON serializer that works on every Unity export platform. It supports all .NET types and you don't need to learn any special serialization annotations -- it uses [SerializeField], like Unity. Full Serializer is required internally by Full Inspector for graph metadata serialization and cannot be removed.",
                ProviderType = "FullInspector.Serializers.FullSerializer.FullSerializerMetadata",
                SerializerType = "FullInspector.FullSerializerSerializer",
                PackagePath = "",
                PackageDirectory = "",
                SerializerGuid = new Guid("bc898177-6ff4-423f-91bb-589bc83d8fde"),
                CanRemove = false
            },

            new fiSerializationPackage {
                Title = "Json.NET",
                Description = "Json.NET is a robust and extremely popular serialization library. FI ships with a version that works on non-AOT platforms. To get AOT support, you need to purchase Json.NET for Unity from the Asset Store.",
                ProviderType = "FullInspector.Serializers.JsonNet.JsonNetMetadata",
                SerializerType = "FullInspector.JsonNetSerializer",
                PackagePath = fiUtility.CombinePaths(fiSettings.RootDirectory, "Serializers/JsonNetPackage.unitypackage"),
                PackageDirectory = fiUtility.CombinePaths(fiSettings.RootDirectory, "Serializers/JsonNet"),
                SerializerGuid = new Guid("330ef139-4bcf-4d72-99e3-ef9ed34b6baf"),
                CanRemove = true
            },

            new fiSerializationPackage {
                Title = "protobuf-net",
                Description = "protobuf-net is, by far, the fastest serializer that FI ships with. However, it's the most difficult to use and has a couple of serialization gotchas. Full Inspector includes a (beta) decompiler that makes it relatively easy to use protobuf-net on AOT platforms.",
                ProviderType = "FullInspector.Serializers.ProtoBufNet.ProtoBufNetMetadata",
                SerializerType = "FullInspector.ProtoBufNetSerializer",
                PackagePath = fiUtility.CombinePaths(fiSettings.RootDirectory, "Serializers/ProtoBufNetPackage.unitypackage"),
                PackageDirectory = fiUtility.CombinePaths(fiSettings.RootDirectory, "Serializers/protobuf-net"),
                SerializerGuid = new Guid("c74b6a96-c4b7-42e8-bfca-2fb8df8d263f"),
                CanRemove = true
            },

            new fiSerializationPackage {
                Title = "BinaryFormatter",
                Description = "You can use BinaryFormatter, but it's slow, hard to view the serialized data, and not terribly robust. Works on most platforms. BinaryFormatter does not support all collection types. If you're interested in using BinaryFormatter, it's strongly recommended to use Full Serializer instead.",
                ProviderType = "FullInspector.Serializers.Formatter.BinaryFormatterMetadata",
                SerializerType = "FullInspector.BinaryFormatterSerializer",
                PackagePath = fiUtility.CombinePaths(fiSettings.RootDirectory, "Serializers/BinaryFormatterPackage.unitypackage"),
                PackageDirectory = fiUtility.CombinePaths(fiSettings.RootDirectory, "Serializers/Formatter"),
                SerializerGuid = new Guid("f3b35090-2cf3-4f89-845d-3743be7571fa"),
                CanRemove = true
            }
        };

        static fiSerializerManagerEditorWindow() {
            // TODO: This will show up too often.
            //if (fiInstalledSerializerManager.HasDefault == false) {
            //    EditorApplication.delayCall += ShowWindow;
            //}
        }

        [MenuItem("Window/Full Inspector/Developer/Show Serializer Manager")]
        public static void ShowWindow() {
            fiEditorWindowUtility.Show<fiSerializerManagerEditorWindow>("Serializer Manager", 880, 390);
        }

        private static GUIStyle TitleStyle;

        private static void SetupStyles() {
            if (TitleStyle == null) {
                TitleStyle = new GUIStyle(EditorStyles.boldLabel);
                TitleStyle.fontSize = 30;
            }
        }

        private static string GetTypeName(Type type) {
            if (type.Namespace == null) {
                return type.Name;
            }

            return type.Namespace + "." + type.Name;
        }

        [DidReloadScripts]
        private static void RepaintOnScriptReload() {
            var window = EditorWindow.FindObjectOfType<fiSerializerManagerEditorWindow>();
            if (window != null) {
                window.Repaint();
            }

            EditorUtility.ClearProgressBar();
        }

        public void Update() {
            // [DidReloadScripts] isn't very reliable, so we are just going to
            // have to repaint constantly :(
            Repaint();
        }

        private static void ShowCompileProgressBar(string header, float amount) {
            amount = Math.Min(amount, 1);
            EditorUtility.DisplayProgressBar(header, "Please wait for recompilation to finish.", amount);

            // A script reload might not occur after a recompile if there is a
            // compile error. In that case, we should just give the script reload
            // notification ourselves, so that we do a repaint and so that we
            // clear the progress bar.
            if (EditorApplication.isCompiling == false) {
                RepaintOnScriptReload();
            }
            else {
                EditorApplication.delayCall += () => ShowCompileProgressBar(header, amount + .025f);
            }
        }

        private static void DisplayOption(fiSerializationPackage package) {
            EditorGUILayout.LabelField(package.Title, EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();

            bool isDefault = false;
            if (fiInstalledSerializerManager.HasDefault) {
                isDefault = package.SerializerGuid == fiInstalledSerializerManager.DefaultMetadata.SerializerGuid;
            }

            // show the remove/import button, depending on if the serializer is
            // loaded
            if (fiInstalledSerializerManager.IsLoaded(package.SerializerGuid)) {
                EditorGUI.BeginDisabledGroup(package.CanRemove == false);

                GUI.color = Color.red;

                if (package.CanRemove == false) {
                    // tooltips don't work on buttons, so just gray the remove
                    // button out
                    GUI.color = Color.gray;
                }

                if (GUILayout.Button("Remove", GUILayout.ExpandHeight(true), GUILayout.Width(65))) {
                    try {
                        ShowCompileProgressBar("Removing Serializer", 0);
                        Directory.Delete(package.PackageDirectory, true);
                    }
                    catch (Exception e) {
                        Debug.LogWarning("When deleting " + package.PackageDirectory + ", caught exception\n" + e);
                    }

                    // The user might not be using .meta files, so we will
                    // silently ignore the meta delete request
                    try {
                        File.Delete(package.PackageDirectory + ".meta");
                    }
                    catch { }

                    fiDefaultSerializerRewriter.GenerateFileExcluding(package.ProviderType, package.SerializerType);
                }
                GUI.color = Color.white;

                EditorGUI.EndDisabledGroup();
            }
            else {
                if (GUILayout.Button("Import", GUILayout.ExpandHeight(true), GUILayout.Width(65))) {
                    AssetDatabase.ImportPackage(package.PackagePath, /*interactive:*/ false);
                    fiDefaultSerializerRewriter.GenerateFileIncluding(package.ProviderType, package.SerializerType);
                }
            }

            // show the set default button if active
            if (isDefault) {
                EditorGUI.BeginDisabledGroup(true);
                GUI.color = Color.green;
                GUILayout.Button("Default \u2713", GUILayout.ExpandHeight(true), GUILayout.Width(90));
                GUI.color = Color.white;
                EditorGUI.EndDisabledGroup();
            }
            else {
                EditorGUI.BeginDisabledGroup(!fiInstalledSerializerManager.IsLoaded(package.SerializerGuid));
                if (GUILayout.Button("Set Default", GUILayout.ExpandHeight(true), GUILayout.Width(90))) {
                    ShowCompileProgressBar("Changing Default Serializer", 0);
                    fiDefaultSerializerRewriter.GenerateFileChangeDefault(package.ProviderType, package.SerializerType);
                }
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.HelpBox(package.Description, MessageType.Info);
            GUILayout.EndHorizontal();
        }

        public void OnGUI() {
            SetupStyles();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Full Inspector Serializer Manager", TitleStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (fiInstalledSerializerManager.HasDefault == false) {
                EditorGUILayout.HelpBox("Full Inspector has detected that there isn't a default " +
                    "serializer. Please select one.", MessageType.Error);
            }
            else {
                EditorGUILayout.HelpBox("You can manage the installed serializers using this window",
                    MessageType.Info);
            }

            foreach (var package in Packages) {
                EditorGUILayout.Space();
                DisplayOption(package);
            }

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Refresh Imported Serializer List")) {
                fiDefaultSerializerRewriter.Generate();
            }
            GUILayout.EndHorizontal();
        }
    }
}