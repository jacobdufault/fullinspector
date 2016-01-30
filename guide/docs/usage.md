# Usage

## Working Guide

<hr />

```c#
using FullInspector;

// MonoBehaviour
public class HelloFullInspector : BaseBehavior {
    public Dictionary<string, GameObject> dict;
}

// ScriptableObject
public class HelloFullInspector : BaseScriptableObject {
    public Dictionary<string, GameObject> dict;
}

// Everything else
[Serializable]
public class HelloFullInspector : BaseObject {
    public Dictionary<string, GameObject> dict;
}
```

<hr />

The easiest way to use Full Inspector is to derive from either `BaseBehavior`, `BaseScriptableObject`, or `BaseObject`. Full Inspector will then just work. If you override `Awake`, make sure you call `base.Awake()`.

If you only want inspector support without serialization support, then you can use [[fiInspectorOnly]](#docs/usage_fiinspectoronly). If you just want to use Full Inspector for just one field or something similar, then you can use [fiValue](#docs/usage_fivalue).

That’s it! Of course, you’ll also have to added your serialization specific annotations if your serializer requires them. If that sounds confusing, then don’t worry. There are examples below that demonstrate this point.

<note>
If deriving from a custom base class is not an option or you wish to limit how Full Inspector interacts with your project, then you can use [fiValue](#docs/usage_fivalue) and [[fiInspectorOnly]](#docs/usage_fiinspectoronly).
</note>

<important>
One potential gotcha with serialization: each `Base*` object instance has every field/property serialized independently. If you have a class instance that is shared across multiple `Base*` instances or multiple fields/properties, it will be deserialized into separate instances. You can ensure that it deserializes into one instance by deriving from a `UnityEngine.Object` child class, such as `BaseScriptableObject` or `SharedInstance<T>`.
</important>

<important>
If you're modifying `Base*` type object instances that are not being inspected, then everything will work *unless* the object is a prefab. If it's a prefab, then you need to call `obj.SaveState()` after you're done editing it so that the changes will be serialized.
<br /> <br />
It's perfectly fine to call `obj.SaveState()` if the object is not a prefab. However, you do not need to as Full Inspector makes this call automatically for you so that you don't have to worry about it.
</important>

When you first install Full Inspector you'll see the [serializer manager](#docs/usage_serializer_manager) popup which will let you pick the default serializer. Full Serializer is recommended for default usage; it works well without annotations and supports every major Unity platform.

Best of all, with Full Serializer you don't need to learn any new serialization annotations - it will work just like Unity, except that everything can now be serialized.

[How do I change my default serializer?](#docs/usage_serializer_manager)

If you need performance, you can easily switch to protobuf-net for one specific component while using Full Serializer for ease of use on the rest of your behaviors. Everything interacts correctly, even when using different serializers.


## More on BaseObject

`BaseObject` derives from `System.Object`, not `UnityEngine.Object` so it is perfect whenever you want to use Full Inspector without the overhead of `MonoBehaviours` or `ScriptableObjects`.

<note>`BaseObject` will use Full Serializer for serialization because it relies on Unity's serialization callbacks. These callbacks are sometimes executed off of the main thread which will cause every other serializer to throw exceptions when they invoke Unity APIs.</note>

Here's how you use `BaseObject`:

```c#
using System;
using System.Collections.Generic;
using FullInspector;
using UnityEngine;

[Serializable]
public class MyObject : BaseObject {
    public Dictionary<string, string> myDictionary;
}

public class BaseObjectDemo : MonoBehaviour {
    public MyObject value1;
    public int[] value2;
}
```

![](docs/images/baseobject_demo.png)

## BaseBehavior examples with different serializers

Here’s an example of the simplicity of Full Serializer:

```c#
public struct Simple<T> {
    public T Value { get; set; }
}

public class FullDemo : BaseBehavior<FullSerializerSerializer> {
    public Simple<int> SimpleInt;
    public Simple<float> SimpleFloat;
}
```

![](docs/images/usage_fullserializer.png)

Here’s an example of how to use Full Inspector with Json.NET:

```c#
public struct Struct {
    public GameObject SpawnObject;
    public Dictionary<string, string> StrStrDict;
}

public class JsonNetStructDemoBehavior :
    BaseBehavior<JsonNetSerializer> {

    public Struct PublicValue;

    [ShowInInspector]
    private Struct _hidden; /* this field is not serialized */
}
```

![](docs/images/usage_jsonnet.png)

If you like annotations, you could have optionally written Struct as

```c#
[JsonObject]
public struct Struct {
    [JsonProperty]
    public GameObject SpawnObject;
    [JsonProperty]
    public Dictionary<string, string> StrStrDict;
}
```

Or perhaps you prefer protobuf-net?

```c#
[ProtoContract]
[ProtoInclude(1, typeof(Implementation1))]
[ProtoInclude(2, typeof(Implementation2))]
public interface IInterface {
}

[ProtoContract]
public class Implementation1 : IInterface {
    [ProtoMember(1)]
    public int A;
}

[ProtoContract]
public class Implementation2 : IInterface {
    [ProtoMember(1)]
    public bool B;

    [ProtoMember(2)]
    public Transform Location;
}

public class InterfaceDemoBehavior :
    BaseBehavior<ProtoBufNetSerializer> {

    public IInterface MyInterface;
}
```

![](docs/images/usage_protobufnet.png)

<important>
There is experimental support for protobuf-net on AOT platforms such as iOS. Please use “Window/Full Inspector/Developer/Create protobuf-net precompiled serializer” before deploying to those targets.

The precompiled serializer requires an explicit installation of mono on OSX (due to Unity build system limitations, the protobuf-net DLL has to be decompiled into C#, and FI includes a tool to do such, but it is written in C# and requires .NET 4 and so cannot be run in the Unity runtime environment).
</important>

Or maybe even BinaryFormatter (not recommended)?

```c#
[Serializable]
public struct Struct<TValue> {
    public Dictionary<string, TValue> MyDict;
    public TValue MissingValue;
}

public class BinaryFormatterStructDemoBehavior :
    BaseBehavior<BinaryFormatterSerializer> {

    public Struct<Transform> SomeStruct;

    public enum Enum {
        ValueA, ValueB, ValueC
    }
    public Dictionary<Enum, string> EnumStringDict;
}
```

![](docs/images/usage_binaryformatter.png)