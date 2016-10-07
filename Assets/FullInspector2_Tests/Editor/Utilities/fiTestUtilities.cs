using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Tests {
    public class fiTestUtilities {
        public static object Serialize(UnityObject target) {
            EditorUtility.SetDirty(target);
            return null;
        }

        public static T DrawPropertyEditor<T>(T obj, fiGraphMetadata metadata) {
            var label = GUIContent.none;

            IPropertyEditor editor = PropertyEditor.Get(typeof(T), null).FirstEditor;

            var height = editor.GetElementHeight(label, obj, metadata.Enter("Root"));
            Rect rect = new Rect(0, 0, 500, height);

            var serializedObj = obj as ISerializationCallbackReceiver;
            if (serializedObj != null) {
                serializedObj.OnBeforeSerialize();
                serializedObj.OnAfterDeserialize();
            }

            obj = editor.Edit(rect, label, obj, metadata.Enter("Root"));

            if (serializedObj != null)
                serializedObj.OnBeforeSerialize();

            return obj;
        }
    }
}