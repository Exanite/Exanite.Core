using System;

namespace Exanite.Core.Numerics;

public partial struct Fixed // : IRootFunctions<Fixed>
{
    // public static Fixed Cbrt(Fixed x);
    // public static Fixed RootN(Fixed x, int n);

    public static Fixed Sqrt(Fixed x)
    {
        return (Fixed)Fixed128.Sqrt(x);
    }

    public static Fixed Hypot(Fixed x, Fixed y)
    {
        var x2 = (Int128)x.Raw * x.Raw;
        var y2 = (Int128)y.Raw * y.Raw;
        var sum = x2 + y2;
        return (Fixed)Fixed128.Sqrt(new Fixed128(sum));
    }

    public static Fixed Hypot(Fixed x, Fixed y, Fixed z)
    {
        var x2 = (Int128)x.Raw * x.Raw;
        var y2 = (Int128)y.Raw * y.Raw;
        var z2 = (Int128)z.Raw * z.Raw;
        var sum = x2 + y2 + z2;
        return (Fixed)Fixed128.Sqrt(new Fixed128(sum));
    }

    public static Fixed Hypot(Fixed x, Fixed y, Fixed z, Fixed w)
    {
        var x2 = (Int128)x.Raw * x.Raw;
        var y2 = (Int128)y.Raw * y.Raw;
        var z2 = (Int128)z.Raw * z.Raw;
        var w2 = (Int128)w.Raw * w.Raw;
        var sum = x2 + y2 + z2 + w2;
        return (Fixed)Fixed128.Sqrt(new Fixed128(sum));
    }
}
