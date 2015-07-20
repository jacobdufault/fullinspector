# Embedding a Full Inspector Editor

It's really easy to embed a Full Inspector editor in any GUI logic, whether it be an `Editor`, `EditorWindow`, or `PropertyDrawer`.

Let's say we have these three classes:

```c#
using System;
using System.Collections.Generic;
using FullInspector;
using UnityEngine;

[Serializable]
public class MyType : BaseObject {
    public Dictionary<int, int> dict;
}

[Serializable]
public class MyTypeContainer {
    public MyType type;
}

public class MyTypeContainerBehaviour : MonoBehaviour {
    public MyType type;
}
```

### EditorWindow

Here's an example of how we use FI in an `EditorWindow`.

```c#
using FullInspector;
using FullInspector.Internal;
using UnityEngine;
using UnityEditor;

public class EmbedDemo : EditorWindow {
    [MenuItem("Test/Embedded FI Editor")]
    private static void Create() {
        GetWindow<EmbedDemo>();
    }

    [SerializeField]
    private MyType instance;
    private fiGraphMetadata metadata = new fiGraphMetadata();

    protected void OnEnable() {
        // This will make animation smooth.
        fiEditorUtility.RepaintableEditorWindows.Add(this);
    }

    protected void OnGUI() {
        var editor = PropertyEditor.Get(typeof(MyType), null);

        // For convenience, FI defines an `EditWithGUILayout` method that just wraps
        //  the calls to `GetElementHeight` and `Edit`.
        instance = editor.FirstEditor.EditWithGUILayout(null, instance, metadata.Enter("Hi"));
    }
}
```

### Editor

Using a custom `Editor` is also easy:
```c#
using FullInspector;
using FullInspector.Internal;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MyTypeContainerBehaviour))]
public class EmbedEditor : Editor {
    public override void OnInspectorGUI() {
        var editor = PropertyEditor.Get(typeof(MyType), null);

        var instance = ((MyTypeContainerBehaviour)target).type;
        instance = editor.FirstEditor.EditWithGUILayout(new GUIContent("Type"), instance, fiPersistentMetadata.GetMetadataFor(target).Enter("type"));
        ((MyTypeContainerBehaviour)target).type = instance;
    }
}
```

### PropertyDrawers

`PropertyDrawers` might be a bit tricky because FI requires the actual object instance - luckily, FI defines some helper methods that make this really easy.

<important>Be careful of writing PropertyDrawers that embed FI, as FI will automatically generate a PropertyEditor->PropertyDrawer binding which can lead you do infinite recursion. This would have happened if we defined the `PropertyDrawer` for `MyType` instead of `MyTypeContainer`</important>

```c#
using FullInspector;
using FullInspector.Internal;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MyTypeContainer))]
public class EmbedPropertyDrawer : PropertyDrawer {
    private PropertyEditorChain _editor = PropertyEditor.Get(typeof(MyType), null);
    private fiGraphMetadata _metadata = new fiGraphMetadata();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        var propertyValue = fiSerializedPropertyUtility.GetTarget(property);
        propertyValue = _editor.FirstEditor.Edit(position, label, propertyValue, _metadata.Enter(property.propertyPath));
        fiSerializedPropertyUtility.WriteTarget(property, propertyValue);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        var propertyValue = fiSerializedPropertyUtility.GetTarget(property);
        return _editor.FirstEditor.GetElementHeight(label, propertyValue, _metadata.Enter(property.propertyPath));
    }
}
```

### Hierarchy Mode

You may also want to push and/or pop hierarchy mode, which determines if the foldout/dropdown arrows will be indented.

```c#
fiEditorGUI.PushHierarchyMode(true);
// code
fiEditorGUI.PopHierarchyMode();
```

which is equivalent to doing

```c#
bool savedHierarchyMode = EditorGUIUtility.hierarchyMode;
EditorGUIUtility.hierarchyMode = true;
// code
EditorGUIUtility.hierarchyMode = savedHierarchyMode;
```
