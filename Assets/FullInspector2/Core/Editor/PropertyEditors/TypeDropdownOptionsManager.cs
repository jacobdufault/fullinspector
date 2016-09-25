using System;
using System.Collections.Generic;
using System.Linq;
using FullSerializer;
using FullSerializer.Internal;
using UnityEngine;
using DisplayedType = FullInspector.Internal.fiReflectionUtility.DisplayedType;

namespace FullInspector.Internal {
    /// <summary>
    /// Manages the options that are displayed to the user in the instance selection drop-down.
    /// </summary>
    internal class TypeDropdownOptionsManager {
        private List<DisplayedType> _options;
        private List<GUIContent> _displayedOptions;

        /// <summary>
        /// Setup the instance option manager for the given type.
        /// </summary>
        public TypeDropdownOptionsManager(Type baseType, bool allowUncreatableTypes) {
            if (allowUncreatableTypes)
                _options = fiReflectionUtility.GetTypesDeriving(baseType);
            else
                _options = fiReflectionUtility.GetCreatableTypesDeriving(baseType);

            _displayedOptions = new List<GUIContent>();
            _displayedOptions.Add(new GUIContent("null (" + baseType.CSharpName() + ")"));
            _displayedOptions.AddRange(from option in _options
                                       select GetOptionName(option, !allowUncreatableTypes));
        }

        private static GUIContent GetOptionName(DisplayedType type, bool addSkipCtorMessage) {
            if (addSkipCtorMessage &&
                type.Type.IsValueType == false &&
                type.Type.GetConstructor(fsPortableReflection.EmptyTypes) == null) {

                return new GUIContent(type.DisplayName + " (skips ctor)");
            }

            return new GUIContent(type.DisplayName);
        }

        /// <summary>
        /// Returns an array of options that should be displayed.
        /// </summary>
        public GUIContent[] GetDisplayOptions() {
            return _displayedOptions.ToArray();
        }

        /// <summary>
        /// Remove any options from the set of display options that are not permanently visible.
        /// </summary>
        public void RemoveExtraneousOptions() {
            // Figure out how long we want to be.
            int desiredLength = 0;
            desiredLength += 1; // null
            desiredLength += _options.Count; // regular items

            // Remove items until we are at the desired length.
            while (_displayedOptions.Count > desiredLength) {
                _displayedOptions.RemoveAt(_displayedOptions.Count - 1);
            }
        }

        public Type GetTypeForIndex(int index, Type existingValue) {
            if (index == 0) return null;

            index -= 1; // For the null item
            if (index < _options.Count) return _options[index].Type;
            index -= _options.Count;

            return existingValue;
        }

        public int GetIndexForType(Type type) {
            int offset = 1;

            // try the regular options
            for (int i = 0; i < _options.Count; ++i) {
                Type option = _options[i].Type;
                if (type == option) {
                    return offset + i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns the index of the option that should be displayed (from GetDisplayOptions())
        /// based on the current object instance.
        /// </summary>
        public int GetDisplayOptionIndex(object instance) {
            if (instance == null) {
                return 0;
            }

            int offset = 1;

            // try the regular options
            Type instanceType = instance.GetType();
            for (int i = 0; i < _options.Count; ++i) {
                Type option = _options[i].Type;
                if (instanceType == option) {
                    return offset + i;
                }
            }

            // we need a new display option
            _displayedOptions.Add(new GUIContent(instance.GetType().CSharpName() + " (cannot reconstruct)"));
            return _displayedOptions.Count - 1;
        }

        /// <summary>
        /// Changes the instance of the given object, if necessary.
        /// </summary>
        public object UpdateObjectInstance(object current, int currentIndex, int updatedIndex) {
            // the index has not changed - there will be no change in object instance
            if (currentIndex == updatedIndex) {
                return current;
            }

            // index 0 is always null
            if (updatedIndex == 0) {
                return null;
            }

            // create an instance of the object
            Type currentType = null;
            if (current != null) currentType = current.GetType();

            Type newType = GetTypeForIndex(updatedIndex, currentType);
            if (newType == null) return null;
            return InspectedType.Get(newType).CreateInstance();
        }
    }
}