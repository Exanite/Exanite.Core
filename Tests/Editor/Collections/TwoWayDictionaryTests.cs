using Exanite.Core.Collections;
using NUnit.Framework;

namespace Exanite.Core.Tests.Editor.Collections
{
    [TestFixture]
    public class TwoWayDictionaryTests
    {
        [Test]
        public void Indexer_EntryExists_ReturnsExpectedEntry()
        {
            var dictionary = new TwoWayDictionary<string, int>
            {
                ["id"] = 5,
            };

            Assert.AreEqual(5, dictionary["id"]);
        }

        [Test]
        public void InverseIndexer_EntryExists_ReturnsExpectedEntry()
        {
            var dictionary = new TwoWayDictionary<string, int>
            {
                ["id"] = 5,
            };

            Assert.AreEqual("id", dictionary.Inverse[5]);
        }

        [Test]
        [TestCase(0, ExpectedResult = 0)]
        [TestCase(5, ExpectedResult = 5)]
        [TestCase(20, ExpectedResult = 20)]
        public int Count_ReturnsExpectedValue(int entriesToAdd)
        {
            var dictionary = new TwoWayDictionary<string, int>();

            for (var i = 0; i < entriesToAdd; i++)
            {
                dictionary.Add(i.ToString(), i);
            }

            return dictionary.Count;
        }

        [Test]
        [TestCase(0, 0, ExpectedResult = 0)]
        [TestCase(5, 1, ExpectedResult = 4)]
        [TestCase(20, 3, ExpectedResult = 17)]
        [TestCase(20, 6, ExpectedResult = 14)]
        public int Count_AfterRemoving_ReturnsExpectedValue(int entriesToAdd, int entriesToRemove)
        {
            var dictionary = new TwoWayDictionary<string, int>();

            for (var i = 0; i < entriesToAdd; i++)
            {
                dictionary.Add(i.ToString(), i);
            }

            for (var i = 0; i < entriesToRemove; i++)
            {
                dictionary.Remove(i.ToString());
            }

            return dictionary.Count;
        }

        [Test]
        public void Inverse_CalledTwice_ReturnsOriginal()
        {
            var dictionary = new TwoWayDictionary<string, int>();

            Assert.AreEqual(dictionary, dictionary.Inverse.Inverse);
        }
    }
}
