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
        Add(new Vector2Int(16, 16), new Vector2Int(0, 0));
        Add(new Vector2Int(16, 16), new Vector2Int(16, 0));
        Add(new Vector2Int(16, 16), new Vector2Int(32, 0));
        Add(new Vector2Int(24, 16), new Vector2Int(0, 16));

        Add(new Vector2Int(16, 16), new Vector2Int(48, 0));

        return;

        void Add(Vector2Int size, Vector2Int expectedPosition)
        {
            Assert.True(packer.TryAdd(size, out var rect));
            Assert.Equal(Rect2Int.FromOffsetSize(expectedPosition, size), rect);
        }
    }
}
