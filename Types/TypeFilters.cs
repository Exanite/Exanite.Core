namespace Exanite.Core.Types
{
    public static class TypeFilters
    {
        public static TypeFilter Smart { get; } = new TypeFilter().InheritanceHierarchy().Interfaces();

#if UNITY_5_3_OR_NEWER
        // Todo public static TypeFilter UnitySmart { get; } = new TypeFilter().InheritanceHierarchy().Interfaces();
#endif
    }
}
