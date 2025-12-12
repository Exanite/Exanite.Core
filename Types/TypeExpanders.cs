namespace Exanite.Core.Types;

public static class TypeExpanders
{
    public static ITypeExpander Self { get; } = new SelfTypeExpander();
    public static ITypeExpander Interface { get; } = new InterfaceTypeExpander();
    public static ITypeExpander InheritanceHierarchy { get; } = new InheritanceHierarchyTypeExpander();
    public static ITypeExpander Surface { get; } = new TypeExpander([new InterfaceTypeExpander(), new InheritanceHierarchyTypeExpander()]);
}
