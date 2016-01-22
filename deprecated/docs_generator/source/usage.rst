.. highlight:: csharp

Usage
=====

Working Guide
-------------
The following list is what you need to keep in mind to ensure that your objects serialize correctly and are fully inspectable using Full Inspector.

- Derive from ``BaseBehavior``, not ``MonoBehaviour``
- If you override ``Awake``, immediately call ``base.Awake()``

That's it! Of course, you'll also have to added your serialization specific annotations if your serializer requires them. If that sounds confusing, then don't worry. There are examples below that demonstrate this point.

If you're using Full Serializer as your default serializer (use "Window/Full Inspector/Developer/Show Serializer Importer" and then change it to the default serializer), then you don't need to change any behavior. Everything will continue to work as it did with Unity's inspector.

.. IMPORTANT::
    One potential gotcha with serialization: each ``BaseBehavior`` field/property is serialized independently. If you have a class instance that is shared across multiple ``BaseBehaviors`` or multiple fields/properties, it will be deserialized into separate instances. You can ensure that it deserializes into one instance by deriving from a ``UnityEngine.Object`` child class, such as ``BaseScriptableObject`` or ``SharedInstance<T>``.

The Full Serializer serializer is recommended for default usage, for for legacy reasons Json.NET is activated by default. Full Serializer works well without annotations, and works on all major Unity platforms.

If you need performance, you can easily switch to protobuf-net for one specific component while using Full Serializer for ease of use on the rest of your behaviors. Everything interacts correctly, even when using different serializers.

Examples
--------

Here's an example of the simplicity of Full Serializer:

.. code:: c#

    public struct Simple<T> {
        public T Value { get; set; }
    }

    public class FullDemo : BaseBehavior<FullSerializerSerializer> {
        public Simple<int> SimpleInt;
        public Simple<float> SimpleFloat;
    }

.. image:: static/usage_fullserializer.png

-------------------------

Here's an example of how to use Full Inspector with Json.NET:

.. code:: c#

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

.. image:: static/usage_jsonnet.png

If you like annotations, you could have optionally written ``Struct`` as

.. code:: c#

    [JsonObject]
    public struct Struct {
        [JsonProperty]
        public GameObject SpawnObject;
        [JsonProperty]
        public Dictionary<string, string> StrStrDict;
    }

-------------------------

Or perhaps you prefer protobuf-net?

.. code:: c#

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

.. image:: static/usage_protobufnet.png

.. IMPORTANT::
    There is experimental support for protobuf-net on AOT platforms such as iOS. Please use *"Window/Full Inspector/Developer/Create protobuf-net precompiled serializer"* before deploying to those targets.

    The precompiled serializer requires an explicit installation of mono on OSX (due to Unity build system limitations, the protobuf-net DLL has to be decompiled into C#, and FI includes a tool to do such, but it is written in C# and requires .NET 4 and so cannot be run in the Unity runtime environment).

-------------------------

Or maybe even BinaryFormatter (beta)?

.. code:: c#

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

.. image:: static/usage_binaryformatter.png

-------------------------

.. IMPORTANT::
    There are many samples inside of *"FullInspector2/Samples"*. The source code for the samples lies in their selected serializer directories *"FullInspector2/Serializer/*/Samples".

    The full source code has been provided and it is highly commented, so please feel free to peruse it as well to understand how Full Inspector works internally.