using System.Collections.Generic;
using NUnit.Framework;

namespace FullInspector.Tests {
    public class MetadataMigrationTests {
        private static fiGraphMetadata.MetadataMigration Migration(int oldIndex, int newIndex) {
            return new fiGraphMetadata.MetadataMigration {
                OldIndex = oldIndex,
                NewIndex = newIndex
            };
        }

        [Test]
        public void Test() {
            var a = new object();
            var b = new object();
            var c = new object();
            var d = new object();

            object[] from = new[] { a, b, c, d };
            object[] to = new[] { a, new object(), b, c, d };
            List<fiGraphMetadata.MetadataMigration> migrations = fiGraphMetadata.ComputeNeededMigrations(from, to);
            CollectionAssert.AreEqual(
                new List<fiGraphMetadata.MetadataMigration>() {
                    Migration(1, 2),
                    Migration(2, 3),
                    Migration(3, 4)
                },
                migrations
            );

            from = new[] { a, b, c, d };
            to = new[] { new object(), a, b, c, d };
            migrations = fiGraphMetadata.ComputeNeededMigrations(from, to);
            CollectionAssert.AreEqual(
                new List<fiGraphMetadata.MetadataMigration>() {
                    Migration(0, 1),
                    Migration(1, 2),
                    Migration(2, 3),
                    Migration(3, 4)
                },
                migrations
            );

            from = new[] { a, b, c, d };
            to = new[] { b, a, new object(), c, d };
            migrations = fiGraphMetadata.ComputeNeededMigrations(from, to);
            CollectionAssert.AreEqual(
                new List<fiGraphMetadata.MetadataMigration>() {
                    Migration(1, 0),
                    Migration(0, 1),
                    Migration(2, 3),
                    Migration(3, 4)
                },
                migrations);

            from = new[] { a, b, c, d };
            to = new[] { a, b, c, d, new object() };
            migrations = fiGraphMetadata.ComputeNeededMigrations(from, to);
            CollectionAssert.AreEqual(
                new List<fiGraphMetadata.MetadataMigration>(),
                migrations);
        }
    }
}