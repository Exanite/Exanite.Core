﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Exanite.Core.Utilities;

namespace Exanite.Core.Collections
{
    public class TwoWayDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyTwoWayDictionary<TKey, TValue>
        where TKey : notnull
        where TValue : notnull
    {
        private readonly Dictionary<TKey, TValue> forward;
        private readonly Dictionary<TValue, TKey> backward;

        public TValue this[TKey key]
        {
            get => Forward[key];

            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

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
        /// This dictionary, but with the keys and values reversed.
        /// </summary>
        public TwoWayDictionary<TValue, TKey> Inverse { get; }
        IReadOnlyTwoWayDictionary<TValue, TKey> IReadOnlyTwoWayDictionary<TKey, TValue>.Inverse => Inverse;

        public ICollection<TKey> Keys => Forward.Keys;
        public ICollection<TValue> Values => Forward.Values;
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => Forward.IsReadOnly || Backward.IsReadOnly;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Forward.Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Forward.Values;

        protected IDictionary<TKey, TValue> Forward => forward;
        protected IDictionary<TValue, TKey> Backward => backward;

        public TwoWayDictionary()
        {
            forward = new Dictionary<TKey, TValue>();
            backward = new Dictionary<TValue, TKey>();
            Inverse = new TwoWayDictionary<TValue, TKey>(this);
        }

        /// <summary>
        /// Creates a new <see cref="TwoWayDictionary{TKey,TValue}"/> by copying the values from the provided <see cref="dictionary"/>.
        /// </summary>
        public TwoWayDictionary(IDictionary<TKey, TValue> dictionary)
        {
            forward = new Dictionary<TKey, TValue>(dictionary.Count);
            backward = new Dictionary<TValue, TKey>(dictionary.Count);
            Inverse = new TwoWayDictionary<TValue, TKey>(this);

            foreach (var kvp in dictionary)
            {
                Add(kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Used internally to create the inverse <see cref="TwoWayDictionary{TKey,TValue}"/>.
        /// </summary>
        private TwoWayDictionary(TwoWayDictionary<TValue, TKey> other)
        {
            forward = other.backward;
            backward = other.forward;

            Inverse = other;
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Forward.Add(key, value);
            Backward.Add(value, key);
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            return Forward.TryGetValue(key, out value);
        }

        public bool ContainsKey(TKey key)
        {
            return Forward.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            if (TryGetValue(key, out var value))
            {
                Forward.Remove(key);
                Backward.Remove(value);

                return true;
            }

            return false;
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

            return false;
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            Forward.CopyTo(array, arrayIndex);
        }
    }
}
