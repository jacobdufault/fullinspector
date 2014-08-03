.. highlight:: csharp

Changelog
=========

2.4
---

New:

- Full Inspector now includes an object backup system for the Unity Editor. Just right-click a behavior and click "Backup". It even works across play mode! Each scene has it's own set of backup data.
- The ``ObjectPropertyEditor`` now has a toggle that allows for an inline view of the selected object.
- Introduce ``BehaviorEditor`` as a mirror to ``PropertyEditor`` system. ``BehaviorEditors`` are used for creating editors for types which derive from ``UnityEngine.Object``. This fixes a critical bug where object references were not displayed properly when ``BaseBehavior``/``BaseScriptableObject`` types had a custom property editor.
- Introduce ``LayoutToolkit``, which makes it easy to create declarative UI's using Unity's immediate mode GUI. It's an allocation free alternative to ``GUILayout`` that also works in ``GUILayout`` restricted areas.
- Introduce serialize selector, which makes it easy to select which serializer you want to use as the default.
- Add support for Full Serializer, an automated serializer that just works with the same serialization interface that Unity uses -- this is a free serializer that works on all of the Unity export platforms.
- You can now use ``[JsonProperty]``, ``[JsonIgnore]``, ``[ProtoMember]``, etc, on ``BaseBehavior``/``BaseScriptableObject`` members!
- Foldouts are now animated
- Introdue the ``Facade<T>``, which allows for editing, say, a ``BaseBehavior`` type in the inspector *without* creating an instance of it.
- A new metadata engine (``fiGraphMetadata``) has been introduced. It stores metadata based on the structure of the object/property editor graph. The ``IPropertyEditor`` API has changed slightly to accommodate it. Additionally, ``ObjectMetadata`` has been renamed to ``fiGlobalMetadata``.
- Added serialization callbacks on ``BaseBehavior``/``BaseScriptableObject`` types with the ``ISerializationCallbacks`` interface.
- protobuf-net serializer can now ignore selected assemblies. See ``TypeModelCreator.AssemblyIgnoreList``.
- ``Gradients`` are now fully supported.
- Added a ``NullSerializer`` that bypasses all serialization. This is useful if you wish to just use Unity serialization but want to utilize the Full Inspector inspector interface.
- Added ``[InspectorHeader]``, an analog to Unity's new ``[Header]`` attribute
- Added ``[InspectorCollectionShowItemDropdown]`` to show dropdown arrows on collection items.
- Added ``[InspectorTextArea]``, to show a text-area instead of a text-field for string types.
- Added ``[InspectorName]`` to set the name to use for a field, property, or method in the inspector.
- Read-only properties will now be displayed by default.
- Serialized fields are now displayed by default. This means that for private fields, adding ``[SerializeField]`` is enough -- no need to also add ``[ShowInInspector]``.
- Introduce ``SharedInstance<T>``, which makes it easy to share object instances across serialization contexts.
- Introduce ``TypeSpecifier<TBaseType>``, which makes using Types in the Inspector even easier! Only types that are assignable to ``TBaseType`` will be pickable in the inspector.

Fixes:

- Fixed inspector rendering issues for object graphs with cycles.
- Fixed inspector rendering issues for recursive type definitions.
- Fix ``LayerMask`` editor when the layer is set to a negative value.
- Read-only properties will now be displayed with a disabled GUI.
- Fixed issue where DateTime editor would change the value in the DateTime.
- Fix private fields on a BaseBehavior/BaseScriptableObject with a ``[SerializedField]`` annotation not being serialized.
- Foldouts are now displayed whenever a sub-item is above a certain height, regardless of the type being edited.
- If a foldout was displayed for an item before and the item shrinks in height, the foldout will now continue to be displayed.
- Selected a type that derives from ``BaseBehavior`` in the abstract type drop-down will no longer emit a warning about component construction.
- ``OnValidate`` will now be called whenever the inspector has changed.
- Dropdown states and the like will now be saved across play-mode (if ``fiSettings.EnableMetadataPersistance`` is enabled)
- Fix protobuf-net serialization issue where only the first ``UnityObject`` reference was restored.
- The ``Rect`` property editor will now display less information.
- Fix issue where overriding a property would display it in the inspector twice.
- Fix deserialization issue with nullable Vector types in Json.NET
- Moving the base FI2 directory around is now fully supported.

Misc:

