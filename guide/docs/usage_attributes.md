# Attribute Customization

You can easily customize Full Inspector with many attributes.

Many of the attributes also take parameters. If the parameter is required, then it will be required when you specify the atttribute, for example, `[InspectorMargin(10)]`. Optional parameters are specified using named parameters like `[InspectorComment("Hi!", Type=CommentType.Info)]`.

Each of these attributes actually ends in "Attribute", for example, the class name of `[InspectorHeader]` is `InspectorHeaderAttribute`.

<note>
Not finding what you're looking for or have something pretty specialized? Try the [toolkit](#docs/extending_toolkit) - it's a lot more powerful and just as fast once you're used to it.
</note>

<hr />

## Organization

These attributes let you control the overall layout or style of the editor.

Name  | Description
:- | :-
`[InspectorButton]` | Display the given method as a button in the inspector. You  can customize the order of that the button is displayed in w.r.t. fields or properties by using [InspectorOrder].
`[InspectorCategory]` | Display this field, property, or method inside of the given tab group / category within the inspector. Each member can be part of multiple categories - simply apply this attribute multiple times.
`[InspectorCollapsedFoldout]` | Forces the given field or property to be collapsed in the inspector.
`[InspectorDivider]` | Draws a divider (horizontal line) above the given field or property.
`[InspectorHeader]` | Draws a header above a property, with some nice text to go along with it. This is an analog to Unity's [Header] attribute.
`[InspectorHideIf]` | This allows a member to be conditionally hidden in the inspector depending upon the state of other variables in object. This does *not* change serialization behavior, only display behavior.
`[InspectorIndent]` | Indents the given editor. This can be useful combined with [InspectorHeader] to draw an indented region in the inspector.
`[InspectorMargin]` | Adds whitespace above the given field or property.
`[InspectorName]` | Override the default name that is used for display in the inspector and use a custom name instead.
`[InspectorOrder]` | Set the display order of an field or property of an object. A field or property without an [InspectorOrder] attribute defaults to order double.MaxValue (which will appear after any ordered properties). The lower the order value, the higher the field or property will appear in the inspector. Each inheritance level receives its own order group.
`[InspectorShowIf]` | This allows a member to be conditionally hidden in the inspector depending upon the state of other variables in object. This does *not* change serialization behavior, only display behavior.
`[InspectorTooltip]` | Adds a tooltip to an field or property that is viewable in the inspector.
`[ShowInInspector]` | The ShowInInspectorAttribute causes the given field or property to be shown in the inspector, even if it is not public. This is the inverse of Unity's [HideInInspector] attribute.

<hr />

## Metadata

Want to add some extra information to the inspector? Look here first.

Name  | Description
:- | :-
`[InspectorComment]` | Add a comment above the given field or property.
`[InspectorDropdownName]` | Annotating a type with this attribute allows you to specify what name it will appear with inside of the abstract type selection dropdown.
`[InspectorTextArea]` | Show a text-area instead of a text-field for a string.

<hr />

## Rendering Style

When you need to modify how an item is rendered.

Name  | Description
:- | :-
`[InspectorDisabled]` | Draws the regular property editor but with a disabled GUI. With the current implementation this is not compatible with other attribute editors.
`[InspectorHidePrimary]` | Do not display the primary inspector. Only attribute property editors will be shown for the given field or property.
`[InspectorNotDefaultConstructed]` | This will prevent Full Inspector from constructing an object instance in the inspector by default.
`[InspectorNullable]` | This will cause Full Inspector to treat the given target class as a nullable property, ie, it does not have to have an instance allocated. If you're using a struct, just mark the type nullable with ?, ie, obj?, and the nullable editor will automatically be used.
`[InspectorRange]` | Keep a numeric value within the given min/max range, with an optional step.
`[InspectorSkipInheritance]` | Prevent the drop-down type selection editor from being shown. This is especially useful for fields of type object.

<hr />

## Collections

Completely customize the appearance of your collection.

Name  | Description
:- | :-
`[InspectorCollectionPager]` | Enables customization of how the pager interface on collections is activated. The pager is used to show a subset of the current collection.
`[InspectorCollectionRotorzFlags]` | Specify the rotorz flags for a collection.
`[InspectorCollectionShowItemDropdown]` | Use this if you wish for each item inside of a collection to have a dropdown arrow. This is disabled by default as it can cause multiple dropdown arrows to be shown next to each-other in certain scenarios.
`[InspectorDatabaseEditor]` | Changes the default editor for IList{T} types to be one that only edits a single item in the collection at a single time. This can be extremely useful, if, for example, you're editing an extremely large list or just want to reduce information overload.
`[InspectorKeyWidth]` | Allows the width of a KeyValuePair to be modified. If you wish to use this inside of a collection/dictionary, please see InspectorCollectionItemAttributes to activate it.
`[InspectorCollectionAddItemAttributes]` | Allows you to customize the `Add` item in, say, a Dictionary or a HashSet. This is analogous to InspectorCollectionItemAttributes so please see the documentation on that class for usage instructions.

`[InspectorCollectionItemAttributes]`:

You can use this interface to customize how rendering of items inside of an collection is done. Usage is slightly unintuitive because C# annotations are not very expressive. Let's say we want to display a comment above every field inside of the list. Here's how we can do it:

```c#
class ObjectsItemAttrs : fiICollectionAttributeProvider {
    public IEnumerable<object> GetAttributes() {
        yield return new InspectorCommentAttribute("Hi!");
    }
}

[InspectorCollectionItemAttributes(typeof(ObjectsItemAttrs))]
public List<object> Objects;
```

Whereas if we were displaying the same thing normally (without the comment) it would be a simple:

```c#
public List<object> Objects;
```

There's quite a bit of boilerplate, but it enables this powerful customization.
