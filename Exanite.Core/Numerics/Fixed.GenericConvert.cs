using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Exanite.Core.Numerics;

public partial struct Fixed
{
    // Create
    // These are based on .NET's own implementation: https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/Int32.cs,718
    // The implementation seems to be the same for all types
    // Note how we have to check both directions

    public static Fixed CreateChecked<TOther>(TOther value) where TOther : INumberBase<TOther>
    {
        Fixed result;
        if (typeof(TOther) == typeof(Fixed))
        {
            result = (Fixed)(object)value;
        }
        else if (!TryConvertFromChecked(value, out result) && !TOther.TryConvertToChecked(value, out result))
        {
            ThrowNotSupportedExceptionForCreate(value);
        }

        return result;
    }

    public static Fixed CreateSaturating<TOther>(TOther value) where TOther : INumberBase<TOther>
    {
        Fixed result;
        if (typeof(TOther) == typeof(Fixed))
        {
            result = (Fixed)(object)value;
        }
        else if (!TryConvertFromSaturating(value, out result) && !TOther.TryConvertToSaturating(value, out result))
        {
            ThrowNotSupportedExceptionForCreate(value);
        }

        return result;
    }

    public static Fixed CreateTruncating<TOther>(TOther value) where TOther : INumberBase<TOther>
    {
        Fixed result;
        if (typeof(TOther) == typeof(Fixed))
        {
            result = (Fixed)(object)value;
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

    public static bool TryConvertFromChecked<TOther>(TOther value, out Fixed result) where TOther : INumberBase<TOther>
    {
        if (TOther.IsInteger(value))
        {
            var longValue = long.CreateChecked(value);
            result = new Fixed(checked(longValue * OneRaw));
            return true;
        }

        try
        {
            var doubleValue = double.CreateChecked(value);
            result = new Fixed(checked((long)double.Round(doubleValue * OneRaw, MidpointRounding.ToEven)));
            return true;
        }
        catch (NotSupportedException)
        {
            result = default;
            return false;
        }
    }

    public static bool TryConvertFromSaturating<TOther>(TOther value, out Fixed result) where TOther : INumberBase<TOther>
    {
        if (TOther.IsInteger(value))
        {
            var longValue = long.CreateSaturating(value);
            if (longValue > (long)MaxValue)
            {
                result = MaxValue;
                return true;
            }

            if (longValue < (long)MinValue)
            {
                result = MinValue;
                return true;
            }

            result = new Fixed(longValue * OneRaw);
            return true;
        }

        try
        {
            var doubleValue = double.CreateSaturating(value);
            if (doubleValue > (double)MaxValue)
            {
                result = MaxValue;
                return true;
            }

            if (doubleValue < (double)MinValue)
            {
                result = MinValue;
                return true;
            }

            result = new Fixed((long)double.Round(doubleValue * OneRaw, MidpointRounding.ToEven));
            return true;
        }
        catch (NotSupportedException)
        {
            result = default;
            return false;
        }
    }

    public static bool TryConvertFromTruncating<TOther>(TOther value, out Fixed result) where TOther : INumberBase<TOther>
    {
        // Not going to support truncating conversation
        return TryConvertFromSaturating(value, out result);
    }

    // TryConvertTo
    // Similar to the Create methods, we have to check both directions here

    public static bool TryConvertToChecked<TOther>(Fixed value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        var decimalValue = (double)value.Raw / OneRaw;
        return TOther.TryConvertFromChecked(decimalValue, out result) || TryConvertFromCheckedFromDouble<TOther, double>(decimalValue, out result);
    }

    public static bool TryConvertToSaturating<TOther>(Fixed value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        var decimalValue = (double)value.Raw / OneRaw;
        return TOther.TryConvertFromSaturating(decimalValue, out result) || TryConvertFromSaturatingFromDouble<TOther, double>(decimalValue, out result);
    }

    public static bool TryConvertToTruncating<TOther>(Fixed value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        var decimalValue = (double)value.Raw / OneRaw;
        return TOther.TryConvertFromTruncating(decimalValue, out result) || TryConvertFromTruncatingFromDouble<TOther, double>(decimalValue, out result);
    }

    // TryConvertTo_FromDouble
    // We can't call the relevant methods on decimal directly since these are explicit interface implementations

    private static bool TryConvertFromCheckedFromDouble<TTo, TFrom>(double value, [MaybeNullWhen(false)] out TTo result)
        where TTo : INumberBase<TTo>
        where TFrom : INumberBase<double>
    {
        return TFrom.TryConvertToChecked(value, out result);
    }

    private static bool TryConvertFromSaturatingFromDouble<TTo, TFrom>(double value, [MaybeNullWhen(false)] out TTo result)
        where TTo : INumberBase<TTo>
        where TFrom : INumberBase<double>
    {
        return TFrom.TryConvertToSaturating(value, out result);
    }

    private static bool TryConvertFromTruncatingFromDouble<TTo, TFrom>(double value, [MaybeNullWhen(false)] out TTo result)
        where TTo : INumberBase<TTo>
        where TFrom : INumberBase<double>
    {
        return TFrom.TryConvertToTruncating(value, out result);
    }
}
