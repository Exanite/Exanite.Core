using System.Numerics;

namespace Exanite.Core.Numerics;

public record struct Rect3
{
    public static readonly Rect3 One = new(Vector3.One, Vector3.Zero);

    public Vector3 Size;
    public Vector3 Offset;

    public Rect3(Vector3 size, Vector3 offset = default)
    {
        Size = size;
        Offset = offset;
    }
}
