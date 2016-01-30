# Inline Object Editing

Full Inspector will automatically display an inline object editor for you when you have a `UnityEngine.Object` reference. This makes it easy to quickly view the current value on a reference or make a quick edit - all without making a context shift or changing the object you're inspecting.

<note>
The inline object editor requires that the object type have `BehaviorEditor` - all `BaseBehavior` and `BaseScriptableObject` types do, as well as `Transform`
</note>

## Demo

Inline object editing just works. It's like magic:

![](docs/images/inline_object_editing.gif)

Here's the `InlineObjectEditing` script:

```c#
using System.Collections.Generic;
using FullInspector;
using UnityEngine;

public class InlineObjectEditing : BaseBehavior {
    public Transform MyTransform;
    public List<ExternalObject> MoreReferences;
}

```

and here's the `ExternalObject` script:

```c#
using System.Collections.Generic;
using FullInspector;

public class ExternalObject : BaseBehavior {
    public float MyFloat;
    public Dictionary<string, string> MyDictionary;
}
```