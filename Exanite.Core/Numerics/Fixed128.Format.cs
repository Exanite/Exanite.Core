using System;
using System.Globalization;
using System.Linq;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

public partial struct Fixed128
{
    private const int ToStringInternalStackBufferSize = 256;

    public override string ToString() => ToString(null, null);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var formatSpan = format.AsSpan();
        var precisionSpan = formatSpan.Length > 1 ? formatSpan[1..] : [];

        // Parse precision for the purpose of pre-allocating memory
        var precision = -1;
        if (precisionSpan.Length != 0)
        {
            if (!uint.TryParse(precisionSpan, CultureInfo.InvariantCulture, out var requestedPrecision))
            {
                throw new FormatException($"The requested format is not supported: {formatSpan}");
            }

            precision = (int)requestedPrecision;
        }

        var formatInfo = NumberFormatInfo.GetInstance(formatProvider);
        var maxLength = GetToStringMaxLength(precision, formatInfo);
        var result = maxLength <= ToStringInternalStackBufferSize ? stackalloc char[ToStringInternalStackBufferSize] : new char[maxLength];
        var isSuccess = TryFormat(result, out var written, format, formatProvider);
        AssertUtility.IsTrue(isSuccess, "Internal: Failed to format fixed point number as a string");

        return result[..written].ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        var formatInfo = NumberFormatInfo.GetInstance(provider);

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
                // Disallow precision specifier for general format (for simplicity)
                // We also always show all significant digits
                if (precision >= 0)
                {
                    charsWritten = 0;
                    return false;
                }

                return TryFormatInternal(destination, out charsWritten, 'G', -1, formatInfo);
            }
            case 'F':
            {
                return TryFormatInternal(destination, out charsWritten, 'F', precision, formatInfo);
            }
            case 'N':
            {
                return TryFormatInternal(destination, out charsWritten, 'N', precision, formatInfo);
            }
            case 'R':
            {
                // Roundtrip just uses the general format
                return TryFormatInternal(destination, out charsWritten, 'G', -1, formatInfo);
            }
            default:
            {
                charsWritten = 0;
                return false;
            }
        }
    }

    private int GetToStringMaxLength(int precision, IFormatProvider? provider)
    {
        if (precision < 0)
        {
            precision = FractionalBitCount;
        }

        var formatInfo = NumberFormatInfo.GetInstance(provider);

        var signCharCount = M.Max(formatInfo.PositiveSign.Length, formatInfo.NegativeSign.Length);
        var integralDigits = (int)M.Ceiling(IntegralBitCount * double.Log10(2));

        var groupSize = formatInfo.NumberGroupSizes.Max();
        var groupSeparatorCharCount = groupSize > 0 ? formatInfo.NumberGroupSeparator.Length * (integralDigits / groupSize) : 0;

        return signCharCount + integralDigits + groupSeparatorCharCount + formatInfo.NumberDecimalSeparator.Length + precision;
    }

    private bool TryFormatInternal(Span<char> destination, out int charsWritten, char formatType, int precision, IFormatProvider? provider)
    {
        var formatInfo = NumberFormatInfo.GetInstance(provider);

        // Further normalize format
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

        // Allocate internal buffer
        var maxLength = GetToStringMaxLength(precision, formatInfo);
        var fullResult = maxLength <= ToStringInternalStackBufferSize ? stackalloc char[ToStringInternalStackBufferSize] : new char[maxLength];
        var unwrittenResult = fullResult;
        var internalCharsWritten = 0;

        // Write leading negative sign
        if (IsNegative(this))
        {
            switch (formatInfo.NumberNegativePattern)
            {
                case 0:
                {
                    unwrittenResult[0] = '(';
                    unwrittenResult = unwrittenResult[1..];
                    internalCharsWritten++;

                    break;
                }
                case 1:
                {
                    foreach (var c in formatInfo.NegativeSign)
                    {
                        unwrittenResult[0] = c;
                        unwrittenResult = unwrittenResult[1..];
                        internalCharsWritten++;
                    }

                    break;
                }
                case 2:
                {
                    foreach (var c in formatInfo.NegativeSign)
                    {
                        unwrittenResult[0] = c;
                        unwrittenResult = unwrittenResult[1..];
                        internalCharsWritten++;
                    }

                    unwrittenResult[0] = ' ';
                    unwrittenResult = unwrittenResult[1..];
                    internalCharsWritten++;

                    break;
                }
                case 3:
                case 4:
                {
                    break;
                }
                default: throw new ArgumentOutOfRangeException(nameof(formatInfo.NumberNegativePattern), $"Invalid value for NumberNegativePattern: {formatInfo.NumberNegativePattern}");
            }
        }

        // Write integral portion
        {
            var integral = M.Abs(Raw) >> Shift;

            var integralFormat = "G";
            if (formatType == 'N')
            {
                integralFormat = "N0";
            }

            if (!integral.TryFormat(unwrittenResult, out var integralCharsWritten, integralFormat, provider))
            {
                charsWritten = 0;
                return false;
            }

            unwrittenResult = unwrittenResult[integralCharsWritten..];
            internalCharsWritten += integralCharsWritten;
        }

        // Write fractional portion
        {
            var fractional = (long)(Raw & Mask);
            if (fractional != 0)
            {
                // Write decimal
                foreach (var c in formatInfo.NumberDecimalSeparator)
                {
                    unwrittenResult[0] = c;
                    unwrittenResult = unwrittenResult[1..];
                    internalCharsWritten++;
                }
            }

            // Prefill with zeroes
            unwrittenResult[..FractionalBitCount].Fill('0');

            // Write digits
            while (fractional != 0)
            {
                fractional *= 10;

                var digit = (int)(fractional >> Shift);
                fractional &= Mask;

                unwrittenResult[0] = (char)(digit + '0');
                unwrittenResult = unwrittenResult[1..];
                internalCharsWritten++;
            }
        }

        // TODO: Apply format

        // Write trailing negative sign
        if (IsNegative(this))
        {
            switch (formatInfo.NumberNegativePattern)
            {
                case 0:
                {
                    unwrittenResult[0] = ')';
                    unwrittenResult = unwrittenResult[1..];
                    internalCharsWritten++;

                    break;
                }
                case 1:
                case 2:
                {
                    break;
                }
                case 3:
                {
                    foreach (var c in formatInfo.NegativeSign)
                    {
                        unwrittenResult[0] = c;
                        unwrittenResult = unwrittenResult[1..];
                        internalCharsWritten++;
                    }

                    break;
                }
                case 4:
                {
                    unwrittenResult[0] = ' ';
                    unwrittenResult = unwrittenResult[1..];
                    internalCharsWritten++;

                    foreach (var c in formatInfo.NegativeSign)
                    {
                        unwrittenResult[0] = c;
                        unwrittenResult = unwrittenResult[1..];
                        internalCharsWritten++;
                    }

                    break;
                }
                default: throw new ArgumentOutOfRangeException(nameof(formatInfo.NumberNegativePattern), $"Invalid value for NumberNegativePattern: {formatInfo.NumberNegativePattern}");
            }
        }

        // Write to destination
        if (destination.Length < internalCharsWritten)
        {
            charsWritten = 0;
            return false;
        }

        fullResult[..internalCharsWritten].CopyTo(destination);
        charsWritten = internalCharsWritten;
        return true;
    }
}
