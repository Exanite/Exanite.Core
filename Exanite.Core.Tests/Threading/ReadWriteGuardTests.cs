using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Exanite.Core.Runtime;
using Exanite.Core.Threading;
using Xunit;

namespace Exanite.Core.Tests.Threading;

public class ReadWriteGuardTests
{
    [Fact]
    public void ReadWhileWrite_Throws()
    {
        var guard = new ReadWriteGuard();

        // Test read-then-write
        Assert.Throws<GuardException>(() =>
        {
            using (guard.EnterReadGuard())
            {
                using (guard.EnterWriteGuard()) {}
            }
        });

        // Ensure still usable
        using (guard.EnterReadGuard()) {}
        using (guard.EnterWriteGuard()) {}

        // Test write-then-read
        Assert.Throws<GuardException>(() =>
        {
            using (guard.EnterWriteGuard())
            {
                using (guard.EnterReadGuard()) {}
            }
        });
    }

    [Fact]
    public void DoubleWrite_Throws()
    {
        var guard = new ReadWriteGuard();

        // Test write-then-write
        Assert.Throws<GuardException>(() =>
        {
            using (guard.EnterWriteGuard())
            {
                using (guard.EnterWriteGuard()) {}
            }
        });

        // Ensure still usable
        using (guard.EnterReadGuard()) {}
        using (guard.EnterWriteGuard()) {}
    }

    [Fact]
    public void CanReadRecursively()
    {
        var guard = new ReadWriteGuard();

        // Test recursive read
        using (guard.EnterReadGuard())
        {
            using (guard.EnterReadGuard())
            {
                using (guard.EnterReadGuard()) {}
            }
        }

        // Ensure still usable
        using (guard.EnterReadGuard()) {}
        using (guard.EnterWriteGuard()) {}
    }

    [Fact]
    public async Task ReadGuard_HandlesThreadContention()
    {
        var guard = new ReadWriteGuard();
        var tasks = new List<Task>();

        // Use a trigger to ensure all guards enter at approximately the same time
        var trigger = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        for (var i = 0; i < 10000; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                await trigger.Task;
                using (guard.EnterReadGuard()) {}
            }, TestContext.Current.CancellationToken));
        }

        trigger.SetResult();
        await Task.WhenAll(tasks);

        // Ensure still usable
        using (guard.EnterReadGuard()) {}
        using (guard.EnterWriteGuard()) {}
    }
}
