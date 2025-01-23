namespace Exanite.Core.Pooling
{
    public struct PoolUsageInfo
    {
        public int MaxInactive;

        public bool AllowResizing;
        public int MaxInactiveResizeCount;

        public int TotalCount;
        public int ActiveCount;
        public int InactiveCount;

        public ulong CreateCount;
        public ulong AcquireCount;
        public ulong ReleaseCount;
        public ulong DestroyCount;
    }
}
