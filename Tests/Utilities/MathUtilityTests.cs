#if !UNITY_2021_3_OR_NEWER
using Assert = NUnit.Framework.Legacy.ClassicAssert;
#endif

using System.Numerics;
using Exanite.Core.Utilities;
using NUnit.Framework;

namespace Exanite.Core.Tests.Utilities
{
    [TestFixture]
    public partial class MathUtilityTests
    {
        [TestCase(5, 0, 10, 0, 100, ExpectedResult = 50f)]
        public float RemapFloat_ReturnsExpectedResult(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return MathUtility.Remap(value, fromMin, fromMax, toMin, toMax);
        }

        [TestCase(10, 0, 3, ExpectedResult = 1f)]
        [TestCase(2, 0, 3, ExpectedResult = 2f)]
        public float WrapFloat_ReturnsExpectedResult(float value, float min, float max)
        {
            return MathUtility.Wrap(value, min, max);
        }

        [TestCase(5, 2, ExpectedResult = 1f)]
        [TestCase(6, 2, ExpectedResult = 0f)]
        [TestCase(-5, 2, ExpectedResult = 1f)]
        public float ModuloFloat_ReturnsExpectedResult(float value, float divisor)
        {
            return MathUtility.Modulo(value, divisor);
        }

        [TestCase(45, 11, ExpectedResult = 44)]
        public int GetNearestMultiple_ReturnsExpectedResult(int value, int multiple)
        {
            return MathUtility.GetNearestMultiple(value, multiple);
        }

        [TestCase(0, ExpectedResult = 2)]
        [TestCase(2, ExpectedResult = 2)]
        [TestCase(5, ExpectedResult = 8)]
        [TestCase(16, ExpectedResult = 16)]
        [TestCase(123, ExpectedResult = 128)]
        public int GetNextPowerOfTwo_ReturnsExpectedResult(int value)
        {
            return MathUtility.GetNextPowerOfTwo(value);
        }

        [TestCase(0, float.Epsilon, ExpectedResult = true)]
        [TestCase(0, 1, ExpectedResult = false)]
        public bool IsApproximatelyEqual_ReturnsExpectedResult(float a, float b)
        {
            return MathUtility.IsApproximatelyEqual(a, b);
        }

        [TestCase(0, double.Epsilon, ExpectedResult = true)]
        [TestCase(0, 1, ExpectedResult = false)]
        public bool IsApproximatelyEqual_ReturnsExpectedResult(double a, double b)
        {
            return MathUtility.IsApproximatelyEqual(a, b);
        }

        [TestCase]
        public void CreatePlane_ReturnsExpectedResult1()
        {
            var plane = MathUtility.CreatePlane(Vector3.UnitX, Vector3.Zero);

            Assert.IsTrue(MathUtility.IsApproximatelyEqual(0, plane.D));
            Assert.IsTrue(MathUtility.IsApproximatelyEqual(Vector3.UnitX, plane.Normal));
        }

        [TestCase]
        public void CreatePlane_ReturnsExpectedResult2()
        {
            var plane = MathUtility.CreatePlane(Vector3.UnitX, Vector3.UnitX);

            Assert.IsTrue(MathUtility.IsApproximatelyEqual(-1, plane.D));
            Assert.IsTrue(MathUtility.IsApproximatelyEqual(Vector3.UnitX, plane.Normal));
        }
    }
}
