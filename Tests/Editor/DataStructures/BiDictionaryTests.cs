using Exanite.Core.DataStructures;
using NUnit.Framework;

namespace Exanite.Core.Tests.Editor.DataStructures
{
    public class BiDictionaryTests
    {
        [Test]
        public void Indexer_EntryExists_ReturnsExpectedEntry()
        {
            BiDictionary<string, int> dictionary = new BiDictionary<string, int>
            {
                ["id"] = 5,
            };

            Assert.AreEqual(dictionary["id"], 5);
        }

        [Test]
        public void InverseIndexer_EntryExists_ReturnsExpectedEntry()
        {
            BiDictionary<string, int> dictionary = new BiDictionary<string, int>
            {
                ["id"] = 5,
            };
            
            Assert.AreEqual(dictionary.Inverse[5], "id");
        }

        [Test]
        [TestCase(0, ExpectedResult = 0)]
        [TestCase(5, ExpectedResult = 5)]
        [TestCase(20, ExpectedResult = 20)]
        public int Count_ReturnsExpectedValue(int entriesToAdd)
        {
            BiDictionary<string, int> dictionary = new BiDictionary<string, int>();

            for (int i = 0; i < entriesToAdd; i++)
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
            BiDictionary<string, int> dictionary = new BiDictionary<string, int>();

            for (int i = 0; i < entriesToAdd; i++)
            {
                dictionary.Add(i.ToString(), i);
            }

            for (int i = 0; i < entriesToRemove; i++)
            {
                dictionary.Remove(i.ToString());
            }

            return dictionary.Count;
        }

        [Test]
        public void Inverse_CalledTwice_ReturnsOriginal()
        {
            BiDictionary<string, int> dictionary = new BiDictionary<string, int>();

            Assert.AreEqual(dictionary, dictionary.Inverse.Inverse);
        }
    }
}
