using System.Numerics;
using Exanite.Core.Numerics;
using Exanite.Core.Utilities;
using Xunit;

namespace Exanite.Core.Tests.Utilities;

public class MathUtilityTests
{
    #region Ranges

    [Theory]
    [InlineData(5, 15, 0.8f, 13f, TestDisplayName = "Basic case")]
    [InlineData(5, 15, 2, 15f, TestDisplayName = "Out of range t")]
    [InlineData(5, 15, -1, 5f, TestDisplayName = "Out of range t 2")]
    public void Lerp_ReturnsExpectedResult(float from, float to, float t, float expected)
    {
        Assert.Equal(expected, M.Lerp(from, to, t));
    }

    [Theory]
    [InlineData(5, 15, 0.8f, 13f, TestDisplayName = "Basic case")]
    [InlineData(5, 15, 2, 25f, TestDisplayName = "Out of range t")]
    [InlineData(5, 15, -1, -5f, TestDisplayName = "Out of range t 2")]
    public void LerpUnclamped_ReturnsExpectedResult(float from, float to, float t, float expected)
    {
        Assert.Equal(expected, M.LerpUnclamped(from, to, t));
    }

    [Theory]
    [InlineData(13, 5, 15, 0, 100, 80f, TestDisplayName = "Basic case")]
    [InlineData(13, 15, 5, 0, 100, 20f, TestDisplayName = "Reversed from range")]
    [InlineData(13, 5, 15, 100, 0, 20f, TestDisplayName = "Reversed to range")]
    [InlineData(16, 5, 15, 0, 100, 100f, TestDisplayName = "Out of range input")]
    [InlineData(16, 15, 5, 0, 100, 0f, TestDisplayName = "Reversed from range + Out of range input")]
    [InlineData(4, 15, 5, 0, 100, 100f, TestDisplayName = "Reversed from range + Out of range input 2")]
    public void Remap_ReturnsExpectedResult(float value, float fromStart, float fromEnd, float toStart, float toEnd, float expected)
    {
        Assert.Equal(expected, M.Remap(value, fromStart, fromEnd, toStart, toEnd));
    }

    [Theory]
    [InlineData(13, 5, 15, 0, 100, 80f, TestDisplayName = "Basic case")]
    [InlineData(13, 15, 5, 0, 100, 20f, TestDisplayName = "Reversed from range")]
    [InlineData(13, 5, 15, 100, 0, 20f, TestDisplayName = "Reversed to range")]
    [InlineData(16, 5, 15, 0, 100, 110f, TestDisplayName = "Out of range input")]
    [InlineData(16, 15, 5, 0, 100, -10f, TestDisplayName = "Reversed from range + Out of range input")]
    [InlineData(4, 15, 5, 0, 100, 110f, TestDisplayName = "Reversed from range 2 + Out of range input")]
    public void RemapUnclamped_ReturnsExpectedResult(float value, float fromStart, float fromEnd, float toStart, float toEnd, float expected)
    {
        Assert.Equal(expected, M.RemapUnclamped(value, fromStart, fromEnd, toStart, toEnd));
    }

    [Theory]
    [InlineData(10, 0, 3, 1f)]
    [InlineData(2, 0, 3, 2f)]
    public void Wrap_ReturnsExpectedResult(float value, float min, float max, float expected)
    {
        Assert.Equal(expected, M.Wrap(value, min, max));
    }

    [Theory]
    [InlineData(5, 2, 1f)]
    [InlineData(6, 2, 0f)]
    [InlineData(-5, 2, 1f)]
    public void Modulo_ReturnsExpectedResult(float value, float divisor, float expected)
    {
        Assert.Equal(expected, M.Modulo(value, divisor));
    }

    #endregion

    #region Floats

    [Theory]
    [InlineData(0, 10, 1, 1f)]
    [InlineData(0, 10, -1, -1f)]
    [InlineData(0, 10, 100, 10f)]
    public void MoveTowards_ReturnsExpectedResult(float current, float target, float maxDelta, float expected)
    {
        Assert.Equal(expected, M.MoveTowards(current, target, maxDelta));
    }

    [Theory]
    [InlineData(0, 90, 30, 30f)] // Straightforward
    [InlineData(0, -270, 30, 30f)] // Wrap
    [InlineData(0, 10, 5, 5f)] // Counter-clockwise
    [InlineData(0, -10, 5, -5f)] // Clockwise
    public void MoveTowardsAngleDegrees_ReturnsExpectedResult(float current, float target, float maxDelta, float expectedResult)
    {
        var result = M.MoveTowardsAngleDegrees(current, target, maxDelta);
        Assert.Equal(expectedResult, result, 2);
    }

