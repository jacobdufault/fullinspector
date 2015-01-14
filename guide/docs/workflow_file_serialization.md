# File Serialization

Great news! Full Inspector makes it really easy use your favorite serializer (Full Serializer, Json.NET, BinaryFormatter, or protobuf-net) to serialize/deserialize content to and from files.

<important>
This serialization method does not support `UnityEngine.Object` references. If you try to serialize a type that contains a `UnityEngine.Object` reference, Full Inspector will throw a `NotSupportedException`.
</important>

In actuality, this API is more general than just file serialization. It serializes directly to and from strings, which can be easily written to files via `File.ReadAllText` and `File.WriteAllText`.

## Serialize to a string

```c#
// Get the serialized state of the object
string content = SerializationHelpers.
    SerializeToContent<SerializedStruct, JsonNetSerializer>(Value);
```

## Deserialize from a string

```c#
// Read in the serialized state
string content = File.ReadAllText(Path);

// Restore the value
var restored = SerializationHelpers.
    DeserializeFromContent<SerializedStruct, JsonNetSerializer>(content);
```

In these samples, `SerializedStruct` is the type being serialized, and `JsonNetSerializer` is the serializer to use. You could also have used `FullSerializerSerializer`, `BinaryFormatterSerializer`, or `ProtoBufNetSerializer` instead of `JsonNetSerializer` to serialize the object.