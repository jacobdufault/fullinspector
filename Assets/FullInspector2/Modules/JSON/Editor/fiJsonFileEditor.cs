using FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using tk = FullInspector.tk<FullSerializer.fsData, FullInspector.tkDefaultContext>;
using System;
using System.Reflection;

namespace FullInspector.Internal {
    // TODO: This is disabled as it currently breaks TextAsset handling for, ie, text files
    // [CustomEditor(typeof(TextAsset))]
    public class fiJsonFileEditor : Editor {
        private static bool IsJsonAsset(UnityObject asset) {
            string path = AssetDatabase.GetAssetPath(asset);
            if (string.IsNullOrEmpty(path)) return false;
            return path.EndsWith(".json");
        }

        public override void OnInspectorGUI() {
            TextAsset asset = (TextAsset)target;

            if (IsJsonAsset(asset)) {
                fsData data;
                fsResult result = fsJsonParser.Parse(asset.text, out data);
                if (result.Succeeded) {
                    PropertyEditor.Get(typeof(fsData), null)
                        .FirstEditor.EditWithGUILayout(GUIContent.none, data, new fiGraphMetadataChild {
                            Metadata = fiPersistentMetadata.GetMetadataFor(target)
                        });

                    return;
                }
            }

            base.OnInspectorGUI();
        }
    }

    [CustomPropertyEditor(typeof(fsData))]
    public class fsDataPropertyEditor : tkControlPropertyEditor<fsData> {
        public fsDataPropertyEditor(Type dataType, ICustomAttributeProvider attrs) : base(dataType) { }

        private static readonly tkControlEditor Editor = new tkControlEditor(
            new tk.StyleProxy {
                Style = new tk.ReadOnly(),
                Control =
                    new tk.VerticalGroup {
                    new tk.ShowIf(data => data.IsBool,
                        tk.PropertyEditor.Create(fiGUIContent.Empty, (data, context) => data.AsBool)),

                    new tk.ShowIf(data => data.IsDouble,
                        tk.PropertyEditor.Create(fiGUIContent.Empty, (data, context) => data.AsDouble)),
                    new tk.ShowIf(data => data.IsInt64,
                        tk.PropertyEditor.Create(fiGUIContent.Empty, (data, context) => data.AsInt64)),

                    new tk.ShowIf(data => data.IsDictionary,
                        tk.PropertyEditor.Create(fiGUIContent.Empty, (data, context) => data.AsDictionary)),

                    new tk.ShowIf(data => data.IsList,
                        tk.PropertyEditor.Create(fiGUIContent.Empty, (data, context) => data.AsList)),
                    }
            }
        );

        protected override tkControlEditor GetControlEditor(GUIContent label, fsData element, fiGraphMetadata graphMetadata) {
            graphMetadata.GetPersistentMetadata<fiDropdownMetadata>().ForceDisable();
            return Editor;
        }
    }
}

// The following is an alternative editor approach. Saved for investigation later. Probably very useful
// if we want to try and write out an actual editor so the content can be edited.
/*
public abstract class JsonValue {
    public static JsonValue Create(fsData data) {
        switch (data.Type) {
            case fsDataType.Array: {
                    var values = new List<JsonValue>();
                    foreach (var item in data.AsList) {
                        values.Add(Create(item));
                    }
                    return new JsonList { Value = values };
                }

            case fsDataType.Boolean:
                return new JsonBool { Value = data.AsBool };

            case fsDataType.Double:
                return new JsonDouble { Value = data.AsDouble };

            case fsDataType.Int64:
                return new JsonInt64 { Value = data.AsInt64 };

            case fsDataType.Null:
                return new JsonNull();

            case fsDataType.Object: {
                    var values = new Dictionary<string, JsonValue>();
                    foreach (var entry in data.AsDictionary) {
                        values[entry.Key] = Create(entry.Value);
                    }
                    return new JsonObject { Value = values };
                }

            case fsDataType.String:
                return new JsonString { Value = data.AsString };
        }

        throw new InvalidOperationException();
    }
}

public class JsonObject : JsonValue {
    public Dictionary<string, JsonValue> Value;
}
public class JsonList : JsonValue {
    public List<JsonValue> Value;
}
public class JsonNull : JsonValue {
}
public class JsonString : JsonValue {
    public string Value;
}
public class JsonDouble : JsonValue {
    public double Value;
}
public class JsonInt64 : JsonValue {
    public Int64 Value;
}
public class JsonBool : JsonValue {
    public bool Value;
}
*/