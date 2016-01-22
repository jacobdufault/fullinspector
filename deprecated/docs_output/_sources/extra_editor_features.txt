.. highlight:: csharp

Extra Editor Features
=====================

- You can right-click any ``BaseBehavior`` or ``BaseScriptableObject`` object instance to back the current state up. This even works across play-mode! Select *"Window/Full Inspector/Backup Window"* to show all active backups.

- You can right-click on any ``BaseBehavior`` or ``BaseScriptableObject`` object instance to manually save its current state or restore its automatically last saved state. This will typically only be useful if there was a deserialization error that you have corrected and you wish to recover the serialized state.

- You can select *"Window/Full Inspector/Developer/Show Serialized State"* in the Unity top-menu to view the currently serialized state of the object directly below the inspector content. This serialized data is modifiable and the state of the behavior will update in real time to the serialized state modifications.

- You can manage the active serializers by using *"Window/FullInspector/Developer/Show Serializer Importer"*.

- You can view the editor for a ``UnityEngine.Object`` reference inline by hitting the dropdown that appears next to it (if no viewer is available, a dropdown will not appear. This is the case for types that do not derive from ``BaseBehavior`` or ``BaseScriptableObject``, with a few exceptions).