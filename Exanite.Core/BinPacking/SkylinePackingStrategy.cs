using System.Collections.Generic;
using System.Runtime.InteropServices;
using Exanite.Core.Numerics;
using Exanite.Core.Utilities;

namespace Exanite.Core.BinPacking;

/// <summary>
/// Skyline packing strategy.
/// </summary>
/// <remarks>
/// Currently designed for online usage (no presorting).
/// Uses the skyline, bottom-left, waste map, bin-next-fit approach.
/// Skyline are merged when y-coordinate matches.
/// <para/>
/// Waste map is implemented as a list of rects sorted by descending area, with the smallest occasionally pruned.
/// Waste map rects are never merged and are split to maximize one rectangle's area over the other.
/// </remarks>
public class SkylinePackingStrategy : IRectPackingStrategy
{
    private const int WasteMapCapacity = 128;
    private const int WasteMapEvictCount = 16;

    private static readonly DescendingRectAreaComparer WasteMapRectComparer = new();

    private readonly Vector2Int totalSize;

    /// <summary>
    /// Sorted by ascending x position.
    /// </summary>
    private readonly List<SkylineBin> skylineBins = new();

    /// <summary>
    /// Sorted by descending width, then by descending height.
    /// </summary>
    private readonly List<Rect2Int> wasteMapBins = new(WasteMapCapacity);

    public SkylinePackingStrategy(Vector2Int size)
    {
        totalSize = size;
        skylineBins.Add(new SkylineBin(Vector2Int.Zero, size.X));
    }

    public bool TryAdd(Vector2Int size, out Rect2Int rect)
    {
        if (TryFindWasteMapBin(size, out var wasteMapBin))
        {
            rect = AddToWasteMapBin(size, wasteMapBin);
            return true;
        }

        if (skylineBins.Count == 0)
        {
            rect = default;
            return false;
        }

        if (TryFindSkylineBin(size, out var skylineBin))
        {
            rect = AddToSkylineBin(size, skylineBin);
            return true;
        }

        rect = default;
        return false;
    }

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

    private Rect2Int AddToSkylineBin(Vector2Int size, CandidateSkylineBin candidateBin)
    {
        var (binIndex, binCount) = candidateBin;

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
            var removedBin = skylineBins[binIndex];
            skylineBins.RemoveAt(binIndex);

            // We handle the last bin below since it is partially overlapped
            if (i < binCount - 1)
            {
                var wasteHeight = firstBin.Position.Y - removedBin.Position.Y;
                if (wasteHeight > 0)
                {
                    var wasteRect = Rect2Int.FromOffsetSize(removedBin.Position, new Vector2Int(removedBin.Width, wasteHeight));
                    AddWasteMapBin(wasteRect);
                }
            }
        }

        // Handle last bin
        {
            var remainingWidth = freeWidth - size.X;
            var usedWidth = lastBin.Width - remainingWidth;

            // Split last bin if there is remaining horizontal space
            if (remainingWidth > 0)
            {
                var rightCornerX = lastBin.Position.X + lastBin.Width;
                skylineBins.Insert(binIndex, new SkylineBin(new Vector2Int(rightCornerX - remainingWidth, lastBin.Position.Y), remainingWidth));
            }

            // Add waste rect if there is remaining vertical space
            var wasteHeight = firstBin.Position.Y - lastBin.Position.Y;
            if (wasteHeight > 0 && usedWidth > 0)
            {
                var wasteRect = Rect2Int.FromOffsetSize(lastBin.Position, new Vector2Int(usedWidth, wasteHeight));
                AddWasteMapBin(wasteRect);
            }
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

    private bool TryFindWasteMapBin(Vector2Int size, out CandidateWasteMapBin candidateBin)
    {
        if (wasteMapBins.Count == 0)
        {
            candidateBin = default;
            return false;
        }

        var index = wasteMapBins.BinarySearch(Rect2Int.FromSize(size), WasteMapRectComparer);
        if (index < 0)
        {
            index = ~index;
        }

        for (var i = M.Min(index, wasteMapBins.Count - 1); i >= 0; i--)
        {
            var bin = wasteMapBins[i];
            if (bin.Size.X >= size.X && bin.Size.Y >= size.Y)
            {
                candidateBin = new CandidateWasteMapBin(i);
                return true;
            }
        }

        candidateBin = default;
        return false;
    }

    private Rect2Int AddToWasteMapBin(Vector2Int size, CandidateWasteMapBin candidateBin)
    {
        var binIndex = candidateBin.BinIndex;
        var bin = wasteMapBins[binIndex];
        wasteMapBins.RemoveAt(binIndex);

        // Split in the direction that maximizes the area of one rectangle over the other
        if (size != bin.Size)
        {
            // H means horizontal split
            // V means vertical split
            var rectAreaH = (bin.Size.Y - size.Y) * bin.Size.X;
            var rectAreaV = (bin.Size.X - size.X) * bin.Size.Y;

            // In case of tie, split vertically since sprites tend to be taller than they are wide
            if (rectAreaV >= rectAreaH)
            {
                // Split vertically
                AddWasteMapBin(Rect2Int.FromOffsetSize(bin.Offset + new Vector2Int(size.X, 0), new Vector2Int(bin.Size.X - size.X, bin.Size.Y)));
                AddWasteMapBin(Rect2Int.FromOffsetSize(bin.Offset + new Vector2Int(0, size.Y), new Vector2Int(size.X, bin.Size.Y - size.Y)));
            }
            else
            {
                // Split horizontally
                AddWasteMapBin(Rect2Int.FromOffsetSize(bin.Offset + new Vector2Int(size.X, 0), new Vector2Int(bin.Size.X - size.X, size.Y)));
                AddWasteMapBin(Rect2Int.FromOffsetSize(bin.Offset + new Vector2Int(0, size.Y), new Vector2Int(bin.Size.X, bin.Size.Y - size.Y)));
            }
        }

        return Rect2Int.FromOffsetSize(bin.Offset, size);
    }

    private void AddWasteMapBin(Rect2Int rect)
    {
        if (wasteMapBins.Count >= WasteMapCapacity)
        {
            // Prune
            CollectionsMarshal.SetCount(wasteMapBins, WasteMapCapacity - WasteMapEvictCount);
        }

        var index = wasteMapBins.BinarySearch(rect, WasteMapRectComparer);
        if (index < 0)
        {
            index = ~index;
        }

        wasteMapBins.Insert(index, rect);
    }

    private record struct SkylineBin(Vector2Int Position, int Width);

    private record struct CandidateSkylineBin(int BinIndex, int BinCount);

    private record struct CandidateWasteMapBin(int BinIndex);

    private class DescendingRectAreaComparer : IComparer<Rect2Int>
    {
        public int Compare(Rect2Int left, Rect2Int right)
        {
            return right.Size.X * right.Size.Y - left.Size.X * left.Size.Y;
        }
    }
}
