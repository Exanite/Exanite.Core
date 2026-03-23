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
        if (!FixedInternalUtility.TryParseToStringFormat(format, out var internalFormat, out var precision))
        {
            throw new FormatException($"The requested format is not supported: {format}");
        }

        var maxLength = GetToStringMaxLength(precision, formatProvider);
        var result = maxLength <= ToStringInternalStackBufferSize ? stackalloc char[ToStringInternalStackBufferSize] : new char[maxLength];
        var isSuccess = TryFormatInternal(result, out var charsWritten, internalFormat, precision, formatProvider);
        AssertUtility.IsTrue(isSuccess, "Internal: Failed to format fixed point number as a string");

        return result[..charsWritten].ToString();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        if (!FixedInternalUtility.TryParseToStringFormat(format, out var internalFormat, out var precision))
        {
            charsWritten = 0;
            return false;
        }

        return TryFormatInternal(destination, out charsWritten, internalFormat, precision, provider);
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

        var groupSize = M.Max(1, formatInfo.NumberGroupSizes.Min());
        var groupSeparatorCharCount = groupSize > 0 ? formatInfo.NumberGroupSeparator.Length * (integralDigits / groupSize) : 0;

        return signCharCount + integralDigits + groupSeparatorCharCount + formatInfo.NumberDecimalSeparator.Length + precision;
    }

    private bool TryFormatInternal(Span<char> destination, out int charsWritten, char internalFormat, int precision, IFormatProvider? provider)
    {
        var formatInfo = NumberFormatInfo.GetInstance(provider);

        // Further normalize format
        switch (internalFormat)
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
                GuardUtility.Throw($"Internal: Incorrect format type: {internalFormat}");

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

        // Split into parts
        var integral = M.Abs(Raw) >> Shift;
        var fractional = (long)(Raw & Mask);

        // Write integral portion
        var integralDigitsStart = internalCharsWritten;
        var integralDigitsWritten = 0;
        {
            var integralFormat = "G";
            if (internalFormat == 'N')
            {
                integralFormat = "N0";
            }

            if (!integral.TryFormat(unwrittenResult, out integralDigitsWritten, integralFormat, provider))
            {
                charsWritten = 0;
                return false;
            }

            unwrittenResult = unwrittenResult[integralDigitsWritten..];
            internalCharsWritten += integralDigitsWritten;
        }

        // Write fractional portion
        var decimalSeparatorWritten = false;
        var fractionalDigitsStart = internalCharsWritten;
        var fractionalDigitsWritten = 0;
        var fractionalHasRemaining = false;
        if (fractional != 0)
        {
            // Write decimal
            foreach (var c in formatInfo.NumberDecimalSeparator)
            {
                unwrittenResult[0] = c;
                unwrittenResult = unwrittenResult[1..];
                internalCharsWritten++;
            }

            decimalSeparatorWritten = true;

            // Use the complement of the fractional portion if negative
            if (IsNegative(this))
            {
                fractional = OneRaw - fractional;
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
                fractionalDigitsWritten++;

                if (precision >= 0 && fractionalDigitsWritten >= (precision + 1))
                {
                    // Track whether any fractional values are remaining
                    // This lets us know whether we need to round up below
                    fractionalHasRemaining = fractional != 0;
                    break;
                }
            }
        }

        // Round
        if (precision >= 0 && precision + 1 == fractionalDigitsWritten)
        {
            // Exclude the last digit from the final result
            internalCharsWritten--;
            fractionalDigitsWritten--;

            // Check if we need to round
            var currentIndex = precision + 1 + fractionalDigitsStart;
            if (fullResult[currentIndex] >= '5')
            {
                // Need to round
                // Check if next digit is even
                var isSuccess = TryGetNextDigit(ref fullResult, currentIndex, out currentIndex);
                AssertUtility.IsTrue(isSuccess, "Internal: There should always be a next digit since we are in the fractional section");

                var isEven = int.IsEvenInteger(fullResult[currentIndex] - '0');
                if (!isEven || fractionalHasRemaining)
                {
                    // Need to round up
                    // Note that we don't bother incrementing the last digit since it gets truncated anyway
                    fullResult[currentIndex]++;

                    // Carry if necessary
                    while (fullResult[currentIndex] > '9')
                    {
                        fullResult[currentIndex] = '0';

                        if (!TryGetNextDigit(ref fullResult, currentIndex, out var nextIndex))
                        {
                            // Insert one and end
                            fullResult[integralDigitsStart..internalCharsWritten].CopyTo(fullResult[(integralDigitsStart + 1)..(internalCharsWritten + 1)]);
                            fullResult[integralDigitsStart] = '1';
                            internalCharsWritten++;
                            break;
                        }

                        // Found the next digit
                        // Increment and continue
                        currentIndex = nextIndex;
                        fullResult[currentIndex]++;
                    }
                }
            }

            static bool TryGetNextDigit(ref Span<char> fullResult, int currentIndex, out int nextIndex)
            {
                currentIndex--;

                while (currentIndex >= 0)
                {
                    if (char.IsDigit(fullResult[currentIndex]))
                    {
                        nextIndex = currentIndex;
                        return true;
                    }

                    currentIndex--;
                }

                nextIndex = 0;
                return false;
            }
        }

        // Pad with 0s if necessary
        if (NeedsPaddedFractional(internalFormat) && fractionalDigitsWritten < precision)
        {
            if (!decimalSeparatorWritten)
            {
                // Write decimal
                foreach (var c in formatInfo.NumberDecimalSeparator)
                {
                    unwrittenResult[0] = c;
                    unwrittenResult = unwrittenResult[1..];
                    internalCharsWritten++;
                    decimalSeparatorWritten = true;
                }
            }

            var zeroesNeeded = precision - fractionalDigitsWritten;
            fractionalDigitsWritten += zeroesNeeded;
            for (var i = 0; i < zeroesNeeded; i++)
            {
                unwrittenResult[0] = '0';
                unwrittenResult = unwrittenResult[1..];
                internalCharsWritten++;
            }
        }

        // Remove decimal if necessary
        if (decimalSeparatorWritten && fractionalDigitsWritten == 0)
        {
            internalCharsWritten -= formatInfo.NumberDecimalSeparator.Length;
        }

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

    private static bool NeedsPaddedFractional(char internalFormat)
    {
        return internalFormat switch
        {
            'F' => true,
            'N' => true,
            _ => false,
        };
    }
}
