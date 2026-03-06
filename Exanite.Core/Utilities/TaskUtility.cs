using System;
using System.Threading.Tasks;

namespace Exanite.Core.Utilities;

public static class TaskUtility
{
    public static Action<Exception> DefaultExceptionHandlerAction { get; set; } = DefaultExceptionHandler;

    public static void Forget(this Task task)
    {
        task.Forget(DefaultExceptionHandlerAction);
    }

    public static void Forget<T>(this Task<T> task)
    {
        task.Forget(DefaultExceptionHandlerAction);
    }

    public static void Forget(this Task task, Action<Exception> exceptionHandler)
    {
        var awaiter = task.GetAwaiter();
        if (awaiter.IsCompleted)
        {
            try
            {
                awaiter.GetResult();
            }
            catch (Exception e)
            {
                exceptionHandler.Invoke(e);
            }
        }
        else
        {
            var capturedAwaiter = awaiter;
            var capturedExceptionHandler = exceptionHandler;
            capturedAwaiter.OnCompleted(() =>
            {
                try
                {
                    capturedAwaiter.GetResult();
                }
                catch (Exception e)
                {
                    capturedExceptionHandler.Invoke(e);
                }
            });
        }
    }

    public static void Forget<T>(this Task<T> task, Action<Exception> exceptionHandler)
    {
        var awaiter = task.GetAwaiter();
        if (awaiter.IsCompleted)
        {
            try
            {
                awaiter.GetResult();
            }
            catch (Exception e)
            {
                exceptionHandler.Invoke(e);
            }
        }
        else
        {
            var capturedAwaiter = awaiter;
            var capturedExceptionHandler = exceptionHandler;
            capturedAwaiter.OnCompleted(() =>
            {
                try
                {
                    capturedAwaiter.GetResult();
                }
                catch (Exception e)
                {
                    capturedExceptionHandler.Invoke(e);
                }
            });
        }
    }

    private static void DefaultExceptionHandler(Exception e)
    {
        Console.WriteLine(e);
    }
}
