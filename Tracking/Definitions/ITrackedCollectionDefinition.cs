using UnityEngine;

namespace Exanite.Core.Tracking.Definitions
{
    public interface ITrackedCollectionDefinition
    {
        public object CreateCollection();

        public void TryAddToCollection(object collection, GameObject gameObject);
        public void TryRemoveFromCollection(object collection, GameObject gameObject);
    }
}