- Optimization pass over the memory allocation profile for Full Inspector (approx 5x reduction, so 100kb -> 20kb).
- Optimization pass over startup time - first inspector views should now be nearly instant.
- Removed warnings from the static inspector.
- Assembly filtering has been improved, hopefully leading to faster reload times.
- Full Inspector will now only scan editor assemblies for custom property editors.
- Abstract type selection dropdown is now sorted by name.
- Parent members will now be displayed before derived members in the inspector.
- Added missing ``DateTimeOffset`` editor.
- Minor polish: variable names like "ID" will now be displayed as "ID" instead of "I D"
- Renamed ``MarginAttribute`` to ``InspectorMarginAttribute``
- Renamed ``TooltipAttribute`` to ``InspectorTooltipAttribute``
- Renamed ``SingleItemListEditorAttribute`` to ``InspectorDatabaseEditorAttribute``
- ``FullInspectorSettings`` has been renamed to ``fiSettings`` and has been moved out of Core/ into the top-level Full Inspector directory.
- Fields/properties are now lazily written. This means that property set methods will be invoked only when the value has actually changed in the case of value-types.
- API NOTICE: ``GetElementHeight`` is now automatically cached. Calling it from an Edit function will return a cached value instead of recomputing it.



2.3
---

New:

- Added the static inspector, which allows easy inspecting of static/global variables directly from within Unity
- Added a generic ``SerializableAction`` and ``SerializableFunc`` as equivalents to ``System.Action`` and ``System.Func``, except they can be serialized (but require a ``UnityEngine.Object`` function source). These are extremely powerful and support full type safety with contra/covariance.
- Methods can now be easily displayed in the inspector using the ``[InspectorButton]`` attribute
- Inspector attribute property editors can now explicitly state what order they should appear in via extending ``IInspectorAttributeOrder``
- Added ``[InspectorHidePrimary]`` property attribute that hides the primary inspector. This is extremely useful for displaying a comment inside of an inspector that is not associated with any editor.
- Added new ``[InspectorDivider]`` attribute that draws a divider in the inspector
- Introduced ``PropertyEditorChain`` abstraction for those writing custom property editors; see docs.
- Add ``EditWithGUILayout`` extension method to ``IPropertyEditor`` that makes a property editor easily usable within a ``GUILayout`` section (note: this is *not* ``GUILayout`` support within property editors -- sorry!)
- Added a property editor for ``System.Type``
- Added a very basic property editor for ``System.ICustomAttributeProvider``
- Added property editors for ``System.DateTime`` and ``System.TimeSpan``
- Added a new ``CommentType`` enum to the ``CommentAttribute``, allowing an info, warning, or error image to be displayed with the comment
- Added a custom property editor for nullable types

Fixes:

- The reflected property editor will now correctly handle cyclic object graphs
- The reflected property editor will no longer infinitely allocate objects to an unlimited depth (allows for recursive type definitions)
- Fix issue where Unity would draw highly indented ``EditorGUI`` methods incorrectly
- Reflection-based allocation code will now instantiate ``ScriptableObjects`` correctly
- Fix null dereference in object modification detector
- Corrected issue where when disabling warnings caused the editor to incorrectly handle null inspector targets
- Fix issue where too much data was being serialized for the base FI types
- The inspected object's state will not be serialized during play mode
- Fix spelling error in ``IAttributePropertyEditor.Attribute``
- ``LayerMaskEditor`` previously generated invalid LayerMasks
- Fixed serialization of ``Color?``, ``Vector2?``, ``Vector3?``, ``Vector4?`` in Json.NET
- protobuf-net will now serialize default values by default (fixes nullable type serialization)

Deprecations:

- ``[Order]`` has been deprecated; use ``[InspectorOrder]`` instead

Misc:

- Performance improvements for ``ICollection`` and ``IDictionary`` property editors
- Code reorganization and cleanup


2.2
---

New:

- Easily serialize and deserialize structs/etc to files
- Mimick the "open script" button present on MonoBehaviors/ScriptableObjects. This can be disabled via the FullInspectorSettings.ShowOpenScriptButton setting
- Editor support for all ``ICollection<T>`` derived types, such as ``HashSet<T>``
- Support new ``Order`` attribute, which allows for custom ordering of fields/properties in the inspector (even across partial types)

Fixes:

- Fix issue where non-prefab override properties would be shown in bold
- Fix issue where inspected object would be deserialized during gameplay from old data
- Inspector is now redrawn constantly while in play mode
- Comments will now display with minimal height
- Updated EasySave2 serializer
- The inspector can now utilize every bit for long/ulong/decimal/double types
- ``Array``/``List``/``ICollection``/``IDictionary`` editors now have a minimum height for, say, an empty class
- ``ICollection`` and ``IDictionary`` now reset the next insertion object to the default value after an insertion
- Fix issue where ``[NonSerialized]`` attribute would be ignored if a ``[SerializeField]`` property was also present

