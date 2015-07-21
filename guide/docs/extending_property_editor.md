# Writing a Property Editor

Full Inspector works its magic via a fully rewritten editing system inspired by PropertyDrawer; however, Full Inspector continues where `PropertyDrawer` stops. You only need to read this section if you're interested in writing a custom property editor.

Writing a `PropertyEditor` is similar to writing a custom `PropertyDrawer`. We’ll go through how to write a `PropertyEditor` through a couple of real examples that are being used in Full Inspector. You can view all of the `PropertyEditors` in "FullInspector2/Modules/Common/Editor".

If you want to completely replace the editor for a component, simply write a `PropertyEditor` for that component type.

<note>
This system is mirrored for `BehaviorEditors`; simply replace `[CustomPropertyEditor]` with `[CustomBehaviorEditor]` and `PropertyEditor` with `BehaviorEditor`.
</note>

## Simple (non-generic) Property Editors

Let’s look at an extremely simple case: the property editor that gets invoked for `ints`.

```c#
[CustomPropertyEditor(typeof(int))]
public class IntPropertyEditor : PropertyEditor<int> {
    public override int Edit(Rect region, GUIContent label, int element, fiGraphMetadata metadata) {
        return EditorGUI.IntField(region, label, element);
    }
    public override float GetElementHeight(GUIContent label, int element, fiGraphMetadata metadata) {
        return EditorStyles.numberField.CalcHeight(label, 1000);
    }
}
```

Notice that this property editor is a public type that derives from `PropertyEditor<int>`. The `int` generic type parameter allows us to override methods that return and accept a parameter of the type we expect, an `int`.

Next notice that this type has an attribute `[CustomPropertyEditor(typeof(int))]`. This attribute notifies Full Inspector that this property editor should be used for `ints`. Full Inspector will automatically discover and use all types with a `CustomPropertyEditor` annotation.

Inside of this editor, we simply delegate editing the `int` object to Unity's `EditorGUI` functions.

## Generic Property Editors

We could have pretty easily written the previous editor using a `PropertyDrawer` - but `PropertyEditors` can do so much more. `PropertyEditors` can also easily edit generic types, something that is impossible for `PropertyDrawers`.

Let's say we have a simple class, `Ref<T>`, which just stores a reference to another object:

```c#
public class Ref<T> {
    public T Value;
}
```

When we use a `Ref<T>` wrapped object in the inspector, there will be a nested `Value` field we want to eliminate. Luckily, it's pretty easy to write a generic property editor to eliminate this unnecessary field.

```
using FullInspector;
using UnityEngine;

[CustomPropertyEditor(typeof(Ref<>))]
public class RefPropertyEditor<ComponentType> : PropertyEditor<Ref<ComponentType>>
    where ComponentType : Component {
    private PropertyEditorChain _componentPropertyEditor = PropertyEditor.Get(typeof(ComponentType), null);
    public override Ref<ComponentType> Edit(Rect region, GUIContent label, Ref<ComponentType> element, fiGraphMetadata metadata) {
        ComponentType component = _componentPropertyEditor.FirstEditor.Edit(region, label, element.Value, metadata.Enter("Value"));
        return new Ref<ComponentType> {
            Value = component
        };
    }
    public override float GetElementHeight(GUIContent label, Ref<ComponentType> element, fiGraphMetadata metadata) {
        return _componentPropertyEditor.FirstEditor.GetElementHeight(label, element.Value, metadata.Enter("Value"));
    }
}
```

### The actual generic argument

So this property editor, at a high level, looks pretty similar, but there is *one* key difference - `RefPropertyEditor` is generic. What is this `ComponentType` generic parameter?

It's simple - when Full Inspector creates this property editor, it will correctly populate the property editor's generic type parameter with a matching one found inside of `Ref<T>`. For example, if we have `Ref<int>`, then a `RefPropertyEditor<int>` will be created. Similarily, if we have `Ref<GameObject>`, a `RefPropertyEditor<GameObject>` will be created instead. This pattern continues for any type `T` inside of `Ref<T>`.

This generic pattern continues for multiple generic types in the same manner. For example, if we have the type `class Pair<T1, T2> {}` then we can write the property editor for it as `class PairPropertyEditor<T1, T2> : PropertyEditor<Pair<T1, T2>>`.

### Delegating the property editor call

Getting back to `RefPropertyEditor`, how in the world should we provide an inspector for a generic type `ComponentType`? It seems impossible! But fear not - it is easy in Full Inspector.

We will retrieve another property editor to use instead using `PropertyEditor.Get(typeof(ComponentType), null)`. This returns a `PropertyEditorChain` instance, which is simply a sequence of property editors which can be used to edit the given type. There will always be at least one editor in this chain, but sometimes there is more than one (for example, if there is a user-defined editor).

