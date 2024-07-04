using System.Collections.Generic;
using Exanite.Core.Tracking.Definitions;
using UnityEngine;

namespace Exanite.Core.Tracking
{
    public class GameObjectTracker : MonoBehaviour
    {
        private readonly Dictionary<ITrackedCollectionDefinition, object> trackedCollections = new();
        private readonly HashSet<GameObject> trackedObjects = new();

        public IReadOnlyCollection<GameObject> TrackedObjects => trackedObjects;

        public TCollection GetCollection<TValue, TCollection>(TrackedCollectionDefinition<TValue, TCollection> definition) where TCollection : notnull
        {
            if (!trackedCollections.TryGetValue(definition, out var collection))
            {
                collection = CreateCollection(definition);
            }

            return (TCollection)collection;
        }

        public void Register(GameObject go)
        {
            if (trackedObjects.Add(go))
            {
                foreach (var (definition, collection) in trackedCollections)
                {
                    definition.TryAddToCollection(collection, go);
                }
            }
        }

        public void Unregister(GameObject go)
        {
            if (trackedObjects.Remove(go))
            {
                foreach (var (definition, collection) in trackedCollections)
                {
                    definition.TryRemoveFromCollection(collection, go);
                }
            }
        }

        private object CreateCollection(ITrackedCollectionDefinition definition)
        {
            var collection = definition.CreateCollection();
            trackedCollections.Add(definition, collection);

            foreach (var trackedObject in trackedObjects)
            {
                definition.TryAddToCollection(collection, trackedObject);
            }

            return collection;
        }
    }
}
