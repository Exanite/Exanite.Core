using System.Collections.Generic;
using Exanite.Core.Numerics;

namespace Exanite.Core.SpritePacking;

/// <summary>
/// Skyline packing strategy.
/// </summary>
/// <remarks>
/// Currently designed for online usage (no presorting).
/// Uses the skyline, bottom-left, waste map, bin-next-fit implementation.
/// Bins are merged when y-coordinate matches.
/// Waste map rects are never merged.
/// </remarks>
public class SkylinePackingStrategy : IRectPackingStrategy
{
    private readonly Vector2Int totalSize;

    /// <summary>
    /// Sorted by ascending x position.
    /// </summary>
    private readonly List<Bin> bins = new();

    /// <summary>
    /// Sorted by descending width, then by descending height.
    /// </summary>
    private readonly List<Rect2Int> wasteMap = new();

    public SkylinePackingStrategy(Vector2Int size)
    {
        totalSize = size;
        bins.Add(new Bin(Vector2Int.Zero, size.X));
    }

    public bool TryAdd(Vector2Int size, out Rect2Int rect)
    {
        // Find bin
        for (var i = 0; i < bins.Count; i++)
        {
            var bin = bins[i];
            var remainingHeight = totalSize.Y - bin.Position.Y;
            if (bin.Width >= size.X && remainingHeight > size.Y)
            {

            }
        }

        throw new System.NotImplementedException();
    }

    private record struct Bin(Vector2Int Position, int Width);
}
