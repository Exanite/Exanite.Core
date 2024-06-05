namespace Exanite.Core.Types
{
    public static class TypeFilters
    {
        public static ITypeFilter Self { get; } = new SelfTypeFilter();
        public static ITypeFilter Interface { get; } = new InterfaceTypeFilter();
        public static ITypeFilter InheritanceHierarchy { get; } = new InheritanceHierarchyTypeFilter();
        public static ITypeFilter Smart { get; } = new TypeFilter().InheritanceHierarchy().Interfaces();

#if UNITY_2021_3_OR_NEWER
        // Todo public static TypeFilter UnitySmart { get; } = new TypeFilter().InheritanceHierarchy().Interfaces();
#endif
    }
}
