using System;
using System.Collections.Generic;

namespace Exanite.Core.Runtime
{
    /// <summary>
    /// Makes it easier to mark objects to be disposed of later. Objects will be disposed in first-in first-out order (stack order).
    /// </summary>
    public class DisposableCollection : IDisposable
    {
        // Either IDisposable or Action
        // This is to avoid allocations while keeping things simple
        private readonly Stack<object> queue = new();

        /// <summary>
        /// Adds a disposable object for disposal.
        /// </summary>
        public T Add<T>(T disposable) where T : IDisposable
        {
            queue.Push(disposable);

            return disposable;
        }

        /// <summary>
        /// Adds an action to be invoked.
        /// </summary>
        public void Add(Action action)
        {
            queue.Push(action);
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
