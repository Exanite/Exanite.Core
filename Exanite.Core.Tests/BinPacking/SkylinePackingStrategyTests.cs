using System;
using System.Collections.Generic;
using Exanite.Core.BinPacking;
using Exanite.Core.Collections;
using Exanite.Core.Numerics;
using Xunit;

namespace Exanite.Core.Tests.BinPacking;

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

    [Fact]
    public void Stress1()
    {
        var packerSize = new Vector2Int(1024, 1024);
        var packer = new SkylinePackingStrategy(packerSize);
        var random = new Random(123);
        var bitmap = new BitSet();
        var rects = new List<Rect2Int>();
        var usedArea = 0;

        for (var i = 0; i < 100000; i++)
        {
            var size = new Vector2Int(random.Next(1, 16 + 1), random.Next(1, 16 + 1));
            if (!packer.TryAdd(size, out var rect))
            {
                break;
            }

            Assert.Equal(size, rect.Size);
            TrackRect(rect, i, packerSize, bitmap, rects);
            usedArea += rect.Size.X * rect.Size.Y;
        }

        Assert.Equal(rects.Count, packer.AddedCount);
        Console.WriteLine($"Total rects packed: {rects.Count}");
        Console.WriteLine($"Packed using skyline: {packer.SkylineAddedCount}");
        Console.WriteLine($"Packed using waste map: {packer.WasteMapAddedCount}");
        Console.WriteLine($"Area utilization: {(float)usedArea / (packerSize.X * packerSize.Y)}");
    }

    [Fact]
    public void Stress2()
    {
        var packerSize = new Vector2Int(1024, 1024);
        var packer = new SkylinePackingStrategy(packerSize);
        var random = new Random(456);
        var bitmap = new BitSet();
        var rects = new List<Rect2Int>();
        var usedArea = 0;

        for (var i = 0; i < 100000; i++)
        {
            var size = new Vector2Int(random.Next(2, 16 + 1), random.Next(2, 16 + 1));
            if (!packer.TryAdd(size, out var rect))
            {
                break;
            }

            Assert.Equal(size, rect.Size);
            TrackRect(rect, i, packerSize, bitmap, rects);
            usedArea += rect.Size.X * rect.Size.Y;
        }

        Assert.Equal(rects.Count, packer.AddedCount);
        Console.WriteLine($"Total rects packed: {rects.Count}");
        Console.WriteLine($"Packed using skyline: {packer.SkylineAddedCount}");
        Console.WriteLine($"Packed using waste map: {packer.WasteMapAddedCount}");
        Console.WriteLine($"Area utilization: {(float)usedArea / (packerSize.X * packerSize.Y)}");
    }

    [Fact]
    public void Stress3()
    {
        var packerSize = new Vector2Int(1024, 1024);
        var packer = new SkylinePackingStrategy(packerSize);
        var random = new Random(456);
        var bitmap = new BitSet();
        var rects = new List<Rect2Int>();
        var usedArea = 0;

        for (var i = 0; i < 100000; i++)
        {
            var size = new Vector2Int(random.Next(2, 8 + 1), random.Next(2, 8 + 1));
            if (!packer.TryAdd(size, out var rect))
            {
                break;
            }

            Assert.Equal(size, rect.Size);
            TrackRect(rect, i, packerSize, bitmap, rects);
            usedArea += rect.Size.X * rect.Size.Y;
        }

        Assert.Equal(rects.Count, packer.AddedCount);
        Console.WriteLine($"Total rects packed: {rects.Count}");
        Console.WriteLine($"Packed using skyline: {packer.SkylineAddedCount}");
        Console.WriteLine($"Packed using waste map: {packer.WasteMapAddedCount}");
        Console.WriteLine($"Area utilization: {(float)usedArea / (packerSize.X * packerSize.Y)}");
    }

    [Fact]
    public void Stress4()
    {
        var packerSize = new Vector2Int(1024, 1024);
        var packer = new SkylinePackingStrategy(packerSize);
        var random = new Random(456);
        var bitmap = new BitSet();
        var rects = new List<Rect2Int>();
        var usedArea = 0;

        for (var i = 0; i < 100000; i++)
        {
            var size = new Vector2Int(random.Next(8, 16 + 1), random.Next(8, 16 + 1));
            if (!packer.TryAdd(size, out var rect))
            {
                break;
            }

            Assert.Equal(size, rect.Size);
            TrackRect(rect, i, packerSize, bitmap, rects);
            usedArea += rect.Size.X * rect.Size.Y;
        }

        Assert.Equal(rects.Count, packer.AddedCount);
        Console.WriteLine($"Total rects packed: {rects.Count}");
        Console.WriteLine($"Packed using skyline: {packer.SkylineAddedCount}");
        Console.WriteLine($"Packed using waste map: {packer.WasteMapAddedCount}");
        Console.WriteLine($"Area utilization: {(float)usedArea / (packerSize.X * packerSize.Y)}");
    }

    [Fact]
    public void Stress_VaryingScales()
    {
        var packerSize = new Vector2Int(1024, 1024);
        var packer = new SkylinePackingStrategy(packerSize);
        var random = new Random(456);
        var bitmap = new BitSet();
        var rects = new List<Rect2Int>();
        var usedArea = 0;

        for (var i = 0; i < 100000; i++)
        {
            var size = new Vector2Int(random.Next(8, 16 + 1), random.Next(8, 16 + 1)) * random.Next(1, 4 + 1);
            if (!packer.TryAdd(size, out var rect))
            {
                break;
            }

            Assert.Equal(size, rect.Size);
            TrackRect(rect, i, packerSize, bitmap, rects);
            usedArea += rect.Size.X * rect.Size.Y;
        }

        Assert.Equal(rects.Count, packer.AddedCount);
        Console.WriteLine($"Total rects packed: {rects.Count}");
        Console.WriteLine($"Packed using skyline: {packer.SkylineAddedCount}");
        Console.WriteLine($"Packed using waste map: {packer.WasteMapAddedCount}");
        Console.WriteLine($"Area utilization: {(float)usedArea / (packerSize.X * packerSize.Y)}");
    }

    private void TrackRect(Rect2Int rect, int iteration, Vector2Int packerSize, BitSet bitmap, List<Rect2Int> rects)
    {
        // This is to ensure that rects are not double allocated
        for (var xI = 0; xI < rect.Size.X; xI++)
        {
            for (var yI = 0; yI < rect.Size.Y; yI++)
            {
                var x = rect.Offset.X + xI;
                var y = rect.Offset.Y + yI;
                var index = y * packerSize.X + x;

                if (bitmap[index])
                {
                    foreach (var existing in rects)
                    {
                        if (existing.Intersects(rect))
                        {
                            Assert.Fail($"Current rect intersects existing rect. Current: {rect}. Existing: {rect}. Iteration: {iteration}");
                        }
                    }

                    // This should not be hit. If so, it's a bug in the test itself
                    Assert.False(bitmap[index]);
                }

                bitmap[index] = true;
            }
        }

        rects.Add(rect);
    }

    private void AddAndAssert(SkylinePackingStrategy packer, Vector2Int size, Vector2Int expectedPosition)
    {
        Assert.True(packer.TryAdd(size, out var rect));
        Assert.Equal(Rect2Int.FromOffsetSize(expectedPosition, size), rect);
    }
}
