using System.Collections.Generic;
using Exanite.Core.Pooling;
using Xunit;

namespace Exanite.Core.Tests;

public class PoolTests
{
    [Fact]
    public void Acquire_AcquireMultipleWithoutReleasing_ReturnsUniqueInstances()
    {
        const int acquireCount = 20;

        var pool = new Pool<A>(create: () => new A());
        var active = new HashSet<A>();

        for (var i = 0; i < acquireCount; i++)
        {
            active.Add(pool.Acquire());
        }

        Assert.Equal(acquireCount, active.Count);
        Assert.Equal(acquireCount, pool.UsageInfo.TotalCount);
        Assert.Equal(acquireCount, pool.UsageInfo.ActiveCount);
        Assert.Equal(0, pool.UsageInfo.InactiveCount);
    }

    [Fact]
    public void Acquire_ReusesInstances()
    {
        const int acquireCountA = 20;
        const int acquireCountB = 10;

        var pool = new Pool<A>(create: () => new A());
        var active = new List<A>();

        for (var i = 0; i < acquireCountA; i++)
        {
            active.Add(pool.Acquire());
        }

        Assert.Equal(acquireCountA, pool.UsageInfo.TotalCount);
        Assert.Equal(acquireCountA, pool.UsageInfo.ActiveCount);
        Assert.Equal(0, pool.UsageInfo.InactiveCount);

        foreach (var instance in active)
        {
            pool.Release(instance);
        }

        active.Clear();

        Assert.Equal(acquireCountA, pool.UsageInfo.TotalCount);
        Assert.Equal(0, pool.UsageInfo.ActiveCount);
        Assert.Equal(acquireCountA, pool.UsageInfo.InactiveCount);

        for (var i = 0; i < acquireCountB; i++)
        {
            active.Add(pool.Acquire());
        }

        Assert.Equal(acquireCountA, pool.UsageInfo.TotalCount);
        Assert.Equal(acquireCountB, pool.UsageInfo.ActiveCount);
        Assert.Equal(acquireCountA - acquireCountB, pool.UsageInfo.InactiveCount);
    }

    [Fact]
    public void Acquire_EnforcesMaxInactiveLimit()
    {
        const int acquireCountA = 20;
        const int maxInactive = 5;

        var pool = new Pool<A>(create: () => new A(), initialMaxInactive: maxInactive, allowResizing: false);
        var active = new List<A>();

        Assert.Equal(maxInactive, pool.UsageInfo.MaxInactive);

        for (var i = 0; i < acquireCountA; i++)
        {
            active.Add(pool.Acquire());
        }

        Assert.Equal(acquireCountA, pool.UsageInfo.TotalCount);
        Assert.Equal(acquireCountA, pool.UsageInfo.ActiveCount);
        Assert.Equal(0, pool.UsageInfo.InactiveCount);

        foreach (var instance in active)
        {
            pool.Release(instance);
            Assert.True(pool.UsageInfo.InactiveCount <= maxInactive);
        }

        Assert.Equal(maxInactive, pool.UsageInfo.InactiveCount);
    }

    public class A;
}
