.. highlight:: csharp

Custom Property Editors
=======================

Full Inspector works its magic via a fully rewritten editing system inspired by ``PropertyDrawer``; however, Full Inspector continues where ``PropertyDrawer`` stops. You only need to read this section if you’re interested in writing a custom property editor.

Writing a property editor is similar to writing a custom ``PropertyDrawer``. We’ll go through how to write a ``PropertyEditor`` through a couple of real examples that are being used in Full Inspector. You can view all of the ``PropertyEditors`` in *"FullInspector2/Core/Editor/PropertyEditors/Common"*.

If you want to completely replace the editor for a component, simply write a ``PropertyEditor`` for that component type.

.. NOTE::
    This system is mirrored for ``BehaviorEditors``; simply replace ``[CustomPropertyEditor]`` with ``[CustomBehaviorEditor]`` and ``PropertyEditor`` with ``BehaviorEditor``.


.. NOTE::
    ``fiGraphMetadata`` is the new metadata system that allows you to store arbitrary metadata on any object. The graph metadata system is hiearchial; that is, metadata storage is based on the actual editing structure.

    If you try to call an ``Edit`` function or similar on an ``IPropertyEditor`` instance, you'll notice that it takes a ``fiGraphMetadataChild`` item. This is retrieved from ``metadata.Enter(...)``. It is critical that you get the value for ... correct; but the rules are simple.

    For the .. argument, if you're editing a collection, it is simply the index of that item in the collection. If you're editing a field or property, it is the name of the property. These two rules are really just guidelines; just make sure that the argument to ``Enter`` is the same for the corresponding element between ``GetElementHeight`` and ``Edit`` calls, even for varying object instances and for varying call times.

    If the ... argument is not the same across call locations and times, then the metadata engine will not work properly and you will not have a good time.


Simple (non-generic) Property Editors
-------------------------------------

Let’s look at an extremely simple case: the property editor that gets invoked for ``ints``.

.. code:: c#

    [CustomPropertyEditor(typeof(int))]
    public class IntPropertyEditor : PropertyEditor<int> {
        public override int Edit(Rect region, GUIContent label, int element, fiGraphMetadata metadata) {
            return EditorGUI.IntField(region, label, element);
        }
        public override float GetElementHeight(GUIContent label, int element, fiGraphMetadata metadata) {
            return EditorStyles.numberField.CalcHeight(label, 1000);
        }
    }

Notice that this property editor is a public type that derives from ``PropertyEditor<int>``. ``PropertyEditor<int>`` (which derives from ``IPropertyEditor``) provides a type-safe version of ``IPropertyEditor``. ``IPropertyEditor`` provides the core API that Full Inspector uses to interact with property editors.

Next notice that this type has an attribute ``[CustomPropertyEditor(typeof(int))]``; this notifies the property editing system that this type can be used to edit ints.

The ``Edit`` callback simply provides the actual Unity editing experience; we just forward the call to ``EditorGUI``; ``GetElementHeight`` returns how tall this property should be for the given label and property element.


Generic Property Editors
------------------------

The previous property editor is also writeable using a ``PropertyDrawer``. However, ``PropertyDrawer`` lacks support for generic types; let’s see how the ``PropertyEditor`` for ``Ref<>`` is written.

.. code:: c#

    [CustomPropertyEditor(typeof(Ref<>))]
    public class RefPropertyEditor<ComponentType> : PropertyEditor<Ref<ComponentType>>
        where ComponentType : Component {
        private IPropertyEditor _componentPropertyEditor = PropertyEditor.Get(typeof(ComponentType));
        public override Ref<ComponentType> Edit(Rect region, GUIContent label, Ref<ComponentType> element, fiGraphMetadata metadata) {
            ComponentType component = (ComponentType)_componentPropertyEditor.Edit(region, label, element.Value);
            return new Ref<ComponentType> {
                Value = component
            };
        }
        public override float GetElementHeight(GUIContent label, Ref<ComponentType> element, fiGraphMetadata metadata) {
            return _componentPropertyEditor.GetElementHeight(label, element.Value);
        }
    }

This property editor looks very similar to the previous non-generic one, except that it is a generic type (``class RefPropertyEditor<ComponentType>``) and its attribute references an open generic type ``Ref<>`` (``[CustomPropertyEditor(typeof(Ref<>))]``).

The only special part of generic property editors is that they have matching generic arguments for the generic property type that they edit.

Here’s another example of how to define generic property editors:

