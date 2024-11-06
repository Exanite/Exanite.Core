using System.Numerics;
using Exanite.Core.Numerics;
using Exanite.Core.Utilities;
using NUnit.Framework;

#if !UNITY_2021_3_OR_NEWER
using Assert = NUnit.Framework.Legacy.ClassicAssert;
#endif

namespace Exanite.Core.Tests.Utilities
{
    [TestFixture]
    public partial class MathUtilityTests
    {
        #region Ranges

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

        #endregion

        #region Floats

        [TestCase(0, 10, 1, ExpectedResult = 1f)]
        [TestCase(0, 10, -1, ExpectedResult = -1f)]
        [TestCase(0, 10, 100, ExpectedResult = 10f)]
        public float MoveTowards_ReturnsExpectedResult(float current, float target, float maxDelta)
        {
            return MathUtility.MoveTowards(current, target, maxDelta);
        }

        [TestCase(0, 90, 30, 30f)] // Straightforward
        [TestCase(0, -270, 30, 30f)] // Wrap
        [TestCase(0, 10, 5, 5f)] // Counter-clockwise
        [TestCase(0, -10, 5, -5f)] // Clockwise
        public void MoveTowardsAngleDegrees_ReturnsExpectedResult(float current, float target, float maxDelta, float expectedResult)
        {
            var result = MathUtility.MoveTowardsAngleDegrees(current, target, maxDelta);
            Assert.AreEqual(expectedResult, result, 0.01f);
        }

        /// <remarks>
        /// Cases are copied from <see cref="MoveTowardsAngleDegrees_ReturnsExpectedResult"/>.
        /// </remarks>
        [TestCase(0, 90, 30, 30f)] // Straightforward
        [TestCase(0, -270, 30, 30f)] // Wrap
        [TestCase(0, 10, 5, 5f)] // Counter-clockwise
        [TestCase(0, -10, 5, -5f)] // Clockwise
        public void MoveTowardsAngleRadians_ReturnsExpectedResult(float current, float target, float maxDelta, float expectedResult)
        {
            var result = MathUtility.Rad2Deg(MathUtility.MoveTowardsAngleRadians(MathUtility.Deg2Rad(current), MathUtility.Deg2Rad(target), MathUtility.Deg2Rad(maxDelta)));
            Assert.AreEqual(expectedResult, result, 0.01f);
        }

        #endregion

        #region Integers

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

        #endregion

        #region IsApproximatelyEqual

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

        #endregion

        #region Planes

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

        [TestCase]
        public void PlaneRaycast_ReturnsExpectedResult1()
        {
            var expectedDistance = 10;
            var expectedPosition = new Vector2(1, 2);

            var plane = MathUtility.CreatePlane(Vector3.UnitZ, Vector3.Zero);
            var ray = new Ray(new Vector3(expectedPosition.X, expectedPosition.Y, -expectedDistance), Vector3.UnitZ, 0);

            var isHit = plane.Raycast(ray, out var distance);

            Assert.IsTrue(isHit);
            Assert.IsTrue(MathUtility.IsApproximatelyEqual(expectedDistance, distance));
            Assert.IsTrue(MathUtility.IsApproximatelyEqual(expectedPosition.Xy0(), ray.GetPoint(distance)));
        }

        [TestCase]
        public void PlaneRaycast_ReturnsExpectedResult2()
        {
            var plane = MathUtility.CreatePlane(Vector3.UnitZ, Vector3.Zero);
            var ray = new Ray(new Vector3(1, 2, -10), -Vector3.UnitZ, 0);

            var isHit = plane.Raycast(ray, out _);

            Assert.IsFalse(isHit);
        }

        #endregion
    }
}
