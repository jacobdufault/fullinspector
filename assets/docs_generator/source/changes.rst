.. highlight:: csharp

Changelog
=========

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