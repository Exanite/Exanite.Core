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

        if (TryFindBin(size, out var candidateBin))
        {
            rect = AddToBin(size, candidateBin.BinIndex, candidateBin.BinCount);
            return true;
        }

        rect = default;
        return false;
    }

    private bool TryFindBin(Vector2Int size, out CandidateBin candidateBin)
    {
        var result = default(CandidateBin?);
        for (var binI = 0; binI < bins.Count; binI++)
        {
            if (CanAddToBin(size, binI, out var binCount))
            {
                if (!result.HasValue)
                {
                    result = new CandidateBin(binI, binCount);
                    continue;
                }

                var currentBin = bins[binI];
                var existingBin = bins[result.Value.BinIndex];
                if (currentBin.Position.Y < existingBin.Position.Y)
                {
                    result = new CandidateBin(binI, binCount);
                }
            }
        }

        candidateBin = result.GetValueOrDefault();
        return result != null;
    }

    private bool CanAddToBin(Vector2Int size, int binIndex, out int binCount)
    {
        var firstBin = bins[binIndex];
        var freeHeight = totalSize.Y - firstBin.Position.Y;
        if (freeHeight < size.Y)
        {
            binCount = 0;
            return false;
        }

        var freeWidth = 0;
        for (var binI = binIndex; binI < bins.Count; binI++)
        {
            var bin = bins[binI];
            if (bin.Position.Y > firstBin.Position.Y)
            {
                // Bin is higher than starting bin -> Cannot fit
                binCount = 0;
                return false;
            }

            freeWidth += bin.Width;
            if (freeWidth >= size.X)
            {
                binCount = binI - binIndex + 1;
                return true;
            }
        }

        binCount = 0;
        return false;
    }

    private Rect2Int AddToBin(Vector2Int size, int binIndex, int binCount)
    {
        var firstBin = bins[binIndex];
        var lastBin = bins[binIndex + binCount - 1];
        var freeHeight = totalSize.Y - firstBin.Position.Y;
        var freeWidth = 0;
        for (var binI = binIndex; binI < binIndex + binCount; binI++)
        {
            var bin = bins[binI];
            freeWidth += bin.Width;
        }

        // Remove overlapped bins
        for (var overlapI = binIndex; overlapI < binIndex + binCount; overlapI++)
        {
            bins.RemoveAt(overlapI);

            // TODO: Add to waste map
        }

        // Split last bin if there is remaining space
        if (freeWidth > size.X)
        {
            var rightCornerX = lastBin.Position.X + lastBin.Width;
            var remainingWidth = freeWidth - size.X;
            bins.Insert(binIndex, new Bin(new Vector2Int(rightCornerX - remainingWidth, lastBin.Position.Y), remainingWidth));
        }

        // Define output rect
        var outputRect = Rect2Int.FromOffsetSize(firstBin.Position, size);

        // Add new bin representing top of inserted rect if there is remaining space
        if (freeHeight > size.Y)
        {
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

        return outputRect;
    }

    private record struct Bin(Vector2Int Position, int Width);

    private record struct CandidateBin(int BinIndex, int BinCount);
}