    /// <remarks>
    /// Cases are copied from <see cref="MoveTowardsAngleDegrees_ReturnsExpectedResult"/>.
    /// </remarks>
    [Theory]
    [InlineData(0, 90, 30, 30f)] // Straightforward
    [InlineData(0, -270, 30, 30f)] // Wrap
    [InlineData(0, 10, 5, 5f)] // Counter-clockwise
    [InlineData(0, -10, 5, -5f)] // Clockwise
    public void MoveTowardsAngleRadians_ReturnsExpectedResult(float current, float target, float maxDelta, float expectedResult)
    {
        var result = M.Rad2Deg(M.MoveTowardsAngleRadians(M.Deg2Rad(current), M.Deg2Rad(target), M.Deg2Rad(maxDelta)));
        Assert.Equal(expectedResult, result, 2);
    }

    #endregion

    #region Integers

    [Theory]
    [InlineData(1, 16, 16)]
    [InlineData(8, 16, 16)]
    [InlineData(16, 16, 16)]
    [InlineData(32, 16, 32)]
    [InlineData(32, 64, 64)]
    [InlineData(127, 64, 128)]
    public void GetAlignedSize_ReturnsExpectedResult(int value, int multiple, int expected)
    {
        Assert.Equal(expected, M.GetAlignedSize(value, multiple));
    }

    [Theory]
    [InlineData(45, 11, 44)]
    [InlineData(55, 11, 55)]
    [InlineData(54, 11, 55)]
    [InlineData(49, 11, 44)]
    [InlineData(50, 11, 55)]
    public void GetNearestMultiple_ReturnsExpectedResult(int value, int multiple, int expected)
    {
        Assert.Equal(expected, M.GetNearestMultiple(value, multiple));
    }

    [Theory]
    [InlineData(0, 2)]
    [InlineData(2, 2)]
    [InlineData(5, 8)]
    [InlineData(16, 16)]
    [InlineData(123, 128)]
    public void GetNextPowerOfTwo_ReturnsExpectedResult(int value, int expected)
    {
        Assert.Equal(expected, M.GetNextPowerOfTwo(value));
    }

    #endregion

    #region ApproximatelyEquals

    [Theory]
    [InlineData(0, float.Epsilon, true)]
    [InlineData(0, 1, false)]
    public void ApproximatelyEquals_Float_ReturnsExpectedResult(float a, float b, bool expected)
    {
        Assert.Equal(expected, M.ApproximatelyEquals(a, b));
    }

    [Theory]
    [InlineData(0, double.Epsilon, true)]
    [InlineData(0, 1, false)]
    public void ApproximatelyEquals_Double_ReturnsExpectedResult(double a, double b, bool expected)
    {
        Assert.Equal(expected, M.ApproximatelyEquals(a, b));
    }

    #endregion

    #region Colors

    [Fact]
    public void HexToSrgb_ReturnsExpectedResult()
    {
        Assert.Equal(new Vector4(1, 1, 1, 1), M.HexToSrgb("fff"));
        Assert.Equal(new Vector4(1, 1, 1, 1), M.HexToSrgb("ffff"));
        Assert.Equal(new Vector4(1, 1, 1, 1), M.HexToSrgb("#fff"));
        Assert.Equal(new Vector4(1, 1, 1, 1), M.HexToSrgb("#ffff"));
        Assert.Equal(new Vector4(1, 1, 1, 1), M.HexToSrgb("#FFFF"));

        Assert.Equal(new Vector4(0x1f / 255f, 0x8e / 255f, 0xc2 / 255f, 1), M.HexToSrgb("#1f8ec2"));
    }

    #endregion

    #region Planes

    [Fact]
    public void CreatePlane_ReturnsExpectedResult1()
    {
        var plane = M.CreatePlane(Vector3.UnitX, Vector3.Zero);

        Assert.True(M.ApproximatelyEquals(0, plane.D));
        Assert.True(M.ApproximatelyEquals(Vector3.UnitX, plane.Normal));
    }

    [Fact]
    public void CreatePlane_ReturnsExpectedResult2()
    {
        var plane = M.CreatePlane(Vector3.UnitX, Vector3.UnitX);

        Assert.True(M.ApproximatelyEquals(-1, plane.D));
        Assert.True(M.ApproximatelyEquals(Vector3.UnitX, plane.Normal));
    }

    [Fact]
    public void PlaneRaycast_ReturnsExpectedResult1()
    {
        var expectedDistance = 10;
        var expectedPosition = new Vector2(1, 2);

        var plane = M.CreatePlane(Vector3.UnitZ, Vector3.Zero);
        var ray = new Ray(new Vector3(expectedPosition.X, expectedPosition.Y, -expectedDistance), Vector3.UnitZ);

        var isHit = plane.Raycast(ray, out var distance);

        Assert.True(isHit);
        Assert.True(M.ApproximatelyEquals(expectedDistance, distance));
        Assert.True(M.ApproximatelyEquals(expectedPosition.Xy0(), ray.GetPoint(distance)));
    }

    [Fact]
    public void PlaneRaycast_ReturnsExpectedResult2()
    {
        var plane = M.CreatePlane(Vector3.UnitZ, Vector3.Zero);
        var ray = new Ray(new Vector3(1, 2, -10), -Vector3.UnitZ);

        var isHit = plane.Raycast(ray, out _);

        Assert.False(isHit);
    }

    #endregion
}
