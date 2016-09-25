using FullInspector.Internal;
using NUnit.Framework;

namespace FullInspector.Tests {
    public class DisplayNameMapperTests {
        [Test]
        public void DisplayNameMapTest() {
            Assert.AreEqual("", fiDisplayNameMapper.Map(null));
            Assert.AreEqual("", fiDisplayNameMapper.Map(""));
            Assert.AreEqual("&&1q", fiDisplayNameMapper.Map("&&1q"));

            Assert.AreEqual("My Property", fiDisplayNameMapper.Map("myProperty"));
            Assert.AreEqual("My Property", fiDisplayNameMapper.Map("my_Property"));
            Assert.AreEqual("My Property", fiDisplayNameMapper.Map("_myProperty"));
            Assert.AreEqual("My Property", fiDisplayNameMapper.Map("_my_Property"));
            Assert.AreEqual("My Property", fiDisplayNameMapper.Map("_my__Property"));
            Assert.AreEqual("My Property", fiDisplayNameMapper.Map("___myProperty"));

            Assert.AreEqual("My Property", fiDisplayNameMapper.Map("MyProperty"));
            Assert.AreEqual("My Property", fiDisplayNameMapper.Map("My_Property"));
            Assert.AreEqual("My Property", fiDisplayNameMapper.Map("_MyProperty"));
            Assert.AreEqual("My Property", fiDisplayNameMapper.Map("_My_Property"));
            Assert.AreEqual("My Property", fiDisplayNameMapper.Map("_My__Property"));
            Assert.AreEqual("My Property", fiDisplayNameMapper.Map("___MyProperty"));

            Assert.AreEqual("Aa", fiDisplayNameMapper.Map("aa"));
            Assert.AreEqual("AA", fiDisplayNameMapper.Map("a_a"));

            Assert.AreEqual("ID", fiDisplayNameMapper.Map("_iD"));
            Assert.AreEqual("ID", fiDisplayNameMapper.Map("___iD"));
            Assert.AreEqual("ID", fiDisplayNameMapper.Map("_ID"));
            Assert.AreEqual("ID", fiDisplayNameMapper.Map("iD"));
            Assert.AreEqual("ID", fiDisplayNameMapper.Map("ID"));

            Assert.AreEqual("IABD", fiDisplayNameMapper.Map("_iABD"));
            Assert.AreEqual("IABD", fiDisplayNameMapper.Map("___iABD"));
            Assert.AreEqual("IABD", fiDisplayNameMapper.Map("_IABD"));
            Assert.AreEqual("IABD", fiDisplayNameMapper.Map("iABD"));
            Assert.AreEqual("IABD", fiDisplayNameMapper.Map("IABD"));

            Assert.AreEqual("_", fiDisplayNameMapper.Map("_"));
            Assert.AreEqual("___", fiDisplayNameMapper.Map("___"));
        }
    }
}