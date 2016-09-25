using FullInspector.Internal;
using System;
using System.Reflection;
using FullSerializer.Internal;
using UnityEngine;

namespace FullInspector {
    /// <summary>
    /// A method that is being inspected, typically for the purpose of a button.
    /// </summary>
    public class InspectedMethod {
        public InspectedMethod(MethodInfo method) {
            Method = method;

            // We can consider methods with all-default parameters as no parameter methods
            foreach (var param in method.GetParameters()) {
                if (param.IsOptional) continue;
                HasArguments = true;
                break;
            }

            DisplayLabel = new GUIContent();

            // DisplayLabel.text
            {
                var attr = fsPortableReflection.GetAttribute<InspectorNameAttribute>(method);
                if (attr != null) {
                    DisplayLabel.text = attr.DisplayName;
                }

                if (string.IsNullOrEmpty(DisplayLabel.text)) {
                    DisplayLabel.text = fiDisplayNameMapper.Map(method.Name);
                }
            }

            // DisplayLabel.tooltip
            {
                var attr = fsPortableReflection.GetAttribute<InspectorTooltipAttribute>(method);
                if (attr != null) {
                    DisplayLabel.tooltip = attr.Tooltip;
                }
            }


        }

        /// <summary>
        /// The wrapped method.
        /// </summary>
        public MethodInfo Method {
            get;
            private set;
        }

        /// <summary>
        /// The name that should be used when displaying the method. This value defaults to
        /// Method.Name but can be overridden with a InspectorButtonAttribute.
        /// </summary>
        public GUIContent DisplayLabel {
            get;
            private set;
        }

        /// <summary>
        /// True if the method has arguments (besides an implicit this).
        /// </summary>
        public bool HasArguments {
            get;
            private set;
        }

        /// <summary>
        /// Invoke the method. This function will never fail.
        /// </summary>
        public void Invoke(object instance) {
            try {
                object[] args = null;

                // support default parameter methods
                var methodParams = Method.GetParameters();
                if (methodParams.Length != 0) {
                    args = new object[methodParams.Length];

                    // NOTE: Based on documentation, it looks like the value you're actually
                    // supposed to use to get default arguments is Type.Missing, but
                    // there appears to be an issue in mono where that is not supported. Instead
                    // we will just fetch the default parameter values and send them.
                    for (int i = 0; i < args.Length; ++i) {
                        args[i] = methodParams[i].DefaultValue;
                    }
                }

                Method.Invoke(instance, args);
            }
            catch (Exception e) {
                Debug.LogException(e);
            }
        }
    }
}