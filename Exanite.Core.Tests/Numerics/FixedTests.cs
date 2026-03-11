using System;
using Exanite.Core.Numerics;
using Exanite.Core.Runtime;
using Exanite.Core.Utilities;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class FixedTests
{
    [Fact]
    public void MinMax_AreSymmetric()
    {
        Assert.Equal(Fixed.MaxValue, -Fixed.MinValue);
    }

    [Theory]
    [InlineData(1, 1, 1)]
    [InlineData(-1, 1, -1)]
    [InlineData(314159, 100000, 3.14159)]
    [InlineData(-314159, 100000, -3.14159)]
    public void FromFraction_ReturnsExpectedResult(int numerator, int denominator, double expected)
    {
        Assert.Equal(expected, (double)Fixed.FromFraction(numerator, denominator), FloatingPointComparer.FromPrecision(Fixed.Precision));
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(0, 1, 0.1)]
    [InlineData(0, 2, 0.2)]
    [InlineData(0, 25, 0.25)]
    [InlineData(1, 0, 1)]
    [InlineData(-1, 0, -1)]
    [InlineData(1, 1, 1.1)]
    [InlineData(-1, 1, -1.1)]
    [InlineData(3, 14159, 3.14159)]
    [InlineData(-3, 14159, -3.14159)]
    public void FromParts_ReturnsExpectedResult_IntOverload(int integral, int fractional, double expected)
    {
        Assert.Equal(expected, (double)Fixed.FromParts(integral, fractional), FloatingPointComparer.FromPrecision(Fixed.Precision));
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(0, 1, 0.1)]
    [InlineData(0, 2, 0.2)]
    [InlineData(0, 25, 0.25)]
    [InlineData(1, 0, 1)]
    [InlineData(-1, 0, -1)]
    [InlineData(1, 1, 1.1)]
    [InlineData(-1, 1, -1.1)]
    [InlineData(3, 14159, 3.14159)]
    [InlineData(-3, 14159, -3.14159)]
    public void CreateParts_ReturnsExpectedResult_LongOverload(long integral, int fractional, double expected)
    {
        Assert.Equal(expected, (double)Fixed.FromParts(integral, fractional), FloatingPointComparer.FromPrecision(Fixed.Precision));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(123)]
    [InlineData(123.456)]
    public void CreateChecked_ReturnsExpectedValue(double input)
    {
        Assert.Equal(input, (double)Fixed.CreateChecked(input), FloatingPointComparer.FromPrecision(Fixed.Precision));
    }

    [Fact]
    public void CreateChecked_HasCorrect_EndpointBehavior()
    {
        Assert.Throws<OverflowException>(() =>
        {
            Fixed.CreateChecked((decimal)Fixed.MaxValue * 2);
        });

        Assert.Throws<OverflowException>(() =>
        {
            Fixed.CreateChecked((long)Fixed.MaxValue * 2);
        });

        Assert.Throws<OverflowException>(() =>
        {
            Fixed.CreateChecked((decimal)Fixed.MinValue * 2);
        });

        Assert.Throws<OverflowException>(() =>
        {
            Fixed.CreateChecked((long)Fixed.MinValue * 2);
        });
    }

    [Fact]
    public void CreateSaturating_HasCorrect_EndpointBehavior()
    {
        Assert.Equal(Fixed.MaxValue, Fixed.CreateSaturating((decimal)Fixed.MaxValue * 2));
        Assert.Equal(Fixed.MaxValue, Fixed.CreateSaturating((long)Fixed.MaxValue * 2));

        Assert.Equal(Fixed.MinValue, Fixed.CreateSaturating((decimal)Fixed.MinValue * 2));
        Assert.Equal(Fixed.MinValue, Fixed.CreateSaturating((long)Fixed.MinValue * 2));
    }

    [Fact]
    public void Other_GenericCreate_ReturnsCorrectValue()
    {
        // This intentionally only tests for normal circumstances (no overflow)
        Assert.Equal(5, byte.CreateChecked((Fixed)5));
        Assert.Equal(5u, uint.CreateChecked((Fixed)5));
        Assert.Equal(5, int.CreateChecked((Fixed)5));
        Assert.Equal(5, float.CreateChecked((Fixed)5));
        Assert.Equal(5, double.CreateChecked((Fixed)5));

        Assert.Equal(5, byte.CreateSaturating((Fixed)5));
        Assert.Equal(5u, uint.CreateSaturating((Fixed)5));
        Assert.Equal(5, int.CreateSaturating((Fixed)5));
        Assert.Equal(5, float.CreateSaturating((Fixed)5));
        Assert.Equal(5, double.CreateSaturating((Fixed)5));

        Assert.Equal(5, byte.CreateTruncating((Fixed)5));
        Assert.Equal(5u, uint.CreateTruncating((Fixed)5));
        Assert.Equal(5, int.CreateTruncating((Fixed)5));
        Assert.Equal(5, float.CreateTruncating((Fixed)5));
        Assert.Equal(5, double.CreateTruncating((Fixed)5));
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(0, 0)]
    [InlineData(-1, -1)]
    [InlineData(123.456, 1)]
    [InlineData(-123.456, -1)]
    public void Sign_ReturnsExpectedValue(double input, int expected)
    {
        Assert.Equal(expected, (int)Fixed.Sign((Fixed)input));
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(0, 1)]
    [InlineData(-1, -1)]
    [InlineData(123.456, 1)]
    [InlineData(-123.456, -1)]
    public void SignNonZero_ReturnsExpectedValue(double input, int expected)
    {
        Assert.Equal(expected, (int)Fixed.SignNonZero((Fixed)input));
    }

    [Theory]
    [InlineData(1, 5, 1)]
    [InlineData(1, 4, 1)]
    [InlineData(3, 14159, 3)]
    [InlineData(-3, 5, -4)]
    public void Floor_ReturnsExpectedValue(long integral, int fractional, int expected)
    {
        Assert.Equal(expected, (int)Fixed.Floor(Fixed.FromParts(integral, fractional)));
    }

    [Theory]
    [InlineData(1, 5, 2)]
    [InlineData(1, 4, 2)]
    [InlineData(3, 14159, 4)]
    [InlineData(-3, 5, -3)]
    public void Ceiling_ReturnsExpectedValue(long integral, int fractional, int expected)
    {
        Assert.Equal(expected, (int)Fixed.Ceiling(Fixed.FromParts(integral, fractional)));
    }

    [Theory]
    [InlineData(1, 5, 2)]
    [InlineData(1, 4, 1)]
    [InlineData(3, 14159, 3)]
    [InlineData(-3, 5, -4)]
    public void Round_ReturnsExpectedValue(long integral, int fractional, int expected)
    {
        Assert.Equal(expected, (int)Fixed.Round(Fixed.FromParts(integral, fractional)));
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(0, 25, 0.5)]
    [InlineData(2, 0, 1.41421)]
    [InlineData(4, 0, 2)]
    [InlineData(36, 0, 6)]
    [InlineData(72, 0, 8.48528)]
    [InlineData(123456, 0, 351.36306)]
    [InlineData(123456789, 0, 11111.11106)]
    [InlineData(123456789012, 0, 351364.18288)]
    public void Sqrt_ReturnsExpectedValue(long integral, int fractional, double expected)
    {
        Assert.Equal(expected, (double)Fixed.Sqrt(Fixed.FromParts(integral, fractional)), FloatingPointComparer.FromPrecision(Fixed.Precision));
    }

    [Fact]
    public void Sqrt_ReturnsExpectedValue_ForGreaterThanOne()
    {
        var current = 1.0;
        var multiplier = 1.025;
        for (var i = 0; i < 1000; i++)
        {
            current *= multiplier;
            AssertEqual(i, current, double.Sqrt(current), (double)Fixed.Sqrt((Fixed)current), FloatingPointComparer.FromPrecision(Fixed.Precision));
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
            AssertEqual(i, current, double.Sqrt(current), (double)Fixed.Sqrt((Fixed)current), FloatingPointComparer.FromPrecision(Fixed.Precision));
        }
    }

    [Fact]
    public void Sqrt_ReturnsExpectedValue_PerfectSquares()
    {
        for (var i = 0; i < 1000000; i++)
        {
            var input = (long)i * i;
            AssertEqual(i, input, double.Sqrt(input), (double)Fixed.Sqrt((Fixed)input), FloatingPointComparer.FromPrecision(Fixed.Precision));
        }
    }

    [Fact]
    public void Sqrt_OfMaxValue_ReturnsExpectedValue()
    {
        Assert.Equal(double.Sqrt((double)Fixed.MaxValue), (double)Fixed.Sqrt(Fixed.MaxValue), FloatingPointComparer.FromPrecision(Fixed.Precision));
    }

    [Fact]
    public void Sqrt_OfEpsilon_ReturnsExpectedValue()
    {
        Assert.Equal(double.Sqrt((double)Fixed.Epsilon), (double)Fixed.Sqrt(Fixed.Epsilon), FloatingPointComparer.FromPrecision(Fixed.Precision));
    }

    [Fact]
    public void Cbrt_ReturnsExpectedValue_ForGreaterThanOne()
    {
        var current = 1.0;
        var multiplier = 1.025;
        for (var i = 0; i < 1000; i++)
        {
            current *= multiplier;
            AssertEqual(i, current, double.Cbrt(current), (double)Fixed.Cbrt((Fixed)current), FloatingPointComparer.FromPrecision(Fixed.Precision));
            AssertEqual(i, -current, double.Cbrt(-current), (double)Fixed.Cbrt((Fixed)(-current)), FloatingPointComparer.FromPrecision(Fixed.Precision));
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
            AssertEqual(i, current, double.Cbrt(current), (double)Fixed.Cbrt((Fixed)current), FloatingPointComparer.FromPrecision(Fixed.Precision - 1));
            AssertEqual(i, -current, double.Cbrt(-current), (double)Fixed.Cbrt((Fixed)(-current)), FloatingPointComparer.FromPrecision(Fixed.Precision - 1));
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
                _ => FloatingPointComparer.FromPrecision(Fixed.Precision),
            };

            AssertEqualIndexCoord(i, x, y, expected, (double)Fixed.Hypot((Fixed)y, (Fixed)x), comparer);

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
        Assert.Equal(expected, (double)Fixed.Sin((Fixed)input), FloatingPointComparer.FromPrecision(Fixed.Precision));
    }

    [Fact]
    public void Sin_ReturnsExpectedValue_ForWideRange()
    {
        var current = 0.0001;
        var multiplier = 1.025;
        for (var i = 0; i < 1350; i++)
        {
            current *= multiplier;

            AssertEqual(i, current, double.Sin(current), (double)Fixed.Sin((Fixed)current), FloatingPointComparer.FromPrecision(Fixed.Precision));
            AssertEqual(i, -current, double.Sin(-current), (double)Fixed.Sin((Fixed)(-current)), FloatingPointComparer.FromPrecision(Fixed.Precision));
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

            AssertEqual(i, current, double.SinPi(current), (double)Fixed.SinPi((Fixed)current), FloatingPointComparer.FromPrecision(Fixed.Precision));
            AssertEqual(i, -current, double.SinPi(-current), (double)Fixed.SinPi((Fixed)(-current)), FloatingPointComparer.FromPrecision(Fixed.Precision));
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

            AssertEqual(i, current, double.Cos(current), (double)Fixed.Cos((Fixed)current), FloatingPointComparer.FromPrecision(Fixed.Precision));
            AssertEqual(i, -current, double.Cos(-current), (double)Fixed.Cos((Fixed)(-current)), FloatingPointComparer.FromPrecision(Fixed.Precision));
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

            AssertEqual(i, current, double.CosPi(current), (double)Fixed.CosPi((Fixed)current), FloatingPointComparer.FromPrecision(Fixed.Precision));
            AssertEqual(i, -current, double.CosPi(-current), (double)Fixed.CosPi((Fixed)(-current)), FloatingPointComparer.FromPrecision(Fixed.Precision));
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
                    _ when M.Abs(expected) > 2 => FloatingPointComparer.FromPrecision(Fixed.Precision - 2), // Slope is >= 5 at this point
                    _ when M.Abs(expected) > 1 => FloatingPointComparer.FromPrecision(Fixed.Precision - 1), // Slope is >= 2 at this point
                    _ => FloatingPointComparer.FromPrecision(Fixed.Precision), // Slope is always >= 1
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
                    _ when M.Abs(expected) > 2 => FloatingPointComparer.FromPrecision(Fixed.Precision - 2), // Slope is >= 5 at this point
                    _ when M.Abs(expected) > 1 => FloatingPointComparer.FromPrecision(Fixed.Precision - 1), // Slope is >= 2 at this point
                    _ => FloatingPointComparer.FromPrecision(Fixed.Precision), // Slope is always >= 1
                };

                AssertEqual(i, input, expected, (double)Fixed.TanPi((Fixed)input), comparer);
            }
        }
    }

    [Fact]
    public void SinCos_ReturnsExpectedValue_ForWideRange()
    {
        var comparer = FloatingPointComparer.FromPrecision(Fixed.Precision);
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
        var comparer = FloatingPointComparer.FromPrecision(Fixed.Precision);
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

            AssertEqualIndexCoord(i, x, y, double.Atan2(y, x), (double)Fixed.Atan2((Fixed)y, (Fixed)x), FloatingPointComparer.FromPrecision(Fixed.Precision - 2));

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
        AssertEqualCoord(x, y, double.Atan2(y, x), (double)Fixed.Atan2((Fixed)y, (Fixed)x), FloatingPointComparer.FromPrecision(Fixed.Precision));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(-1, 0)]
    [InlineData(0, 1)]
    [InlineData(0, -1)]
    public void Atan2_ReturnsExpectedValue_ForPointsOnAxes(double x, double y)
    {
        AssertEqualCoord(x, y, double.Atan2(y, x), (double)Fixed.Atan2((Fixed)y, (Fixed)x), FloatingPointComparer.FromPrecision(Fixed.Precision));
    }

    [Fact]
    public void Atan_ReturnsExpectedValue_ForWideRange()
    {
        var current = 0.0001;
        var multiplier = 1.025;
        for (var i = 0; i < 1350; i++)
        {
            current *= multiplier;

            AssertEqual(i, current, double.Atan(current), (double)Fixed.Atan((Fixed)current), FloatingPointComparer.FromPrecision(Fixed.Precision));
            AssertEqual(i, -current, double.Atan(-current), (double)Fixed.Atan((Fixed)(-current)), FloatingPointComparer.FromPrecision(Fixed.Precision));
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
            AssertEqual(i, input, double.Asin(input), (double)Fixed.Asin((Fixed)input), FloatingPointComparer.FromPrecision(Fixed.Precision - 2));
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
            AssertEqual(i, input, double.Acos(input), (double)Fixed.Acos((Fixed)input), FloatingPointComparer.FromPrecision(Fixed.Precision - 2));
        }
    }

    [Fact]
    public void Log2_ReturnsRoughly16ForEpsilon()
    {
        // Note it does not actually return 16
        var result = (double)Fixed.Log2(Fixed.Epsilon);
        Assert.Equal(-16, result, FloatingPointComparer.FromPrecision(Fixed.Precision));
        Assert.NotEqual(-16, result);
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
            AssertEqual(i, current, double.Log2(current), (double)Fixed.Log2((Fixed)current), FloatingPointComparer.FromPrecision(Fixed.Precision));
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
                _ when M.Abs(i) > 750 => FloatingPointComparer.FromPrecision(Fixed.Precision - 2),
                _ when M.Abs(i) > 350 => FloatingPointComparer.FromPrecision(Fixed.Precision - 1),
                _ => FloatingPointComparer.FromPrecision(Fixed.Precision),
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
            AssertEqual(i, current, double.Log(current), (double)Fixed.Log((Fixed)current), FloatingPointComparer.FromPrecision(Fixed.Precision));
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
            AssertEqual(i, current, double.Log10(current), (double)Fixed.Log10((Fixed)current), FloatingPointComparer.FromPrecision(Fixed.Precision - 1));
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
            var logBase = current % 10;
            var expected = double.Log(current, logBase);
            var comparer = FloatingPointComparer.FromTolerance((decimal)expected * 0.01M);
            AssertEqualLogBase(i, current, logBase, double.Log(current, logBase), (double)Fixed.Log((Fixed)current, (Fixed)logBase), comparer);
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
            var comparer = FloatingPointComparer.FromTolerance((decimal)expected * 0.001M);
            AssertEqual(i, current, double.Exp2(current), (double)Fixed.Exp2((Fixed)current), comparer);
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

    private void AssertEqual(int i, double input, double expected, double actual, FloatingPointComparer comparer)
    {
        Assert.True(comparer.Equals(expected, actual), $"""
            Failed at i: {i}
            Input:       {input}
            Expected:    {expected}
            Actual:      {actual}
            Difference:  {(decimal)M.Abs(expected - actual)}
            Tolerance:   {comparer.Tolerance}
            """);
    }

    private void AssertEqualLogBase(int i, double input, double logBase, double expected, double actual, FloatingPointComparer comparer)
    {
        Assert.True(comparer.Equals(expected, actual), $"""
            Failed at i: {i}
            Input:       Log({input}, {logBase})
            Expected:    {expected}
            Actual:      {actual}
            Difference:  {(decimal)M.Abs(expected - actual)}
            Tolerance:   {comparer.Tolerance}
            """);
    }

    private void AssertEqualCoord(double x, double y, double expected, double actual, FloatingPointComparer comparer)
    {
        Assert.True(comparer.Equals(expected, actual), $"""
            Failed for input: ({x}, {y})
            Expected:   {expected}
            Actual:     {actual}
            Difference: {(decimal)M.Abs(expected - actual)}
            Tolerance:  {comparer.Tolerance}
            """);
    }

    private void AssertEqualIndexCoord(int i, double x, double y, double expected, double actual, FloatingPointComparer comparer)
    {
        Assert.True(comparer.Equals(expected, actual), $"""
            Failed at i: {i}
            Input:       ({x}, {y})
            Expected:    {expected}
            Actual:      {actual}
            Difference:  {(decimal)M.Abs(expected - actual)}
            Tolerance:   {comparer.Tolerance}
            """);
    }
}
