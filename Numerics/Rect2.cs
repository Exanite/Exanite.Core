using System.Numerics;

namespace Exanite.Core.Numerics;

public record struct Rect2
{
    public static readonly Rect2 One = new(Vector2.One, Vector2.Zero);

    public Vector2 Size;
    public Vector2 Offset;

    public Rect2(Vector2 size, Vector2 offset = default)
    {
        Size = size;
        Offset = offset;
    }
}
