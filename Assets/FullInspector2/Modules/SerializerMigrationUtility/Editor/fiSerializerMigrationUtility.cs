using FullInspector.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace FullInspector.Modules {
    public static class fiSerializerMigrationUtility {
        /// <summary>
        /// Changes the serialization data for the given GameObject.
        /// </summary>
        /// <param name="go">The UnityObject to migrate (either a ScriptableObject, GameObject, or Component). If it is a GameObject, then all child components will also be scanned.</param>
        /// <param name="fromSerializer">The current serializer</param>
        /// <param name="toSerializer">The new serializer</param>
        public static void MigrateUnityObject(UnityObject obj, Type fromSerializer, Type toSerializer) {
            var restoreState = typeof(fiISerializedObjectUtility).GetMethod("RestoreState").MakeGenericMethod(fromSerializer);
            var saveState = typeof(fiISerializedObjectUtility).GetMethod("SaveState").MakeGenericMethod(toSerializer);

            bool warningState = fiSettings.EmitWarnings;
            fiSettings.EmitWarnings = true;

            if (obj is GameObject) {
                var go = (GameObject)obj;
                foreach (var behavior in go.GetComponentsInChildren(typeof(ISerializedObject), /*includeInactive:*/ true)) {
                    ChangeStates(behavior, restoreState, saveState);
                }
            }
            else if (obj is ScriptableObject || obj is Component) {
                ChangeStates(obj, restoreState, saveState);
            }

            fiSettings.EmitWarnings = warningState;
        }

        private static void ChangeStates(UnityObject target, MethodInfo restoreState, MethodInfo saveState) {
            object result = restoreState.Invoke(null, new object[] { target });
            if ((bool)result == false) {
                Debug.LogWarning("Skipping " + target + " -- unable to successfuly deserialize", target);
                return;
            }

            ISerializedObject serializedObj = (ISerializedObject)target;
            var savedKeys = new List<string>(serializedObj.SerializedStateKeys);
            var savedValues = new List<string>(serializedObj.SerializedStateValues);
            var savedRefs = new List<UnityObject>(serializedObj.SerializedObjectReferences);

            result = saveState.Invoke(null, new object[] { target });
            if ((bool)result == false) {
                Debug.LogWarning("Skipping " + target + " -- unable to successfuly serialize", target);

                serializedObj.SerializedStateKeys = savedKeys;
                serializedObj.SerializedStateValues = savedValues;
                serializedObj.SerializedObjectReferences = savedRefs;

                return;
            }

            Debug.Log("Successfully migrated " + target, target);
            EditorUtility.SetDirty(target);
        }

        /// <summary>
        /// Returns all scene specific objects that use Full Inspector.
        /// </summary>
        /// <returns></returns>
        public static List<UnityObject> GetSceneObjects() {
            var result = new List<UnityObject>();

            foreach (var obj in Resources.FindObjectsOfTypeAll(typeof(UnityObject))) {
                //foreach (var obj in Resources.FindObjectsOfTypeAll(typeof(UnityObject))) {
                // a persistent object is *not* part of the scene
                if (EditorUtility.IsPersistent(obj)) {
                    continue;
                }

                if (obj is GameObject) {
                    var go = (GameObject)obj;

                    // don't display temporary objects
                    if ((obj.hideFlags & HideFlags.DontSave) != HideFlags.None ||
                        (obj.hideFlags & HideFlags.HideAndDontSave) != HideFlags.None) {
                        continue;
                    }

                    // for a scene, we only need to care about top-level GameObjects
                    if (go.transform.parent != null) continue;

                    // the game object must also have some ISerializedObject components
                    var serializedObjChildren = go.GetComponentsInChildren(typeof(ISerializedObject));
                    if (serializedObjChildren == null || serializedObjChildren.Length == 0) continue;
                }

                else if (obj is ScriptableObject) {
                    // ScriptableObjects need to just derive from ISerializedObject
                    if (obj is ISerializedObject == false) continue;
                }

                else {
                    // note: we do not process components here
                    continue;
                }

                result.Add(obj);
            }

            return result;
        }

        /// <summary>
        /// Returns all persistent objects that use Full Inspector.
        /// </summary>
        public static List<UnityObject> GetPersistentObjects() {
            var result = new List<UnityObject>();

            result.AddRange(fiEditorUtility.GetAllAssetsOfType(typeof(ISerializedObject)));
            result.AddRange(fiEditorUtility.GetAllPrefabsOfType(typeof(ISerializedObject)));

            return result;
        }
    }
}