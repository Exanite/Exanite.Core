using System.Diagnostics;
using System.Numerics;

namespace Exanite.Core.Numerics
{
    public struct Ray
    {
        public Vector3 Origin;
        public Vector3 Direction;
        public float Length;

        public Vector3 DirectionMagnitude => Direction * Length;
        public Vector3 End => Origin + Direction * Length;

        public Ray(Vector3 origin, Vector3 direction, float length)
        {
            Debug.Assert(length >= 0);
            Debug.Assert(!float.IsInfinity(length));

            Origin = origin;
            Direction = direction;
            Length = length;
        }

        public Vector3 GetPoint(float distance)
        {
            return Origin + Direction * distance;
        }
    }
}
