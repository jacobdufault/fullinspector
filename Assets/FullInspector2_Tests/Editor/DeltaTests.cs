using System.Collections.Generic;
using FullInspector.Internal;
using NUnit.Framework;
using UnityEngine;

public class DeltaTests {
    private class HostObject {
        public SimpleObject nested;
    }

    private class SimpleObject {
        public SimpleObject nested;
        public int a;
        public int b;
    }

    private class EnumHolder {
        public enum EnumValue {
            A, B, C
        }

        public EnumValue enumValue;
    }

    private class DerivedObject : SimpleObject {
        public int c;
    }

    public struct StructA {
        public int a;
        public StructB structB;

        public override string ToString() {
            return "StructA [a=" + a + ", structB=" + structB + "]";
        }
    }

    public struct StructB {
        public int b;

        public override string ToString() {
            return "StructB [b=" + b + "]";
        }
    }

    private class CollectionHolder {
        public Dictionary<string, string> dict = new Dictionary<string, string>();
        public List<int> list = new List<int>();
        public bool[] arr;

        public CollectionHolder nested;
    }
    
    [Test]
    public static void ApplyEnum() {
        var before = new EnumHolder { enumValue = EnumHolder.EnumValue.A };
        var after = new EnumHolder { enumValue = EnumHolder.EnumValue.B };

        var delta = new SavedObjectDelta(new SavedObject(after),
                                         new SavedObject(before));
        delta.ApplyChanges(ref before);

        Assert.AreEqual(after.enumValue, before.enumValue);
    }

    [Test]
    public static void ApplyVector3() {
        var previous = Vector3.zero;
        var current = Vector3.one;

        var delta = SavedObjectDelta.FromObjects(current, previous);
        delta.ApplyChanges(ref previous);

        Assert.AreEqual(current, previous);
    }

    [Test]
    public static void ApplyNestedStruct() {
        var current = new StructA { a = 2, structB = new StructB { b = 3 } };
        var previous = new StructA { a = 1, structB = new StructB { b = 1 } };

        var delta = SavedObjectDelta.FromObjects(current, previous);
        delta.ApplyChanges(ref previous);

        Assert.AreEqual(current, previous);
    }

    [Test]
    public static void ChangeInstanceTypeToNull() {
        var before = new HostObject() {
            nested = new DerivedObject()
        };
        var after = new HostObject();

        var delta = SavedObjectDelta.FromObjects(after, before);
        delta.ApplyChanges(ref before);

        Assert.AreEqual(after.nested, before.nested);
    }

    [Test]
    public static void SanityTest() {
        object v0 = new Vector3(1, 1, 1);
        object v1 = new Vector3(1, 1, 1);
        Assert.AreEqual(v0, v1);
    }

    [Test]
    public static void CycleTest() {
        // TODO: This needs to be fixed. The cyclic object should appear in the dict.
        Debug.LogError("Fix CycleTest to include cycled object");

        var obj = new SimpleObject();
        obj.a = 1;
        obj.b = 2;
        obj.nested = obj;

        Dictionary<ObjectDataPath[], object> model = new SavedObject(obj).state;
        Dictionary<string, object> readableModel = SavedObjectDelta.ReadableModel(model);

        Assert.AreEqual(3, readableModel.Count);
        Assert.AreEqual(obj.GetType(), readableModel["GetType()"]);
        Assert.AreEqual(obj.a, readableModel["a"]);
        Assert.AreEqual(obj.b, readableModel["b"]);
        //Assert.AreEqual(obj.nested, readableModel["nested"]);


        // Also test vectors, which are cyclic on normalized
        new SavedObject(new Vector3(1, 1, 1));
    }

    [Test]
    public static void RunCollectionTest() {
        var obj = new CollectionHolder();
        obj.dict["foo1"] = "bar1";
        obj.dict["foo2"] = "bar2";
        obj.list.Add(1);
        obj.list.Add(2);
        obj.arr = new[] { true, false };
        obj.nested = new CollectionHolder();
        obj.nested.dict["nested1"] = "ack1";
        obj.nested.arr = new[] { true };

        Dictionary<ObjectDataPath[], object> model = new SavedObject(obj).state;
        Dictionary<string, object> readableModel = SavedObjectDelta.ReadableModel(model);

        // TODO: If list is null, we will include it, but if it is empty we will not

        Assert.AreEqual(11, readableModel.Count);
        Assert.AreEqual(obj.GetType(), readableModel["GetType()"]);
        Assert.AreEqual("bar1", readableModel["dict.foo1"]);
        Assert.AreEqual("bar2", readableModel["dict.foo2"]);
        Assert.AreEqual(1, readableModel["list.0"]);
        Assert.AreEqual(2, readableModel["list.1"]);
        Assert.AreEqual(true, readableModel["arr.0"]);
        Assert.AreEqual(false, readableModel["arr.1"]);
        Assert.AreEqual(obj.nested.GetType(), readableModel["nested:GetType()"]);
        Assert.AreEqual("ack1", readableModel["nested:dict.nested1"]);
        Assert.AreEqual(true, readableModel["nested:arr.0"]);
        Assert.AreEqual(null, readableModel["nested:nested"]);
    }

