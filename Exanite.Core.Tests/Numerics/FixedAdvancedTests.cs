using Exanite.Core.Numerics;
using Exanite.Core.Runtime;
using Exanite.Core.Utilities;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class FixedAdvancedTests
{
    [Fact]
    public void Sqrt_ReturnsExpectedValue_ForGreaterThanOne()
    {
        var current = 1.0;
        var multiplier = 1.025;
        for (var i = 0; i < 1000; i++)
        {
            current *= multiplier;
            AssertEqual(i, current, double.Sqrt(current), (double)Fixed.Sqrt((Fixed)current), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
        }
    }

    [Fact]
    public void Sqrt_ReturnsExpectedValue_ForLessThanOne()
    {
        var current = 1.0;
        var multiplier = 0.995;
        for (var i = 0; i < 1000; i++)
        {
            current *= multiplier;
            AssertEqual(i, current, double.Sqrt(current), (double)Fixed.Sqrt((Fixed)current), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
        }
    }

    [Fact]
    public void Sqrt_ReturnsExpectedValue_PerfectSquares()
    {
        for (var i = 0; i < 1000000; i++)
        {
            var input = (long)i * i;
            AssertEqual(i, input, double.Sqrt(input), (double)Fixed.Sqrt((Fixed)input), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
        }
    }

    [Fact]
    public void Sqrt_OfMaxValue_ReturnsExpectedValue()
    {
        Assert.Equal(double.Sqrt((double)Fixed.MaxValue), (double)Fixed.Sqrt(Fixed.MaxValue), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
    }

    [Fact]
    public void Sqrt_OfEpsilon_ReturnsExpectedValue()
    {
        Assert.Equal(double.Sqrt((double)Fixed.Epsilon), (double)Fixed.Sqrt(Fixed.Epsilon), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
    }

    [Fact]
    public void Cbrt_ReturnsExpectedValue_ForGreaterThanOne()
    {
        var current = 1.0;
        var multiplier = 1.025;
        for (var i = 0; i < 1000; i++)
        {
            current *= multiplier;
            AssertEqual(i, current, double.Cbrt(current), (double)Fixed.Cbrt((Fixed)current), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
            AssertEqual(i, -current, double.Cbrt(-current), (double)Fixed.Cbrt((Fixed)(-current)), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
        }
    }

    [Fact]
    public void Cbrt_ReturnsExpectedValue_ForLessThanOne()
    {
        var current = 1.0;
        var multiplier = 0.995;
        for (var i = 0; i < 1000; i++)
        {
            current *= multiplier;
            AssertEqual(i, current, double.Cbrt(current), (double)Fixed.Cbrt((Fixed)current), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision - 1));
            AssertEqual(i, -current, double.Cbrt(-current), (double)Fixed.Cbrt((Fixed)(-current)), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision - 1));
        }
    }

    [Fact]
    public void Hypot_ReturnsExpectedValue_ForWideRange()
    {
        var radiusMultiplier = 1.025;
        var angleDelta = 0.234;
        var currentRadius = 0.0001;
        var currentAngle = 0.0;
        for (var i = 0; i < 1650; i++)
        {
            var x = M.Cos(currentAngle) * currentRadius;
            var y = M.Sin(currentAngle) * currentRadius;

            var expected = double.Hypot(y, x);
            var comparer = i switch
            {
                _ when M.Abs(i) > 1500 => FloatingPointComparer.FromTolerance((decimal)expected * 0.0000000001M),
                _ when M.Abs(i) > 1000 => FloatingPointComparer.FromTolerance((decimal)expected * 0.00000000001M),
                _ => FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision),
            };

            AssertEqualIndexCoord(i, x, y, expected, (double)Fixed.Hypot((Fixed)x, (Fixed)y), comparer);

            currentRadius *= radiusMultiplier;
            currentAngle += angleDelta;
        }
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(double.Pi / 2, 1)]
    [InlineData(double.Pi, 0)]
    [InlineData(3 * double.Pi / 2, -1)]
    [InlineData(2 * double.Pi, 0)]
    [InlineData(1.04719, 0.86602)] // pi/3
    [InlineData(123456, -0.74028)]
    [InlineData(123456789, 0.99011)]
    public void Sin_ReturnsExpectedValue(double input, double expected)
    {
        Assert.Equal(expected, (double)Fixed.Sin((Fixed)input), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
    }

    [Fact]
    public void Sin_ReturnsExpectedValue_ForWideRange()
    {
        var current = 0.0001;
        var multiplier = 1.025;
        for (var i = 0; i < 1350; i++)
        {
            current *= multiplier;

            AssertEqual(i, current, double.Sin(current), (double)Fixed.Sin((Fixed)current), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
            AssertEqual(i, -current, double.Sin(-current), (double)Fixed.Sin((Fixed)(-current)), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
        }
    }

    [Fact]
    public void SinPi_ReturnsExpectedValue_ForWideRange()
    {
        var current = 0.0001;
        var multiplier = 1.025;
        for (var i = 0; i < 1350; i++)
        {
            current *= multiplier;

            AssertEqual(i, current, double.SinPi(current), (double)Fixed.SinPi((Fixed)current), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
            AssertEqual(i, -current, double.SinPi(-current), (double)Fixed.SinPi((Fixed)(-current)), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
        }
    }

    [Fact]
    public void Cos_ReturnsExpectedValue_ForWideRange()
    {
        var current = 0.0001;
        var multiplier = 1.025;
        for (var i = 0; i < 1350; i++)
        {
            current *= multiplier;

            AssertEqual(i, current, double.Cos(current), (double)Fixed.Cos((Fixed)current), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
            AssertEqual(i, -current, double.Cos(-current), (double)Fixed.Cos((Fixed)(-current)), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
        }
    }

    [Fact]
    public void CosPi_ReturnsExpectedValue_ForWideRange()
    {
        var current = 0.0001;
        var multiplier = 1.025;
        for (var i = 0; i < 1350; i++)
        {
            current *= multiplier;

            AssertEqual(i, current, double.CosPi(current), (double)Fixed.CosPi((Fixed)current), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
            AssertEqual(i, -current, double.CosPi(-current), (double)Fixed.CosPi((Fixed)(-current)), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
        }
    }

    [Fact]
    public void Tan_ReturnsExpectedValue_ForWideRange()
    {
        var current = 0.0001;
        var multiplier = 1.025;
        for (var i = 0; i < 1350; i++)
        {
            current *= multiplier;

            Assert(current);
            Assert(-current);

            continue;

            void Assert(double input)
            {
                var expected = double.Tan(input);
                var comparer = expected switch
                {
                    // Slope is equal to tan(x)^2 + 1
                    _ when M.Abs(expected) > 100 => FloatingPointComparer.FromTolerance((decimal)expected * 0.1M), // Slope is >= 10001 at this point
                    _ when M.Abs(expected) > 10 => FloatingPointComparer.FromTolerance((decimal)expected * 0.01M), // Slope is >= 101 at this point
                    _ when M.Abs(expected) > 2 => FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision - 2), // Slope is >= 5 at this point
                    _ when M.Abs(expected) > 1 => FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision - 1), // Slope is >= 2 at this point
                    _ => FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision), // Slope is always >= 1
                };

                AssertEqual(i, input, expected, (double)Fixed.Tan((Fixed)input), comparer);
            }
        }
    }

    [Fact]
    public void TanPi_ReturnsExpectedValue_ForWideRange()
    {
        var current = 0.0001;
        var multiplier = 1.025;
        for (var i = 0; i < 1350; i++)
        {
            current *= multiplier;

            Assert(current);
            Assert(-current);

            continue;

            void Assert(double input)
            {
                var expected = double.TanPi(input);
                var comparer = expected switch
                {
                    // Slope is equal to tan(x)^2 + 1
                    _ when M.Abs(expected) > 100 => FloatingPointComparer.FromTolerance((decimal)expected * 0.1M), // Slope is >= 10001 at this point
                    _ when M.Abs(expected) > 10 => FloatingPointComparer.FromTolerance((decimal)expected * 0.01M), // Slope is >= 101 at this point
                    _ when M.Abs(expected) > 2 => FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision - 2), // Slope is >= 5 at this point
                    _ when M.Abs(expected) > 1 => FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision - 1), // Slope is >= 2 at this point
                    _ => FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision), // Slope is always >= 1
                };

                AssertEqual(i, input, expected, (double)Fixed.TanPi((Fixed)input), comparer);
            }
        }
    }

    [Fact]
    public void SinCos_ReturnsExpectedValue_ForWideRange()
    {
        var comparer = FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision);
        var current = 0.0001;
        var multiplier = 1.025;
        for (var i = 0; i < 1350; i++)
        {
            current *= multiplier;

            Assert(current);
            Assert(-current);

            continue;

            void Assert(double input)
            {
                var expected = double.SinCos(input);
                var actual = Fixed.SinCos((Fixed)input);

                AssertEqual(i, input, expected.Sin, (double)actual.Sin, comparer);
                AssertEqual(i, input, expected.Cos, (double)actual.Cos, comparer);
            }
        }
    }

    [Fact]
    public void SinCosPi_ReturnsExpectedValue_ForWideRange()
    {
        var comparer = FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision);
        var current = 0.0001;
        var multiplier = 1.025;
        for (var i = 0; i < 1350; i++)
        {
            current *= multiplier;

            Assert(current);
            Assert(-current);

            continue;

            void Assert(double input)
            {
                var expected = double.SinCosPi(input);
                var actual = Fixed.SinCosPi((Fixed)input);

                AssertEqual(i, input, expected.SinPi, (double)actual.SinPi, comparer);
                AssertEqual(i, input, expected.CosPi, (double)actual.CosPi, comparer);
            }
        }
    }

    [Fact]
    public void Atan2_ReturnsExpectedValue_ForWideRange()
    {
        var radiusDelta = 0.0234;
        var angleDelta = 0.234;
        var currentRadius = 0.0;
        var currentAngle = 0.0;
        for (var i = 0; i < 1234; i++)
        {
            var x = M.Cos(currentAngle) * currentRadius;
            var y = M.Sin(currentAngle) * currentRadius;

            AssertEqualIndexCoord(i, x, y, double.Atan2(y, x), (double)Fixed.Atan2((Fixed)y, (Fixed)x), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision - 2));

            currentRadius += radiusDelta;
            currentAngle += angleDelta;
        }
    }

    [Theory]
    [InlineData(10, 1)]
    [InlineData(1, 10)]
    [InlineData(-1, 10)]
    [InlineData(-10, 1)]
    [InlineData(-10, -1)]
    [InlineData(-1, -10)]
    [InlineData(1, -10)]
    [InlineData(10, -1)]
    public void Atan2_ReturnsExpectedValue_ForReciprocalCases(double x, double y)
    {
        AssertEqualCoord(x, y, double.Atan2(y, x), (double)Fixed.Atan2((Fixed)y, (Fixed)x), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(-1, 0)]
    [InlineData(0, 1)]
    [InlineData(0, -1)]
    public void Atan2_ReturnsExpectedValue_ForPointsOnAxes(double x, double y)
    {
        AssertEqualCoord(x, y, double.Atan2(y, x), (double)Fixed.Atan2((Fixed)y, (Fixed)x), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
    }

    [Fact]
    public void Atan_ReturnsExpectedValue_ForWideRange()
    {
        var current = 0.0001;
        var multiplier = 1.025;
        for (var i = 0; i < 1350; i++)
        {
            current *= multiplier;

            AssertEqual(i, current, double.Atan(current), (double)Fixed.Atan((Fixed)current), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
            AssertEqual(i, -current, double.Atan(-current), (double)Fixed.Atan((Fixed)(-current)), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
        }
    }

    [Fact]
    public void Asin_ReturnsExpectedValue_ForWideRange()
    {
        var iterations = 1234;
        var from = -1.0;
        var to = 1.0;
        for (var i = 0; i < iterations; i++)
        {
            var input = M.Lerp(from, to, (double)i / (iterations - 1));
            AssertEqual(i, input, double.Asin(input), (double)Fixed.Asin((Fixed)input), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision - 2));
        }
    }

    [Fact]
    public void Acos_ReturnsExpectedValue_ForWideRange()
    {
        var iterations = 1234;
        var from = -1.0;
        var to = 1.0;
        for (var i = 0; i < iterations; i++)
        {
            var input = M.Lerp(from, to, (double)i / (iterations - 1));
            AssertEqual(i, input, double.Acos(input), (double)Fixed.Acos((Fixed)input), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision - 2));
        }
    }

    [Fact]
    public void Log2_ReturnsShiftForEpsilon()
    {
        var result = (double)Fixed.Log2(Fixed.Epsilon);
        Assert.Equal(-Fixed.Shift, result);
    }

    [Fact]
    public void Log2_ThrowsForInvalidInputs()
    {
        Assert.Throws<GuardException>(() =>
        {
            Fixed.Log2(0);
        });

        Assert.Throws<GuardException>(() =>
        {
            Fixed.Log2(-1);
        });
    }

    [Fact]
    public void Log2_ReturnsExpectedValue_ForGreaterThanOne()
    {
        var current = 1.0;
        var multiplier = 1.025;
        for (var i = 0; i < 1000; i++)
        {
            current *= multiplier;
            AssertEqual(i, current, double.Log2(current), (double)Fixed.Log2((Fixed)current), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
        }
    }

    [Fact]
    public void Log2_ReturnsExpectedValue_ForLessThanOne()
    {
        var current = 1.0;
        var multiplier = 0.995;
        for (var i = 0; i < 1000; i++)
        {
            current *= multiplier;
            var expected = double.Log2(current);
            var comparer = i switch
            {
                _ when M.Abs(i) > 750 => FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision - 2),
                _ when M.Abs(i) > 350 => FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision - 1),
                _ => FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision),
            };

            AssertEqual(i, current, expected, (double)Fixed.Log2((Fixed)current), comparer);
        }
    }

    [Fact]
    public void Log_ReturnsExpectedValue_ForWideRange()
    {
        var current = 0.25;
        var multiplier = 1.025;
        for (var i = 0; i < 1000; i++)
        {
            current *= multiplier;
            AssertEqual(i, current, double.Log(current), (double)Fixed.Log((Fixed)current), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
        }
    }

    [Fact]
    public void Log10_ReturnsExpectedValue_ForWideRange()
    {
        var current = 0.25;
        var multiplier = 1.025;
        for (var i = 0; i < 1000; i++)
        {
            current *= multiplier;
            AssertEqual(i, current, double.Log10(current), (double)Fixed.Log10((Fixed)current), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision - 1));
        }
    }

    [Fact]
    public void LogBase_ReturnsExpectedValue_ForWideRange()
    {
        var current = 0.25;
        var multiplier = 1.025;
        for (var i = 0; i < 1000; i++)
        {
            current *= multiplier;
            var logBase = (current * 100) % 10;
            var expected = double.Log(current, logBase);
            var comparer = FloatingPointComparer.FromTolerance((decimal)expected * 0.01M, FloatingPointComparer.ToleranceFromPrecision(FixedTestConstants.BaseExpectedPrecision));
            AssertEqualLogBase(i, current, logBase, expected, (double)Fixed.Log((Fixed)current, (Fixed)logBase), comparer);
        }
    }

    [Fact]
    public void Exp2_ReturnsExpectedValue_ForWideRange()
    {
        var current = 0.01;
        var multiplier = 1.025;
        for (var i = 0; i < 340; i++)
        {
            current *= multiplier;
            var expected = double.Exp2(current);
            var comparer = FloatingPointComparer.FromTolerance((decimal)expected * 0.0000105M, FloatingPointComparer.ToleranceFromPrecision(FixedTestConstants.BaseExpectedPrecision));
            AssertEqual(i, current, expected, (double)Fixed.Exp2((Fixed)current), comparer);
        }
    }

    [Fact]
    public void Exp2_BehavesForBoundaryValues()
    {
        Assert.Equal(1, Fixed.Exp2(0));
        Assert.Equal(Fixed.MaxValue, Fixed.Exp2(100));
        Assert.Equal(Fixed.MaxValue, Fixed.Exp2(Fixed.IntegralBitCount));
        Assert.Equal(0, Fixed.Exp2(-(Fixed.Shift + 1)));
        Assert.Equal(0, Fixed.Exp2(-100));
    }

    [Fact]
    public void Pow_ReturnsExpectedValue_ForWideRange()
    {
        var current = 0.25;
        var multiplier = 1.025;
        for (var i = 0; i < 150; i++)
        {
            current *= multiplier;
            var exponent = (current * 100) % 10;
            var expected = double.Pow(current, exponent);
            var comparer = FloatingPointComparer.FromTolerance((decimal)expected * 0.0005M, FloatingPointComparer.ToleranceFromPrecision(FixedTestConstants.BaseExpectedPrecision));
            AssertEqualPowBase(i, current, exponent, expected, (double)Fixed.Pow((Fixed)current, (Fixed)exponent), comparer);
        }
    }

    [Fact]
    public void Pow_ReturnsExpectedValue_ForSmallIntegerExponents()
    {
        for (var exponent = -3; exponent <= 3; exponent++)
        {
            var current = 0.25;
            var multiplier = 1.025;
            for (var i = 0; i < 150; i++)
            {
                current *= multiplier;

                {
                    var expected = double.Pow(current, exponent);
                    AssertEqualPowBase(i, current, exponent, expected, (double)Fixed.Pow((Fixed)current, exponent), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
                }

                {
                    var expected = double.Pow(-current, exponent);
                    AssertEqualPowBase(i, -current, exponent, expected, (double)Fixed.Pow((Fixed)(-current), exponent), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
                }
            }
        }
    }

    [Fact]
    public void RootN_ReturnsExpectedValue_ForWideRange()
    {
        var current = 0.25;
        var multiplier = 1.025;

        for (var i = 0; i < 150; i++)
        {
            current *= multiplier;

            for (var j = -10; j < 10; j++)
            {
                var root = j;
                if (root == 0)
                {
                    continue;
                }

                {
                    var expected = double.RootN(current, root);
                    var comparer = FloatingPointComparer.FromTolerance((decimal)expected * 0.00006M, FloatingPointComparer.ToleranceFromPrecision(FixedTestConstants.BaseExpectedPrecision));
                    AssertEqualRootN(i, current, root, expected, (double)Fixed.RootN((Fixed)current, root), comparer);
                }

                if (!int.IsEvenInteger(root))
                {
                    var expected = double.RootN(-current, root);
                    var comparer = FloatingPointComparer.FromTolerance((decimal)expected * 0.00006M, FloatingPointComparer.ToleranceFromPrecision(FixedTestConstants.BaseExpectedPrecision));
                    AssertEqualRootN(i, -current, root, expected, (double)Fixed.RootN((Fixed)(-current), root), comparer);
                }
            }
        }
    }

    [Fact]
    public void RootN_ReturnsExpectedValue_ForSmallIntegerExponents()
    {
        for (var root = -3; root <= 3; root++)
        {
            if (root == 0)
            {
                continue;
            }

            var current = 0.25;
            var multiplier = 1.025;
            for (var i = 0; i < 150; i++)
            {
                current *= multiplier;

                {
                    var expected = double.RootN(current, root);
                    AssertEqualPowBase(i, current, root, expected, (double)Fixed.RootN((Fixed)current, root), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
                }

                if (!int.IsEvenInteger(root))
                {
                    var expected = double.RootN(-current, root);
                    AssertEqualPowBase(i, -current, root, expected, (double)Fixed.RootN((Fixed)(-current), root), FloatingPointComparer.FromPrecision(FixedTestConstants.BaseExpectedPrecision));
                }
            }
        }
    }

    [Fact]
    public void RootN_BehavesForBoundaryValues()
    {
        Assert.Throws<GuardException>(() =>
        {
            Fixed.RootN(1, 0);
        });

        Assert.Throws<GuardException>(() =>
        {
            Fixed.RootN(-1, 2);
        });

        Assert.Throws<GuardException>(() =>
        {
            Fixed.RootN(-1, 4);
        });

        Assert.Throws<GuardException>(() =>
        {
            Fixed.RootN(-1, 6);
        });
    }

    private void AssertEqual(int i, double input, double expected, double actual, FloatingPointComparer comparer)
    {
        Assert.True(comparer.Equals(expected, actual), $"""
            Failed at i: {i}
            Input:       {(decimal)input}
            Expected:    {(decimal)expected}
            Actual:      {(decimal)actual}
            Difference:  {(decimal)M.Abs(expected - actual)}
            Tolerance:   {comparer.Tolerance}
            """);
    }

    private void AssertEqualCoord(double x, double y, double expected, double actual, FloatingPointComparer comparer)
    {
        Assert.True(comparer.Equals(expected, actual), $"""
            Failed for input: ({(decimal)x}, {(decimal)y})
            Expected:   {(decimal)expected}
            Actual:     {(decimal)actual}
            Difference: {(decimal)M.Abs(expected - actual)}
            Tolerance:  {comparer.Tolerance}
            """);
    }

    private void AssertEqualIndexCoord(int i, double x, double y, double expected, double actual, FloatingPointComparer comparer)
    {
        Assert.True(comparer.Equals(expected, actual), $"""
            Failed at i: {i}
            Input:       ({(decimal)x}, {(decimal)y})
            Expected:    {(decimal)expected}
            Actual:      {(decimal)actual}
            Difference:  {(decimal)M.Abs(expected - actual)}
            Tolerance:   {comparer.Tolerance}
            """);
    }

    private void AssertEqualLogBase(int i, double input, double logBase, double expected, double actual, FloatingPointComparer comparer)
    {
        Assert.True(comparer.Equals(expected, actual), $"""
            Failed at i: {i}
            Input:       Log({(decimal)input}, {(decimal)logBase})
            Expected:    {(decimal)expected}
            Actual:      {(decimal)actual}
            Difference:  {(decimal)M.Abs(expected - actual)}
            Tolerance:   {comparer.Tolerance}
            """);
    }

    private void AssertEqualPowBase(int i, double powBase, double exponent, double expected, double actual, FloatingPointComparer comparer)
    {
        Assert.True(comparer.Equals(expected, actual), $"""
            Failed at i: {i}
            Input:       Pow({(decimal)powBase}, {(decimal)exponent})
            Expected:    {(decimal)expected}
            Actual:      {(decimal)actual}
            Difference:  {(decimal)M.Abs(expected - actual)}
            Tolerance:   {comparer.Tolerance}
            """);
    }

    private void AssertEqualRootN(int i, double input, double root, double expected, double actual, FloatingPointComparer comparer)
    {
        Assert.True(comparer.Equals(expected, actual), $"""
            Failed at i: {i}
            Input:       RootN({(decimal)input}, {(decimal)root})
            Expected:    {(decimal)expected}
            Actual:      {(decimal)actual}
            Difference:  {(decimal)M.Abs(expected - actual)}
            Tolerance:   {comparer.Tolerance}
            """);
    }
}
