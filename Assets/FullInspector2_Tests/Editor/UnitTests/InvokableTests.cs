using System;
using NUnit.Framework;
using UnityEngine;

namespace FullInspector.Tests {
    public class MethodContainer : ScriptableObject {
        public int InvokeCount;

        public void Action() {
            ++InvokeCount;
        }
        public void ActionInt(int a) {
            ++InvokeCount;
        }
        public void ActionIntDouble(int a, double b) {
            ++InvokeCount;
        }
        public void ActionIntDoubleBool(int a, double b, bool c) {
            ++InvokeCount;
        }

        public int IntFunc() {
            ++InvokeCount; return 1;
        }
        public int IntFuncInt(int a) {
            ++InvokeCount; return 1;
        }
        public int IntFuncIntDouble(int a, double b) {
            ++InvokeCount; return 1;
        }
        public int IntFuncIntDoubleBool(int a, double b, bool c) {
            ++InvokeCount; return 1;
        }
    }

    public class InvokableTests {
        [Test]
        public void TestActionInvokeFail() {
            var container = ScriptableObject.CreateInstance<MethodContainer>();

            var action0 = new SerializedAction {
                MethodContainer = container,
                MethodName = "Fake"
            };
            var action1 = new SerializedAction<int> {
                MethodContainer = null,
                MethodName = null,
            };
            var action2 = new SerializedAction<int> {
                MethodContainer = null,
                MethodName = string.Empty,
            };
            var action3 = new SerializedAction<int> {
                MethodContainer = null,
                MethodName = "Action",
            };
            var action4 = new SerializedAction<int, double> {
                MethodContainer = container,
                MethodName = string.Empty
            };
            var action5 = new SerializedAction<int, double, bool> {
                MethodContainer = container,
                MethodName = null
            };

            Assert.False(action0.CanInvoke);
            Assert.False(action1.CanInvoke);
            Assert.False(action2.CanInvoke);
            Assert.False(action3.CanInvoke);
            Assert.False(action4.CanInvoke);
            Assert.False(action5.CanInvoke);

            Assert.Throws<InvalidOperationException>(() => action0.Invoke());
            Assert.Throws<InvalidOperationException>(() => action1.Invoke(1));
            Assert.Throws<InvalidOperationException>(() => action2.Invoke(1));
            Assert.Throws<InvalidOperationException>(() => action3.Invoke(1));
            Assert.Throws<InvalidOperationException>(() => action4.Invoke(1, 1.0));
            Assert.Throws<InvalidOperationException>(() => action5.Invoke(1, 1.0, true));
        }

        [Test]
        public void TestActionInvokeSuccess() {
            var container = ScriptableObject.CreateInstance<MethodContainer>();

            var action = new SerializedAction {
                MethodContainer = container,
                MethodName = "Action"
            };
            var actionInt = new SerializedAction<int> {
                MethodContainer = container,
                MethodName = "ActionInt"
            };
            var actionIntDouble = new SerializedAction<int, double> {
                MethodContainer = container,
                MethodName = "ActionIntDouble"
            };
            var actionIntDoubleBool = new SerializedAction<int, double, bool> {
                MethodContainer = container,
                MethodName = "ActionIntDoubleBool"
            };

            Assert.IsTrue(action.CanInvoke);
            Assert.IsTrue(actionInt.CanInvoke);
            Assert.IsTrue(actionIntDouble.CanInvoke);
            Assert.IsTrue(actionIntDoubleBool.CanInvoke);

            action.Invoke();
            actionInt.Invoke(1);
            actionIntDouble.Invoke(1, 1.0);
            actionIntDoubleBool.Invoke(1, 1.0, true);

            Assert.AreEqual(4, container.InvokeCount);
        }

        [Test]
        public void TestFuncInvokeFail() {
            var container = ScriptableObject.CreateInstance<MethodContainer>();

            var func0 = new SerializedFunc<int> {
                MethodContainer = container,
                MethodName = "Fake"
            };
            var func1 = new SerializedFunc<int, int> {
                MethodContainer = null,
                MethodName = null,
            };
            var func2 = new SerializedFunc<int, int> {
                MethodContainer = null,
                MethodName = string.Empty,
            };
            var func3 = new SerializedFunc<int, int> {
                MethodContainer = null,
                MethodName = "Action",
            };
            var func4 = new SerializedFunc<int, double, int> {
                MethodContainer = container,
                MethodName = string.Empty
            };
            var func5 = new SerializedFunc<int, double, bool, int> {
                MethodContainer = container,
                MethodName = null
            };

            Assert.False(func0.CanInvoke);
            Assert.False(func1.CanInvoke);
            Assert.False(func2.CanInvoke);
            Assert.False(func3.CanInvoke);
            Assert.False(func4.CanInvoke);
            Assert.False(func5.CanInvoke);

            Assert.Throws<InvalidOperationException>(() => func0.Invoke());
            Assert.Throws<InvalidOperationException>(() => func1.Invoke(1));
            Assert.Throws<InvalidOperationException>(() => func2.Invoke(1));
            Assert.Throws<InvalidOperationException>(() => func3.Invoke(1));
            Assert.Throws<InvalidOperationException>(() => func4.Invoke(1, 1.0));
            Assert.Throws<InvalidOperationException>(() => func5.Invoke(1, 1.0, true));
        }

        [Test]
        public void TestFuncInvokeSuccess() {
            var container = ScriptableObject.CreateInstance<MethodContainer>();

            var intFunc = new SerializedFunc<int> {
                MethodContainer = container,
                MethodName = "IntFunc"
            };
            var intFuncInt = new SerializedFunc<int, int> {
                MethodContainer = container,
                MethodName = "IntFuncInt"
            };
            var intFuncIntDouble = new SerializedFunc<int, double, int> {
                MethodContainer = container,
                MethodName = "IntFuncIntDouble"
            };
            var intFuncIntDoubleBool = new SerializedFunc<int, double, bool, int> {
                MethodContainer = container,
                MethodName = "IntFuncIntDoubleBool"
            };

            Assert.IsTrue(intFunc.CanInvoke);
            Assert.IsTrue(intFuncInt.CanInvoke);
            Assert.IsTrue(intFuncIntDouble.CanInvoke);
            Assert.IsTrue(intFuncIntDoubleBool.CanInvoke);

            Assert.AreEqual(1, intFunc.Invoke());
            Assert.AreEqual(1, intFuncInt.Invoke(1));
            Assert.AreEqual(1, intFuncIntDouble.Invoke(1, 1.0));
            Assert.AreEqual(1, intFuncIntDoubleBool.Invoke(1, 1.0, true));

            Assert.AreEqual(4, container.InvokeCount);
        }
    }
}