using System.Numerics;
using Exanite.Core.Numerics;
using Exanite.Core.Utilities;
using NUnit.Framework;

namespace Exanite.Core.Tests.Utilities;

[TestFixture]
public class MathUtilityTests
{
    #region Ranges

    [TestCase(13, 5, 15, 0, 100, ExpectedResult = 80f, TestName = "Basic case")]
    [TestCase(13, 15, 5, 0, 100, ExpectedResult = 20f, TestName = "Reversed from range")]
    [TestCase(13, 5, 15, 100, 0, ExpectedResult = 20f, TestName = "Reversed to range")]
    [TestCase(16, 5, 15, 0, 100, ExpectedResult = 100f, TestName = "Should clamp")]
    [TestCase(16, 15, 5, 0, 100, ExpectedResult = 0f, TestName = "Reversed from range + Should clamp")]
    [TestCase(4, 15, 5, 0, 100, ExpectedResult = 100f, TestName = "Reversed from range 2 + Should clamp")]
    public float RemapFloat_ReturnsExpectedResult(float value, float fromStart, float fromEnd, float toStart, float toEnd)
    {
        return M.Remap(value, fromStart, fromEnd, toStart, toEnd);
    }

    [TestCase(10, 0, 3, ExpectedResult = 1f)]
    [TestCase(2, 0, 3, ExpectedResult = 2f)]
    public float WrapFloat_ReturnsExpectedResult(float value, float min, float max)
    {
        return M.Wrap(value, min, max);
    }

    [TestCase(5, 2, ExpectedResult = 1f)]
    [TestCase(6, 2, ExpectedResult = 0f)]
    [TestCase(-5, 2, ExpectedResult = 1f)]
    public float ModuloFloat_ReturnsExpectedResult(float value, float divisor)
    {
        return M.Modulo(value, divisor);
    }

    #endregion

    #region Floats

    [TestCase(0, 10, 1, ExpectedResult = 1f)]
    [TestCase(0, 10, -1, ExpectedResult = -1f)]
    [TestCase(0, 10, 100, ExpectedResult = 10f)]
    public float MoveTowards_ReturnsExpectedResult(float current, float target, float maxDelta)
    {
        return M.MoveTowards(current, target, maxDelta);
    }

    [TestCase(0, 90, 30, 30f)] // Straightforward
    [TestCase(0, -270, 30, 30f)] // Wrap
    [TestCase(0, 10, 5, 5f)] // Counter-clockwise
    [TestCase(0, -10, 5, -5f)] // Clockwise
    public void MoveTowardsAngleDegrees_ReturnsExpectedResult(float current, float target, float maxDelta, float expectedResult)
    {
        var result = M.MoveTowardsAngleDegrees(current, target, maxDelta);
        Assert.That(result, Is.EqualTo(expectedResult).Within(0.01f));
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
        var result = M.Rad2Deg(M.MoveTowardsAngleRadians(M.Deg2Rad(current), M.Deg2Rad(target), M.Deg2Rad(maxDelta)));
        Assert.That(result, Is.EqualTo(expectedResult).Within(0.01f));
    }

    #endregion

    #region Integers

    [TestCase(1, 16, ExpectedResult = 16)]
    [TestCase(8, 16, ExpectedResult = 16)]
    [TestCase(16, 16, ExpectedResult = 16)]
    [TestCase(32, 16, ExpectedResult = 32)]
    [TestCase(32, 64, ExpectedResult = 64)]
    [TestCase(127, 64, ExpectedResult = 128)]
    public int GetAlignedSize_ReturnsExpectedResult(int value, int multiple)
    {
        return M.GetAlignedSize(value, multiple);
    }

    [TestCase(45, 11, ExpectedResult = 44)]
    [TestCase(55, 11, ExpectedResult = 55)]
    [TestCase(54, 11, ExpectedResult = 55)]
    [TestCase(49, 11, ExpectedResult = 44)]
    [TestCase(50, 11, ExpectedResult = 55)]
    public int GetNearestMultiple_ReturnsExpectedResult(int value, int multiple)
    {
        return M.GetNearestMultiple(value, multiple);
    }

