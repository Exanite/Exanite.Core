using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Threading;
using Exanite.Core.Utilities;

namespace Exanite.Core.Threading
{
    /// <summary>
    /// Ensures posted callbacks are always ran on a specific thread. Callbacks will be ran when <see cref="Run"/> is called.
    /// </summary>
    /// <remarks>
    /// Recommended usage for games is to call <see cref="Run"/> during each update on the main thread.
    /// </remarks>
    public class ThreadSynchronizationContext : SynchronizationContext
    {
        private readonly ConcurrentQueue<Callback> callbacks = new();

        /// <summary>
        /// The thread callbacks should be executed on.
        /// </summary>
        public Thread TargetThread { get; set; }

        /// <param name="targetThread">The thread callbacks should be executed on.</param>
        public ThreadSynchronizationContext(Thread targetThread)
        {
            TargetThread = targetThread;
        }

        public override void Send(SendOrPostCallback callback, object state)
        {
            Post(callback, state);
        }

        public override void Post(SendOrPostCallback callback, object state)
        {
            callbacks.Enqueue(new Callback(callback, state));
        }

        public override SynchronizationContext CreateCopy()
        {
            // See https://stackoverflow.com/questions/21062440/the-purpose-of-synchronizationcontext-createcopy

            return this;
        }

        /// <summary>
        /// Runs the stored callbacks on the current thread.
        /// </summary>
        public void Run()
        {
            if (Thread.CurrentThread != TargetThread)
            {
                throw new InvalidOperationException($"{nameof(Run)} must be ran on the target thread.");
            }

            while (callbacks.TryDequeue(out var callback))
            {
                callback.Invoke();
            }
        }

        [StructLayout(LayoutKind.Auto)]
        private readonly struct Callback
        {
            private readonly SendOrPostCallback callback;
            private readonly object state;

            public Callback(SendOrPostCallback callback, object state)
            {
                this.callback = callback;
                this.state = state;
            }

            public void Invoke()
            {
                try
                {
                    callback(state);
                }
                catch (Exception e)
                {
                    DebugUtility.Log(e);
                }
            }
        }
    }
}
