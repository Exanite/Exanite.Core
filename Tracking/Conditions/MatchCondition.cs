using UnityEngine;

namespace Exanite.Core.Tracking.Conditions
{
    public delegate bool MatchCondition<T>(GameObject gameObject, out T narrowedValue);
}
