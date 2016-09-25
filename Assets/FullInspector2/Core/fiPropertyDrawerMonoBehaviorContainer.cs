using UnityEngine;

namespace FullInspector.Internal {
    /// <summary>
    /// Proxy container for the property drawer editor so that we can invoke the
    /// PropertyDrawer EditorGUI method on Item.
    /// </summary>
    public class fiPropertyDrawerMonoBehaviorContainer<T> : MonoBehaviour {
        public T Item;
    }
}