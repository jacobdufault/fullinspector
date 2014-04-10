.. highlight:: csharp

Extra Editor Features
=====================

- You can right-click on any component which derives from ``BaseBehavior`` to manually save its current state or restore its automatically last saved state. This will typically only be useful if there was a deserialization error that you have corrected and you wish to recover the serialized state.

- You can select *"Window/FullInspector/Show Serialized State"* in the Unity top-menu to view the currently serialized state of the object directly below the inspector content. This serialized data is modifiable and the state of the behavior will update in real time to the serialized state modifications.