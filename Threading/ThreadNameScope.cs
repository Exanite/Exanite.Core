using System;
using System.Threading;

namespace Exanite.Core.Threading
{
    public struct ThreadNameScope : IDisposable
    {
        private readonly string? originalName;

        public ThreadNameScope(string name)
        {
            originalName = Thread.CurrentThread.Name;
            Thread.CurrentThread.Name = name;
        }

        public void Dispose()
        {
            Thread.CurrentThread.Name = originalName;
        }
    }
}
