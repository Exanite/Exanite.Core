using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Exanite.Core.Numerics;

public partial struct Fixed
{
    // Create

    public static Fixed CreateChecked<TOther>(TOther value) where TOther : INumberBase<TOther>
    {
        // Based on .NET's own implementation: https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/Int32.cs,718
        Fixed result;
        if (typeof(TOther) == typeof(Fixed))
        {
            result = (Fixed)(object)value;
        }
        else if (!TryConvertFromChecked(value, out result) && !TOther.TryConvertToChecked(value, out result))
        {
            throw new NotSupportedException($"Failed to create a fixed point value from the provided value: {value}");
        }

        return result;
    }

    public static Fixed CreateSaturating<TOther>(TOther value) where TOther : INumberBase<TOther>
    {
        // Based on .NET's own implementation: https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/Int32.cs,718
        Fixed result;
        if (typeof(TOther) == typeof(Fixed))
        {
            result = (Fixed)(object)value;
        }
        else if (!TryConvertFromSaturating(value, out result) && !TOther.TryConvertToSaturating(value, out result))
        {
            throw new NotSupportedException($"Failed to create a fixed point value from the provided value: {value}");
        }

        return result;
    }

    public static Fixed CreateTruncating<TOther>(TOther value) where TOther : INumberBase<TOther>
    {
        // Based on .NET's own implementation: https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/Int32.cs,718
        Fixed result;
        if (typeof(TOther) == typeof(Fixed))
        {
            result = (Fixed)(object)value;
        }
        else if (!TryConvertFromTruncating(value, out result) && !TOther.TryConvertToTruncating(value, out result))
        {
            throw new NotSupportedException($"Failed to create a fixed point value from the provided value: {value}");
        }

        return result;
    }

    // TryConvertFrom

    public static bool TryConvertFromChecked<TOther>(TOther value, out Fixed result) where TOther : INumberBase<TOther>
    {
        if (TOther.IsInteger(value))
        {
            var longValue = long.CreateChecked(value);
            result = new Fixed(checked(longValue * OneValue));
            return true;
        }

        try
        {
            var decimalValue = decimal.CreateChecked(value);
            result = new Fixed(checked((long)(decimalValue * OneValue)));
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
            if (longValue > MaxValue)
            {
                result = MaxValue;
                return true;
            }

            if (longValue < MinValue)
            {
                result = MinValue;
                return true;
            }

            result = new Fixed(longValue * OneValue);
            return true;
        }

        try
        {
            var decimalValue = decimal.CreateSaturating(value);
            if (decimalValue > MaxValue)
            {
                result = MaxValue;
                return true;
            }

            if (decimalValue < MinValue)
            {
                result = MinValue;
                return true;
            }

            result = new Fixed((long)(decimalValue * OneValue));
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
        // TODO
        if (TOther.IsInteger(value))
        {
            var longValue = long.CreateTruncating(value);
            result = new Fixed(longValue * OneValue);
            return true;
        }

        try
        {
            var decimalValue = decimal.CreateTruncating(value);
            result = new Fixed((long)(decimalValue * OneValue));
            return true;
        }
        catch (NotSupportedException)
        {
            result = default;
            return false;
        }
    }

    public static bool TryConvertToChecked<TOther>(Fixed value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        var decimalValue = (decimal)value.value / OneValue;
        return TOther.TryConvertFromChecked(decimalValue, out result);
    }
    public static bool TryConvertToSaturating<TOther>(Fixed value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> => TryConvertToChecked(value, out result);
    public static bool TryConvertToTruncating<TOther>(Fixed value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther> => TryConvertToChecked(value, out result);
}
