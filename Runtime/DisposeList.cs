using System;
using System.Collections.Generic;

namespace Exanite.Core.Runtime
{
    public class DisposeList : IDisposable
    {
        // Either IDisposable or Action
        // This is to avoid allocations while keeping things simple
        private readonly Stack<object> queue = new();

        public T Add<T>(T value) where T : IDisposable
        {
            queue.Push(value);

            return value;
        }

        public void Dispose()
        {
            while (queue.TryPop(out var value))
            {
                if (value is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                if (value is Action action)
                {
                    action.Invoke();
                }
            }
        }
    }
}
