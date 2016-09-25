using System;
using System.Linq;

namespace FullInspector.Internal {
    public class fiSharedInstanceUtility {
        /// <summary>
        /// Returns a SharedInstance type that Unity can serialize for the given generic SharedInstance type.
        /// This returns null if the type is not yet created. Create it with 
        /// SharedInstanceScriptGenerator.GenerateScript(instanceType);
        /// </summary>
        public static Type GetSerializableType(Type sharedInstanceType) {
            return fiRuntimeReflectionUtility.AllSimpleTypesDerivingFrom(sharedInstanceType).FirstOrDefault();
        }
    }
}