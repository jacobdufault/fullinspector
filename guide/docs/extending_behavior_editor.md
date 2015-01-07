# Writing a Behavior Editor

// TODO

`BehaviorEditors` are very similar to Unity's `Editor`, except that they are quite a bit more powerful.

### Why does there need to be a separation between BehaviorEditors and PropertyEditors?

One of the advanced features that Full Inspector supports, inline object editing, requires that the edited object have a `BehaviorEditor` to avoid infinite recursion loops.

## Registering Behavior Editors

Full Inspector discovers `BehaviorEditors` by scanning for types with a `[CustomBehaviorEditor]` attribute. This attribute provides a little bit of required metadata for Full Inspector - namely, the type that the editor applies to.

For example, if we have a `BaseBehavior` or `BaseScriptableObject` type called `MyObj`, ie,

```c#
using FullInspector;

public class MyObj : BaseBehavior { /* ... */ }
```

then we can create a `BehaviorEditor` for it by adding this code to an Editor folder:

```c#
using FullInspector;

[CustomBehaviorEditor(typeof(MyObj))]
public class MyObjEditor : BehaviorEditor<MyObj> { /* ... */ }
```

There are different base classes you can derive from - `BehaviorEditor<TBehavior>` if you want to completely rewrite the editing logic, or `DefaultBehaviorEditor<TBehavior>` if you just want to add a completely custom editor section before/after the default editor.

`[CustomBehaviorEditor]` optionally takes an additional keyword argument - `Inherit`. This specifies if the editor should be used for derived types. By default, it is true.

## Adding Editor Logic Before or After the Default Editor

It's really easy to add a section before or after the default editor.

Let's say we want to add some help information before and after this behavior:

```c#
using FullInspector;

public class MyBehavior : BaseBehavior<FullSerializerSerializer> {
    public int fromDefaultEditor1, fromDefaultEditor2, fromDefaultEditor3;
}
```

We can do this easily by deriving from `DefaultBehaviorEditor<ExtendedDefaultEditor>`.

```c#
using FullInspector;
using UnityEditor;
using UnityEngine;

[CustomBehaviorEditor(typeof(MyBehavior))]
public class MyBehaviorEditor : DefaultBehaviorEditor<MyBehavior> {
    protected override void OnBeforeEdit(Rect rect, MyBehavior behavior,
        fiGraphMetadata metadata) {

        rect.height -= 3; // margin
        EditorGUI.HelpBox(rect, "Hello, this is a custom before section",
            MessageType.Info);
    }

    protected override float OnBeforeEditHeight(ExtendedDefaultEditor behavior,
        fiGraphMetadata metadata) {

        return 30;
    }

    protected override void OnAfterEdit(Rect rect, MyBehavior behavior,
        fiGraphMetadata metadata) {

        // margin
        rect.y += 3;
        rect.height -= 3;
            
        EditorGUI.HelpBox(rect, "Hello, this is a custom after section",
            MessageType.Info);
    }

    protected override float OnAfterEditHeight(MyBehavior behavior,
        fiGraphMetadata metadata) {

        return 30;
    }
}
```

If you only wanted to run editor code before the default editor, then you only need to override `OnBeforeHeight/OnBeforeEditHeight`, and similarily `OnAfterEdit/OnAfterEditHeight` for running editor code after the default editor.

<note>
`DefaultBehaviorEditor<T>` is not included in versions before or at 2.5.0. [Here](https://gist.github.com/jacobdufault/433eae19d25ee2b15887) is the source code for it. Just place the file into an Editor folder.
</note>

## Calling the Default Editor

If you need something even more sophisticated than just adding a section before or after the default behavior editor, then it is still extremely easy.

You can easily call into the default behavior editor by using `DefaultBehaviorEditor.Edit/GetHeight`. For example, here is the boilerplate needed to begin writing your own custom editor that (somehow) uses the default editor:

```c#
using FullInspector;
using UnityEngine;

public class MyBehavior : BaseBehavior {
}

[CustomBehaviorEditor(typeof(MyBehavior))]
public class MyBehaviorEditor : BehaviorEditor<MyBehavior> {
    protected override void OnEdit(Rect rect, MyBehavior behavior,
        fiGraphMetadata metadata) {

        DefaultBehaviorEditor.Edit(rect, behavior, metadata);
    }

    protected override float OnGetHeight(MyBehavior behavior,
        fiGraphMetadata metadata) {

        return DefaultBehaviorEditor.GetHeight(behavior, metadata);
    }

    protected override void OnSceneGUI(MyBehavior behavior) {
    }
}
```