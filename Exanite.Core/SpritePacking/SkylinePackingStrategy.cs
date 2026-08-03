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
    private readonly List<SkylineBin> skylineBins = new();

    /// <summary>
    /// Sorted by descending width, then by descending height.
    /// </summary>
    private readonly List<Rect2Int> wasteMapBins = new();

    private Vector2Int smallestSeen = new(int.MaxValue, int.MaxValue);

    public SkylinePackingStrategy(Vector2Int size)
    {
        totalSize = size;
        skylineBins.Add(new SkylineBin(Vector2Int.Zero, size.X));
    }

    public bool TryAdd(Vector2Int size, out Rect2Int rect)
    {
        if (skylineBins.Count == 0)
        {
            rect = default;
            return false;
        }

        if (TryFindSkylineBin(size, out var candidateBin))
        {
            rect = AddToSkylineBin(size, candidateBin.BinIndex, candidateBin.BinCount);
            return true;
        }

        rect = default;
        return false;
    }

    // private bool TryFindWasteMapBin(Vector2Int size)
    // {
    //
    // }

    private bool TryFindSkylineBin(Vector2Int size, out CandidateSkylineBin candidateBin)
    {
        var result = default(CandidateSkylineBin?);
        for (var binI = 0; binI < skylineBins.Count; binI++)
        {
            if (CanAddToSkylineBin(size, binI, out var binCount))
            {
                if (!result.HasValue)
                {
                    result = new CandidateSkylineBin(binI, binCount);
                    continue;
                }

                var currentBin = skylineBins[binI];
                var existingBin = skylineBins[result.Value.BinIndex];
                if (currentBin.Position.Y < existingBin.Position.Y)
                {
                    result = new CandidateSkylineBin(binI, binCount);
                }
            }
        }

        candidateBin = result.GetValueOrDefault();
        return result != null;
    }

    private bool CanAddToSkylineBin(Vector2Int size, int binIndex, out int binCount)
    {
        var firstBin = skylineBins[binIndex];
        var freeHeight = totalSize.Y - firstBin.Position.Y;
        if (freeHeight < size.Y)
        {
            binCount = 0;
            return false;
        }

        var freeWidth = 0;
        for (var binI = binIndex; binI < skylineBins.Count; binI++)
        {
            var bin = skylineBins[binI];
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

    private Rect2Int AddToSkylineBin(Vector2Int size, int binIndex, int binCount)
    {
        var firstBin = skylineBins[binIndex];
        var lastBin = skylineBins[binIndex + binCount - 1];
        var freeHeight = totalSize.Y - firstBin.Position.Y;
        var freeWidth = 0;
        for (var binI = binIndex; binI < binIndex + binCount; binI++)
        {
            var bin = skylineBins[binI];
            freeWidth += bin.Width;
        }

        // Remove overlapped bins
        for (var i = 0; i < binCount; i++)
        {
            skylineBins.RemoveAt(binIndex);

            // TODO: Add to waste map
        }

        // Split last bin if there is remaining space
        if (freeWidth > size.X)
        {
            var rightCornerX = lastBin.Position.X + lastBin.Width;
            var remainingWidth = freeWidth - size.X;
            skylineBins.Insert(binIndex, new SkylineBin(new Vector2Int(rightCornerX - remainingWidth, lastBin.Position.Y), remainingWidth));
        }

        // Define output rect
        var outputRect = Rect2Int.FromOffsetSize(firstBin.Position, size);

        // Add new bin representing top of inserted rect if there is remaining space
        if (freeHeight > size.Y)
        {
            var newBin = new SkylineBin(firstBin.Position + new Vector2Int(0, size.Y), size.X);
            skylineBins.Insert(binIndex, newBin);

            // Merge adjacent
            if (binIndex - 1 >= 0)
            {
                var previousBin = skylineBins[binIndex - 1];
                if (previousBin.Position.Y == newBin.Position.Y)
                {
                    newBin = new SkylineBin(previousBin.Position, previousBin.Width + newBin.Width);
                    skylineBins[binIndex - 1] = newBin;
                    skylineBins.RemoveAt(binIndex);
                    binIndex--;
                }
            }

            if (binIndex + 1 < skylineBins.Count)
            {
                var nextBin = skylineBins[binIndex + 1];
                if (nextBin.Position.Y == newBin.Position.Y)
                {
                    newBin = new SkylineBin(nextBin.Position, nextBin.Width + newBin.Width);
                    skylineBins[binIndex + 1] = newBin;
                    skylineBins.RemoveAt(binIndex);
                    binIndex--;
                }
            }
        }

        return outputRect;
    }

    private record struct SkylineBin(Vector2Int Position, int Width);

    private record struct CandidateSkylineBin(int BinIndex, int BinCount);
}
