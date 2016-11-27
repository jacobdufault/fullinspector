using System;
using System.Collections.Generic;
using System.IO;
using FullInspector.Internal;
using UnityEngine;

namespace FullInspector {
    /// <summary>
    /// Extend this interface with any class if you wish to modify any of the
    /// settings. *Do not* make the modifications directly inside of this file,
    /// as that is brittle to DLL based deploys as well as Full Inspector
    /// upgrades.
    /// </summary>
    public interface fiSettingsProcessor {
        /// <summary>
        /// This is invoked before any code uses fiSettings. Use it to customize
        /// fiSettings as you see fit. This function may be invoked off of the
        /// main thread, which means that Unity API functions are
        /// *not* available.
        /// </summary>
        void Process();
    }

    /// <summary>
    /// This class contains some settings that can be used to customize the
    /// behavior of the Full Inspector.
    /// </summary>
    public class fiSettings {
        /// <summary>
        /// If true, multi-object editing will be enabled. Note that multi-object
        /// editing is still extremely experimental and has a large number of
        /// bugs.
        /// </summary>
        public static bool EnableMultiEdit = true;

        /// <summary>
        /// If true, InspectorOrder elements will be ordered globally instead of
        /// local per element.
        /// </summary>
        public static bool EnableGlobalOrdering = false;

        /// <summary>
        /// If true, then fiSettingsProcessor support will be enabled. The
        /// initial fiSettingsProcessor initialization may take some time so it
        /// is sometimes desirable turn this off by setting
        /// EnableSettingsProcessor to false.
        ///
        /// If you're using a DLL, the only way to customize settings (outside of
        /// compiling a DLL with your fiSettings config options baked in) is to
        /// use a fiSettingsProcessor instance.
        ///
        /// Note that this setting will *only* be honored if modified directly
        /// here, before the static constructor is run. It cannot be modified
        /// from a fiSettingsProcessor.
        /// </summary>
        public static bool EnableSettingsProcessor = true;

        /// <summary>
        /// Full Inspector will optionally log lots of data. If this is set to
        /// true, then Full Inspector will emit the logging information.
        /// </summary>
        public static bool EnableLogs = false;

        /// <summary>
        /// If set to true, then Full Serializer and Json.NET will serialize data
        /// using formatted JSON output. This is useful if you have multiple
        /// developers merging prefabs/assets, as it makes the data slightly more
        /// mergeable.
        /// </summary>
        public static bool PrettyPrintSerializedJson = false;

        /// <summary>
        /// The default type of comment that is used when you have
        /// [InspectorComment("str")]
        /// </summary>
        public static CommentType DefaultCommentType = CommentType.Info;

        /// <summary>
        /// Should the inline object editor be displayed even if the object does
        /// not necessarily use a FI editor? This is not directly supported but
        /// may be useful - custom inspectors that Unity themselves have written
        /// will almost certainly not render correctly.
        /// </summary>
        public static bool ForceDisplayInlineObjectEditor = false;

        /// <summary>
        /// Should Full Inspector use animation when toggling and foldout and in
        /// other situations? This option is purely cosmetic.
        /// </summary>
        public static bool EnableAnimation = true;

        /// <summary>
        /// A scene has just been saved. Should all IScriptableObjects be checked
        /// to see if they need to be saved? This is disabled by default because
        /// it causes a performance hit when saving and unless you have an
        /// extremely strange user scenario where you are not using the inspector
        /// to edit a BaseBehavior, everything will save correctly.
        /// </summary>
        public static bool ForceSaveAllAssetsOnSceneSave = false;

        /// <summary>
        /// A recompilation has been detected. Should all IScriptableObjects be
        /// checked to see if they need to be saved? This is disabled by default
        /// because it causes a performance hit when saving and unless you have
        /// an extremely strange user scenario where you are not using the
        /// inspector to edit a BaseBehavior, everything will save correctly.
        /// </summary>
        public static bool ForceSaveAllAssetsOnRecompilation = false;

        /// <summary>
        /// A recompilation has been detected. Should all IScriptableObjects be
        /// checked to see if they need to be restored? This is disabled by
        /// default because it causes a performance hit.
        /// </summary>
        public static bool ForceRestoreAllAssetsOnRecompilation = false;

