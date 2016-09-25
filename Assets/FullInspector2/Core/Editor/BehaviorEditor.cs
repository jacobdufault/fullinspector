using FullInspector.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using FullSerializer.Internal;

namespace FullInspector {

    public sealed class BehaviorEditor {
        /// <summary>
        /// A list of all types that have a CustomBehaviorEditorAttribute attribute.
        /// </summary>
        private static List<Type> _editorTypes;

        /// <summary>
        /// Cached property editors.
        /// </summary>
        private static Dictionary<Type, IBehaviorEditor> _cachedBehaviorEditors;

        static BehaviorEditor() {
            _cachedBehaviorEditors = new Dictionary<Type, IBehaviorEditor>();

            // fetch all CustomPropertyEditorAttribute types
            _editorTypes = new List<Type>();
            foreach (var editorType in
                from assembly in fiRuntimeReflectionUtility.GetUserDefinedEditorAssemblies()
                from type in assembly.GetTypesWithoutException()

                where type.IsAbstract == false
                where type.IsInterface == false

                where fsPortableReflection.HasAttribute<CustomBehaviorEditorAttribute>(type)

                select type) {

                _editorTypes.Add(editorType);
            }
        }

        /// <summary>
        /// If there are multiple user-defined property editors that report that they can edit a
        /// specific type, we sort the applicability of the property editor based on how close it's
        /// reported edited type is to the actual property type. This allows for, say, the
        /// IListPropertyEditor to override the ICollectionPropertyEditor.
        /// </summary>
        private static void SortByPropertyTypeRelevance(List<IBehaviorEditor> editors) {
            editors.Sort((a, b) => {
                Type targetA = fsPortableReflection.GetAttribute<CustomBehaviorEditorAttribute>(a.GetType()).BehaviorType;
                Type targetB = fsPortableReflection.GetAttribute<CustomBehaviorEditorAttribute>(b.GetType()).BehaviorType;

                if (targetA.HasParent(targetB)) {
                    return -1;
                }

                return 1;
            });
        }

        /// <summary>
        /// Returns a set of property editors that can be used to edit the given property type.
        /// </summary>
        private static IBehaviorEditor CreateEditor(Type behaviorType) {
            // user-defined behavior editors
            var added = new List<IBehaviorEditor>();
            foreach (Type editorType in _editorTypes) {
                var editor = BehaviorEditorTools.TryCreateEditor(behaviorType, editorType);
                if (editor != null) {
                    added.Add(editor);
                }
            }
            SortByPropertyTypeRelevance(added);

            if (added.Count > 0) {
                return added[0];
            }

            // there is no user-defined editor, so we use the default one
            return DefaultBehaviorEditor.Instance;
        }

        /// <summary>
        /// Returns a behavior editor that can be used to edit the given behavior type.
        /// </summary>
        /// <param name="behaviorType">The type of behavior to edit. This should derive from
        /// BaseBehavior.</param>
        public static IBehaviorEditor Get(Type behaviorType) {
            IBehaviorEditor editor;
            if (_cachedBehaviorEditors.TryGetValue(behaviorType, out editor) == false) {
                editor = CreateEditor(behaviorType);
                _cachedBehaviorEditors[behaviorType] = editor;
            }
            return editor;
        }
    }

}