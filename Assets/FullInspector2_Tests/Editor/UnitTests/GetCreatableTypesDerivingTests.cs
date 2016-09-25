using System.Linq;
using System.Collections.Generic;
using FullInspector.Internal;
using NUnit.Framework;
using FullSerializer;

namespace FullInspector.Tests {
    public class GetCreatableTypesDerivingTests {
        private static void AssertContainsType<T>(List<fiReflectionUtility.DisplayedType> types) {
            foreach (var type in types) {
                if (type.Type == typeof(T))
                    return;
            }

            Assert.Fail("Expected " + typeof(T).CSharpName() + " to be in " + types);
        }

        public class AbstractClassTypeTest {
            public abstract class Base { }
            public class Derived : Base { }
            public class DerivedDerived : Derived { }

            [Test]
            public void AbstractClassType() {
                var derived = fiReflectionUtility.GetCreatableTypesDeriving(typeof(Base)).ToList();

                Assert.AreEqual(2, derived.Count);
                AssertContainsType<Derived>(derived);
                AssertContainsType<DerivedDerived>(derived);
            }
        }

        public class ClassTypeTest {
            public class Base { }
            public class Derived : Base { }
            public class DerivedDerived : Derived { }

            [Test]
            public void ClassType() {
                var derived = fiReflectionUtility.GetCreatableTypesDeriving(typeof(Base)).ToList();

                Assert.AreEqual(3, derived.Count);
                AssertContainsType<Base>(derived);
                AssertContainsType<Derived>(derived);
                AssertContainsType<DerivedDerived>(derived);
            }
        }

        public class GenericClassTest {
            public class Base<T1> { }
            public class DerivedG<T1> : Base<T1> { }
            public class DerivedO : Base<object> { }
            public class DerivedDerivedGO : DerivedG<object> { }
            public class DerivedDerivedG<T2> : DerivedG<T2> { }
            public class DerivedDerivedO : DerivedO { }

            [Test]
            public void GenericClass() {
                var derived = fiReflectionUtility.GetCreatableTypesDeriving(typeof(Base<object>)).ToList();

                Assert.AreEqual(6, derived.Count);
                AssertContainsType<Base<object>>(derived);
                AssertContainsType<DerivedO>(derived);
                AssertContainsType<DerivedG<object>>(derived);
                AssertContainsType<DerivedDerivedO>(derived);
                AssertContainsType<DerivedDerivedG<object>>(derived);
                AssertContainsType<DerivedDerivedGO>(derived);
            }

            public class Base<T1, T2> { }
            public class DerivedGG<T1, T2> : Base<T1, T2> { }
            public class DerivedGI<T1> : Base<T1, int> { }
            public class DerivedOG<T2> : Base<object, T2> { }
            public class DerivedOI : Base<object, int> { }
            public class DerivedScrewey<T2, T1> : Base<T1, T2> { }

            [Test]
            public void GenericClassMultipleParameters() {
                var derived = fiReflectionUtility.GetCreatableTypesDeriving(typeof(Base<object, int>)).ToList();

                Assert.AreEqual(6, derived.Count);
                AssertContainsType<Base<object, int>>(derived);
                AssertContainsType<DerivedGG<object, int>>(derived);
                AssertContainsType<DerivedGI<object>>(derived);
                AssertContainsType<DerivedOG<int>>(derived);
                AssertContainsType<DerivedOI>(derived);
                AssertContainsType<DerivedScrewey<int, object>>(derived);
            }
        }

        public class GenericAbstractClassTest {
            public abstract class Base<T1> { }
            public class DerivedG<T1> : Base<T1> { }
            public class DerivedO : Base<object> { }
            public class DerivedDerivedGO : DerivedG<object> { }
            public class DerivedDerivedG<T2> : DerivedG<T2> { }
            public class DerivedDerivedO : DerivedO { }

            [Test]
            public void GenericAbstractClass() {
                var derived = fiReflectionUtility.GetCreatableTypesDeriving(typeof(Base<object>)).ToList();

                Assert.AreEqual(5, derived.Count);
                AssertContainsType<DerivedO>(derived);
                AssertContainsType<DerivedG<object>>(derived);
                AssertContainsType<DerivedDerivedO>(derived);
                AssertContainsType<DerivedDerivedG<object>>(derived);
                AssertContainsType<DerivedDerivedGO>(derived);
            }

