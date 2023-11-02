using System;
using System.Runtime.InteropServices;
using System.Threading;
using Exanite.Core.Utilities;

namespace Exanite.Core.Threading
{
    // Modified version of https://github.com/Cysharp/UniTask/blob/5cc97c7f0085624b6ef57853d70b404440060cef/src/UniTask/Assets/Plugins/UniTask/Runtime/UniTaskSynchronizationContext.cs

    /// <summary>
    /// SynchronizationContext that can be manually run.
    /// <para/>
    /// Recommended usage for games is to call <see cref="Run"/> during each update on the main thread.
    /// </summary>
    public class ManualSynchronizationContext : SynchronizationContext
    {
        private const int MaxArrayLength = 0X7FEFFFFF;
        private const int InitialSize = 16;

        private SpinLock gate = new(false);
        private bool isDequing;

        private int actionListCount;
        private Callback[] actionList = new Callback[InitialSize];

        private int waitingListCount;
        private Callback[] waitingList = new Callback[InitialSize];

        private int opCount;

        public override void Send(SendOrPostCallback d, object state)
        {
            d(state);
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            var lockTaken = false;
            try
            {
                gate.Enter(ref lockTaken);

                if (isDequing)
                {
                    // Ensure capacity
                    if (waitingList.Length == waitingListCount)
                    {
                        var newLength = waitingListCount * 2;
                        if ((uint)newLength > MaxArrayLength)
                        {
                            newLength = MaxArrayLength;
                        }

                        var newArray = new Callback[newLength];
                        Array.Copy(waitingList, newArray, waitingListCount);
                        waitingList = newArray;
                    }

                    waitingList[waitingListCount] = new Callback(d, state);
                    waitingListCount++;
                }
                else
                {
                    // Ensure capacity
                    if (actionList.Length == actionListCount)
                    {
                        var newLength = actionListCount * 2;
                        if ((uint)newLength > MaxArrayLength)
                        {
                            newLength = MaxArrayLength;
                        }

                        var newArray = new Callback[newLength];
                        Array.Copy(actionList, newArray, actionListCount);
                        actionList = newArray;
                    }

                    actionList[actionListCount] = new Callback(d, state);
                    actionListCount++;
                }
            }
            finally
            {
                if (lockTaken)
                {
                    gate.Exit(false);
                }
            }
        }

        public override void OperationStarted()
        {
            Interlocked.Increment(ref opCount);
        }

        public override void OperationCompleted()
        {
            Interlocked.Decrement(ref opCount);
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
            {
                var lockTaken = false;
                try
                {
                    gate.Enter(ref lockTaken);
                    if (actionListCount == 0)
                    {
                        return;
                    }

                    isDequing = true;
                }
                finally
                {
                    if (lockTaken)
                    {
                        gate.Exit(false);
                    }
                }
            }

            for (var i = 0; i < actionListCount; i++)
            {
                var action = actionList[i];
                actionList[i] = default;
                action.Invoke();
            }

            {
                var lockTaken = false;
                try
                {
                    gate.Enter(ref lockTaken);
                    isDequing = false;

                    var swapTempActionList = actionList;

                    actionListCount = waitingListCount;
                    actionList = waitingList;

                    waitingListCount = 0;
                    waitingList = swapTempActionList;
                }
                finally
                {
                    if (lockTaken)
                    {
                        gate.Exit(false);
                    }
                }
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
