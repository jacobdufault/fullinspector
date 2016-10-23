using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FullInspector.Internal {
    public struct SavedObjectDelta {
        // The delta will save all of the changes needed to convert
        // |previousState| into |currentState|.
        public SavedObjectDelta(SavedObject currentState, SavedObject previousState)
            : this(currentState, previousState, false) {
        }

        public SavedObjectDelta(SavedObject currentState, SavedObject previousState, bool dumpString) {
            CreateDelta(currentState.state, previousState.state,
                        out toAdd, out toModify, out toRemove, dumpString);
        }

        public static SavedObjectDelta FromObjects(object current, object previous) {
            return FromObjects(current, previous, false);
        }

        public static SavedObjectDelta FromObjects(object current, object previous, bool dump) {
            return new SavedObjectDelta(new SavedObject(current),
                                        new SavedObject(previous),
                                        dump);
        }

        public void ApplyChanges<T>(ref T o) {
            object wrapper = o;
            ApplyChanges(ref wrapper);
            o = (T)wrapper;
        }

        public void ApplyChanges(ref object o) {
            Apply(ref o, toAdd, toModify, toRemove);
        }

        private static Dictionary<ObjectDataPath[], object> AllocateDict() {
            return new Dictionary<ObjectDataPath[], object>(new ObjectDataPath.EqualityComparer());
        }

        public static void WriteNavigation(ObjectDataPath[] path, object root, object newContext) {
            // TODO: Fix this method. It breaks things.
            for (int i = path.Length - 1; i > 0; --i) {
                object parent = ReadNavigation(path, i - 1, root, false);
                if (parent == null)
                    break;

                ObjectDataPath pathElement = path[i - 1];
                // byType uses the parent element to adjust the type, so we have
                // to go back two elements instead of just one.
                if (path[i].byType != null) {
                    if (i - 2 < 0)
                        break;
                    pathElement = path[i - 2];
                }
                pathElement.Write(parent, newContext);

                if (i > 0)
                    newContext = ReadNavigation(path, i - 1, root, false);
            }
        }

        public static object ReadNavigation(ObjectDataPath[] path, object root, bool create) {
            return ReadNavigation(path, path.Length - 1, root, create);
        }

        public static object ReadNavigation(ObjectDataPath[] path, int to, object root, bool create) {
            object context = root;

            for (int i = 0; i < to; ++i) {
                if (path[i + 1].byType != null)
                    break;

                object c;
                if (!path[i].Read(root, out c))
                    break;

                if (create && c == null) {
                    c = path[i].CreateInstance();
                    path[i].Write(root, c);
                }
                context = c;
            }

            return context;
        }

        public static void Apply(ref object target,
                                 Dictionary<ObjectDataPath[], object> toAdd,
                                 Dictionary<ObjectDataPath[], object> toModify,
                                 List<ObjectDataPath[]> toRemove) {
            // - We remove first, because we will generate a diff that removes
            //   the null value and then adds the expected value back.
            // - We modify next, since we might change the object type.
            // TODO: If we change the object type then we need to restore the
            //       state :/
            foreach (ObjectDataPath[] path in toRemove) {
                object context = ReadNavigation(path, target, /*create:*/false);

                // Context may have been removed already.
                if (context == null)
                    continue;

                path[path.Length - 1].Remove(context);
            }

            foreach (var property in toModify) {
                ObjectDataPath[] path = property.Key;
                object newValue = property.Value;

                object context = ReadNavigation(path, target, /*create:*/false);
                // Debug.Log("Modifying " + DumpToString(property) + " on " +
                // context);
                path[path.Length - 1].Write(context, newValue);
                WriteNavigation(path, target, context);
            }

            foreach (var property in toAdd) {
                ObjectDataPath[] path = property.Key;
                object addedValue = property.Value;
                object context = ReadNavigation(path, target, /*create:*/true);
                // Debug.Log("Adding " + DumpToString(property) + " on " +
                // context);
                path[path.Length - 1].Write(context, addedValue);
            }
        }

        // Return the set of operations needed to make |previousState| look like
        // |currentState|.
        public static void CreateDelta(
                Dictionary<ObjectDataPath[], object> currentState,
                Dictionary<ObjectDataPath[], object> previousState,
                out Dictionary<ObjectDataPath[], object> toAdd,
                out Dictionary<ObjectDataPath[], object> toModify,
                out List<ObjectDataPath[]> toRemove,
                bool dumpString) {
            toAdd = AllocateDict();
            toModify = AllocateDict();
            toRemove = new List<ObjectDataPath[]>();

            foreach (var entry in previousState) {
                // End has it, but start doesn't. We need to remove the key.
                if (currentState.ContainsKey(entry.Key) == false) {
                    toRemove.Add(entry.Key);
                    continue;
                }

                object startObj = currentState[entry.Key];
                object endObj = entry.Value;

                // If the objects are the same, do not modify.
                if (ReferenceEquals(startObj, endObj) || startObj.Equals(endObj))
                    continue;

                toModify.Add(entry.Key, startObj);
            }

            // Start has it, but end doesn't.
            foreach (var entry in currentState) {
                if (previousState.ContainsKey(entry.Key) == false) {
                    toAdd.Add(entry.Key, entry.Value);
                }
            }

            if (dumpString) {
                var msg = "Computed delta (click to see):" + Environment.NewLine + Environment.NewLine;
                msg += DumpToString("current", ReadableModel(currentState)) + Environment.NewLine;
                msg += DumpToString("previous", ReadableModel(previousState));
                msg += Environment.NewLine + "----" + Environment.NewLine + Environment.NewLine;
                msg += DumpToString("toAdd", ReadableModel(toAdd)) + Environment.NewLine;
                msg += DumpToString("toModify", ReadableModel(toModify)) + Environment.NewLine;
                msg += DumpToString("toRemove", toRemove.Select(r => ObjectDataPath.ToString(r)).ToList());
                Debug.Log(msg);
            }
        }

        public static Dictionary<string, object> ReadableModel(Dictionary<ObjectDataPath[], object> model) {
            var readable = new Dictionary<string, object>();
            foreach (var entry in model) {
                string navString = ObjectDataPath.ToString(entry.Key);
                //Debug.Log("Adding " + navString);
                readable.Add(navString, entry.Value);
            }
            return readable;
        }

        private static string DumpToString(KeyValuePair<ObjectDataPath[], object> property) {
            return ObjectDataPath.ToString(property.Key) + " => " + property.Value;// + " (" + property.Value.GetType() + ")";
        }

        private static string DumpToString(string name, Dictionary<string, object> dict) {
            var result = new StringBuilder();
            result.AppendLine("Dumping " + name + " (" + dict.Count + " elements)");

            foreach (var entry in dict) {
                result.AppendLine(entry.Key + " => " + entry.Value);
            }

            return result.ToString();
        }
        private static string DumpToString(string name, List<string> list) {
            var result = new StringBuilder();
            result.AppendLine("Dumping " + name + "(" + list.Count + " elements)");

            foreach (var entry in list) {
                result.AppendLine(entry);
            }

            return result.ToString();
        }

        public List<ObjectDataPath[]> toRemove;
        public Dictionary<ObjectDataPath[], object> toModify;
        public Dictionary<ObjectDataPath[], object> toAdd;
    }
}