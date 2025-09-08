namespace Exanite.Core.Numerics;

public record struct Rect3Int
{
    public Vector3Int Size;
    public Vector3Int Offset;

    public Rect3Int(Vector3Int size, Vector3Int offset = default)
    {
        Size = size;
        Offset = offset;
    }
}

