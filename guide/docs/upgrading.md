# Upgrading

Please always do a clean import of Full Inspector (delete the old version before importing the new one), otherwise you may receive a number of difficult to diagnose issues.

Version 2.5 moves all scripts that are specific to your project into a `FullInspector2_Generated` folder, which will be persistent even across package upgrades. This will make the process of migrating to newer versions much more seamless.

The `BaseBehavior` and `BaseScriptableObject` types  were removed in the default distribution for 2.5, as Full Inspector now allows the default serializer to be easily selection upon import.

## The easiest way to update

1. Import Full Inspector into an empty project
2. Exit Unity completely
3. Open up your actual project, delete just the `FullInspector2` folder.
4. Copy over the version of Full Inspector you just imported into the empty project.
5. Restart Unity, you're good to go! Delete your dummy project.

## Q/A: No `BaseBehavior` or `BaseScriptableObject` type

If you are getting error messages about no `BaseBehavior` \ `BaseScriptableObject` type, then there are a number of ways to solve this issue.

#### Download Premade Generated Folder

- Download one of the following generated folders for your selected serializer
    - [BinaryFormatter](assets/FullInspector2_Generated-BinaryFormatter.unitypackage)
    - [FullSerializer](assets/FullInspector2_Generated-FullSerializer.unitypackage)
    - [Json.NET](assets/FullInspector2_Generated-JsonNet.unitypackage)
    - [protobuf-net](assets/FullInspector2_Generated-protobuf-net.unitypackage)
- Import the downloaded *unitypackage* file into your project.

This will only work if you have not removed any of the serializers from a default installation. If you have, then you will have to hand-modify `FullInspector2_Generated/fiLoadedSerializers.cs` or use one of the following two methods.

#### Import into an Empty Project

- Import Full Inspector into an empty project.
- The serializer import window will now display.
    - If it does not, open it by using the menu item `Window/Full Inspector/Developer/Show Serializer Manager`.
- Pick your default serializer in the window and wait for Unity to finish recompiling.
- Copy `Assets/FullInspector2_Generated` to your target project.


#### Manually Create the Files

Add these classes somewhere in a C# file within your project (Full Serializer):

```
namespace FullInspector {
    public abstract class BaseBehavior :
        BaseBehavior<FullSerializerSerializer> {}
    public abstract class BaseScriptableObject :
        BaseScriptableObject<FullSerializerSerializer> {}
    public abstract class SharedInstance<T> :
        SharedInstance<T, FullSerializerSerializer> {}
}
```

If you wish to use Json.NET instead of Full Serializer, use this code snippet instead:

```
namespace FullInspector {
    public abstract class BaseBehavior :
        BaseBehavior<JsonNetSerializer> {}
    public abstract class BaseScriptableObject :
        BaseScriptableObject<JsonNetSerializer> {}
    public abstract class SharedInstance<T> :
        SharedInstance<T, JsonNetSerializer> {}
}
```

and so forth for protobuf-net with `ProtoBufNetSerializer` and BinaryFormatter with `BinaryFormatterSerializer`.