To actually retreive an `IPropertyEditor` to do the editing, we use `PropertyEditorChain.FirstEditor`. We then delegate the implementation to the editors `Edit`/`GetElementHeight` methods.

### Metadata

There's one last important concept going on in this property editor - and that is the metadata system. The metadata system provides external storage on top of the edited object. One example here is if a foldout is active or inactive. The metadata system can store either persistent or temporary data.

The metadata system is conceptually a tree (*not* a graph - there are no cycles) that mirrors the edited object structure. Using it is pretty simple. If you're editing a property named "Foo" and calling into a child property editor, simple invoke `metadata.Enter("Foo")`. If the property is named "Okay", then call `metadata.Enter("Okay")`. If you're editing an array/collection, then you can also pass in the array/collection indicies into the metadata.

For the `Enter` argument, these name/index rules are really just guidelines; just make sure that the argument to `Enter` is the same for the corresponding element between `GetElementHeight` and `Edit` calls, even for varying object instances and for varying call times.

It's important to get the metadata calls correct, otherwise things will go wrong. For example, the graph metadata system is used to significantly enhance the performance of FI - for example, if you call `GetElementHeight` from inside an `Edit` invocation, then you'll get a cached value instead of redoing the entire computation. If the `Enter` argument varies between the `GetElementHeight` and `Edit` calls, then the heights of items within the inspector will get messed up.

#### Adding new temporary metadata

Temporary metadata means that the metadata is lost between editor serialization reloads (such as entering/exiting play mode or compiling scripts) - however, it *is* persistent across editor frames.

It's really easy to add new temporary metadata. Simply derive from `IGraphMetadataItemNotPersistent`:

```c#
public class fiAnimationMetadata : IGraphMetadataItemNotPersistent {
    // your metadata
}
```

#### Adding new Persistent Metadata

Adding new persistent metadata is somewhat painful because it integrates directly with Unity's serializer which does not support inheritance. Here is how the dropdown metadata is implemented:

```c#
// The actual metadata class
[Serializable]
public class fiDropdownMetadata : IGraphMetadataItemPersistent {
    // ... actual data - this is serialized using Unity's serializer and nothing else (due to a low latency requirement)
}

// boilerplate to integrate with Unity below:

// To serialize the graph metadata
[Serializable] public class fiDropdownGraphMetadataSerializer : fiGraphMetadataSerializer<fiDropdownMetadata> { }

// A component for Unity to store the data within
[AddComponentMenu("")]
public class fiDropdownMetadataStorageComponent : fiBaseStorageComponent<fiDropdownGraphMetadataSerializer> { }

// To provide the presistent metadata system information about our types
public class fiDropdownMetadataProvider : fiPersistentEditorStorageMetadataProvider<fiDropdownMetadata, fiDropdownGraphMetadataSerializer> { }
```

## Inherited Property Editors

So you’ve gone digging through the property editors and have noticed that there is no `ListPropertyEditor`! How does Full Inspector provide editing for `List`, `LinkedList`, etc types?

It's simple: Full Inspector allows property editors to be inherited to child types. So, if you look closely, there is actually an `IListPropertyEditor` and an `IDictionaryPropertyEditor`. Why don’t we take a closer look at the `IListPropertyEditor`?

```c#
[CustomPropertyEditor(typeof(IList<>), Inherit = true)]
public class IListPropertyEditor<TList, TData> : PropertyEditor<TList>
        /* constraints omitted */ {
    /* implementation omitted */
}
```

We've omitted lots of details here which are not relevant.

Notice that `IListPropertyEditor` takes two generic arguments, despite the fact that `IList` takes only one! This is because property editors can optionally specifically an additional generic parameter that will be replaced with the derived type that the property editor is editing (this additional generic parameter isn't very useful in non-inherited editors, so it was omitted in the prior examples). So for `List<int>`, the property editor will be an instance of `IListPropertyEditor<List<int>, int>`.

It's also important to notice that for the attribute `[CustomPropertyEditor(typeof(IList<>), Inherit = true)]` the field `Inherit` has been set to true.

<note>For code-sharing reasons, `IListPropertyEditor` actually derives from `BaseCollectionPropertyEditor`, but we'll ignore that here.</note>

## Attribute Property Editors

Feel free to reference [the old docs](http://jacobdufault.github.io/fullinspector/assets/docs/custom_property_editors.html#attribute-property-editors) for now regarding attribute property editors, but the next major update (2.7) will rewrite how attribute property editors work so the docs have not been migrated to the new system. The new attribute editing system should be sigificantly more flexible and powerful.