.. code:: c#

    public class Pair<T1, T2> { }
    [CustomPropertyEditor(typeof(Pair<,>))]
    public class PairPropertyEditor<T1, T2> : PropertyEditor<Pair<T1, T2>> { /*omitted*/ }

Again notice that the pattern holds.

Let’s get back to the ``RefPropertyEditor``. It doesn’t look like ``Edit`` and ``GetElementHeight`` do much except forward calls to some weird value called ``_componentPropertyEditor``. This ``_componentPropertyEditor`` is actually the property editor for the component type that the ``RefPropertyEditor`` is editing. This is one of the key patterns for writing generic property editors: we defer editing the actual generic parameters to some other property editor. More complex generic property editors (for example, the dictionary or list ones) do more work before dispatching to other property editors, but the core idea remains the same.


Inherited Property Editors
--------------------------

So you’ve gone digging through the property editors and have noticed that there is no ``ListPropertyEditor``! How does Full Inspector provide editing for ``List``, ```LinkedList```, ... types? Full Inspector also provides property editors which are inherited to their child types. So, if you look closely, there is actually an ``IListPropertyEditor`` and an ``IDictionaryPropertyEditor``. Why don’t we take a closer look at the ``IListPropertyEditor``?

.. code:: c#

    [CustomPropertyEditor(typeof(IList<>), Inherit = true)]
    public class IListPropertyEditor<TList, TData> : PropertyEditor<TList>
            /* constraints omitted */ {
        /* implementation omitted */
    }

The code and constraints behind the property editor has been omitted; they are not relevant to inherited property editors.

Notice that ``IListPropertyEditor`` takes two generic arguments, despite the fact that IList takes only one! This is because for every inherited property editor, the first generic argument is always the derived type that the property editor is editing. So for ``List<int>``, the property editor will be an instance of ``IListPropertyEditor<List<int>, int>``

Also notice that for the attribute ``[CustomPropertyEditor(typeof(IList<>), Inherit = true)]``, inherit has been set to true; by default, it is false.

Inherited property editors also work with non-generic types. The property editor for non-generic types can have either zero or one generic arguments; if it has one, it will be the actual property type the property editor is editing.


Attribute Property Editors
--------------------------

Property editors can also be activated via attributes. Let's take a look at the ``CommentAttributeEditor``.

First, we'll need to take a look at the ``CommentAttribute``. It is simple and merely stores a string comment.

.. code:: c#

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
        AttributeTargets.Class | AttributeTargets.Struct)]
    public class CommentAttribute : Attribute {
        public string Comment;

        public CommentAttribute(string comment) {
            Comment = comment;
        }
    }

Now let's take a look at the actual ``CommentAttributeEditor``.

.. code:: c#

    [CustomAttributePropertyEditor(typeof(CommentAttribute), ReplaceOthers = false)]
    public class CommentAttributeEditor<T> : AttributePropertyEditor<T, CommentAttribute> {
        protected override T Edit(Rect region, GUIContent label, T element, CommentAttribute attribute, fiGraphMetadata metadata) {
            EditorGUI.HelpBox(region, attribute.Comment, MessageType.None);
            return element;
        }

        protected override float GetElementHeight(GUIContent label, T element, CommentAttribute attribute, fiGraphMetadata metadata) {
            return CommentUtils.GetCommentHeight(attribute.Comment);
        }
    }

Notice that we declare this property editor slightly differently from other editors. First, we inherit from ``AttributePropertyEditor``, not ``PropertyEditor``. The type parameters on ``AttributePropertyEditor`` are 1st, the type being edited, and 2nd, the attribute type.

In ``CommentAttributeEditor<T>``, the generic type T is instantiated with the field/property type that the attribute has been applied to. If you inherit from, say, ``AttributePropertyEditor<int, MyCustomAttribute>``, then the property editor will only be applied for fields/properties that store ``ints``.

Let's also take a look at the attribute ``CustomAttributePropertyEditor``. The first parameter ``typeof(CommentAttribute)`` specifies the attribute that activates this editor. The second parameter, ``ReplaceOthers``, is a pretty cool feature that gives attribute property editors quite a bit more flexibility. If ``ReplaceOthers`` is true, then when the attribute property editor is encountered, it will be the final property editor applied to the field/property. Because this sample is a comment editor, we want to show the actual editor underneath the comment, so we set ``ReplaceOthers`` to ``false``.

How do we use the ``CommentAttributeEditor``? It's simple; just use the attribute above fields/properties. Here's a quick example:

.. code:: c#

    public class SomeBehavior : BaseBehavior {
        [Comment("This is some awesome comment")]
        public int MyField;
    }