# Adding a Serializer

// TODO: rewrite this, create a custom example

You have a serialization framework and want to use it inside of Full Inspector; how do you do that? Luckily, Full Inspector has fantastic support for third party serializers.

The current serializers are located in “FullInspector2/Serializers”. Feel free to take a look at the existing ones.

The process to create a new serializer is simple. Simply derive from `BaseSerializer` and implement the abstract methods.

<important>
If you come across a `UnityEngine.Object` (or derived type) reference, make sure to serialize it using `ISerializationOperator.StoreObjectReference` and deserialize it using `ISerializationOperator.RetrieveObjectReference`. This ensures that the object reference will be treated correctly, when, e.g., the object becomes a prefab. It will also ensure that an appropriate error is raised when the user tries to serialize a `UnityEngine.Object` reference to disk.

These methods convert `UnityEngine.Object` references into unique ids. The ids are unique relative to the behavior being serialized and will not be unique across multiple behaviors.
</important>

Lets say you created a new serializer called `MySerializer`. To use it in a behavior or scriptable object, just derive from `BaseBehavior<MySerializer>`.