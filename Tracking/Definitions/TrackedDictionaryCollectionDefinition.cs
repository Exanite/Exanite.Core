using System;
using System.Collections.Generic;
using Exanite.Core.Tracking.Conditions;
using JetBrains.Annotations;
using UnityEngine;

namespace Exanite.Core.Tracking.Definitions
{
    public class TrackedDictionaryCollectionDefinition<TKey, TValue> : TrackedCollectionDefinition<TValue, IReadOnlyDictionary<TKey, TValue>>
    {
        private readonly Func<TValue, TKey> getKey;
        
        public TrackedDictionaryCollectionDefinition([NotNull] MatchCondition<TValue> matchCondition, [NotNull] Func<TValue, TKey> getKey) : base(matchCondition)
        {
            this.getKey = getKey;
        }

        public override IReadOnlyDictionary<TKey, TValue> CreateCollection()
        {
            return new Dictionary<TKey, TValue>();
        }

        public override void TryAddToCollection(IReadOnlyDictionary<TKey, TValue> collection, GameObject gameObject)
        {
            if (IsMatch(gameObject, out var value))
            {
                var typedCollection = (Dictionary<TKey, TValue>)collection;
                typedCollection.Add(getKey.Invoke(value), value);
            }
        }

        public override void TryRemoveFromCollection(IReadOnlyDictionary<TKey, TValue> collection, GameObject gameObject)
        {
            if (IsMatch(gameObject, out var value))
            {
                var typedCollection = (Dictionary<TKey, TValue>)collection;
                typedCollection.Remove(getKey.Invoke(value));
            }
        }
    }
}
