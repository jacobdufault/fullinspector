using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FullInspector.Internal {
    [CustomPropertyEditor(typeof(LayerMask))]
    public class LayerMaskEditor : PropertyEditor<LayerMask> {
        private static string[] layers;

        public static LayerMask LayerMaskField(Rect region, GUIContent label, LayerMask selected) {
            // NOTE: Sign bit is treated as "everything" by Unity, so it cannot be a layer. That's why we
            // iterate to 31, *not* 32.

            // Cache set of layers that are being used.
            if (layers == null) {
                var layerNames = new List<string>();
                for (int i = 0; i < 32; ++i) {
                    string layerName = LayerMask.LayerToName(i);
                    if (string.IsNullOrEmpty(layerName) == false)
                        layerNames.Add(layerName);
                }
                layers = layerNames.ToArray();
            }

            // Map the current LayerMask value from its LayerMask assosciation into our assosciation for layers.
            int mappedValue = 0;
            for (int i = 0; i < 32; ++i) {
                int layerId = selected.value & (1 << i);
                if (layerId != 0) {
                    string layerName = LayerMask.LayerToName(i);
                    mappedValue |= (1 << Array.FindIndex(layers, t => t == layerName));
                }
            }

            int mask = EditorGUI.MaskField(region, label, mappedValue, layers);

            if (mask == -1)
                return new LayerMask { value = -1 };

            // NOTE: Only go up to 31. We ignore the sign bit.
            // NOTE: We can avoid GC by allocating and pooling arrays of the required size. We could
            //       run a population count on mask to figure out the array size to use.
            var setNames = new List<string>();
            for (int i = 0; i < 31; ++i) {
                if ((mask & (1 << i)) != 0) {
                    setNames.Add(layers[i]);
                }
            }

            return LayerMask.GetMask(setNames.ToArray());
        }

        public override LayerMask Edit(Rect region, GUIContent label, LayerMask element, fiGraphMetadata metadata) {
            return LayerMaskField(region, label, element);
        }

        public override float GetElementHeight(GUIContent label, LayerMask element, fiGraphMetadata metadata) {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}