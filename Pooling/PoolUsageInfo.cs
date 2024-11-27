namespace Exanite.Core.Pooling
{
    public struct PoolUsageInfo
    {
        public int MaxInactive;

        public int TotalCount;
        public int ActiveCount;
        public int InactiveCount;

        public int CreateCount;
        public int AcquireCount;
        public int ReleaseCount;
        public int DestroyCount;
    }
}