        /// <summary>
        /// If this is set to true, then Full Inspector will attempt to
        /// automatically instantiate all reference fields/properties in an
        /// object. This will negatively impact the performance for creating
        /// objects (lots of reflection is used).
        /// </summary>
        public static bool AutomaticReferenceInstantation = false;

        /// <summary>
        /// If this is set to true, then when the reflected inspector encounters
        /// a property that is null it will attempt to create an instance of that
        /// property. This is most similar to how Unity operates. Please note
        /// that this will not instantiate fields/properties that are hidden from
        /// the inspector. Additionally, this will not instantiate fields which
        /// do not have a default constructor.
        /// </summary>
        public static bool InspectorAutomaticReferenceInstantation = true;

        /// <summary>
        /// Should public properties/fields automatically be shown in the
        /// inspector? If this is true, then only properties annotated with
        /// [ShowInInspector] will be shown. [HideInInspector] will never be
        /// necessary.
        ///
        /// *PLEASE NOTE* this does not impact how data is serialized! Public
        ///  properties/fields will *still* be serialized!
        /// </summary>
        public static bool InspectorRequireShowInInspector = false;

        /// <summary>
        /// Should auto-properties be serialized (and thus inspected) by default?
        /// If this is set to false, then you can still serialize auto-properties
        /// by using [SerializeField].
        /// </summary>
        public static bool SerializeAutoProperties = true;

        /// <summary>
        /// Should Full Inspector emit warnings when it detects a possible data
        /// loss (such as a renamed or removed variable) or general serialization
        /// issue?
        /// </summary>
        public static bool EmitWarnings = false;

        /// <summary>
        /// Should Full Inspector emit logs about graph metadata that it has
        /// culled? This may be useful if you have written a custom property
        /// editor but changes to your graph metadata are not being persisted for
        /// some reason.
        /// </summary>
        public static bool EmitGraphMetadataCulls = false;

        /// <summary>
        /// The minimum height a child property editor has to be before a foldout
        /// is displayed
        /// </summary>
        public static float MinimumFoldoutHeight = 80;

        /// <summary>
        /// Display an "open script" button that Unity will typically display.
        /// </summary>
        public static bool EnableOpenScriptButton = true;

        /// <summary>
        /// If set to true, then multithreaded serialization/deserialization will
        /// be forcibly disabled. It is **strong** recommended that you leave
        /// this as false. If set to true, object serialization will be a bit
        /// less robust w.r.t. Instantiation (you need to call SaveState()) -
        /// more importantly, performance will be worse. The only real reason to
        /// set this setting to true is if you want the serialization callbacks
        /// to execute on the main thread - however, you should probably just run
        /// that logic in Awake().
        /// </summary>
        public static bool ForceDisableMultithreadedSerialization = false;

        /// <summary>
        /// What percentage of an editor's width will be used for labels?
        /// </summary>
        public static float LabelWidthPercentage = .45f;
        public static float LabelWidthOffset = 30f;
        public static float LabelWidthMax = 600;
        public static float LabelWidthMin = 0;

        /// <summary>
        /// If an inspector has only one category (see InspectorCategory), should
        /// it be displayed?
        /// </summary>
        public static bool DisplaySingleCategory = true;

        /// <summary>
        /// The default length of a collection before the paging interface is
        /// shown, which will let you view a subset of the collection, making it
        /// easier to manage and inspect. If you do not want the pager to *ever*
        /// activate by default, set this to a negative value (such as -1). If
        /// you want the pager to *always* activate, set this to 0. Otherwise,
        /// set this to a reasonably large value such as 20.
        /// </summary>
        public static int DefaultPageMinimumCollectionLength = 20;

        /// <summary>
        /// The root directory that Full Inspector resides in. Please update this
        /// value if you change the root directory -- if you don't a potentially
        /// expensive scan will be performed to locate the root directory. This
        /// has a trailing slash.
        /// </summary>
        public static string RootDirectory = "Assets/FullInspector2/";

        /// <summary>
        /// If this is set, the list will be use to limit what types are shown
        /// in the type selection. Also used in the ScriptableObjects inspector.
        /// </summary>
        public static List<string> TypeSelectionDefaultFilters;

