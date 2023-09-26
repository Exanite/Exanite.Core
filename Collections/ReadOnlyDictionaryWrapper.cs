using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Exanite.Core.Collections
{
    // Based on code found here: https://stackoverflow.com/questions/13593900/how-to-get-around-lack-of-covariance-with-ireadonlydictionary
    public class ReadOnlyDictionaryWrapper<TKey, TValue, TReadOnlyValue> : IReadOnlyDictionary<TKey, TReadOnlyValue> where TValue : TReadOnlyValue
    {
        private readonly IDictionary<TKey, TValue> dictionary;

        public ReadOnlyDictionaryWrapper(IDictionary<TKey, TValue> dictionary)
        {
            this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        }

        public TReadOnlyValue this[TKey key] => dictionary[key];

        public IEnumerable<TKey> Keys => dictionary.Keys;
        public IEnumerable<TReadOnlyValue> Values => dictionary.Values.Cast<TReadOnlyValue>();

        public int Count => dictionary.Count;

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TReadOnlyValue value)
        {
            TValue untypedValue;
            var result = dictionary.TryGetValue(key, out untypedValue);
            value = untypedValue;

            return result;
        }

        public IEnumerator<KeyValuePair<TKey, TReadOnlyValue>> GetEnumerator()
        {
            return dictionary
                .Select(x => new KeyValuePair<TKey, TReadOnlyValue>(x.Key, x.Value))
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
