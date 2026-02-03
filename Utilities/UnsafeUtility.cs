using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Exanite.Core.Utilities;

public static class UnsafeUtility
{
    /// <summary>
    /// Gets a pointer to a UTF-8 string literal.
    /// See: https://github.com/dotnet/runtime/discussions/102949
    /// </summary>
    public static unsafe byte* AddressOf(ReadOnlySpan<byte> utf8StringLiteral)
    {
        return (byte*)Unsafe.AsPointer(ref Unsafe.AsRef(in MemoryMarshal.AsRef<byte>(utf8StringLiteral)));
    }

    /// <summary>
    /// Gets the alignment of an unmanaged type.
    /// See: https://stackoverflow.com/questions/77212211/how-do-i-find-the-alignment-of-a-struct-in-c
    /// </summary>
    [SkipLocalsInit]
    public static unsafe int AlignmentOf<T>() where T : unmanaged
    {
        Unsafe.SkipInit(out AlignmentHelper<T> helper);
        return (int)((nuint)Unsafe.AsPointer(ref helper.Target) - (nuint)Unsafe.AsPointer(ref helper));
    }

    internal struct AlignmentHelper<T> where T : unmanaged
    {
#pragma warning disable CS0649
        public byte Padding;
        public T Target;
#pragma warning restore CS0649
    }
}
