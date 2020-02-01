using System;
using System.Collections;
using System.Collections.Generic;
using Exanite.Core.Extensions;

namespace Exanite.Core.DataStructures
{
    /// <summary>
    /// A two way dictionary
    /// </summary>
    public class BiDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> forward;
        private readonly IDictionary<TValue, TKey> backward;
        private readonly BiDictionary<TValue, TKey> inverse;

        public TValue this[TKey key]
        {
            get
            {
                return forward[key];
            }

            set
            {
                forward[key] = value;
                backward[value] = key;
            }
        }

        public int Count
        {
            get
            {
                if (forward.Count != backward.Count)
                {
                    throw new Exception("Internal state mismatched");
                }

                return forward.Count;
            }
        }

        /// <summary>
        /// Reverse of this <see cref="BiDictionary{TKey, TValue}"/>
        /// </summary>
        public BiDictionary<TValue, TKey> Inverse
        {
            get
            {
                return inverse;
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return forward.Keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                return forward.Values;
            }
        }
        
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
                return forward.IsReadOnly || backward.IsReadOnly;
            }
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get
            {
                return forward.Keys;
            }
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get
            {
                return forward.Values;
            }
        }

        /// <summary>
        /// Creates a new <see cref="BiDictionary{TKey, TValue}"/>
        /// </summary>
        public BiDictionary()
        {
            forward = new Dictionary<TKey, TValue>();
            backward = new Dictionary<TValue, TKey>();
            inverse = new BiDictionary<TValue, TKey>(this);
        }

        /// <summary>
        /// Creates a new <see cref="BiDictionary{TKey, TValue}"/> and copies the values from the provided <paramref name="dictionary"/>
        /// </summary>
        public BiDictionary(IDictionary<TKey, TValue> dictionary)
        {
            forward = new Dictionary<TKey, TValue>(dictionary.Count);
            backward = new Dictionary<TValue, TKey>(dictionary.Count);
            inverse = new BiDictionary<TValue, TKey>(this);

            foreach (var item in dictionary)
            {
                forward.Add(item.Key, item.Value);
                backward.Add(item.Value, item.Key);
            }
        }

        /// <summary>
        /// Used internally to create the inverse <see cref="BiDictionary{TKey, TValue}"/>
        /// </summary>
        /// <param name="other"></param>
        private BiDictionary(BiDictionary<TValue, TKey> other)
        {
            forward = other.backward;
            backward = other.forward;

            inverse = other;
        }

        public void Add(TKey key, TValue value)
        {
            forward.Add(key, value);
            backward.Add(value, key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return forward.TryGetValue(key, out value);
        }

        public bool ContainsKey(TKey key)
        {
            return forward.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            if (TryGetValue(key, out TValue value))
            {
                forward.Remove(key);
                backward.Remove(value);

                return true;
            }
            else
            {
                return false;
            }
        }

        public void Clear()
        {
            forward.Clear();
            backward.Clear();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            forward.Add(item);
            backward.Add(item.AsReverse());
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return forward.Contains(item);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (forward.Contains(item) && backward.Contains(item.AsReverse()))
            {
                forward.Remove(item);
                backward.Remove(item.AsReverse());

                return true;
            }
            else
            {
                return false;
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            forward.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return forward.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
