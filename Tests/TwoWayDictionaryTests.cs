using Exanite.Core.Collections;
using Xunit;

namespace Exanite.Core.Tests;

public class TwoWayDictionaryTests
{
    [Fact]
    public void Indexer_EntryExists_ReturnsExpectedEntry()
    {
        var dictionary = new TwoWayDictionary<string, int>
        {
            ["id"] = 5,
        };

        Assert.Equal(5, dictionary["id"]);
    }

    [Fact]
    public void InverseIndexer_EntryExists_ReturnsExpectedEntry()
    {
        var dictionary = new TwoWayDictionary<string, int>
        {
            ["id"] = 5,
        };

        Assert.Equal("id", dictionary.Inverse[5]);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(5, 5)]
    [InlineData(20, 20)]
    public void Count_ReturnsExpectedResult(int entriesToAdd, int expectedCount)
    {
        var dictionary = new TwoWayDictionary<string, int>();

        for (var i = 0; i < entriesToAdd; i++)
        {
            dictionary.Add(i.ToString(), i);
        }

        Assert.Equal(expectedCount, dictionary.Count);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(5, 1, 4)]
    [InlineData(20, 3, 17)]
    [InlineData(20, 6, 14)]
    public void Count_AfterRemoving_ReturnsExpectedResult(int entriesToAdd, int entriesToRemove, int expectedCount)
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

        Assert.Equal(expectedCount, dictionary.Count);
    }

    [Fact]
    public void Inverse_CalledTwice_ReturnsOriginal()
    {
        var dictionary = new TwoWayDictionary<string, int>();

        Assert.Equal(dictionary, dictionary.Inverse.Inverse);
    }
}