    [TestCase(0, ExpectedResult = 2)]
    [TestCase(2, ExpectedResult = 2)]
    [TestCase(5, ExpectedResult = 8)]
    [TestCase(16, ExpectedResult = 16)]
    [TestCase(123, ExpectedResult = 128)]
    public int GetNextPowerOfTwo_ReturnsExpectedResult(int value)
    {
        return M.GetNextPowerOfTwo(value);
    }

    #endregion

    #region IsApproximatelyEqual

    [TestCase(0, float.Epsilon, ExpectedResult = true)]
    [TestCase(0, 1, ExpectedResult = false)]
    public bool IsApproximatelyEqual_ReturnsExpectedResult(float a, float b)
    {
        return M.IsApproximatelyEqual(a, b);
    }

    [TestCase(0, double.Epsilon, ExpectedResult = true)]
    [TestCase(0, 1, ExpectedResult = false)]
    public bool IsApproximatelyEqual_ReturnsExpectedResult(double a, double b)
    {
        return M.IsApproximatelyEqual(a, b);
    }

    #endregion

    #region Colors

    [TestCase]
    public void HexToSrgb_ReturnsExpectedResult()
    {
        Assert.That(M.HexToSrgb("fff"), Is.EqualTo(new Vector4(1, 1, 1, 1)));
        Assert.That(M.HexToSrgb("ffff"), Is.EqualTo(new Vector4(1, 1, 1, 1)));
        Assert.That(M.HexToSrgb("#fff"), Is.EqualTo(new Vector4(1, 1, 1, 1)));
        Assert.That(M.HexToSrgb("#ffff"), Is.EqualTo(new Vector4(1, 1, 1, 1)));
        Assert.That(M.HexToSrgb("#FFFF"), Is.EqualTo(new Vector4(1, 1, 1, 1)));

        Assert.That(M.HexToSrgb("#1f8ec2"), Is.EqualTo(new Vector4(0x1f / 255f, 0x8e / 255f, 0xc2 / 255f, 1)));
    }

    #endregion

    #region Planes

    [TestCase]
    public void CreatePlane_ReturnsExpectedResult1()
    {
        var plane = M.CreatePlane(Vector3.UnitX, Vector3.Zero);

        Assert.That(M.IsApproximatelyEqual(0, plane.D), Is.True);
        Assert.That(M.IsApproximatelyEqual(Vector3.UnitX, plane.Normal), Is.True);
    }

    [TestCase]
    public void CreatePlane_ReturnsExpectedResult2()
    {
        var plane = M.CreatePlane(Vector3.UnitX, Vector3.UnitX);

        Assert.That(M.IsApproximatelyEqual(-1, plane.D), Is.True);
        Assert.That(M.IsApproximatelyEqual(Vector3.UnitX, plane.Normal), Is.True);
    }

    [TestCase]
    public void PlaneRaycast_ReturnsExpectedResult1()
    {
        var expectedDistance = 10;
        var expectedPosition = new Vector2(1, 2);

        var plane = M.CreatePlane(Vector3.UnitZ, Vector3.Zero);
        var ray = new Ray(new Vector3(expectedPosition.X, expectedPosition.Y, -expectedDistance), Vector3.UnitZ);

        var isHit = plane.Raycast(ray, out var distance);

        Assert.That(isHit, Is.True);
        Assert.That(M.IsApproximatelyEqual(expectedDistance, distance), Is.True);
        Assert.That(M.IsApproximatelyEqual(expectedPosition.Xy0(), ray.GetPoint(distance)), Is.True);
    }

    [TestCase]
    public void PlaneRaycast_ReturnsExpectedResult2()
    {
        var plane = M.CreatePlane(Vector3.UnitZ, Vector3.Zero);
        var ray = new Ray(new Vector3(1, 2, -10), -Vector3.UnitZ);

        var isHit = plane.Raycast(ray, out _);

        Assert.That(isHit, Is.False);
    }

    #endregion
}
