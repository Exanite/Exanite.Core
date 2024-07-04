using System;
using Exanite.Core.Tracking.Conditions;
using UnityEngine;

namespace Exanite.Core.Tracking.Definitions
{
    public abstract class TrackedCollectionDefinition<TValue, TCollection> : ITrackedCollectionDefinition where TCollection : notnull
    {
        /// <summary>
        /// This technically can be implemented as an abstract method instead,
        /// but a delegate is preferred because it allows users of the class
        /// to customize the <see cref="MatchCondition{TValue}"/> without creating a new class.
        /// </summary>
        protected MatchCondition<TValue> IsMatch { get; }

        protected TrackedCollectionDefinition(MatchCondition<TValue> matchCondition)
        {
            IsMatch = matchCondition ?? throw new ArgumentNullException(nameof(matchCondition));
        }

        public abstract TCollection CreateCollection();

        public abstract void TryAddToCollection(TCollection collection, GameObject gameObject);

        public abstract void TryRemoveFromCollection(TCollection collection, GameObject gameObject);

        object ITrackedCollectionDefinition.CreateCollection()
        {
            return CreateCollection();
        }

        void ITrackedCollectionDefinition.TryAddToCollection(object collection, GameObject gameObject)
        {
            TryAddToCollection((TCollection)collection, gameObject);
        }

        void ITrackedCollectionDefinition.TryRemoveFromCollection(object collection, GameObject gameObject)
        {
            TryRemoveFromCollection((TCollection)collection, gameObject);
        }
    }
}
