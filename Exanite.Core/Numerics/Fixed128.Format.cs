using System;
using System.Globalization;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

public partial struct Fixed128
{
    public override string ToString() => ToString(null, null);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var formatSpan = format.AsSpan();
        var precisionSpan = formatSpan.Length > 1 ? formatSpan[1..] : [];

        // Parse precision for the purpose of pre-allocating memory
        var precision = 0;
        if (precisionSpan.Length != 0)
        {
            if (!uint.TryParse(precisionSpan, CultureInfo.InvariantCulture, out var requestedPrecision))
            {
                throw new FormatException($"The requested format is not supported: {formatSpan}");
            }

            precision = (int)requestedPrecision;
        }

        // The heap array size is a bit excessive here, but realistically, the heap allocation path is never taken
        var result = precision <= FractionalBitCount ? stackalloc char[64] : new char[64 + M.Abs(precision)];
        var isSuccess = TryFormat(result, out var written, format, formatProvider);
        AssertUtility.IsTrue(isSuccess, "Internal: Failed to format fixed point number as a string");

        return result[..written].ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        // Parse the format
        // -1 is used to represent an unspecified amount of precision
        var formatType = 'G';
        var precision = -1;
        if (format.Length > 1)
        {
            formatType = char.ToUpper(format[0]);

            var precisionSpan = format[1..];
            if (precisionSpan.Length != 0)
            {
                if (!int.TryParse(precisionSpan, CultureInfo.InvariantCulture, out var requestedPrecision))
                {
                    charsWritten = 0;
                    return false;
                }

                precision = requestedPrecision;
            }
        }

        // Map to internal format
        switch (formatType)
        {
            case 'G':
            {
                return TryFormatInternal(destination, out charsWritten, 'G', precision, provider);
            }
            case 'F':
            {
                return TryFormatInternal(destination, out charsWritten, 'F', precision, provider);
            }
            case 'N':
            {
                return TryFormatInternal(destination, out charsWritten, 'N', precision, provider);
            }
            case 'R':
            {
                return TryFormatInternal(destination, out charsWritten, 'G', -1, provider);
            }
            default:
            {
                charsWritten = 0;
                return false;
            }
        }
    }

    private bool TryFormatInternal(Span<char> destination, out int charsWritten, char formatType, int precision, IFormatProvider? provider)
    {
        // Further normalize format
        var formatInfo = NumberFormatInfo.GetInstance(provider);
        switch (formatType)
        {
            case 'G':
            {
                // Do nothing
                break;
            }
            case 'F':
            case 'N':
            {
                // Precision for F and N default to NumberFormatInfo.NumberDecimalDigits, if not specified
                if (precision < 0)
                {
                    precision = formatInfo.NumberDecimalDigits;
                }

                break;
            }
            default:
            {
                GuardUtility.Throw($"Internal: Incorrect format type: {formatType}");

                charsWritten = 0;
                return false;
            }
        }

        // TODO
    }
}