        /// <summary>
        /// If this is set, any type that matches any list element
        /// will never be shown in the type selection.
        /// Also used in the ScriptableObjects inspector.
        /// </summary>
        public static List<string> TypeSelectionBlacklist;

        /// <summary>
        /// This is automatically configured based on RootDirectory. This has a
        /// trailing slash.
        /// </summary>
        public static string RootGeneratedDirectory;

        static fiSettings() {
            // Create settings and enable customization
            if (fiSettings.EnableSettingsProcessor) {
                foreach (var instance in fiRuntimeReflectionUtility.GetAssemblyInstances<fiSettingsProcessor>()) {
                    instance.Process();
                }
            }

            // Make sure have our root directory properly setup
            if (fiUtility.IsEditor) {
                EnsureRootDirectory();
            }

            if (RootGeneratedDirectory == null) {
                RootGeneratedDirectory = RootDirectory.TrimEnd('/') + "_Generated/";
            }

            if (fiUtility.IsEditor) {
                if (fiDirectory.Exists(RootGeneratedDirectory) == false) {
                    Debug.Log("Creating directory at " + RootGeneratedDirectory);
                    fiDirectory.CreateDirectory(RootGeneratedDirectory);
                }
            }
        }

        /// <summary>
        /// Ensures that fiSettings.RootDirectory points to a folder named
        /// "FullInspector2". If it doesn't, then this will perform a scan over
        /// all of the content inside of the Assets folder looking for that
        /// directory and will notify the user of the results.
        /// </summary>
        private static void EnsureRootDirectory() {
            if (RootDirectory == null || fiDirectory.Exists(RootDirectory) == false) {
                Debug.Log("Failed to find FullInspector root directory at \"" + RootDirectory +
                    "\"; running scan to find it.");

                string foundPath = FindDirectoryPathByName("Assets", "FullInspector2");
                if (foundPath == null) {
                    Debug.LogError("Unable to locate \"FullInspector2\" directory. Please make sure that " +
                        "Full Inspector is located within \"FullInspector2\"");
                }
                else {
                    foundPath = foundPath.Replace('\\', '/').TrimEnd('/') + '/';
                    RootDirectory = foundPath;
                    Debug.Log("Found FullInspector at \"" + foundPath + "\". Please add the following code to your project in a non-Editor folder:\n\n" +
                        FormatCustomizerForNewPath(foundPath));
                }
            }
        }

        private static string FormatCustomizerForNewPath(string path) {
            return
                "using FullInspector;" + Environment.NewLine +
                Environment.NewLine +
                "public class UpdateFullInspectorRootDirectory : fiSettingsProcessor {" + Environment.NewLine +
                "    public void Process() {" + Environment.NewLine +
                "        fiSettings.RootDirectory = \"" + path + "\";" + Environment.NewLine +
                "    }" + Environment.NewLine +
                "}" + Environment.NewLine;
        }

        /// <summary>
        /// Locates a directory of the given name or returns null if the
        /// directory is not contained within the specificed initial
        /// currentDirectory.
        /// </summary>
        /// <param name="currentDirectory">
        /// The directory to begin the recursive search in.
        /// </param>
        /// <param name="targetDirectory">
        /// The name of the directory that we want to locate.
        /// </param>
        /// <returns>
        /// The full directory path for the given directory name, or null.
        /// </returns>
        private static string FindDirectoryPathByName(string currentDirectory, string targetDirectory) {
            // normalize targetDirectory so we can use == instead of EndsWith in
            // the for loop.
            targetDirectory = Path.GetFileName(targetDirectory);

            foreach (string subdir in fiDirectory.GetDirectories(currentDirectory)) {
                // note: subdir is fully qualified w.r.t. currentDirectory we use
                // Path.GetFileName because subdir may end with /, but
                // targetDirectory may not.
                if (Path.GetFileName(subdir) == targetDirectory) {
                    return subdir;
                }

                string result = FindDirectoryPathByName(subdir, targetDirectory);
                if (result != null) {
                    return result;
                }
            }

            return null;
        }
    }
}
