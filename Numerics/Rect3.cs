using System.Numerics;

namespace Exanite.Core.Numerics;

public record struct Rect3
{
    public Vector3 Size;
    public Vector3 Offset;

    public Rect3(Vector3 size, Vector3 offset = default)
    {
        Size = size;
        Offset = offset;
    }
}