using System.Numerics;

namespace Exanite.Core.Numerics;

public record struct Rect3
{
    public static readonly Rect3 One = new(Vector3.One, Vector3.Zero);

    public Vector3 Size;
    public Vector3 Offset;

    public Rect3(Vector3 size, Vector3 offset = default)
    {
        Size = size;
        Offset = offset;
    }

    public readonly Rect3 Scale(Vector3 size)
    {
        return new Rect3(
            new Vector3(size.X * Size.X, size.Y * Size.Y, size.Z * Size.Z),
            new Vector3(size.X * Offset.X, size.Y * Offset.Y, size.Z * Offset.Z));
    }

    public readonly Rect3Int ScaleToInt(Vector3Int size)
    {
        return new Rect3Int(
            new Vector3Int((int)(size.X * Size.X), (int)(size.Y * Size.Y), (int)(size.Z * Size.Z)),
            new Vector3Int((int)(size.X * Offset.X), (int)(size.Y * Offset.Y), (int)(size.Z * Offset.Z)));
    }
}
