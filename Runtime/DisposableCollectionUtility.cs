using System;

namespace Exanite.Core.Runtime
{
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
