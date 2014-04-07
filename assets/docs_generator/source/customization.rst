.. highlight:: csharp

Customization
=============

There are a couple of special attributes that you can apply to your class members to provide some easy inspector customization.


========================        ======================================================================
**Attribute**                   **Description**
------------------------        ----------------------------------------------------------------------
CommentAttribute                Add a comment below the given field/property/type

TooltipAttribute                Add a tooltip viewable after hovering over the field/property

MarginAttribute                 Add space above the given field/property

HiddenAttribute                 Don’t show this attribute in the inspector (by default, every member is shown, even private ones)

ShowInInspectorAttribute        Force a field or property to be displayed in the inspector. Non-public fields/properties are not shown by default.

HideInInspectorAttribute        Force a field or property to be hidden in the inspector. Only public fields/properties are shown by default.

NotSerializedAttribute          Do not serialize this field or property. Identical to NonSerialized, except that it can also be applied to properties.
========================        ======================================================================

Please see ``FullInspectorSettings`` to customize how Full Inspector operates. It is located at *FullInspector/FullInspector/FullInspectorSettings.cs*


==========================================      ============================================================
**Setting**                                     **Description**
------------------------------------------      ------------------------------------------------------------
ForceSaveAllAssetsOnSceneSave                   A scene has just been saved. If true, then every ``BaseBehavior`` will be saved, not just the ones that have been modified.

ForceSaveAllAssetsOnRecompilation               A recompilation has been detected. If true, then every ``BaseBehavior`` will be saved, not just the ones that have been modified.

ForceRestoreAllAssetsOnRecompilation            A recompilation has been detected. If true, then every ``BaseBehavior`` will be restored.

AutomaticReferenceInstantation                  If true, then all fields/properties in a object that has no deserialization data will be instantiated to their default constructor value.

InspectorAutomaticReferenceInstantation         If true, then the reflected inspector will automatically instantiated null values to their default constructor value.

InspectorAutomaticallyShowPublicProperties      If true, then the inspector will automatically show public fields and properties. If this is false, then only properties with a ``[ShowInInspector]`` attribute will be shown in the inspector.
==========================================      ============================================================
