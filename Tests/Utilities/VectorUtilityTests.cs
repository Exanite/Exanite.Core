#if UNITY_2021_3_OR_NEWER
using System.Numerics;
using Exanite.Core.Numerics;
using NUnit.Framework;

namespace Exanite.Core.Tests.Utilities
{
    [TestFixture]
    public class VectorUtilityTests
    {
        [Test]
        public void InverseSwizzleVector3_ReversesSwizzleVector3([Values] Vector3Swizzle swizzle)
        {
            var original = new Vector3(1, 2, 3);
            Vector3 result;

            result = original.Swizzle(swizzle).InverseSwizzle(swizzle);
            Assert.AreEqual(original, result);
        }
    }
}
#endif
