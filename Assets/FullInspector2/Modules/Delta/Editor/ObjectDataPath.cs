using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FullInspector.Internal {
    /// <summary>
    /// Provides a route to read/write data inside of an object. That data can be
    /// (recursively) stored within a field, property, or collection.
    /// </summary>
    public struct ObjectDataPath {
        public class EqualityComparer : IEqualityComparer<ObjectDataPath[]> {
            public bool Equals(ObjectDataPath[] x, ObjectDataPath[] y) {
                if (x.Length != y.Length)
                    return false;
                for (int i = 0; i < x.Length; ++i) {
                    if (x[i] != y[i])
                        return false;
                }
                return true;
            }

            public int GetHashCode(ObjectDataPath[] obj) {
                int hash = 17;
                for (int i = 0; i < obj.Length; ++i)
                    hash = (hash * 31) + obj[i].GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// The data path stores the type of the object.
        /// </summary>
        public ObjectDataPath(ObjectDataPath[] getType) {
            byProperty = null;
            byListIndex = -1;
            byDictKey = null;
            byType = getType;
        }

        /// <summary>
        /// The data path accesses the given property.
        /// </summary>
        public ObjectDataPath(InspectedProperty property) {
            byProperty = property;
            byListIndex = -1;
            byDictKey = null;
            byType = null;
        }

        /// <summary>
        /// The data path accesses an element inside of an IList instance
        /// (including arrays). The list instance resides at |property|.
        /// </summary>
        public ObjectDataPath(InspectedProperty property, int listIndex) {
            byProperty = property;
            byListIndex = listIndex;
            byDictKey = null;
            byType = null;
        }

        /// <summary>
        /// The data path accesses an element inside of an IDictionary instance.
        /// The dictionary instance resides at |property|.
        /// </summary>
        public ObjectDataPath(InspectedProperty property, object dictKey) {
            byProperty = property;
            byListIndex = -1;
            byDictKey = dictKey;
            byType = null;
        }

        public ObjectDataPath[] byType;
        public InspectedProperty byProperty;
        public int byListIndex;
        public object byDictKey;

        /// <summary>
        /// Special marker object used to notify Write to write the default/null
        /// value.
        /// </summary>
        private static object s_RemoveObject = new object();

        public object CreateInstance() {
            return InspectedType.Get(byProperty.StorageType).CreateInstance();
        }

        public bool Read(object obj, out object result) {
            if (byType != null) {
                result = obj;
                return true;
            }

            // The property was potentially found on a different type. We need to
            // update it to associate with this type. It's very possible the
            // property will not even apply to context, in which case Write
            // becomes a no-op.
            InspectedProperty propertyToUse = byProperty;
            if (byProperty.MemberInfo.DeclaringType != obj.GetType()) {
                var childProp = InspectedType.Get(obj.GetType()).GetPropertyByName(byProperty.Name);
                if (childProp != null) {
                    propertyToUse = childProp;
                }
                else {
                    result = null;
                    return false;
                }
            }

            var read = propertyToUse.Read(obj);

            if (byListIndex >= 0) {
                result = ((IList)read)[byListIndex];
                return true;
            }
            if (byDictKey != null) {
                result = ((IDictionary)read)[byDictKey];
                return true;
            }

            result = read;
            return true;
        }

        public void Remove(object context) {
            Write(context, s_RemoveObject);
        }

        public void Write(object context, object value) {
            if (byType != null) {
                if (value != s_RemoveObject)
                    value = InspectedType.Get((Type)value).CreateInstance();
                byType[byType.Length - 1].Write(context, value);
                return;
            }

            // The property was potentially found on a different type. We need to
            // update it to associate with this type. It's very possible the
            // property will not even apply to context, in which case Write
            // becomes a no-op.
            InspectedProperty propertyToUse = byProperty;
            if (byProperty.MemberInfo.DeclaringType != context.GetType()) {
                var childProp = InspectedType.Get(context.GetType()).GetPropertyByName(byProperty.Name);
                if (childProp != null) {
                    propertyToUse = childProp;
                }
                else {
                    return;
                }
            }

            if (byListIndex >= 0) {
                var read = propertyToUse.Read(context);
                var list = (IList)read;

                var elementType = InspectedType.Get(propertyToUse.StorageType).ElementType;

                if (value == s_RemoveObject) {
                    fiListUtility.RemoveAt(ref list, elementType, byListIndex);
                }
                else {
                    while (byListIndex >= list.Count) {
                        fiListUtility.Add(ref list, elementType);
                    }
                    list[byListIndex] = value;
                }

                // Changing list will not update the actual array reference, so
                // we have to write back to the stored object.
                if (list is Array)
                    propertyToUse.Write(context, list);

                return;
            }

            if (byDictKey != null) {
                var read = propertyToUse.Read(context);
                var dict = (IDictionary)read;

                // TODO: Have a separate Write/Remove command, since you might
                //       want to set a dict field to null.
                if (value == s_RemoveObject)
                    dict.Remove(byDictKey);
                else
                    dict[byDictKey] = value;

                return;
            }

            if (value != s_RemoveObject) {
                propertyToUse.Write(context, value);
            }
            else {
                propertyToUse.Write(context, null);
            }
        }

        public override string ToString() {
            if (byType != null)
                return "GetType()";
            if (byListIndex >= 0)
                return byProperty.Name + "." + byListIndex;
            if (byDictKey != null)
                return byProperty.Name + "." + byDictKey;
            return byProperty.Name;
        }

        public static string ToString(IEnumerable<ObjectDataPath> navs) {
            return string.Join(":", navs.Select(n => n.ToString()).ToArray());
        }

        public static bool operator ==(ObjectDataPath a, ObjectDataPath b) {
            return a.byProperty == b.byProperty &&
                a.byListIndex == b.byListIndex &&
                a.byDictKey == b.byDictKey;
        }

        public static bool operator !=(ObjectDataPath a, ObjectDataPath b) {
            return !(a == b);
        }

        public override bool Equals(object a) {
            return a is ObjectDataPath && ((ObjectDataPath)a) == this;
        }

        public override int GetHashCode() {
            int hash = 17;
            hash = hash * 31 + byListIndex.GetHashCode();
            if (byProperty != null)
                hash = hash * 31 + byProperty.GetHashCode();
            if (byDictKey != null)
                hash = hash * 31 + byDictKey.GetHashCode();
            return hash;
        }
    }
}