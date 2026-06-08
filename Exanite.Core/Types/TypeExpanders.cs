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
    public static ITypeExpander Interfaces { get; } = new InterfacesTypeExpander();

    /// <summary>
    /// Type expander that returns the class inheritance hierarchy of the input type.
    /// The input type is included as part of the results.
    /// </summary>
    /// <remarks>
    /// Interfaces are not returned.
    /// </remarks>
    public static ITypeExpander Hierarchy { get; } = new HierarchyTypeExpander();

    /// <summary>
    /// Type expander that returns the union of the <see cref="Hierarchy"/> and <see cref="Interfaces"/> type expanders.
    /// The input type is included as part of the results.
    /// </summary>
    public static ITypeExpander HierarchyAndInterfaces { get; } = new TypeExpander([new InterfacesTypeExpander(), new HierarchyTypeExpander()]);
}
