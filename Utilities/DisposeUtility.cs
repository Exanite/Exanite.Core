#nullable enable

using System;
using System.Threading.Tasks;

namespace Exanite.Core.Utilities
{
    public static class DisposeUtility
    {
        public static async ValueTask CastAndDispose(this IAsyncDisposable? resource)
        {
            if (resource != null)
            {
                await resource.DisposeAsync();
            }
        }

        public static async ValueTask CastAndDispose(this IDisposable? resource)
        {
            if (resource is IAsyncDisposable resourceAsyncDisposable)
            {
                await resourceAsyncDisposable.DisposeAsync();
            }
            else if (resource != null)
            {
                resource.Dispose();
            }
        }
    }
}