            public abstract class Base<T1, T2> { }
            public class DerivedGG<T1, T2> : Base<T1, T2> { }
            public class DerivedGI<T1> : Base<T1, int> { }
            public class DerivedOG<T2> : Base<object, T2> { }
            public class DerivedOI : Base<object, int> { }
            public class DerivedScrewey<T2, T1> : Base<T1, T2> { }

            [Test]
            public void GenericAbstractClassMultipleParameters() {
                var derived = fiReflectionUtility.GetCreatableTypesDeriving(typeof(Base<object, int>)).ToList();

                Assert.AreEqual(5, derived.Count);
                AssertContainsType<DerivedGG<object, int>>(derived);
                AssertContainsType<DerivedGI<object>>(derived);
                AssertContainsType<DerivedOG<int>>(derived);
                AssertContainsType<DerivedOI>(derived);
                AssertContainsType<DerivedScrewey<int, object>>(derived);
            }
        }

        public class GenericInterfaceTest {
            public interface IInterface<T1> { }
            public class DerivedG<T1> : IInterface<T1> { }
            public class DerivedO : IInterface<object> { }
            public class DerivedDerivedGO : DerivedG<object> { }
            public class DerivedDerivedG<T2> : DerivedG<T2> { }
            public class DerivedDerivedO : DerivedO { }

            [Test]
            public void GenericInterface() {
                var derived = fiReflectionUtility.GetCreatableTypesDeriving(typeof(IInterface<object>)).ToList();

                Assert.AreEqual(5, derived.Count);
                AssertContainsType<DerivedO>(derived);
                AssertContainsType<DerivedG<object>>(derived);
                AssertContainsType<DerivedDerivedO>(derived);
                AssertContainsType<DerivedDerivedG<object>>(derived);
                AssertContainsType<DerivedDerivedGO>(derived);
            }

            public interface IInterface<T1, T2> { }
            public class DerivedGG<T1, T2> : IInterface<T1, T2> { }
            public class DerivedGI<T1> : IInterface<T1, int> { }
            public class DerivedOG<T2> : IInterface<object, T2> { }
            public class DerivedOI : IInterface<object, int> { }
            public class DerivedScrewey<T2, T1> : IInterface<T1, T2> { }

            [Test]
            public void GenericInterfaceMultipleParameters() {
                var derived = fiReflectionUtility.GetCreatableTypesDeriving(typeof(IInterface<object, int>)).ToList();

                Assert.AreEqual(5, derived.Count);
                AssertContainsType<DerivedGG<object, int>>(derived);
                AssertContainsType<DerivedGI<object>>(derived);
                AssertContainsType<DerivedOG<int>>(derived);
                AssertContainsType<DerivedOI>(derived);
                AssertContainsType<DerivedScrewey<int, object>>(derived);
            }
        }

        /*
        public class ContravarianceTests {
            public interface IContravariantBase<in T> { }
            public class ContravariantDerivedO : IContravariantBase<object> { }
            public class ContravariantDerivedG<T> : IContravariantBase<T> { }

            [Test]
            public void ContravarianceTest() {
                var derived = ReflectionUtilities.GetCreatableTypesDeriving(typeof(IContravariantBase<string>)).ToList();

                //Debug.Log(derived[0]);
                Assert.AreEqual(2, derived.Count);
                AssertContainsType<ContravariantDerivedO>(derived);
                AssertContainsType<ContravariantDerivedG<string>>(derived);
            }
        }

        public class CovarianceTests {
            public interface ICovariantBase<out T> { }
            public class CovariantDerivedS : ICovariantBase<string> { }
            public class CovariantDerivedG<T> : ICovariantBase<T> { }

            [Test]
            public void CovarianceTest() {
                var derived = ReflectionUtilities.GetCreatableTypesDeriving(typeof(ICovariantBase<object>)).ToList();

                Assert.AreEqual(2, derived.Count);
                AssertContainsType<CovariantDerivedS>(derived);
                AssertContainsType<CovariantDerivedG<object>>(derived);
            }
        }
        */
    }
}
