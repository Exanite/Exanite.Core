namespace Exanite.Core.Types;

public static class TypeExpanders
{
    /// <summary>
    /// Type expander that returns the input type.
    /// </summary>
    public static ITypeExpander Self { get; } = new SelfTypeExpander();

    /// <summary>
    /// Type expander that returns all interfaces implemented by the input type.
    /// </summary>
    public static ITypeExpander Interface { get; } = new InterfaceTypeExpander();

    /// <summary>
    /// Type expander that returns the class inheritance hierarchy of the input type.
    /// The input type is included as part of the results.
    /// </summary>
    /// <remarks>
    /// Interfaces are not returned.
    /// </remarks>
    public static ITypeExpander InheritanceHierarchy { get; } = new InheritanceHierarchyTypeExpander();

    /// <summary>
    /// Type expander that returns the class inheritance hierarchy of the input type and the interfaces implemented by the input type.
    /// The input type is included as part of the results.
    /// </summary>
    public static ITypeExpander Surface { get; } = new TypeExpander([new InterfaceTypeExpander(), new InheritanceHierarchyTypeExpander()]);
}
