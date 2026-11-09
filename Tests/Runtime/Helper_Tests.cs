using NUnit.Framework;
using OmiLAXR.Pseudonymization.Helper;

namespace OmiLAXR.Pseudonymization.Tests
{
    public class Helper_Tests
    {
        [Test]
        public void UriHelper_Combine_Appends_Relative_Paths()
        {
            // Assert
            Assert.AreEqual("https://example.org/pseudo/en",
                UriHelper.CombineToString("https://example.org/pseudo/", "en"));
            Assert.AreEqual("https://example.org/a/b/c",
                UriHelper.CombineToString("https://example.org/", "a/", "b/", "c"));
            Assert.AreEqual("https://example.org/a/b",
                UriHelper.Combine("https://example.org/", "a/", "b").ToString());
        }

        [Test]
        public void UriHelper_Combine_Absolute_Uri_Wins()
        {
            Assert.AreEqual("https://other.org/x",
                UriHelper.CombineToString("https://example.org/a/", "https://other.org/x"));
        }

        [Test]
        public void QueryBuilder_Builds_Query_And_Skips_Null_Values()
        {
            // Arrange
            var qb = new QueryBuilder("https://example.org/pseudo/en");

            // Act
            qb.Append("capitalize", true).Append("ignored", null).Append("count", 3);

            // Assert
            Assert.AreEqual(2, qb.Entries.Count);
            CollectionAssert.AreEqual(new[] { "capitalize=True", "count=3" }, qb.GetAssignments());
            Assert.AreEqual("capitalize=True&count=3", qb.GetQueryParams());
            Assert.AreEqual("https://example.org/pseudo/en?capitalize=True&count=3", qb.ToString());
        }

        [Test]
        public void QueryBuilder_Clear_Removes_Entries()
        {
            // Arrange
            var qb = new QueryBuilder("https://example.org/").Append("a", 1);

            // Act
            qb.Clear();

            // Assert
            Assert.IsEmpty(qb.Entries);
            Assert.AreEqual("https://example.org/?", qb.ToString());
        }
    }
}
