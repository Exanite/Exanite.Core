using System;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

public partial struct Fixed
{
    public static Fixed Sqrt(Fixed x)
    {
        return (Fixed)Fixed128.SqrtFast(x);
    }

    public static Fixed Cbrt(Fixed x)
    {
        return (Fixed)Fixed128.CbrtFast(x);
    }

    public static Fixed RootN(Fixed x, int n)
    {
        if (x < 0 && ((n & 1) == 0))
        {
            GuardUtility.Throw("Cannot take an even root of a negative number");
        }

        switch (n)
        {
            case -3:
            {
                return Reciprocal(Cbrt(x));
            }
            case -2:
            {
                return Reciprocal(Sqrt(x));
            }
            case -1:
            {
                return Reciprocal(x);
            }
            case 0:
            {
                GuardUtility.Throw("Cannot take the 0th root of a number");
                return 0;
            }
            case 1:
            {
                return x;
            }
            case 2:
            {
                return Sqrt(x);
            }
            case 3:
            {
                return Cbrt(x);
            }
        }

        // Use identity:
        // root_n(x) = 2^(log2(x) / n)
        var result = Exp2(Log2(Abs(x)) / n);
        var isNegative = IsNegative(x);
        return isNegative ? -result : result;
    }

    public static Fixed Hypot(Fixed x, Fixed y)
    {
        checked
        {
            var x2 = (Int128)x.Raw * x.Raw;
            var y2 = (Int128)y.Raw * y.Raw;
            var sum = x2 + y2;
            return (Fixed)Fixed128.SqrtFast(new Fixed128(sum));
        }
    }

    public static Fixed Hypot(Fixed x, Fixed y, Fixed z)
    {
        checked
        {
            var x2 = (Int128)x.Raw * x.Raw;
            var y2 = (Int128)y.Raw * y.Raw;
            var z2 = (Int128)z.Raw * z.Raw;
            var sum = x2 + y2 + z2;
            return (Fixed)Fixed128.SqrtFast(new Fixed128(sum));
        }
    }

    public static Fixed Hypot(Fixed x, Fixed y, Fixed z, Fixed w)
    {
        checked
        {
            var x2 = (Int128)x.Raw * x.Raw;
            var y2 = (Int128)y.Raw * y.Raw;
            var z2 = (Int128)z.Raw * z.Raw;
            var w2 = (Int128)w.Raw * w.Raw;
            var sum = x2 + y2 + z2 + w2;
            return (Fixed)Fixed128.SqrtFast(new Fixed128(sum));
        }
    }
}
