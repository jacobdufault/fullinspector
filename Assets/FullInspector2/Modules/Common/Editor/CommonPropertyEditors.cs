using System;
using UnityEditor;
using UnityEngine;

// This file contains PropertyEditor definitions for common types.

namespace FullInspector.Modules {
    /// <summary>
    /// Provides an Edit field that uses a string so that types of varying bit width (greater than
    /// 32 bits, which is what IntField/FloatField is limited to) can be represented properly in the
    /// editor without losing data.
    /// </summary>
    internal static class NumericTypeEditorHelper {
        /// <summary>
        /// Edit the given type using a TextField. Convert.ChangeType will be used to convert the
        /// given type T to and from a string.
        /// </summary>
        public static T Edit<T>(Rect region, GUIContent label, T instance) {
            var asString = (string)Convert.ChangeType(instance, typeof(string));
            var updatedString = EditorGUI.TextField(region, label, asString);
            try {
                return (T)Convert.ChangeType(updatedString, typeof(T));
            }
            catch {
                return instance;
            }
        }
    }

    [CustomPropertyEditor(typeof(bool))]
    public class BoolPropertyEditor : PropertyEditor<bool> {
        public override bool Edit(Rect region, GUIContent label, bool element, fiGraphMetadata metadata) {
            return EditorGUI.Toggle(region, label, element);
        }
        public override float GetElementHeight(GUIContent label, bool element, fiGraphMetadata metadata) {
            return EditorStyles.toggle.CalcHeight(label, 1000);
        }
    }
    [CustomPropertyEditor(typeof(byte))]
    public class BytePropertyEditor : PropertyEditor<byte> {
        public override byte Edit(Rect region, GUIContent label, byte element, fiGraphMetadata metadata) {
            return (byte)EditorGUI.IntField(region, label, element);
        }
        public override float GetElementHeight(GUIContent label, byte element, fiGraphMetadata metadata) {
            return EditorStyles.numberField.CalcHeight(label, 1000);
        }
    }
    [CustomPropertyEditor(typeof(sbyte))]
    public class SBytePropertyEditor : PropertyEditor<sbyte> {
        public override sbyte Edit(Rect region, GUIContent label, sbyte element, fiGraphMetadata metadata) {
            return (sbyte)EditorGUI.IntField(region, label, element);
        }
        public override float GetElementHeight(GUIContent label, sbyte element, fiGraphMetadata metadata) {
            return EditorStyles.numberField.CalcHeight(label, 1000);
        }
    }
    [CustomPropertyEditor(typeof(short))]
    public class ShortPropertyEditor : PropertyEditor<short> {
        public override short Edit(Rect region, GUIContent label, short element, fiGraphMetadata metadata) {
            return (short)EditorGUI.IntField(region, label, element);
        }
        public override float GetElementHeight(GUIContent label, short element, fiGraphMetadata metadata) {
            return EditorStyles.numberField.CalcHeight(label, 1000);
        }
    }
    [CustomPropertyEditor(typeof(ushort))]
    public class UShortPropertyEditor : PropertyEditor<ushort> {
        public override ushort Edit(Rect region, GUIContent label, ushort element, fiGraphMetadata metadata) {
            return (ushort)EditorGUI.IntField(region, label, element);
        }
        public override float GetElementHeight(GUIContent label, ushort element, fiGraphMetadata metadata) {
            return EditorStyles.numberField.CalcHeight(label, 1000);
        }
    }
    [CustomPropertyEditor(typeof(char))]
    public class CharPropertyEditor : PropertyEditor<char> {
        public override char Edit(Rect region, GUIContent label, char element, fiGraphMetadata metadata) {
            var updated = EditorGUI.TextField(region, label, element.ToString());
            if (updated.Length == 0)
                return element;
            return updated[0];
        }
        public override float GetElementHeight(GUIContent label, char element, fiGraphMetadata metadata) {
            return EditorStyles.textField.CalcHeight(label, 1000);
        }
    }

    [CustomPropertyEditor(typeof(int))]
    public class IntPropertyEditor : PropertyEditor<int> {
        public override int Edit(Rect region, GUIContent label, int element, fiGraphMetadata metadata) {
            return EditorGUI.IntField(region, label, element);
        }
        public override float GetElementHeight(GUIContent label, int element, fiGraphMetadata metadata) {
            return EditorStyles.numberField.CalcHeight(label, 1000);
        }
    }
    [CustomPropertyEditor(typeof(uint))]
    public class UIntPropertyEditor : PropertyEditor<uint> {
        public override uint Edit(Rect region, GUIContent label, uint element, fiGraphMetadata metadata) {
            return NumericTypeEditorHelper.Edit(region, label, element);
        }
        public override float GetElementHeight(GUIContent label, uint element, fiGraphMetadata metadata) {
            return EditorStyles.numberField.CalcHeight(label, 1000);
        }
    }
    [CustomPropertyEditor(typeof(long))]
    public class LongPropertyEditor : PropertyEditor<long> {
        public override long Edit(Rect region, GUIContent label, long element, fiGraphMetadata metadata) {
            return NumericTypeEditorHelper.Edit(region, label, element);
        }
        public override float GetElementHeight(GUIContent label, long element, fiGraphMetadata metadata) {
            return EditorStyles.numberField.CalcHeight(label, 1000);
        }
    }
    [CustomPropertyEditor(typeof(ulong))]
    public class ULongPropertyEditor : PropertyEditor<ulong> {
        public override ulong Edit(Rect region, GUIContent label, ulong element, fiGraphMetadata metadata) {
            return NumericTypeEditorHelper.Edit(region, label, element);
        }
        public override float GetElementHeight(GUIContent label, ulong element, fiGraphMetadata metadata) {
            return EditorStyles.numberField.CalcHeight(label, 1000);
        }
    }
    [CustomPropertyEditor(typeof(string))]
    public class StringPropertyEditor : PropertyEditor<string> {
        public override string Edit(Rect region, GUIContent label, string element, fiGraphMetadata metadata) {
            return EditorGUI.TextField(region, label, element);
        }

