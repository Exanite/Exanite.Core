using System.Collections.Generic;

namespace Exanite.Core.Collections
{
    public interface IReadOnlyTwoWayDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        public IReadOnlyTwoWayDictionary<TValue, TKey> Inverse { get; }
    }
}
