using System.Numerics;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

public struct Ray
{
    /// <summary>
    /// Values higher than this will be clamped down.
    /// This is to maintain acceptable floating point accuracy.
    /// </summary>
    public const float MaxRayLength = 1_000_000;

    public Vector3 Origin { get; set; }
    public Vector3 Direction { get; set; }

    private float length;
    public float Length
    {
        readonly get => length;
        set => length = M.Min(value, MaxRayLength);
    }

    public Vector3 DirectionMagnitude => Direction * Length;
    public Vector3 End => Origin + Direction * Length;

    public Ray(Vector3 origin, Vector3 direction, float length = MaxRayLength)
    {
        AssertUtility.IsTrue(length >= 0, $"{nameof(length)} must be greater than or equal to 0");

        Origin = origin;
        Direction = direction;
        Length = length;
    }

    public Vector3 GetPoint(float distance)
    {
        return Origin + Direction * distance;
    }
}
