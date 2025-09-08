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
}

