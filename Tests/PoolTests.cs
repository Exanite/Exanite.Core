using System.Collections.Generic;
using Exanite.Core.Pooling;
using NUnit.Framework;

#if !UNITY_2021_3_OR_NEWER
using Assert = NUnit.Framework.Legacy.ClassicAssert;
#endif

namespace Exanite.Core.Tests
{
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

            Assert.AreEqual(acquireCount, active.Count);
            Assert.AreEqual(acquireCount, pool.CountAll);
            Assert.AreEqual(acquireCount, pool.CountActive);
            Assert.AreEqual(0, pool.CountInactive);
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

            Assert.AreEqual(acquireCountA, pool.CountAll);
            Assert.AreEqual(acquireCountA, pool.CountActive);
            Assert.AreEqual(0, pool.CountInactive);

            foreach (var instance in active)
            {
                pool.Release(instance);
            }

            active.Clear();

            Assert.AreEqual(acquireCountA, pool.CountAll);
            Assert.AreEqual(0, pool.CountActive);
            Assert.AreEqual(acquireCountA, pool.CountInactive);

            for (var i = 0; i < acquireCountB; i++)
            {
                active.Add(pool.Acquire());
            }

            Assert.AreEqual(acquireCountA, pool.CountAll);
            Assert.AreEqual(acquireCountB, pool.CountActive);
            Assert.AreEqual(acquireCountA - acquireCountB, pool.CountInactive);
        }

        [TestCase]
        public void Acquire_EnforcesMaxInactiveLimit()
        {
            const int acquireCountA = 20;
            const int maxInactive = 5;

            var pool = new Pool<A>(create: () => new A(), maxInactive: maxInactive);
            var active = new List<A>();

            Assert.AreEqual(maxInactive, pool.MaxInactive);

            for (var i = 0; i < acquireCountA; i++)
            {
                active.Add(pool.Acquire());
            }

            Assert.AreEqual(acquireCountA, pool.CountAll);
            Assert.AreEqual(acquireCountA, pool.CountActive);
            Assert.AreEqual(0, pool.CountInactive);

            foreach (var instance in active)
            {
                pool.Release(instance);
                Assert.IsTrue(pool.CountInactive <= maxInactive);
            }

            Assert.AreEqual(maxInactive, pool.CountInactive);
        }

        public class A {}
    }
}
