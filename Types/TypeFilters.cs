namespace Exanite.Core.Types;

public static partial class TypeFilters
{
    public static ITypeFilter Self { get; } = new SelfTypeFilter();
    public static ITypeFilter Interface { get; } = new InterfaceTypeFilter();
    public static ITypeFilter InheritanceHierarchy { get; } = new InheritanceHierarchyTypeFilter();
    public static ITypeFilter Smart { get; } = new TypeFilter().InheritanceHierarchy().Interfaces();
}