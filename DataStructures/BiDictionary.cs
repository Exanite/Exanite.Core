using System;
using System.Collections;
using System.Collections.Generic;
using Exanite.Core.Extensions;
using Sirenix.Serialization;
using UnityEngine;

namespace Exanite.Core.DataStructures
{
    /// <summary>
    /// A two way dictionary
    /// </summary>
    [Serializable]
    public class BiDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [OdinSerialize, HideInInspector] private readonly Dictionary<TKey, TValue> forward;
        private readonly Dictionary<TValue, TKey> backward;
        [OdinSerialize, HideInInspector] private readonly BiDictionary<TValue, TKey> inverse;

        public TValue this[TKey key]
        {
            get
            {
                return Forward[key];
            }

            set
            {
                Forward[key] = value;
                Backward[value] = key;
            }
        }

        public int Count
        {
            get
            {
                if (Forward.Count != Backward.Count)
                {
                    throw new Exception("Internal state mismatched");
                }

                return Forward.Count;
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
                return Forward.Keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                return Forward.Values;
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
                return Forward.IsReadOnly || Backward.IsReadOnly;
            }
        }

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys
        {
            get
            {
                return Forward.Keys;
            }
        }

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values
        {
            get
            {
                return Forward.Values;
            }
        }

        protected IDictionary<TKey, TValue> Forward
        {
            get
            {
                return forward;
            }
        }

        protected IDictionary<TValue, TKey> Backward
        {
            get
            {
                return backward;
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
            Forward.Add(key, value);
            Backward.Add(value, key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Forward.TryGetValue(key, out value);
        }

        public bool ContainsKey(TKey key)
        {
            return Forward.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            if (TryGetValue(key, out TValue value))
            {
                Forward.Remove(key);
                Backward.Remove(value);

                return true;
            }
            else
            {
                return false;
            }
        }

        public void Clear()
        {
            Forward.Clear();
            Backward.Clear();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Forward.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Forward.Add(item);
            Backward.Add(item.AsReverse());
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return Forward.Contains(item);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            if (Forward.Contains(item) && Backward.Contains(item.AsReverse()))
            {
                Forward.Remove(item);
                Backward.Remove(item.AsReverse());

                return true;
            }
            else
            {
                return false;
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Forward.CopyTo(array, arrayIndex);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Backward.Clear();

            foreach (var item in Forward)
            {
                Backward.Add(item.Value, item.Key);
            }
        }
    }
}
