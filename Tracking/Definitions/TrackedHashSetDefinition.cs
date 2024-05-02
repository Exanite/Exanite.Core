using System.Collections.Generic;
using Exanite.Core.Tracking.Conditions;
using JetBrains.Annotations;
using UnityEngine;

namespace Exanite.Core.Tracking.Definitions
{
    public class TrackedHashSetDefinition<T> : TrackedCollectionDefinition<T, HashSet<T>>
    {
        public TrackedHashSetDefinition([NotNull] MatchCondition<T> matchCondition) : base(matchCondition) {}

        public override HashSet<T> CreateCollection()
        {
            return new HashSet<T>();
        }

        public override void TryAddToCollection(HashSet<T> collection, GameObject gameObject)
        {
            if (IsMatch(gameObject, out var narrowedValue))
            {
                collection.Add(narrowedValue);
            }
        }

        public override void TryRemoveFromCollection(HashSet<T> collection, GameObject gameObject)
        {
            if (IsMatch(gameObject, out var narrowedValue))
            {
                collection.Remove(narrowedValue);
            }
        }
    }
}
