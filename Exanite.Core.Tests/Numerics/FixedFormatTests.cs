using System;
using System.Globalization;
using Exanite.Core.Numerics;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class FixedFormatTests
{
    public static TheoryData<Fixed, string?, string> ToString_ReturnsCorrectValue_Data()
    {
        // Based on https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
        return
        [
            // Supported:
            // G, F, N, R
            //
            // Lowercase variants should also work
            //
            // Not supported:
            // C, D, E, P, X

            // Default formatting (G)
            new TheoryDataRow<Fixed, string?, string>(Fixed.Zero, null, "0"),
            new TheoryDataRow<Fixed, string?, string>(Fixed.One, null, "1"),
            new TheoryDataRow<Fixed, string?, string>(Fixed.NegativeOne, null, "-1"),
            new TheoryDataRow<Fixed, string?, string>(Fixed.Half, null, "0.5"),

            new TheoryDataRow<Fixed, string?, string>(Fixed.Zero, "G", "0"),
            new TheoryDataRow<Fixed, string?, string>(Fixed.One, "G", "1"),
            new TheoryDataRow<Fixed, string?, string>(Fixed.NegativeOne, "G", "-1"),
            new TheoryDataRow<Fixed, string?, string>(Fixed.Half, "G", "0.5"),

            new TheoryDataRow<Fixed, string?, string>(Fixed.Zero, "g", "0"),
            new TheoryDataRow<Fixed, string?, string>(Fixed.One, "g", "1"),
            new TheoryDataRow<Fixed, string?, string>(Fixed.NegativeOne, "g", "-1"),
            new TheoryDataRow<Fixed, string?, string>(Fixed.Half, "g", "0.5"),

            // Fixed-point (F)
            new TheoryDataRow<Fixed, string?, string>(Fixed.FromDecimal(123, 456, 3), "F2", "123.46"), // Round for display
            new TheoryDataRow<Fixed, string?, string>(Fixed.FromDecimal(0, 5, 1), "F0", "0"), // Round to even
            new TheoryDataRow<Fixed, string?, string>(Fixed.FromDecimal(1, 5, 1), "F0", "2"), // Round to even
            new TheoryDataRow<Fixed, string?, string>(Fixed.FromDecimal(999, 999, 3), "F0", "1000"), // Carry rounded values

            new TheoryDataRow<Fixed, string?, string>(Fixed.One, "F64", "1.0000000000000000000000000000000000000000000000000000000000000000"), // Pad zeroes

            // Numeric (N)
            new TheoryDataRow<Fixed, string?, string>(Fixed.One * 1000, "N", "1,000"),

            new TheoryDataRow<Fixed, string?, string>(Fixed.FromDecimal(1, 25, 2), "N0", "1"),
            new TheoryDataRow<Fixed, string?, string>(Fixed.FromDecimal(1, 25, 2), "N1", "1.2"),
            new TheoryDataRow<Fixed, string?, string>(Fixed.FromDecimal(1, 25, 2), "N2", "1.25"),
            new TheoryDataRow<Fixed, string?, string>(Fixed.FromDecimal(1, 25, 2), "N3", "1.250"),

            new TheoryDataRow<Fixed, string?, string>(Fixed.FromDecimal(9999, 99, 2), "N0", "10,000"),

            // Precision testing (Epsilon)
            new TheoryDataRow<Fixed, string?, string>(Fixed.Epsilon, "G", "0.0000152587890625"),
            new TheoryDataRow<Fixed, string?, string>(Fixed.Epsilon, "R", "0.0000152587890625"),
            new TheoryDataRow<Fixed, string?, string>(Fixed.Epsilon, "F15", "0.000015258789062"),

            // Edge cases
            new TheoryDataRow<Fixed, string?, string>(Fixed.MinValue, "G", "-140737488355327.9999847412109375"),
        ];
    }

    [Theory]
    [MemberData(nameof(ToString_ReturnsCorrectValue_Data))]
    public void ToString_ReturnsCorrectValue_InvariantCulture(Fixed value, string? format, string expected)
    {
        var result = value.ToString(format, CultureInfo.InvariantCulture);
        Assert.Equal(expected, result);
    }

    public static TheoryData<Fixed, string, string> ToString_RespectsFormatProvider_Data()
    {
        var arSaFormatInfo = NumberFormatInfo.GetInstance(new CultureInfo("ar-SA"));
        var arSaExpected = $"{arSaFormatInfo.NegativeSign}1\u066C234\u066B5"; // NumberNegativePattern=1, so the negative sign goes in front

        return
        [
            new TheoryDataRow<Fixed, string, string>(Fixed.FromDecimal(1234, 5, 1), "en-US", "1,234.5"),
            new TheoryDataRow<Fixed, string, string>(Fixed.FromDecimal(1234, 5, 1), "de-DE", "1.234,5"),
            new TheoryDataRow<Fixed, string, string>(Fixed.FromDecimal(1234, 5, 1), "en-US", "1,234.5"),
            new TheoryDataRow<Fixed, string, string>(Fixed.FromDecimal(1234, 5, 1), "fa-IR", "1٬234٫5"),
            new TheoryDataRow<Fixed, string, string>(Fixed.FromDecimal(-1234, 5, 1), "ar-SA", arSaExpected),
        ];
    }

    [Theory]
    [MemberData(nameof(ToString_RespectsFormatProvider_Data))]
    public void ToString_RespectsFormatProvider(Fixed value, string cultureName, string expected)
    {
        var culture = new CultureInfo(cultureName);
        var result = value.ToString("N1", culture);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToString_UsesGeneralFormat_ByDefault()
    {
        var value = Fixed.FromDecimal(10, 75, 2);
        Assert.Equal("10.75", value.ToString(null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void TryFormat_ReturnsTrue_IfBufferIsLargeEnough()
    {
        var value = Fixed.One; // "1"
        Span<char> destination = stackalloc char[1];

        var isSuccess = value.TryFormat(destination, out var charsWritten, "G", CultureInfo.InvariantCulture);

        Assert.True(isSuccess);
        Assert.Equal("1", destination[..charsWritten].ToString());
    }

    [Fact]
    public void TryFormat_ReturnsFalse_IfBufferIsTooSmall()
    {
        var value = Fixed.FromDecimal(123, 45, 2); // "123.45"
        Span<char> destination = stackalloc char[2];

        var isSuccess = value.TryFormat(destination, out var charsWritten, "G", CultureInfo.InvariantCulture);

        Assert.False(isSuccess);
        Assert.Equal(0, charsWritten);
    }

    [Fact]
    public void TryFormat_Parse_CanRoundtrip()
    {
        var current = 0.0001;
        var multiplier = 1.025;
        for (var i = 0; i < 1350; i++)
        {
            current *= multiplier;

            var input = (Fixed)current;
            AssertEqualRoundtrip(i, input, Fixed.Parse(input.ToString("R", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture));
        }
    }

    private void AssertEqualRoundtrip(int i, Fixed input, Fixed expected)
    {
        Assert.True(expected == input, $"""
            Failed at i: {i}
            Input:       {input}
            Expected:    {expected}
            Actual:      {input}
            """);
    }
}
