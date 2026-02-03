using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Exanite.Core.Utilities;
using Xunit;

namespace Exanite.Core.Tests.Utilities;

public class UnsafeUtilityTests
{
    [Fact]
    public unsafe void AddressOfUtf8Literal()
    {
        var safe = "Hello"u8;

        var unsafeAddress = UnsafeUtility.AddressOf(safe);
        var unsafeData = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(unsafeAddress);

        Assert.NotEqual(0ul, (ulong)unsafeAddress);
        Assert.Equal(safe.Length, unsafeData.Length);
        Assert.Equal("Hello", Encoding.UTF8.GetString(unsafeData));
    }

    [Fact]
    public void AlignmentOf()
    {
        Assert.Equal(1, UnsafeUtility.AlignmentOf<byte>());
        Assert.Equal(4, UnsafeUtility.AlignmentOf<int>());
        Assert.Equal(8, UnsafeUtility.AlignmentOf<nint>());
        Assert.Equal(4, UnsafeUtility.AlignmentOf<Vector3>());
        Assert.Equal(4, UnsafeUtility.AlignmentOf<Matrix4x4>());
    }
}
