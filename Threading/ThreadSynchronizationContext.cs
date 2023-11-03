using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Threading;
using Exanite.Core.Utilities;

namespace Exanite.Core.Threading
{
    /// <summary>
    /// Ensures posted callbacks are always ran on a specific thread.
    /// If the callback is posted on the configured thread, it will be ran immediately.
    /// Otherwise, the callback will be ran the next time Run() is called.
    /// <para/>
    /// Recommended usage for games is to call <see cref="Run"/> during each update on the main thread.
    /// </summary>
    public class ThreadSynchronizationContext : SynchronizationContext
    {
        private const int MaxRecursivePostCount = 50;

        private int recursivePostCount = 0;

        private readonly ConcurrentQueue<Callback> callbacks = new();

        /// <param name="targetThread">The thread callbacks should be executed on.</param>
        public ThreadSynchronizationContext(Thread targetThread)
        {
            TargetThread = targetThread;
        }

        /// <summary>
        /// The thread callbacks should be executed on.
        /// </summary>
        public Thread TargetThread { get; set; }

        public override void Send(SendOrPostCallback callback, object state)
        {
            Post(callback, state);
        }

        public override void Post(SendOrPostCallback callback, object state)
        {
            if (Thread.CurrentThread == TargetThread)
            {
                // Based on https://github.com/kekyo/SynchContextSample/blob/master/SynchContextSample/QueueSynchronizationContext.cs
                if (recursivePostCount < MaxRecursivePostCount)
                {
                    recursivePostCount++;
                    callback(state);
                    recursivePostCount--;

                    return;
                }
            }

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
