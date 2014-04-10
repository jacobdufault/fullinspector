.. highlight:: csharp

Serialization to a File
=======================

Great news! Full Inspector makes it really easy use your favorite serializer (Json.NET, BinaryFormatter, or protobuf-net) to serialize/deserialize content to and from files.

.. IMPORTANT::
    This serialization method does not support ``UnityEngine.Object`` references. If you try to serialize a type that contains a ``UnityEngine.Object`` reference, you will get a ``NotSupportedException``.

In actuality, this API is more general than just file serialization. It serializes directly to and from strings, which can be easily written to files via ``File.ReadAllText`` and ``File.WriteAllText``.

.. TIP::
    If you would like to see a live demo of this, open up ``samples-other`` and navigate to the "DiskSerializedBehavior" GameObject.

Serialize to a string
---------------------
.. code:: c#

    // Get the serialized state of the object
    string content = SerializationHelpers.
        SerializeToContent<SerializedStruct, JsonNetSerializer>(Value);

Deserialize from a string
-------------------------

.. code:: c#

    // Read in the serialized state
    string content = File.ReadAllText(Path);

    // Restore the value
    var restored = SerializationHelpers.
        DeserializeFromContent<SerializedStruct, JsonNetSerializer>(content);


Explanation
-----------
In these samples, ``SerializedStruct`` is the type being serialized, and ``JsonNetSerializer`` is the serializer to use. You could also have used ``BinaryFormatterSerializer`` or ``ProtoBufNetSerializer`` instead of ``JsonNetSerializer`` to serialize the object.