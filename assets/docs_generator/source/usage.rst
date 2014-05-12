.. highlight:: csharp

Usage
=====

Working Guide
-------------
The following list is what you need to keep in mind to ensure that your objects serialize correctly and are fully inspectable using Full Inspector.

- Derive from ``BaseBehavior``, not ``MonoBehaviour``
- If you override ``Awake``, immediately call ``base.Awake()``

That's it! Of course, you'll also have to added your serialization specific annotations if your serializer requires them. If that sounds confusing, then don't worry. There are examples below that demonstrate this point.

.. IMPORTANT::
    If you have custom save-game logic, make sure that you run ``FullInspectorSaveManager.SaveAll()`` before your save logic; it will ensure that every ``BaseBehavior`` instance is ready to go through Unity serialization. Saves can be detected automatically in the editor but not in a published build.

    It's worth stating that the ``FullInspectorSaveManager.SaveAll()`` is the robust, but slow, method of serializing data. If you're saving your game and looking to increase performance, you can selectively specify which objects to serialize via calling ``SaveState()`` on them.  ``SaveAll()`` does this via iterating every object that derives from ``ISerializedObject``.

.. DANGER::

    If you're working with an object in the editor and are modifying it without using the inspector, make sure to restore the serialized data by calling ``RestoreState()``. After you're done modifying it, call ``SaveState()``.

    This should go away in Unity 4.5, when serialization callbacks have landed.

.. IMPORTANT::
    One potential gotcha with serialization: each ``BaseBehavior`` is serialized independently. If you have a class instance that is shared across multiple ``BaseBehaviors``, it will be deserialized into separate instances. You can ensure that it deserializes into one instance by deriving from a ``UnityEngine.Object`` child class, such as ``BaseScriptableObject``.

You're recommended to use the Json.NET serializer. It works well without annotations and has support for every major platform that Unity can export to (via another asset store product).

If you need performance, you can easily switch to protobuf-net for one specific component while using Json.NET for ease of use on the rest of your behaviors. Everything interacts correctly, even when using different serializers.

Examples
--------

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

    If you have a private member on a ``BaseBehavior`` type that you want serialized, you need to use ``SerializeField``, *not* ``JsonMember`` or ``ProtoMember``.

    For example,

    .. code:: c#

        public class Serializes : BaseBehavior {
            [SerializeField]
            private GenericHolder<int> SerializeField;
        }

    will serialize correctly.

    However,

    .. code:: c#

        public class DoesNotSerialize : BaseBehavior {
            [JsonProperty]
            private GenericHolder<int> DoesNotSerializeField;
        }

    does not serialize as expected.

    ``[JsonProperty]`` and ``[ProtoMember]`` support will be added in a future version.

-------------------------


Full Inspector also includes experimental support for EasySave2. If none of these four serializers is your favorite, then it's extremely easy to add support for a new one.

.. IMPORTANT::
    There are many samples inside of *"FullInspector2/Samples"*.

    The full source code has been provided and it is highly commented, so please feel free to peruse it as well to understand how Full Inspector works internally.