using System.Numerics;
using Exanite.Core.Utilities;

namespace Exanite.Core.Interpolation;

public class Vector2Interpolator : Interpolator<Vector2>
{
    public override Vector2 Lerp(Vector2 previous, Vector2 current, float time)
    {
        return M.LerpUnclamped(previous, current, time);
    }
}
