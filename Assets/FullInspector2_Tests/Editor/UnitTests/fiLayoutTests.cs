using FullInspector.LayoutToolkit;
using NUnit.Framework;
using UnityEngine;

namespace FullInspector.Tests {
    public class LayoutTests {
        [Test]
        public void SimpleVerticalLayout() {
            fiVerticalLayout vertical = new fiVerticalLayout() {
                { "1", 10 },
                { 5 },
                { "2", 20 }
            };

            Assert.AreEqual(10 + 5 + 20, vertical.Height);

            var initialRect = new Rect(0, 0, 100, 100);
            Assert.AreEqual(new Rect(0, 0, 100, 10), vertical.GetSectionRect("1", initialRect));
            Assert.AreEqual(new Rect(0, 15, 100, 20), vertical.GetSectionRect("2", initialRect));
        }

        [Test]
        public void SimpleHorizontalLayout() {
            fiHorizontalLayout horizontal = new fiHorizontalLayout() {
                { "1", 10, new fiLayoutHeight(10) },
                { "2", new fiLayoutHeight(10) },
                { "3", 20, new fiLayoutHeight(20) }
            };

            Assert.AreEqual(20, horizontal.Height);

            var initialRect = new Rect(0, 0, 100, 100);
            Assert.AreEqual(new Rect(0, 0, 10, 10), horizontal.GetSectionRect("1", initialRect));
            Assert.AreEqual(new Rect(10, 0, 100 - 10 - 20, 10), horizontal.GetSectionRect("2", initialRect));
            Assert.AreEqual(new Rect(80, 0, 20, 20), horizontal.GetSectionRect("3", initialRect));
        }

        [Test]
        public void CenterLayout() {
            var centered = new fiCenterVertical(new fiLayoutHeight("1", 20));
            Assert.AreEqual(20, centered.Height);
            Assert.AreEqual(new Rect(0, 40, 100, 20), centered.GetSectionRect("1", new Rect(0, 0, 100, 100)));

            var horizontalContainer = new fiHorizontalLayout() {
                { 30, centered }
            };
            Assert.AreEqual(20, horizontalContainer.Height);
            Assert.AreEqual(new Rect(0, 40, 30, 20), horizontalContainer.GetSectionRect("1", new Rect(0, 0, 100, 100)));

            var verticalContainer = new fiVerticalLayout() {
                { centered }
            };
            Assert.AreEqual(20, verticalContainer.Height);
            Assert.AreEqual(new Rect(0, 40, 100, 20), verticalContainer.GetSectionRect("1", new Rect(0, 0, 100, 100)));
        }
    }
}