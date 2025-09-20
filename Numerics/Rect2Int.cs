namespace Exanite.Core.Numerics;

public record struct Rect2Int
{
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
}

