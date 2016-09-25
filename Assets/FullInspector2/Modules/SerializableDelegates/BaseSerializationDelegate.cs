using FullSerializer.Internal;
using System;
using System.Reflection;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Modules {
    public class BaseSerializationDelegate {
        /// <summary>
        /// The container that will be used as a context when invoking the method.
        /// </summary>
        public UnityObject MethodContainer;

        /// <summary>
        /// The name of the method that will be invoked on the container.
        /// </summary>
        public string MethodName;

        /// <summary>
        /// Construct a new, empty delegate. The delegate cannot be invoked.
        /// </summary>
        public BaseSerializationDelegate()
            : this(null, string.Empty) {
        }

        /// <summary>
        /// Construct a delegate that will target the given container with the specified method.
        /// </summary>
        public BaseSerializationDelegate(UnityObject methodContainer, string methodName) {
            MethodContainer = methodContainer;
            MethodName = methodName;
        }

        /// <summary>
        /// Returns true if the delegate can currently be invoked.
        /// </summary>
        public bool CanInvoke {
            get {
                return
                    MethodContainer != null &&
                    string.IsNullOrEmpty(MethodName) == false &&
                    MethodContainer.GetType().GetFlattenedMethod(MethodName) != null;
            }
        }

        /// <summary>
        /// Internal helper method to invoke the delegate with the given parameters.
        /// </summary>
        protected object DoInvoke(params object[] parameters) {
            if (MethodContainer == null) {
                throw new InvalidOperationException("Attempt to invoke delegate without a method container");
            }
            if (string.IsNullOrEmpty(MethodName)) {
                throw new InvalidOperationException("Attempt to invoke delegate without a selected method");
            }

            MethodInfo method = MethodContainer.GetType().GetFlattenedMethod(MethodName);
            if (method == null) {
                throw new InvalidOperationException("Unable to locate method " + MethodName + " in container " + MethodContainer);
            }

            return method.Invoke(MethodContainer, parameters);
        }
    }
}