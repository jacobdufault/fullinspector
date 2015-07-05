# Writing a Property Editor

// TODO

One of the updated samples:
```
using FullInspector;
using UnityEngine;

public class Ref<T> {
    public T Value;
}

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
