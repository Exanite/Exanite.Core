using System.Collections.Generic;
using Exanite.Core.Numerics;
using Exanite.Core.Utilities;

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
        if (bins.Count == 0)
        {
            rect = default;
            return false;
        }

        for (var i = 0; i < bins.Count; i++)
        {
            if (TryAddToBin(size, i, out rect))
            {
                return true;
            }
        }

        rect = default;
        return false;
    }

    private bool TryAddToBin(Vector2Int size, int binIndex, out Rect2Int rect)
    {
        var firstBin = bins[0];
        var freeWidth = 0;
        for (var i = 0; i < bins.Count; i++)
        {
            var bin = bins[i];
            if (bin.Position.Y >= firstBin.Position.Y)
            {
                // Bin is higher than starting bin -> Cannot fit
                rect = default;
                return false;
            }

            freeWidth += bin.Width;
            if (freeWidth >= size.X)
            {
                // Found location -> Insert
            }
        }

        rect = default;
        return false;
    }

    private record struct Bin(Vector2Int Position, int Width);
}
