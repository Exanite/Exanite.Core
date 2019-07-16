using System.Collections.Generic;

namespace Exanite.Core.DataStructures
{
    /// <summary>
    /// List with a limited size
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LimitedList<T> : List<T>
    {
        /// <summary>
        /// Max entries that are allowed in this list
        /// </summary>
        public int MaxEntries;

        /// <summary>
        /// Creates a limited list
        /// </summary>
        /// <param name="maxEntries">Max entries that are allowed in this list</param>
        public LimitedList(int maxEntries) : base()
        {
            MaxEntries = maxEntries;
        }

        /// <summary>
        /// Creates a limited list
        /// </summary>
        /// <param name="maxEntries">Max entries that are allowed in this list</param>
        /// <param name="collection">Collection to fill the LimitedList with</param>
        public LimitedList(int maxEntries, IEnumerable<T> collection) : base(collection)
        {
            if (GetAmountInCollection(collection) > maxEntries)
            {
                throw new CollectionOutOfRangeException();
            }

            MaxEntries = maxEntries;
        }

        /// <summary>
        /// Creates a limited list
        /// </summary>
        /// <param name="maxEntries">Max entries that are allowed in this list</param>
        /// <param name="capacity">Capacity of the list</param>
        public LimitedList(int maxEntries, int capacity) : base(capacity)
        {
            MaxEntries = maxEntries;
        }

        /// <summary>
        /// Adds an item to the list
        /// </summary>
        /// <param name="item">Item to add</param>
        public new void Add(T item)
        {
            if (CanAddMoreEntries(1))
            {
                base.Add(item);
            }
            else
            {
                throw new LimitReachedException();
            }
        }

        /// <summary>
        /// Adds a collection to the list
        /// </summary>
        /// <param name="collection">Collection to add</param>
        public new void AddRange(IEnumerable<T> collection)
        {
            if (CanAddMoreEntries(GetAmountInCollection(collection)))
            {
                base.AddRange(collection);
            }
            else
            {
                throw new LimitReachedException();
            }
        }

        /// <summary>
        /// Adds an item to the list at the provided index
        /// </summary>
        /// <param name="index">Index to add the item at</param>
        /// <param name="item">Item to add</param>
        public new void Insert(int index, T item)
        {
            if (CanAddMoreEntries(1))
            {
                base.Insert(index, item);
            }
            else
            {
                throw new LimitReachedException();
            }
        }

        /// <summary>
        /// Adds a collection to the list at the provided index
        /// </summary>
        /// <param name="index">Index to add the collection at</param>
        /// <param name="collection">Collection to add</param>
        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            if (CanAddMoreEntries(GetAmountInCollection(collection)))
            {
                base.InsertRange(index, collection);
            }
            else
            {
                throw new LimitReachedException();
            }
        }

        /// <summary>
        /// Returns if it is possible to add the amount of items to the list
        /// </summary>
        /// <param name="amount">Amount of items to add</param>
        /// <returns>Is it possible to add the amount of items to the list</returns>
        public bool CanAddMoreEntries(int amount)
        {
            return amount + Count <= MaxEntries;
        }

        private int GetAmountInCollection(IEnumerable<T> collection)
        {
            int amount = 0;

            foreach (var item in collection)
            {
                amount++;
            }

            return amount;
        }

        /// <summary>
        /// Exception raised when the limit is reached, but more entries are attempted to be added
        /// </summary>
        [System.Serializable]
        public class LimitReachedException : System.Exception
        {
            /// <summary>
            /// Creates a new <see cref="LimitReachedException"/>
            /// </summary>
            public LimitReachedException() : base("Limit reached, cannot add more entries") { }
        }

        /// <summary>
        /// Exception raised when the collection provided is larger than the provided max entries
        /// </summary>
        [System.Serializable]
        public class CollectionOutOfRangeException : System.Exception
        {
            /// <summary>
            /// Creates a new <see cref="CollectionOutOfRangeException"/>
            /// </summary>
            public CollectionOutOfRangeException() : base("Collection provided is larger than the provided max entries") { }
        }
    }

    /// <summary>
    /// Static extensions class for <see cref="LimitedList{T}"/>
    /// </summary>
    public static class LimitedList
    {
        /// <summary>
        /// Returns a <see cref="LimitedList{T}"/> from a <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="T">Type of <see cref="IEnumerable{T}"/></typeparam>
        /// <param name="collection">Collection to perform this method on</param>
        /// <param name="maxEntries">Max entries of the <see langword="new"/> <see cref="LimitedList{T}"/></param>
        /// <returns>New <see cref="LimitedList{T}"/></returns>
        public static LimitedList<T> ToLimitedList<T>(this IEnumerable<T> collection, int maxEntries)
        {
            return new LimitedList<T>(maxEntries, collection);
        }
    }
}