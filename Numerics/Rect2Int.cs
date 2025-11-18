using System.Numerics;

namespace Exanite.Core.Numerics;

public record struct Rect2Int
{
    public static readonly Rect2Int Zero = default;
    public static readonly Rect2Int One = new(Vector2Int.One);

    public Vector2Int Size;
    public Vector2Int Offset;

    public Rect2Int(Vector2Int size, Vector2Int offset = default)
    {
        Size = size;
        Offset = offset;
    }

    public static explicit operator Rect2Int(Rect2 rect)
    {
        return new Rect2Int((Vector2Int)rect.Size, (Vector2Int)rect.Offset);
    }

    public static implicit operator Rect2(Rect2Int value)
    {
        return new Rect2(value.Size, value.Offset);
    }

    public readonly bool Contains(Vector2 position)
    {
        return position.X >= Offset.X
            && position.Y >= Offset.Y
            && position.X < Offset.X + Size.X
            && position.Y < Offset.Y + Size.Y;
    }
}
