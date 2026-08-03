using Exanite.Core.Numerics;
using Exanite.Core.SpritePacking;
using Xunit;

namespace Exanite.Core.Tests.SpritePacking;

public class SkylinePackingStrategyTests
{
    [Fact]
    public void TryAdd()
    {
        var packer = new SkylinePackingStrategy(new Vector2Int(64, 64));

        // First row
        AddAndAssert(packer, new Vector2Int(16, 16), new Vector2Int(0, 0));
        AddAndAssert(packer, new Vector2Int(16, 16), new Vector2Int(16, 0));
        AddAndAssert(packer, new Vector2Int(16, 16), new Vector2Int(32, 0));

        // Second row
        AddAndAssert(packer, new Vector2Int(24, 16), new Vector2Int(0, 16));

        // Should be added to remaining space in first row
        AddAndAssert(packer, new Vector2Int(16, 16), new Vector2Int(48, 0));
    }

    [Fact]
    public void WasteMap()
    {
        var packer = new SkylinePackingStrategy(new Vector2Int(4, 4));

        // Tall
        AddAndAssert(packer, new Vector2Int(1, 3), new Vector2Int(0, 0));

        // Wide and forms a roof
        AddAndAssert(packer, new Vector2Int(4, 1), new Vector2Int(0, 3));

        // Small, should be inserted into waste map space
        AddAndAssert(packer, new Vector2Int(1, 1), new Vector2Int(1, 0));
    }

    private void AddAndAssert(SkylinePackingStrategy packer, Vector2Int size, Vector2Int expectedPosition)
    {
        Assert.True(packer.TryAdd(size, out var rect));
        Assert.Equal(Rect2Int.FromOffsetSize(expectedPosition, size), rect);
    }
}