Misc:

- Improved the Json.NET serialization error reporting message


2.1
----

New:

- Full Inspector now supports property editors that are activated via attributes
- Full Inspector now has undo/redo support!
- Prefab override properties will now be displayed in bold
- Support OnSceneGUI for IPropertyEditors or IAttributePropertyEditor types
- A collapsed interface/abstract/inherited type will now display the current type in the foldout header
- A collapsed array/list/dictionary will now show the number of elements in the collapsed container

Samples:

- Added two simple sample games (platformer and arena)

API:

- Default implementations are now provided for all (Attribute)PropertyEditor<TElement> methods, which simplifies implementing, for example, just an OnSceneGUI method.

Fixes:

- Vector2 and Vector3 now display properly in non-wide inspectors
- Added a PropertyEditor and serialization support for LayerMasks
- Fixed foldouts for interfaces
- Fixed issue where prefab children would sometimes not properly display their serialized state
- Fixed issue where [ShowInInspector] fields/properties could not be displayed if they were not also serialized.

Misc:

- CommentAttribute and MarginAttribute no longer have any special support within Full Inspector
- [JsonObject(MemberSerialization.OptIn)] is no longer required for the Json.NET serializer
- Replaced SurrogateSelector with custom implementation; SurrogateSelector could not disambiguate between two types with the same name but in different assemblies.
- BinaryFormatter is now using a custom SerializationBinder that is more robust to type changes


2.0
---

This is a big update! It is unfortunately not backwards compatible.

- Ref has been removed!
- You no longer have to annotate your ``BaseBehavior`` derived type. In ``BaseBehaviors``, public fields / properties are serialized by default. You can also serialize private fields by annotating them with ``[SerializeField]``. Fields / properties will not be serialized if they are annotated with ``[NonSerialized]`` or ``[NotSerialized]``.
- A (beta) BinaryFormatter serializer has been added
- An experimental EasySave2 serializer has been added
- Full Inspector now resides inside a *"FullInspector2"* folder due to a directory reorganization. Please make sure to delete your *"FullInspector"* directory if upgrading.
- Added a setting to disable automatic display of public properties in the inspector (``FullInspectorSettings.InspectorAutomaticallyShowPublicPropertie``)
- Added a setting to automatically restore all assets on recompilation (``FullInspectorSettings.ForceRestoreAllAssetsOnRecompilation``)
- Json.NET compatibility fixes for Json.NET from the Asset Store.
- Better prefab revert support
- (beta) protobuf-net one-click precompilation support
- Namespace changes, most things moved into FullInspector.Internal
- Foldouts are no longer displayed for structs -- sorry! Hopefully this will get reenabled in the future, but it the current solution doesn't support it.
- ``SaveState`` now marks an object as dirty if running in the editor
- The samples have been redesigned and sample scenes are now included.

1.2
---

- Use ``AbstractPropertyEditor`` if the inspected type has any derived types
- Support generic derived types in the ``AbstractPropertyEditor``
- Support object instantiation in the inspector for types without default constructors
- Full Inspector now only shows public variables by default in the inspector. Non-public properties / fields can be shown by adding a ``[ShowInInspector]`` attribute
- Deprecate ``[Hidden]`` in favor of ``[HideInInspector]``
- Json.NET modified to support compilation with UnityVS
- Correct foldout width so that the header element is usable when foldout is active
- The enum property editor now identifies ``[Flags]`` enums and shows a mask popup
- Support custom editors for enum types
- Allow inherited property editors to be overridden
- Added button sample
- Make it obvious in the inspector when a ``UnityEngine.Object`` is not wrapped in a ``Ref<>``

1.10
----

- Better deserialization error recovery, particularly if the variable type has changed.
- Added support for arbitrary serialization framework support; Full Inspector now ships with serializers for protobuf-net and Json.NET.
- ``ScriptableObjects`` are now fully supported.
- Added implicit conversions for ``Ref``, simplifying its usage.
- The reflected property editor will now automatically display a fold-out for child ``PropertyEditors`` that are relatively tall.
- Added user setting to automatically instantiate *all* references (even private ones) in objects when the object has no deserialization data (``FullInspectorSettings.AutomaticReferenceInstantation``).
- Added user setting to disable automatic object instantiation in the inspector (``FullInspectorSettings.InspectorAutomaticReferenceInstantation``).
- Added more content to the manual, added a QA section.

1.01
----

- Initial release!