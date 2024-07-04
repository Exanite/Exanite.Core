using System;
using System.Collections.Generic;
using Exanite.Core.Tracking.Conditions;
using UnityEngine;

namespace Exanite.Core.Tracking.Definitions
{
    public class TrackedDictionaryCollectionDefinition<TKey, TValue> : TrackedCollectionDefinition<TValue, Dictionary<TKey, TValue>>
    {
        private readonly Func<TValue, TKey> getKey;

        public TrackedDictionaryCollectionDefinition(MatchCondition<TValue> matchCondition, Func<TValue, TKey> getKey) : base(matchCondition)
        {
            this.getKey = getKey;
        }

        public override Dictionary<TKey, TValue> CreateCollection()
        {
            return new Dictionary<TKey, TValue>();
        }

        public override void TryAddToCollection(Dictionary<TKey, TValue> collection, GameObject gameObject)
        {
            if (IsMatch(gameObject, out var value))
            {
                collection.Add(getKey.Invoke(value), value);
            }
        }

        public override void TryRemoveFromCollection(Dictionary<TKey, TValue> collection, GameObject gameObject)
        {
            if (IsMatch(gameObject, out var value))
            {
                collection.Remove(getKey.Invoke(value));
            }
        }
    }
}
