using System.Numerics;

namespace Exanite.Core.Numerics;

public record struct Rect2
{
    public Vector2 Size;
    public Vector2 Offset;

    public Rect2(Vector2 size, Vector2 offset = default)
    {
        Size = size;
        Offset = offset;
    }
}