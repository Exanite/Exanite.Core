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

    // TODO: This is incorrect. This looks for the left-most, but not bottom-most, then left-most. Easy enough to fix though.
    private bool TryAddToBin(Vector2Int size, int binIndex, out Rect2Int rect)
    {
        var firstBin = bins[binIndex];
        var remainingHeight = totalSize.Y - firstBin.Position.Y;
        if (remainingHeight < size.Y)
        {
            rect = default;
            return false;
        }

        var freeWidth = 0;
        for (var binI = binIndex; binI < bins.Count; binI++)
        {
            var bin = bins[binI];
            if (bin.Position.Y > firstBin.Position.Y)
            {
                // Bin is higher than starting bin -> Cannot fit
                rect = default;
                return false;
            }

            freeWidth += bin.Width;
            if (freeWidth >= size.X)
            {
                // We have enough space -> Insert

                // Remove overlapped bins
                for (var overlapI = binIndex; overlapI <= binI; overlapI++)
                {
                    bins.RemoveAt(overlapI);

                    // TODO: Add to waste map
                }

                // Split last bin if it has remaining space
                if (freeWidth > size.X)
                {
                    var rightCornerX = bin.Position.X + bin.Width;
                    var remainingWidth = freeWidth - size.X;
                    bins.Insert(binIndex, new Bin(new Vector2Int(rightCornerX - remainingWidth, bin.Position.Y), remainingWidth));
                }

                // Define output rect
                rect = Rect2Int.FromOffsetSize(firstBin.Position, size);

                if (remainingHeight - size.Y > 0)
                {
                    // Add new bin representing top of inserted rect
                    var newBin = new Bin(firstBin.Position + new Vector2Int(0, size.Y), size.X);
                    bins.Insert(binIndex, newBin);

                    // Merge adjacent
                    if (binIndex - 1 >= 0)
                    {
                        var previousBin = bins[binIndex - 1];
                        if (previousBin.Position.Y == newBin.Position.Y)
                        {
                            newBin = new Bin(previousBin.Position, previousBin.Width + newBin.Width);
                            bins[binIndex - 1] = newBin;
                            bins.RemoveAt(binIndex);
                            binIndex--;
                        }
                    }

                    if (binIndex + 1 < bins.Count)
                    {
                        var nextBin = bins[binIndex + 1];
                        if (nextBin.Position.Y == newBin.Position.Y)
                        {
                            newBin = new Bin(nextBin.Position, nextBin.Width + newBin.Width);
                            bins[binIndex + 1] = newBin;
                            bins.RemoveAt(binIndex);
                            binIndex--;
                        }
                    }
                }

                return true;
            }
        }

        rect = default;
        return false;
    }

    private record struct Bin(Vector2Int Position, int Width);
}
