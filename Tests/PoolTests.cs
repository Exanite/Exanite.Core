using System.Collections.Generic;
using Exanite.Core.Pooling;
using NUnit.Framework;

namespace Exanite.Core.Tests;

[TestFixture]
public class PoolTests
{
    [TestCase]
    public void Acquire_AcquireMultipleWithoutReleasing_ReturnsUniqueInstances()
    {
        const int acquireCount = 20;

        var pool = new Pool<A>(create: () => new A());
        var active = new HashSet<A>();

        for (var i = 0; i < acquireCount; i++)
        {
            active.Add(pool.Acquire());
        }

        Assert.That(active.Count, Is.EqualTo(acquireCount));
        Assert.That(pool.UsageInfo.TotalCount, Is.EqualTo(acquireCount));
        Assert.That(pool.UsageInfo.ActiveCount, Is.EqualTo(acquireCount));
        Assert.That(pool.UsageInfo.InactiveCount, Is.EqualTo(0));
    }

    [TestCase]
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

        Assert.That(pool.UsageInfo.TotalCount, Is.EqualTo(acquireCountA));
        Assert.That(pool.UsageInfo.ActiveCount, Is.EqualTo(acquireCountA));
        Assert.That(pool.UsageInfo.InactiveCount, Is.EqualTo(0));

        foreach (var instance in active)
        {
            pool.Release(instance);
        }

        active.Clear();

        Assert.That(pool.UsageInfo.TotalCount, Is.EqualTo(acquireCountA));
        Assert.That(pool.UsageInfo.ActiveCount, Is.EqualTo(0));
        Assert.That(pool.UsageInfo.InactiveCount, Is.EqualTo(acquireCountA));

        for (var i = 0; i < acquireCountB; i++)
        {
            active.Add(pool.Acquire());
        }

        Assert.That(pool.UsageInfo.TotalCount, Is.EqualTo(acquireCountA));
        Assert.That(pool.UsageInfo.ActiveCount, Is.EqualTo(acquireCountB));
        Assert.That(pool.UsageInfo.InactiveCount, Is.EqualTo(acquireCountA - acquireCountB));
    }

    [TestCase]
    public void Acquire_EnforcesMaxInactiveLimit()
    {
        const int acquireCountA = 20;
        const int maxInactive = 5;

        var pool = new Pool<A>(create: () => new A(), initialMaxInactive: maxInactive, allowResizing: false);
        var active = new List<A>();

        Assert.That(pool.UsageInfo.MaxInactive, Is.EqualTo(maxInactive));

        for (var i = 0; i < acquireCountA; i++)
        {
            active.Add(pool.Acquire());
        }

        Assert.That(pool.UsageInfo.TotalCount, Is.EqualTo(acquireCountA));
        Assert.That(pool.UsageInfo.ActiveCount, Is.EqualTo(acquireCountA));
        Assert.That(pool.UsageInfo.InactiveCount, Is.EqualTo(0));

        foreach (var instance in active)
        {
            pool.Release(instance);
            Assert.That(pool.UsageInfo.InactiveCount <= maxInactive, Is.True);
        }

        Assert.That(pool.UsageInfo.InactiveCount, Is.EqualTo(maxInactive));
    }

    public class A {}
}
