using Exanite.Core.Numerics;
using Exanite.Core.Utilities;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class Fixed128BasicTests
{
    [Fact]
    public void Reciprocal_IsMorePreciseThanStandardDivision_ForGreaterThanOne()
    {
        var current = 1.0M;
        var multiplier = 1.025M;
        var lessPreciseCount = 0;
        var morePreciseCount = 0;
        for (var i = 0; i < 1000; i++)
        {
            current *= multiplier;

            var doubleResult = 1 / current;
            var naiveResult = 1 / (Fixed128)current;
            var specializedResult = Fixed128.Reciprocal((Fixed128)current);

            var naiveDifference = M.Abs(doubleResult - (decimal)naiveResult);
            var specializedDifference = M.Abs(doubleResult - (decimal)specializedResult);

            if (specializedDifference < naiveDifference)
            {
                morePreciseCount++;
            }

            if (specializedDifference > naiveDifference)
            {
                lessPreciseCount++;
            }
        }

        Assert.True(morePreciseCount > lessPreciseCount);
    }

    [Fact]
    public void Reciprocal_IsMorePreciseThanStandardDivision_ForLessThanOne()
    {
        var current = 1.0M;
        var multiplier = 0.995M;
        var lessPreciseCount = 0;
        var morePreciseCount = 0;
        for (var i = 0; i < 1000; i++)
        {
            current *= multiplier;

            var doubleResult = 1 / current;
            var naiveResult = 1 / (Fixed128)current;
            var specializedResult = Fixed128.Reciprocal((Fixed128)current);

            var naiveDifference = M.Abs(doubleResult - (decimal)naiveResult);
            var specializedDifference = M.Abs(doubleResult - (decimal)specializedResult);

            if (specializedDifference < naiveDifference)
            {
                morePreciseCount++;
            }

            if (specializedDifference > naiveDifference)
            {
                lessPreciseCount++;
            }
        }

        Assert.True(morePreciseCount > lessPreciseCount);
    }
}