    [Test]
    public static void RunFieldPropertyTest() {
        var obj = new SimpleObject {
            a = 1,
            b = 2,
            nested = new SimpleObject {
                a = 3,
                b = 4
            }
        };
        Dictionary<ObjectDataPath[], object> model = new SavedObject(obj).state;
        Dictionary<string, object> readableModel = SavedObjectDelta.ReadableModel(model);

        Assert.AreEqual(7, readableModel.Count);
        Assert.AreEqual(obj.GetType(), readableModel["GetType()"]);
        Assert.AreEqual(1, readableModel["a"]);
        Assert.AreEqual(2, readableModel["b"]);
        Assert.AreEqual(obj.nested.GetType(), readableModel["nested:GetType()"]);
        Assert.AreEqual(3, readableModel["nested:a"]);
        Assert.AreEqual(4, readableModel["nested:b"]);
        Assert.AreEqual(null, readableModel["nested:nested"]);
    }

    [Test]
    public static void RunCollectionApplyTest() {
        var start = new CollectionHolder();
        start.dict["foo1"] = "bar1";
        start.dict["foo2"] = "bar2";
        start.list = new List<int> { 1, 2 };
        start.arr = new[] { true, false };

        var end = new CollectionHolder();
        end.dict["foo1"] = "baz";
        end.list = new List<int> { 1, 3, 4 };
        end.arr = new bool[] {};

        // Compute in-memory representation.
        var startModel = new SavedObject(start);
        var endModel = new SavedObject(end);

        // Compute differental.
        var delta = new SavedObjectDelta(startModel, endModel);

        // Apply differental.
        delta.ApplyChanges(ref end);

        // Compare.
        CollectionAssert.AreEqual(start.dict, end.dict);
        CollectionAssert.AreEqual(start.list, end.list);
        CollectionAssert.AreEqual(start.arr, end.arr);
    }

    [Test]
    public static void CheckReadableModelIncludesInstances() {
        var model = new SimpleObject {
            a = 1,
            b = 2,
            nested = new SimpleObject {
                a = 3,
                b = 4
            }
        };

        Dictionary<string, object> readable = SavedObjectDelta.ReadableModel(new SavedObject(model).state);
        Assert.AreEqual(7, readable.Count);
        Assert.AreEqual(model.GetType(), readable["GetType()"]);
        Assert.AreEqual(model.a, readable["a"]);
        Assert.AreEqual(model.b, readable["b"]);
        Assert.AreEqual(model.nested.GetType(), readable["nested:GetType()"]);
        Assert.AreEqual(model.nested.a, readable["nested:a"]);
        Assert.AreEqual(model.nested.b, readable["nested:b"]);
        Assert.AreEqual(model.nested.nested, readable["nested:nested"]);
    }

    [Test]
    public static void RunSimpleApplyTest() {
        var start = new SimpleObject {
            a = 1,
            b = 2,
            nested = new SimpleObject {
                a = 3,
                b = 4
            }
        };

        var end = new SimpleObject {
            a = 2,
            b = 2
        };

        var startModel = new SavedObject(start);
        var endModel = new SavedObject(end);
        var delta = new SavedObjectDelta(startModel, endModel);

        delta.ApplyChanges(ref end);

        Assert.AreEqual(end.a, start.a);
        Assert.AreEqual(end.b, start.b);
        Assert.AreEqual(end.nested.a, start.nested.a);
        Assert.AreEqual(end.nested.b, start.nested.b);
        Assert.AreEqual(end.nested.nested, start.nested.nested);
    }

    [Test]
    public static void TestApplyMissingProperty() {
        var start = new DerivedObject {
            a = 1,
            b = 2,
            c = 3
        };
        var end = new DerivedObject {
            a = 2,
            b = 2,
            c = 5
        };
        var delta = new SavedObjectDelta(new SavedObject(start), new SavedObject(end));

        var simpleObj = new SimpleObject() {
            b = 20
        };
        delta.ApplyChanges(ref simpleObj);

        Assert.AreEqual(start.a, simpleObj.a);
        Assert.AreEqual(20, simpleObj.b);
    }

    [Test]
    public static void ChangeInstanceType() {
        var start = new HostObject {
            nested = new DerivedObject {
                c = 5
            }
        };

        var end = new HostObject {
            nested = new SimpleObject {
                a = 1,
                b = 2
            }
        };

        var startModel = new SavedObject(start);
        var endModel = new SavedObject(end);

        var delta = new SavedObjectDelta(startModel, endModel);
        delta.ApplyChanges(ref end);

        Assert.AreEqual(start.nested.GetType(), end.nested.GetType());
        Assert.AreEqual(start.nested.a, end.nested.a);
        Assert.AreEqual(start.nested.b, end.nested.b);
        Assert.AreEqual(((DerivedObject)start.nested).c, ((DerivedObject)end.nested).c);
    }

    [Test]
    public static void SimpleIntegrationExample() {
        var inspected = new SimpleObject() {
            a = 1,
            b = 2
        };
        var previousState = new SavedObject(inspected);

        inspected.a = 5;

        var currentState = new SavedObject(inspected);

        var delta = new SavedObjectDelta(currentState, previousState);

        var hello = new SimpleObject();
        delta.ApplyChanges(ref hello);

        Assert.AreEqual(5, hello.a);
        Assert.AreEqual(0, hello.b);
    }

}