using System.Numerics;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

public struct Ray
{
    public Vector3 Origin;
    public Vector3 Direction;
    public float Length;

    public Vector3 DirectionMagnitude => Direction * Length;
    public Vector3 End => Origin + Direction * Length;

    public Ray(Vector3 origin, Vector3 direction, float length)
    {
        AssertUtility.IsTrue(length >= 0, $"{nameof(length)} must be greater than or equal to 0");
        AssertUtility.IsTrue(!float.IsInfinity(length), $"{nameof(length)} must not be infinity");

        Origin = origin;
        Direction = direction;
        Length = length;
    }

    public Vector3 GetPoint(float distance)
    {
        return Origin + Direction * distance;
    }
}