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
            Assert.AreEqual(acquireCount, pool.TotalCount);
            Assert.AreEqual(acquireCount, pool.ActiveCount);
            Assert.AreEqual(0, pool.InactiveCount);
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

            Assert.AreEqual(acquireCountA, pool.TotalCount);
            Assert.AreEqual(acquireCountA, pool.ActiveCount);
            Assert.AreEqual(0, pool.InactiveCount);

            foreach (var instance in active)
            {
                pool.Release(instance);
            }

            active.Clear();

            Assert.AreEqual(acquireCountA, pool.TotalCount);
            Assert.AreEqual(0, pool.ActiveCount);
            Assert.AreEqual(acquireCountA, pool.InactiveCount);

            for (var i = 0; i < acquireCountB; i++)
            {
                active.Add(pool.Acquire());
            }

            Assert.AreEqual(acquireCountA, pool.TotalCount);
            Assert.AreEqual(acquireCountB, pool.ActiveCount);
            Assert.AreEqual(acquireCountA - acquireCountB, pool.InactiveCount);
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

            Assert.AreEqual(acquireCountA, pool.TotalCount);
            Assert.AreEqual(acquireCountA, pool.ActiveCount);
            Assert.AreEqual(0, pool.InactiveCount);

            foreach (var instance in active)
            {
                pool.Release(instance);
                Assert.IsTrue(pool.InactiveCount <= maxInactive);
            }

            Assert.AreEqual(maxInactive, pool.InactiveCount);
        }

        public class A {}
    }
}
