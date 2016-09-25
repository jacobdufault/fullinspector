using FullInspector.Internal;
using FullSerializer;
using FullSerializer.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

#if !UNITY_EDITOR && UNITY_WSA
// For System.Reflection.TypeExtensions
using System.Reflection;
#endif

namespace FullInspector {
    /// <para>
    /// A Facade is a really unique abstraction behind an object. It allows for an inspector to be
    /// shown an editor for an object of any type _without_ an instance of that type (though
    /// instances of the edited types are required -- though usually that's not an issue). See the
    /// example below for a primary use case for the Facade.
    /// </para>
    /// <para>
    /// Here's an example. Let's say that you have a behavior, MyBehavior, as defined below:
    ///
    ///     public class MyBehavior : BaseBehavior {
    ///         public int X, Y, Z;
    ///     }
    /// 
    /// You want to create another behavior that can construct preinitialized instances of
    /// MyBehavior, but don't want to use the prefab system. While this *is* doable, it requires
    /// lots of trickery and abstraction that just makes everything really messy. With Facade, it
    /// becomes extremely simple.
    /// 
    ///     public class ReferencingBehavior : BaseBehavior {
    ///         public Facade{MyBehavior} BehaviorDefinition;
    ///         public Facade{Rect} RectDefinition; // We would never need to facade a rect, but it
    ///                                             // demos the behavior.
    ///         
    ///         private MyBehavior _constructedBehavior;
    ///         private Rect _constructedRect;
    ///         
    ///         public void OnEnable() {
    ///             // You can use PopulateInstance when custom object construction is necessary.
    ///             _constructedBehavior = gameObject.AddComponent{MyBehavior}();
    ///             BehaviorDefinition.PopulateInstance(MyBehavior);
    ///             
    ///             // Or just ConstructInstance when the default ctor will work.
    ///             _constructedRect = RectDefinition.ConstructInstance();
    ///         }
    ///     }
    ///     
    /// Then, an inline editor is shown for BehaviorDefinition (identical to the editor typically
    /// shown for MyBehavior), where the state can be customized locally.
    /// 
    /// The Facade is not always necessary, but there are certain situations where it is an
    /// absolute life-saver.
    /// </para>
    /// <remarks>
    /// The Facade will use the default serializer. At the moment, this cannot be customized.
    /// Please also note that only the top-level element is not instantiated; edited properties
    /// are frequently created and recycled.
    /// </remarks>
    /// <typeparam name="T">The type of object that we are a facade / proxy for.</typeparam>
    public class Facade<T> {
        /// <summary>
        /// Populate an pre-constructed instance with the data stored inside of the facade.
        /// </summary>
        /// <param name="instance">The object instance to populate.</param>
        public void PopulateInstance(ref T instance) {
            if (instance.GetType() != InstanceType) {
                Debug.LogWarning("PopulateInstance: Actual Facade type is different " +
                    "(instance.GetType() = " + instance.GetType().CSharpName() +
                    ", InstanceType = " + InstanceType.CSharpName() + ")");
            }

            Type defaultSerializer = fiInstalledSerializerManager.DefaultMetadata.SerializerType;
            var serializer = (BaseSerializer)fiSingletons.Get(defaultSerializer);
            var serializationOperator = new ListSerializationOperator() {
                SerializedObjects = ObjectReferences
            };

            InspectedType inspectedType = InspectedType.Get(instance.GetType());
            foreach (var tuple in FacadeMembers) {
                string name = tuple.Key;

                InspectedProperty property = inspectedType.GetPropertyByName(name);
                if (property != null) {
                    try {
                        object deserializedMember = serializer.Deserialize(
                            property.StorageType.Resolve(), tuple.Value, serializationOperator);
                        property.Write(instance, deserializedMember);
                    }
                    catch (Exception e) {
                        Debug.LogError("Skipping property " + name + " in facade due to " +
                            "deserialization exception.\n" + e);
                    }
                }
            }

        }

        /// <summary>
        /// Constructs a new instance (using the default constructor) of the given facade object.
        /// </summary>
        /// <returns>The populated instance.</returns>
        public T ConstructInstance() {
            var obj = (T)Activator.CreateInstance(InstanceType);
            PopulateInstance(ref obj);
            return obj;
        }

        /// <summary>
        /// Constructs a new instance (using either the default constructor or AddComponent) of the given
        /// facade object.
        /// </summary>
        /// <param name="context">The GameObect to add the Component derived type to, if applicable.</param>
        /// <remarks>This override is extremely useful if T is an interface type and you want to support MonoBehaviour derived
        /// components but do not want to deal with the hassle of actually constructing said instance types.</remarks>
        /// <returns>The populated instance.</returns>
        public T ConstructInstance(GameObject context) {
            T obj;

            if (typeof(Component).IsAssignableFrom(InstanceType)) {
                obj = (T)(object)context.AddComponent(InstanceType);
            }
            else {
                obj = (T)Activator.CreateInstance(InstanceType);
            }

            PopulateInstance(ref obj);
            return obj;
        }

        /// <summary>
        /// The actual type of the facade.
        /// </summary>
        public Type InstanceType;

        /// <summary>
        /// The raw members of the facade, in serialized format. Modifying this by hand is not
        /// recommended.
        /// </summary>
        public Dictionary<string, string> FacadeMembers = new Dictionary<string, string>();

        /// <summary>
        /// UnityObject references that were encountered during serialization of the facade.
        /// Modifying this by hand is not recommended.
        /// </summary>
        public List<UnityObject> ObjectReferences = new List<UnityObject>();
    }
}