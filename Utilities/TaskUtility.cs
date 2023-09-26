using System;
using System.Threading.Tasks;

namespace Exanite.Core.Utilities
{
    public static class TaskUtility
    {
        public static async void Forget(this Task task)
        {
            try
            {
                await task;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }
    }
}
