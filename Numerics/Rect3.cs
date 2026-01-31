using System.Numerics;

namespace Exanite.Core.Numerics;

public record struct Rect3
{
    public static readonly Rect3 Zero = default;
    public static readonly Rect3 One = FromSize(Vector3.One);

    public Vector3 Offset;
    public Vector3 Size;

    public static Rect3 FromSize(Vector3 size)
    {
        return new Rect3()
        {
            Size = size,
        };
    }

    public static Rect3 FromOffsetSize(Vector3 offset, Vector3 size)
    {
        return new Rect3()
        {
            Offset = offset,
            Size = size,
        };
    }

    public readonly Rect3 Scale(Vector3 scale)
    {
        return FromOffsetSize(scale * Offset, scale * Size);
    }

    public readonly Rect3Int ScaleToInt(Vector3Int scale)
    {
        return Rect3Int.FromOffsetSize((Vector3Int)(scale * Offset), (Vector3Int)(scale * Size));
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
