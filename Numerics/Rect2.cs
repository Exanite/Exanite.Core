using System.Numerics;

namespace Exanite.Core.Numerics;

public record struct Rect2
{
    public static readonly Rect2 Zero = default;
    public static readonly Rect2 One = new(Vector2.One);

    public Vector2 Size;
    public Vector2 Offset;

    public Rect2(Vector2 size, Vector2 offset = default)
    {
        Size = size;
        Offset = offset;
    }

    public readonly Rect2 Scale(Vector2 size)
    {
        return new Rect2(
            new Vector2(size.X * Size.X, size.Y * Size.Y),
            new Vector2(size.X * Offset.X, size.Y * Offset.Y));
    }

    public readonly Rect2Int ScaleToInt(Vector2Int size)
    {
        return new Rect2Int(
            new Vector2Int((int)(size.X * Size.X), (int)(size.Y * Size.Y)),
            new Vector2Int((int)(size.X * Offset.X), (int)(size.Y * Offset.Y)));
    }

    public readonly bool Contains(Vector2 position)
    {
        return position.X >= Offset.X
               && position.Y >= Offset.Y
               && position.X < Offset.X + Size.X
               && position.Y < Offset.Y + Size.Y;
    }
}
