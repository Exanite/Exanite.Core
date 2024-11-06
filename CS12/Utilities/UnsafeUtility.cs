#if NETCOREAPP
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Exanite.Core.Utilities
{
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
    }
}
#endif
