using System;
using UnityEngine;

namespace FullInspector {
    /// <summary>
    /// This will activate the Full Inspector editor for the given type. It is an alternative
    /// to deriving from BaseBehavior{NullSerializer}. NOTE: This does not enable any serialization
    /// support - make sure that Unity can properly serialize the object!
    /// </summary>
    // TODO: figure out how to make this work with field arrays and lists
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field)]
    public sealed class fiInspectorOnlyAttribute : PropertyAttribute { }


    /// <summary>
    /// Deriving from this class will activate the Full Inspector editor for the given type. It is an alternative
    /// to deriving from BaseBehavior{NullSerializer}. NOTE: This does not enable any serialization
    /// support - make sure that Unity can properly serialize the object!
    /// </summary>
    public abstract class fiInspectorOnly { }
}