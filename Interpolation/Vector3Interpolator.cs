using System.Numerics;
using Exanite.Core.Utilities;

namespace Exanite.Core.Interpolation;

public class Vector3Interpolator : Interpolator<Vector3>
{
    public override Vector3 Lerp(Vector3 previous, Vector3 current, float time)
    {
        return M.LerpUnclamped(previous, current, time);
    }
}
