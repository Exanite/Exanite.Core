using System;
using System.Collections.Generic;
using Exanite.Core.Utilities;

namespace Exanite.Core.Runtime
{
    /// <summary>
    /// Makes it easier to mark objects to be disposed of later. Objects will be disposed in first-in first-out order (stack order).
    /// </summary>
    public class DisposableCollection : IDisposable
    {
        // Either IDisposable or Action
        // This is to avoid allocations while keeping things simple
        private readonly Stack<object> stack = new();

        internal T Add<T>(T disposable) where T : IDisposable
        {
            stack.Push(disposable);

            return disposable;
        }

        internal Action Add(Action action)
        {
            stack.Push(action);

            return action;
        }

        public void Dispose()
        {
            while (stack.TryPop(out var value))
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

    public static class DisposableCollectionUtility
    {
        /// <summary>
        /// Adds a disposable object for disposal.
        /// </summary>
        public static T AddTo<T>(this T disposable, DisposableCollection collection) where T : IDisposable
        {
            collection.Add(disposable);

            return disposable;
        }

        /// <summary>
        /// Adds an action to be invoked.
        /// </summary>
        public static Action AddTo(this Action action, DisposableCollection collection)
        {
            collection.Add(action);

            return action;
        }
    }
}
