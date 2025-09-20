using System.Numerics;

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

    public static explicit operator Rect3Int(Rect2 rect)
    {
        return new Rect3Int(new Vector3Int((int)rect.Size.X, (int)rect.Size.Y, 0), new Vector3Int((int)rect.Offset.X, (int)rect.Offset.Y, 0));
    }

    public static implicit operator Rect2(Rect3Int value)
    {
        return new Rect2(new Vector2(value.Size.X, value.Size.Y), new Vector2(value.Offset.X, value.Offset.Y));
    }

    public readonly bool Contains(Vector3 position)
    {
        return position.X >= Offset.X
               && position.Y >= Offset.Y
               && position.Z >= Offset.Z
               && position.X < Offset.X + Size.X
               && position.Y < Offset.Y + Size.Y
               && position.Z < Offset.Z + Size.Z;
    }
}

