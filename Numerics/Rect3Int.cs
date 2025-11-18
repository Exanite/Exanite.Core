using System.Numerics;

namespace Exanite.Core.Numerics;

public record struct Rect3Int
{
    public static readonly Rect3Int Zero = default;
    public static readonly Rect3Int One = new(Vector3Int.One);

    public Vector3Int Size;
    public Vector3Int Offset;

    public Rect3Int(Vector3Int size, Vector3Int offset = default)
    {
        Size = size;
        Offset = offset;
    }

    public static explicit operator Rect3Int(Rect3 rect)
    {
        return new Rect3Int(new Vector3Int((int)rect.Size.X, (int)rect.Size.Y, (int)rect.Size.Z), new Vector3Int((int)rect.Offset.X, (int)rect.Offset.Y, (int)rect.Offset.Z));
    }

    public static implicit operator Rect3(Rect3Int value)
    {
        return new Rect3(new Vector3(value.Size.X, value.Size.Y, value.Size.Z), new Vector3(value.Offset.X, value.Offset.Y, value.Offset.Z));
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
