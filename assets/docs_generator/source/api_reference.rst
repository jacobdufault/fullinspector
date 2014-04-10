.. highlight:: csharp

API Reference
=============

.. warning::
    This API reference is not yet complete. Each of these types (and internal ones) are *much* more heavily commented. This is just an overview of the important parts.

.. tip::
    The recommended way to explore the Full Inspector API is to use code completion on the ``FullInspector`` namespace. The ``FullInspector.Internal`` namespace contains types that are likely not going to be useful outside of internal logic in Full Inspector.

This document contains the primary API endpoints that you may be interested in.

Serialization
-------------

``interface ISerializedObject``: Contains the core API that is needed for the serialization of objects.

- ``void SaveState()``: Save the current state of the object.
- ``void RestoreState()``: Restore the last saved state.
- ``bool Restored { get; set; }``: Get or set if the object has been restored.
- ``List<UnityObject> SerializedObjectReferences { get; set; }``: A list of UnityObject references that were enocuntered during serialization.
- ``List<string> SerializedStateKeys { get; set; }``: A list of property names that were serialized.
- ``List<string> SerializedStateValues { get; set; }``: A list of serialized property values, corresponding by index to ``SerializedStateKeys``.

``class BaseBehavior``: Derive from this to enable Full Inspector.

- ``void SaveState()``: Save the current state of the component so that it can be serialized by Unity.
- ``void RestoreState()``: Restore the last saved state. This only applies to properties that were serialized by Full Inspector and not properties that were serialized by Unity.

``class SerializationHelpers``: Provides some common methods for serialization.

- ``public static void SaveState<TSerializer>(ISerializedObject obj)``: Save the state of the given object using the selected serializer.
- ``public static void RestoreState<TSerializer>(ISerializedObject obj)``: Restore the state of the given object using the selected serializer.
- ``public static T Clone<T, TSerializer>(T obj)``: Clone the given object using the given serializer.

Serializers
-----------

``class BinaryFormatterSerializer : FormatterSerializer<BinaryFormatter>``: Provides serialization support for BinaryFormatter.

``class FormatterSerializer<TFormatter> : BaseSerializer where TFormatter : IFormatter, new()``: Provides serialization support for ``IFormatter`` types.

``interface IFormatterWorker``: Derive to modify the IFormatter surrogate selector. The derived type is automatically instantiated and invoked if it has a default constructor.

- ``void Work(DictionarySurrogateSelector surrogates, StreamingContext context)``: Automatically invoked so that the ``DictionarySurrogateSelector`` can be modified by custom code.

``class JsonNetSerializer``: Provides serialization support for Json.NET.

``class ProtoBufNetSerializer``: Provides serialization support for protobuf-net.

``interface IProtoModelWorker``: Allows an object to modify the protobuf-net ``RuntimeTypeModel``. The derived type is automatically instantiated and invoked if it has a default constructor.

- ``void Work(RuntimeTypeModel model)``: Automatically invoked so that custom code can modify the ``RuntimeTypeModel``.

Editors
-------

``class PropertyEditor``: Contains the main entry point for working with property editors.

- ``public static IPropertyEditor Get(Type propertyType, ICustomAttributeProvider editedAttributes, Type[] bannedPropertyEditorTypes)``: Fetch a property editor for the given property type. For example, if propertyType is typeof(GameObject[]), an array property editor will be returned that can edit a GameObject[] array. editedAttributes contains the attributes that are applied to the given propertyType (from, for example, a field or a property). editedAttributes can be null if there are no associated attributes that should be applied. bannedPropertyEditorTypes contains a set of property editors that should *not* be returned.

``interface IPropertyEditor``: Contains the API that Full Inspector uses for interacting with property editors. You probably want to derive from `PropertyEditor<T>` instead of this.

- ``object Edit(Rect region, GUIContent label, object element)``: Edit the given object with the given label in the given rect.
- ``float GetElementHeight(GUIContent label, object element)``: Returns how tall the property editor needs to be for the given element and label.

``class CustomPropertyEditorAttribute : Attribute``: Mark a type as a property editor.

``interface IAttributePropertyEditor : IPropertyEditor``: Contains the API that Full Inspector uses for interacting with attribute-based property editors. You probably want to derive from ``AttributePropertyEditor<TElement, TAttribute>`` instead of this type.


``class AttributePropertyEditor<TElement, TAttribute> : IAttributePropertyEditor``: Provides a default implementation for the API that Full Inspector uses for interacting with attribute-based property editors.

``class CustomAttributePropertyEditorAttribute(Type attributeActivator, bool replaceOthers)``: Specify this as an attribute property editor using the given attribute type to activate the editor. If replaceOthers is true, then this attribute editor will replace the default property editor and any attribute editors that follow it.

``class CustomPropertyEditorAttribute(Type propertyType, bool inherit=true)``: Specify this as a property editor for the given property type. If inherit is true, then this property editor will also be used for derived types.

``class FullInspectorCommonSerializedObjectEditor : Editor``: Derive from this type to enable the Full Inspector editing experience. The type that is being edited needs to also extend ``ISerializedObject``.
