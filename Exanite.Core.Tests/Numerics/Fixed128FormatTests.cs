using System;
using System.Globalization;
using Exanite.Core.Numerics;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class Fixed128FormatTests
{
    public static TheoryData<Fixed128, string?, string> ToString_ReturnsCorrectString_Data()
    {
        // Based on https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
        return
        [
            // G and F should behave the same.
            // N is similar, but with group separators.
            // R is the same as G
            //
            // Lowercase variants should also work.
            //
            // Not supported:
            // P
            // X

            // TODO: All code below this is AI generated and not yet reviewed. Use with caution.

            // Default formatting (G)
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.Zero, null, "0"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.One, null, "1"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.NegativeOne, null, "-1"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.Half, null, "0.5"),

            new TheoryDataRow<Fixed128, string?, string>(Fixed128.Zero, "G", "0"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.One, "G", "1"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.NegativeOne, "G", "-1"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.Half, "G", "0.5"),

            new TheoryDataRow<Fixed128, string?, string>(Fixed128.Zero, "g", "0"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.One, "g", "1"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.NegativeOne, "g", "-1"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.Half, "g", "0.5"),

            // Fixed-point (F)
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.FromDecimal(123, 456, 3), "F2", "123.46"), // Rounds for display
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.FromDecimal(0, 5, 1), "F0", "0"), // Round to even
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.FromDecimal(1, 5, 1), "F0", "2"), // Round to even // TODO: Check which midpoint rounding should be used

            // Numeric (N)
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.One * 1000, "N0", "1,000"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.FromDecimal(1, 25, 2), "N2", "1.25"),

            new TheoryDataRow<Fixed128, string?, string>(Fixed128.One * 1000, "n0", "1,000"),
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.FromDecimal(1, 25, 2), "n2", "1.25"),



            // Precision testing (Epsilon)
            new TheoryDataRow<Fixed128, string?, string>(Fixed128.Epsilon, "F32", "0.00000000023283064365386962890625"),
        ];
    }

    [Theory]
    [MemberData(nameof(ToString_ReturnsCorrectString_Data))]
    public void ToString_ReturnsCorrectString_InvariantCulture(Fixed128 value, string? format, string expected)
    {
        var result = value.ToString(format, CultureInfo.InvariantCulture);
        Assert.Equal(expected, result);
    }

    public static TheoryData<Fixed128, string, string> ToString_RespectsFormatProvider_Data()
    {
        return
        [
            // Germany uses dot for thousands and comma for decimal
            new TheoryDataRow<Fixed128, string, string>(Fixed128.FromDecimal(1234, 5, 1), "N1", "1.234,5"), // TODO: This is wrong
            // US uses comma for thousands and dot for decimal
            new TheoryDataRow<Fixed128, string, string>(Fixed128.FromDecimal(1234, 5, 1), "N1", "1,234.5"),
            // French uses space for thousands and comma for decimal
            new TheoryDataRow<Fixed128, string, string>(Fixed128.FromDecimal(1234, 5, 1), "N1", "1\u00A0234,5"),
        ];
    }

    [Theory]
    [MemberData(nameof(ToString_RespectsFormatProvider_Data))]
    public void ToString_RespectsFormatProvider(Fixed128 value, string cultureName, string expected)
    {
        var culture = new CultureInfo(cultureName);
        var result = value.ToString("N1", culture);

        // Normalize spaces for the French culture test if necessary
        var actual = result.Replace('\u202F', '\u00A0'); // TODO: Document what these characters are

        Assert.Equal(expected, actual);
    }

    [Fact] // TODO: Fix inconsistent naming convention
    public void ToString_NoArguments_UsesDefaultFormat()
    {
        var value = Fixed128.FromDecimal(10, 75, 2);
        var expected = "10.75"; // Default Invariant behavior usually

        Assert.Equal(expected, value.ToString(null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void TryFormat_ReturnsTrue_IfBufferIsLargeEnough()
    {
        var value = Fixed128.One;
        Span<char> destination = stackalloc char[10];

        var isSuccess = value.TryFormat(destination, out var charsWritten, "G", CultureInfo.InvariantCulture);

        Assert.True(isSuccess);
        Assert.Equal("1", destination[..charsWritten].ToString());
    }

    [Fact]
    public void TryFormat_ReturnsFalse_IfBufferIsTooSmall()
    {
        var value = Fixed128.FromDecimal(123, 45, 2); // "123.45"
        Span<char> destination = stackalloc char[2];

        var isSuccess = value.TryFormat(destination, out var charsWritten, "G", CultureInfo.InvariantCulture);

        Assert.False(isSuccess);
        Assert.Equal(0, charsWritten);
    }

    // TODO: Add round trip tests to ensure bit perfect precision
}
