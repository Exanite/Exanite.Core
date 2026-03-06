using System.Numerics;

namespace Exanite.Core.Numerics;

public record struct Rect2
{
    public static readonly Rect2 Zero = default;
    public static readonly Rect2 One = FromSize(Vector2.One);

    public Vector2 Offset;
    public Vector2 Size;

    public static Rect2 FromSize(Vector2 size)
    {
        return new Rect2()
        {
            Size = size,
        };
    }

    public static Rect2 FromOffsetSize(Vector2 offset, Vector2 size)
    {
        return new Rect2()
        {
            Offset = offset,
            Size = size,
        };
    }

    public readonly Rect2 Scale(Vector2 scale)
    {
        return FromOffsetSize(scale * Offset, scale * Size);
    }

    public readonly Rect2Int ScaleToInt(Vector2Int scale)
    {
        return Rect2Int.FromOffsetSize((Vector2Int)(scale * Offset), (Vector2Int)(scale * Size));
    }

    public readonly bool Contains(Vector2 position)
    {
        return position.X >= Offset.X
            && position.Y >= Offset.Y
            && position.X < Offset.X + Size.X
            && position.Y < Offset.Y + Size.Y;
    }
}
