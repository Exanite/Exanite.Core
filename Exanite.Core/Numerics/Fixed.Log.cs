namespace Exanite.Core.Numerics;

public partial struct Fixed
{
    public static Fixed Log2(Fixed x)
    {
        return (Fixed)Fixed128.Log2Fast(x);
    }

    public static Fixed Log(Fixed x) => Log2(x) * new Fixed(LogETwoRaw);
    public static Fixed Log10(Fixed x) => Log2(x) * new Fixed(Log10TwoRaw);

    // This is very inaccurate
    // Might revisit later
    public static Fixed Log(Fixed x, Fixed newBase) => Log2(x) / Log2(newBase);
}
