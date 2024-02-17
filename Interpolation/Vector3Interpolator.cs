#if UNITY_2021_3_OR_NEWER
using UnityEngine;

namespace Exanite.Core.Interpolation
{
    public class Vector3Interpolator : Interpolator<Vector3>
    {
        public override Vector3 Lerp(Vector3 previous, Vector3 current, float time)
        {
            return Vector3.LerpUnclamped(previous, current, time);
        }
    }
}
#endif