        public override float GetElementHeight(GUIContent label, string element, fiGraphMetadata metadata) {
            return EditorStyles.textField.CalcHeight(label, 1000);
        }
    }

    [CustomPropertyEditor(typeof(float))]
    public class FloatPropertyEditor : PropertyEditor<float> {
        public override float Edit(Rect region, GUIContent label, float element, fiGraphMetadata metadata) {
            return EditorGUI.FloatField(region, label, element);
        }

        public override float GetElementHeight(GUIContent label, float element, fiGraphMetadata metadata) {
            return EditorStyles.numberField.CalcHeight(label, 1000);
        }
    }

    [CustomPropertyEditor(typeof(double))]
    public class DoublePropertyEditor : PropertyEditor<double> {
        public override double Edit(Rect region, GUIContent label, double element, fiGraphMetadata metadata) {
            return NumericTypeEditorHelper.Edit(region, label, element);
        }

        public override float GetElementHeight(GUIContent label, double element, fiGraphMetadata metadata) {
            return EditorStyles.numberField.CalcHeight(label, 1000);
        }
    }

    [CustomPropertyEditor(typeof(decimal))]
    public class DecimalPropertyEditor : PropertyEditor<decimal> {
        public override decimal Edit(Rect region, GUIContent label, decimal element, fiGraphMetadata metadata) {
            return NumericTypeEditorHelper.Edit(region, label, element);
        }

        public override float GetElementHeight(GUIContent label, decimal element, fiGraphMetadata metadata) {
            return EditorStyles.numberField.CalcHeight(label, 1000);
        }
    }

    [CustomPropertyEditor(typeof(Vector2))]
    public class Vector2PropertyEditor : PropertyEditor<Vector2> {
        public override Vector2 Edit(Rect region, GUIContent label, Vector2 element, fiGraphMetadata metadata) {
            return EditorGUI.Vector2Field(region, label, element);
        }

        public override float GetElementHeight(GUIContent label, Vector2 element, fiGraphMetadata metadata) {
            if (EditorGUIUtility.wideMode == false) {
                return EditorStyles.label.CalcHeight(GUIContent.none, 1000) +
                    EditorStyles.numberField.CalcHeight(GUIContent.none, 1000);
            }

            return EditorStyles.numberField.CalcHeight(GUIContent.none, 1000);
        }
    }
    [CustomPropertyEditor(typeof(Vector3))]
    public class Vector3PropertyEditor : PropertyEditor<Vector3> {
        public override Vector3 Edit(Rect region, GUIContent label, Vector3 element, fiGraphMetadata metadata) {
            return EditorGUI.Vector3Field(region, label, element);
        }

        public override float GetElementHeight(GUIContent label, Vector3 element, fiGraphMetadata metadata) {
            if (EditorGUIUtility.wideMode == false) {
                return EditorStyles.label.CalcHeight(GUIContent.none, 1000) +
                    EditorStyles.numberField.CalcHeight(GUIContent.none, 1000);
            }

            return EditorStyles.numberField.CalcHeight(GUIContent.none, 1000);
        }
    }

    [CustomPropertyEditor(typeof(Vector4))]
    public class Vector4PropertyEditor : PropertyEditor<Vector4> {
        public override Vector4 Edit(Rect region, GUIContent label, Vector4 element, fiGraphMetadata metadata) {
            // Unity doesn't have a GUIContent override for Vector4 fields...
            return EditorGUI.Vector4Field(region, label.text, element);
        }

        public override float GetElementHeight(GUIContent label, Vector4 element, fiGraphMetadata metadata) {
            // Even if the label is empty, the Vector4Field will refuse to display one just one
            // line.
            return EditorStyles.numberField.CalcHeight(GUIContent.none, 1000) +
                EditorStyles.label.CalcHeight(label, 1000);
        }
    }

    [CustomPropertyEditor(typeof(Bounds))]
    public class BoundsPropertyEditor : PropertyEditor<Bounds> {

        public override Bounds Edit(Rect region, GUIContent label, Bounds element, fiGraphMetadata metadata) {
            return EditorGUI.BoundsField(region, label, element);
        }

        public override float GetElementHeight(GUIContent label, Bounds element, fiGraphMetadata metadata) {
            if (string.IsNullOrEmpty(label.text)) {
                return EditorStyles.label.CalcHeight(label, 1000) * 2;
            }
            return EditorStyles.label.CalcHeight(label, 1000) * 3;
        }
    }

    [CustomPropertyEditor(typeof(Color))]
    public class ColorPropertyEditor : PropertyEditor<Color> {

        public override Color Edit(Rect region, GUIContent label, Color element, fiGraphMetadata metadata) {
            return EditorGUI.ColorField(region, label, element);
        }

        public override float GetElementHeight(GUIContent label, Color element, fiGraphMetadata metadata) {
            return EditorStyles.colorField.CalcHeight(label, 1000);
        }
    }

    [CustomPropertyEditor(typeof(AnimationCurve))]
    public class AnimationCurvePropertyEditor : PropertyEditor<AnimationCurve> {

        public override AnimationCurve Edit(Rect region, GUIContent label, AnimationCurve element, fiGraphMetadata metadata) {
            if (element == null) {
                element = new AnimationCurve();
            }

            return EditorGUI.CurveField(region, label, element);
        }

        public override float GetElementHeight(GUIContent label, AnimationCurve element, fiGraphMetadata metadata) {
            return EditorStyles.label.CalcHeight(label, 1000);
        }
    }
}