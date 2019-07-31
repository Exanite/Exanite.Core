using UnityEngine;

namespace Exanite.Core.Extensions
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Transforms a rotation from local space to world space
        /// </summary>
        public static Quaternion TransformRotation(this Transform transform, Quaternion rotation)
        {
            var direction = rotation * Vector3.forward; // reflect then rotate
            
            if (transform.lossyScale.x < 0)
            {
                direction.x *= -1;
            }

            if (transform.lossyScale.y < 0)
            {
                direction.y *= -1;
            }

            if (transform.lossyScale.z < 0)
            {
                direction.z *= -1;
            }

            rotation = Quaternion.LookRotation(direction, Vector3.up);
            rotation = transform.rotation * rotation;

            return rotation;
        }

        /// <summary>
        /// Transforms a rotation from world space to local space. This is the opposite of TransformRotation
        /// </summary>
        public static Quaternion InverseTransformRotation(this Transform transform, Quaternion rotation)
        {
            rotation = Quaternion.Inverse(transform.rotation) * rotation; // inverse rotate then reflect

            var direction = rotation * Vector3.forward;

            if (transform.lossyScale.x < 0)
            {
                direction.x *= -1;
            }

            if (transform.lossyScale.y < 0)
            {
                direction.y *= -1;
            }

            if (transform.lossyScale.z < 0)
            {
                direction.z *= -1;
            }

            rotation = Quaternion.LookRotation(direction, Vector3.up);

            return rotation;
        }
    }
}
