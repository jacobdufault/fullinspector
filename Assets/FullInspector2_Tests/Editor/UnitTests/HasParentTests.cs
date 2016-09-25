using NUnit.Framework;
using FullInspector.Internal;

namespace FullInspector.Tests {
    public class HasParentTests {
        class Parent { }
        class Parent<T> { }
        interface IParent { }
        interface IParent<T> { }

        class NotParent { }
        class NotParent<T> { }
        interface INotParent { }
        interface INotParent<T> { }

        class ChildNonGeneric0 : Parent, IParent, IParent<int> { }
        class ChildNonGeneric1 : Parent<int>, IParent, IParent<int> { }
        class ChildGeneric0<T> : Parent, IParent, IParent<T> { }
        class ChildGeneric1<T> : Parent<T>, IParent, IParent<T> { }

        [Test]
        public void HasParentTest() {
            Assert.IsFalse(typeof(Parent).HasParent(typeof(Parent)));
            Assert.IsFalse(typeof(Parent<int>).HasParent(typeof(Parent<int>)));
            Assert.IsFalse(typeof(Parent<int>).HasParent(typeof(Parent<>)));

            Assert.IsTrue(typeof(ChildNonGeneric0).HasParent(typeof(Parent)));
            Assert.IsTrue(typeof(ChildNonGeneric0).HasParent(typeof(IParent)));
            Assert.IsTrue(typeof(ChildNonGeneric0).HasParent(typeof(IParent<int>)));
            Assert.IsFalse(typeof(ChildNonGeneric0).HasParent(typeof(NotParent)));
            Assert.IsFalse(typeof(ChildNonGeneric0).HasParent(typeof(NotParent<int>)));
            Assert.IsFalse(typeof(ChildNonGeneric0).HasParent(typeof(INotParent)));
            Assert.IsFalse(typeof(ChildNonGeneric0).HasParent(typeof(INotParent<int>)));

            Assert.IsTrue(typeof(ChildNonGeneric1).HasParent(typeof(Parent<int>)));
            Assert.IsTrue(typeof(ChildNonGeneric1).HasParent(typeof(IParent)));
            Assert.IsTrue(typeof(ChildNonGeneric1).HasParent(typeof(IParent<int>)));
            Assert.IsFalse(typeof(ChildNonGeneric1).HasParent(typeof(NotParent)));
            Assert.IsFalse(typeof(ChildNonGeneric1).HasParent(typeof(NotParent<int>)));
            Assert.IsFalse(typeof(ChildNonGeneric1).HasParent(typeof(INotParent)));
            Assert.IsFalse(typeof(ChildNonGeneric1).HasParent(typeof(INotParent<int>)));

            Assert.IsTrue(typeof(ChildGeneric0<int>).HasParent(typeof(Parent)));
            Assert.IsTrue(typeof(ChildGeneric0<int>).HasParent(typeof(IParent)));
            Assert.IsTrue(typeof(ChildGeneric0<int>).HasParent(typeof(IParent<int>)));
            Assert.IsFalse(typeof(ChildGeneric0<int>).HasParent(typeof(NotParent)));
            Assert.IsFalse(typeof(ChildGeneric0<int>).HasParent(typeof(NotParent<int>)));
            Assert.IsFalse(typeof(ChildGeneric0<int>).HasParent(typeof(INotParent)));
            Assert.IsFalse(typeof(ChildGeneric0<int>).HasParent(typeof(INotParent<int>)));

            Assert.IsTrue(typeof(ChildGeneric1<int>).HasParent(typeof(Parent<int>)));
            Assert.IsTrue(typeof(ChildGeneric1<int>).HasParent(typeof(IParent)));
            Assert.IsTrue(typeof(ChildGeneric1<int>).HasParent(typeof(IParent<int>)));
            Assert.IsFalse(typeof(ChildGeneric1<int>).HasParent(typeof(NotParent)));
            Assert.IsFalse(typeof(ChildGeneric1<int>).HasParent(typeof(NotParent<int>)));
            Assert.IsFalse(typeof(ChildGeneric1<int>).HasParent(typeof(INotParent)));
            Assert.IsFalse(typeof(ChildGeneric1<int>).HasParent(typeof(INotParent<int>)));
        }
    }
}