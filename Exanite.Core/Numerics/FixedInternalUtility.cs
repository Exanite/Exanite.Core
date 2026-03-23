using System;
using System.Diagnostics;
using System.Globalization;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

internal static class FixedInternalUtility
{
    public static void ValidateNumberStyle(NumberStyles specified, NumberStyles allowed)
    {
        var notAllowed = specified & ~allowed;
        GuardUtility.IsTrue(notAllowed == 0, $"Unsupported number format: {notAllowed}");
    }

    public static bool TryParseToStringFormat(ReadOnlySpan<char> format, out char internalFormat, out int precision)
    {
        internalFormat = 'G';
        precision = -1;
        if (format.Length > 0)
        {
            internalFormat = char.ToUpper(format[0]);

            if (format.Length > 1)
            {
                var precisionSpan = format[1..];
                if (precisionSpan.Length != 0)
                {
                    if (!int.TryParse(precisionSpan, CultureInfo.InvariantCulture, out var requestedPrecision))
                    {
                        return false;
                    }

                    precision = requestedPrecision;
                }
            }
        }

        switch (internalFormat)
        {
            // Roundtrip just uses the general format
            case 'R':
            case 'G':
            {
                // Disallow precision specifier for general format (for simplicity)
                if (precision >= 0)
                {
                    return false;
                }

                internalFormat = 'G';
                return true;
            }
            case 'F':
            case 'N':
            {
                return true;
            }
            default:
            {
                return false;
            }
        }
    }

    public static void ThrowOverflowException()
    {
        throw new OverflowException();
    }

    [Conditional("DEBUG")]
    public static void AssertExpectedRange(long x, int shift, decimal inclusiveMin, decimal exclusiveMax)
    {
        AssertUtility.IsFalse((decimal)x / (1L << shift) < inclusiveMin, "Internal: Value is less than the required minimum");
        AssertUtility.IsFalse((decimal)x / (1L << shift) >= exclusiveMax, "Internal: Value is greater than the required maximum");
    }

    [Conditional("DEBUG")]
    public static void AssertExpectedRange(Int128 x, int shift, decimal inclusiveMin, decimal exclusiveMax)
    {
        AssertUtility.IsFalse((decimal)x / (1L << shift) < inclusiveMin, "Internal: Value is less than the required minimum");
        AssertUtility.IsFalse((decimal)x / (1L << shift) >= exclusiveMax, "Internal: Value is greater than the required maximum");
    }
}
