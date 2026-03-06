using System.Numerics;

namespace Exanite.Core.Numerics;

public record struct Rect2Int
{
    public static readonly Rect2Int Zero = default;
    public static readonly Rect2Int One = FromSize(Vector2Int.One);

    public Vector2Int Offset;
    public Vector2Int Size;

    public static Rect2Int FromSize(Vector2Int size)
    {
        return new Rect2Int()
        {
            Size = size,
        };
    }

    public static Rect2Int FromOffsetSize(Vector2Int offset, Vector2Int size)
    {
        return new Rect2Int()
        {
            Offset = offset,
            Size = size,
        };
    }

    public static explicit operator Rect2Int(Rect2 rect)
    {
        return FromOffsetSize((Vector2Int)rect.Offset, (Vector2Int)rect.Size);
    }

    public static implicit operator Rect2(Rect2Int value)
    {
        return Rect2.FromOffsetSize(value.Offset, value.Size);
    }

    public readonly bool Contains(Vector2 position)
    {
        return position.X >= Offset.X
            && position.Y >= Offset.Y
            && position.X < Offset.X + Size.X
            && position.Y < Offset.Y + Size.Y;
    }
}
