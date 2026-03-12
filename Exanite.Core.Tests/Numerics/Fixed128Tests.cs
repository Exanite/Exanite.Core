using Exanite.Core.Numerics;
using Xunit;

namespace Exanite.Core.Tests.Numerics;

public class Fixed128Tests
{
    [Fact]
    public void Reciprocal_DoesNotOverflow_ForEpsilon()
    {
        var result = Fixed128.Reciprocal(Fixed128.Epsilon);
        Assert.True(result > Fixed128.Zero);
    }
}
