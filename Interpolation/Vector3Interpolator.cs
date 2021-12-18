using UnityEngine;

namespace Exanite.Core.Interpolation
{
    public class Vector3Interpolator
    {
        public Vector3 Current;
        public Vector3 Previous;

        private float lastPushTime;

        public Vector3 Update(float time, float timeBetweenPushes, float extrapolationMultiplier = 0.5f)
        {
            var timeSinceLastPush = time - lastPushTime;
            timeSinceLastPush += extrapolationMultiplier * timeBetweenPushes;
            
            var t = timeSinceLastPush / timeBetweenPushes;

            return Vector3.LerpUnclamped(Previous, Current, t);
        }

        public void PushNext(Vector3 next, float time)
        {
            Previous = Current;
            Current = next;

            lastPushTime = time;
        }
    }
}