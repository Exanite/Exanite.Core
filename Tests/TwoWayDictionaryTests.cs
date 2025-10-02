using Exanite.Core.Collections;
using NUnit.Framework;

namespace Exanite.Core.Tests;

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

        Assert.That(dictionary["id"], Is.EqualTo(5));
    }

    [Test]
    public void InverseIndexer_EntryExists_ReturnsExpectedEntry()
    {
        var dictionary = new TwoWayDictionary<string, int>
        {
            ["id"] = 5,
        };

        Assert.That(dictionary.Inverse[5], Is.EqualTo("id"));
    }

    [Test]
    [TestCase(0, ExpectedResult = 0)]
    [TestCase(5, ExpectedResult = 5)]
    [TestCase(20, ExpectedResult = 20)]
    public int Count_ReturnsExpectedResult(int entriesToAdd)
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
    public int Count_AfterRemoving_ReturnsExpectedResult(int entriesToAdd, int entriesToRemove)
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

        Assert.That(dictionary.Inverse.Inverse, Is.EqualTo(dictionary));
    }
}
