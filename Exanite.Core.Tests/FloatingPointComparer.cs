using System.Collections.Generic;
using Exanite.Core.Utilities;

namespace Exanite.Core.Tests;

public class FloatingPointComparer : IEqualityComparer<float>, IEqualityComparer<double>, IEqualityComparer<decimal>
{
    public readonly decimal Tolerance;

    private FloatingPointComparer(decimal tolerance)
    {
        this.Tolerance = tolerance;
    }

    public static FloatingPointComparer FromTolerance(decimal tolerance)
    {
        return new FloatingPointComparer(M.Abs(tolerance));
    }

    public static FloatingPointComparer FromPrecision(int precision)
    {
        return new FloatingPointComparer(ToleranceFromPrecision(precision));
    }

    public static decimal ToleranceFromPrecision(int precision)
    {
        return (decimal)double.Pow(0.1, precision);
    }

    // Assumes left is expected and right is actual since this is designed for XUnit
    public bool Equals(float expected, float actual) => M.ApproximatelyEquals(expected, actual, (float)Tolerance);
    public bool Equals(double expected, double actual) => M.ApproximatelyEquals(expected, actual, (double)Tolerance);
    public bool Equals(decimal expected, decimal actual) => M.ApproximatelyEquals(expected, actual, Tolerance);

    // Unused
    public int GetHashCode(float value) => 0;
    public int GetHashCode(double value) => 0;
    public int GetHashCode(decimal value) => 0;
}
