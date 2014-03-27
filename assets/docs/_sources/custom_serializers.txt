.. highlight:: csharp

Custom Serializers
==================

You have a serialization framework and want to use it inside of Full Inspector; how do you do that? Luckily, Full Inspector has fantastic support for third party serializers.

The current serializers are located in *"FullInspector2/Serializers"*. Feel free to take a look at the existing ones.

The process to create a new serializer is simple. Simply derive from ``BaseSerializer`` and implement the abstract methods.

.. IMPORTANT::
    If you come across a ``UnityEngine.Object`` (or derived type) reference, make sure to serialize it using ``SerializationHelpers.StoreObjectReference`` and deserialize it using ``SerializationHelpers.RetrieveObjectReference``. This ensures that the object reference will be treated correctly, when, e.g., the object becomes a prefab.

    These methods convert ``UnityEngine.Object`` references into unique ids. The ids are unique relative to the behavior being serialized and will not be unique across multiple behaviors.

Lets say you created a new serializer called ``MySerializer``. To use it in a behavior or scriptable object, just derive from ``BaseBehavior<MySerializer>``.