using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Exanite.Core.Numerics;

public partial struct Fixed128
{
    // Create
    // These are based on .NET's own implementation: https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/Int32.cs,718
    // The implementation seems to be the same for all types
    // Note how we have to check both directions

    public static Fixed128 CreateChecked<TOther>(TOther value) where TOther : INumberBase<TOther>
    {
        Fixed128 result;
        if (typeof(TOther) == typeof(Fixed128))
        {
            result = (Fixed128)(object)value;
        }
        else if (!TryConvertFromChecked(value, out result) && !TOther.TryConvertToChecked(value, out result))
        {
            ThrowNotSupportedExceptionForCreate(value);
        }

        return result;
    }

    public static Fixed128 CreateSaturating<TOther>(TOther value) where TOther : INumberBase<TOther>
    {
        Fixed128 result;
        if (typeof(TOther) == typeof(Fixed128))
        {
            result = (Fixed128)(object)value;
        }
        else if (!TryConvertFromSaturating(value, out result) && !TOther.TryConvertToSaturating(value, out result))
        {
            ThrowNotSupportedExceptionForCreate(value);
        }

        return result;
    }

    public static Fixed128 CreateTruncating<TOther>(TOther value) where TOther : INumberBase<TOther>
    {
        Fixed128 result;
        if (typeof(TOther) == typeof(Fixed128))
        {
            result = (Fixed128)(object)value;
        }
        else if (!TryConvertFromTruncating(value, out result) && !TOther.TryConvertToTruncating(value, out result))
        {
            ThrowNotSupportedExceptionForCreate(value);
        }

        return result;
    }

    [DoesNotReturn]
    private static void ThrowNotSupportedExceptionForCreate<TOther>(TOther value) where TOther : INumberBase<TOther>
    {
        throw new NotSupportedException($"Failed to create a fixed point value from the provided value: {value}");
    }

    // TryConvertFrom

    public static bool TryConvertFromChecked<TOther>(TOther value, out Fixed128 result) where TOther : INumberBase<TOther>
    {
        if (TOther.IsInteger(value))
        {
            var int128Value = Int128.CreateChecked(value);
            result = new Fixed128(checked(int128Value * OneRaw));
            return true;
        }

        try
        {
            var decimalValue = decimal.CreateChecked(value);
            result = new Fixed128(checked((Int128)(decimalValue * OneRaw)));
            return true;
        }
        catch (NotSupportedException)
        {
            result = default;
            return false;
        }
    }

    public static bool TryConvertFromSaturating<TOther>(TOther value, out Fixed128 result) where TOther : INumberBase<TOther>
    {
        if (TOther.IsInteger(value))
        {
            var int128Value = Int128.CreateSaturating(value);
            if (int128Value > (Int128)MaxValue)
            {
                result = MaxValue;
                return true;
            }

            if (int128Value < (Int128)MinValue)
            {
                result = MinValue;
                return true;
            }

            result = new Fixed128(int128Value * OneRaw);
            return true;
        }

        try
        {
            var decimalValue = decimal.CreateSaturating(value);
            if (decimalValue > (decimal)MaxValue)
            {
                result = MaxValue;
                return true;
            }

            if (decimalValue < (decimal)MinValue)
            {
                result = MinValue;
                return true;
            }

            result = new Fixed128((Int128)(decimalValue * OneRaw));
            return true;
        }
        catch (NotSupportedException)
        {
            result = default;
            return false;
        }
    }

    public static bool TryConvertFromTruncating<TOther>(TOther value, out Fixed128 result) where TOther : INumberBase<TOther>
    {
        // Not going to support truncating conversation
        return TryConvertFromSaturating(value, out result);
    }

    // TryConvertTo
    // Similar to the Create methods, we have to check both directions here

    public static bool TryConvertToChecked<TOther>(Fixed128 value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        var decimalValue = (decimal)value.Raw / OneRaw;
        return TOther.TryConvertFromChecked(decimalValue, out result) || TryConvertFromCheckedFromDecimal<TOther, decimal>(decimalValue, out result);
    }

    public static bool TryConvertToSaturating<TOther>(Fixed128 value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        var decimalValue = (decimal)value.Raw / OneRaw;
        return TOther.TryConvertFromSaturating(decimalValue, out result) || TryConvertFromSaturatingFromDecimal<TOther, decimal>(decimalValue, out result);
    }

    public static bool TryConvertToTruncating<TOther>(Fixed128 value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        var decimalValue = (decimal)value.Raw / OneRaw;
        return TOther.TryConvertFromTruncating(decimalValue, out result) || TryConvertFromTruncatingFromDecimal<TOther, decimal>(decimalValue, out result);
    }

    // TryConvertTo_FromDecimal
    // We can't call the relevant methods on decimal directly since these are explicit interface implementations

    private static bool TryConvertFromCheckedFromDecimal<TTo, TFrom>(decimal value, [MaybeNullWhen(false)] out TTo result)
        where TTo : INumberBase<TTo>
        where TFrom : INumberBase<decimal>
    {
        return TFrom.TryConvertToChecked(value, out result);
    }

    private static bool TryConvertFromSaturatingFromDecimal<TTo, TFrom>(decimal value, [MaybeNullWhen(false)] out TTo result)
        where TTo : INumberBase<TTo>
        where TFrom : INumberBase<decimal>
    {
        return TFrom.TryConvertToSaturating(value, out result);
    }

    private static bool TryConvertFromTruncatingFromDecimal<TTo, TFrom>(decimal value, [MaybeNullWhen(false)] out TTo result)
        where TTo : INumberBase<TTo>
        where TFrom : INumberBase<decimal>
    {
        return TFrom.TryConvertToTruncating(value, out result);
    }
}
