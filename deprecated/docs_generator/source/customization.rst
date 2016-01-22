.. highlight:: csharp

Customization
=============

There are a couple of special attributes that you can apply to your class members to provide some easy inspector customization.


=============================   ======================================================================
**Attribute**                   **Description**
-----------------------------   ----------------------------------------------------------------------

[HideInInspector]				Force a field or property to be hidden in the inspector. Only public fields/properties are shown by default.

[InspectorButton]				Draw the method as a button in the inspector.

[InspectorComment]				Add a comment below the given field/property/type

[InspectorDisabled]				Draw the inspector with a disabled GUI, so that the user cannot interact with it.

[InspectorDivider]				Draw a divider in the inspector.

[InspectorHeader]				Draw a header above the given field or attribute with some nice text to go along with the header.

[InspectorHidden]				Don’t show this attribute in the inspector (by default, every member is shown, even private ones)

[InspectorHidePrimary]			Hide the primary editor, only showing the attribute ones. This is primarily used to draw a margin or a comment that stands alone.

[InspectorIndent]				Indent the given field or property in the inspector.

[InspectorMargin]				Add space above the given field/property

[InspectorName]					Give the field, property, or method a custom name in the inspector.

[InspectorOrder]				Foricbly specify the order to display the field, property, or method.

[InspectorSkipInheritance]		Do not show the inheritance drop-down.

[InspectorTextArea]				Draw the string using a fixed-height text area.

[InspectorTooltip]				Add a tooltip to the given field, property, or method.

[NotSerialized]					Do not serialize this field or property. Identical to NonSerialized, except that it can also be applied to properties.

[ShowInInspector]				Force a field or property to be displayed in the inspector. Non-public fields/properties are not shown by default.

[VerifyPrefabType]				Verify that the given ``UnityObject`` reference has the given prefab type.

=============================   ======================================================================

Please see ``fiSettings`` to customize how Full Inspector operates. It is located at *FullInspector2/fiSettings.cs*. The settings are all contained within fiSettings

`fiSettings`, with it's default values, is duplicated below:

.. code:: c#

    /// <summary>
    /// This class contains some settings that can be used to customize the behavior of the Full
    /// Inspector.
    /// </summary>
    public static class fiSettings {
        /// <summary>
        /// A scene has just been saved. Should all IScriptableObjects be checked to see if they
        /// need to be saved? This is disabled by default because it causes a performance hit when
        /// saving and unless you have an extremely strange user scenario where you are not using
        /// the inspector to edit a BaseBehavior, everything will save correctly.
        /// </summary>
        public static bool ForceSaveAllAssetsOnSceneSave = false;

        /// <summary>
        /// A recompilation has been detected. Should all IScriptableObjects be checked to see if
        /// they need to be saved? This is disabled by default because it causes a performance hit
        /// when saving and unless you have an extremely strange user scenario where you are not
        /// using the inspector to edit a BaseBehavior, everything will save correctly.
        /// </summary>
        public static bool ForceSaveAllAssetsOnRecompilation = false;

        /// <summary>
        /// A recompilation has been detected. Should all IScriptableObjects be checked to see if
        /// they need to be restored? This is disabled by default because it causes a performance
        /// hit.
        /// </summary>
        public static bool ForceRestoreAllAssetsOnRecompilation = false;

        /// <summary>
        /// If this is set to true, then Full Inspector will attempt to automatically instantiate
        /// all reference fields/properties in an object. This will negatively impact the
        /// performance for creating objects (lots of reflection is used).
        /// </summary>
        public static bool AutomaticReferenceInstantation = false;

        /// <summary>
        /// If this is set to true, then when the reflected inspector encounters a property that is
        /// null it will attempt to create an instance of that property. This is most similar to how
        /// Unity operates. Please note that this will not instantiate fields/properties that are
        /// hidden from the inspector. Additionally, this will not instantiate fields which do not
        /// have a default constructor.
        /// </summary>
        public static bool InspectorAutomaticReferenceInstantation = true;

        /// <summary>
        /// Should public properties/fields automatically be shown in the inspector? If this is
        /// false, then only properties annotated with [ShowInInspector] will be shown.
        /// [HideInInspector] will never be necessary.
        /// </summary>
        [Obsolete("This setting is now ignored")]
        public static bool InspectorAutomaticallyShowPublicProperties = true;

        /// <summary>
        /// Should Full Inspector emit warnings when it detects a possible data loss (such as a
        /// renamed or removed variable) or general serialization issue?
        /// </summary>
        public static bool EmitWarnings = false;

        /// <summary>
        /// Should Full Inspector emit logs about graph metadata that it has culled? This may be
        /// useful if you have written a custom property editor but changes to your graph metadata
        /// are not being persisted for some reason.
        /// </summary>
        public static bool EmitGraphMetadataCulls = false;

        /// <summary>
        /// The minimum height a child property editor has to be before a foldout is displayed
        /// </summary>
        public const float MinimumFoldoutHeight = 80;

        /// <summary>
        /// Display an "open script" button that Unity will typically display.
        /// </summary>
        public const bool EnableOpenScriptButton = true;

        /// <summary>
        /// What percentage of an editor's width will be used for labels?
        /// </summary>
        public const float LabelWidthPercentage = .35f;
        public const float LabelWidthMax = 400;
        public const float LabelWidthMin = 0;

        /// <summary>
        /// Should Full Inspector persist graph metadata across play-mode and
        /// across Unity sessions? If this is true, then reload time will be slightly increased.
        /// </summary>
        /// <remarks>Metadata persistence is currently an experimental feature. It works well right now, but
        /// it *may* cause a 5-60s lock-up if the inspector goes into an infinite recursion.</remarks>
        public static bool EnableMetadataPersistence = false;

        /// <summary>
        /// The root directory that Full Inspector resides in. Please update this value if you change
        /// the root directory -- if you don't a potentially expensive scan will be performed to locate
        /// the root directory.
        /// </summary>
        public static string RootDirectory = "Assets/FullInspector2";

        /// <summary>
        /// If a configuration error is detected, should fiSettings.cs be opened in the external
        /// editor automatically?
        /// </summary>
        public static bool OpenSettingsScriptOnConfigError = true;
    }
