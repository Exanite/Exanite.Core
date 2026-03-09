using System.Collections.Generic;
using Exanite.Core.Utilities;

namespace Exanite.Core.Tests;

public class FloatingPointComparer : IEqualityComparer<float>, IEqualityComparer<double>, IEqualityComparer<decimal>
{
    private readonly decimal tolerance;

    private FloatingPointComparer(decimal tolerance)
    {
        this.tolerance = tolerance;
    }

    public static FloatingPointComparer FromTolerance(decimal tolerance)
    {
        return new FloatingPointComparer(tolerance);
    }

    public static FloatingPointComparer FromPrecision(int precision)
    {
        return new FloatingPointComparer((decimal)double.Pow(0.1, precision));
    }

    public bool Equals(float left, float right) => M.ApproximatelyEquals(left, right, (float)tolerance);
    public bool Equals(double left, double right) => M.ApproximatelyEquals(left, right, (double)tolerance);
    public bool Equals(decimal left, decimal right) => M.ApproximatelyEquals(left, right, tolerance);

    public int GetHashCode(float value) => 0;
    public int GetHashCode(double value) => 0;
    public int GetHashCode(decimal value) => 0;
}
