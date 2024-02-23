using System;
using Exanite.Core.Utilities;

namespace Exanite.Core.Collections
{
    /// <summary>
    /// Represents a fixed-sized circular queue.
    /// </summary>
    public class RingBuffer<T>
    {
        private readonly T[] array;
        private readonly int bitmask;

        private int read;
        private int write;

        /// <summary>
        /// Creates a new <see cref="RingBuffer{T}"/>.
        /// </summary>
        /// <param name="capacity">
        /// Power of two capacity of the buffer.
        /// </param>
        public RingBuffer(int capacity)
        {
            capacity = MathUtility.GetNextPowerOfTwo(capacity);
            array = new T[capacity];

            bitmask = capacity - 1;
        }

        /// <summary>
        /// Gets or sets the object at the specified index.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        public T this[int index]
        {
            get
            {
                ValidateIndex(index);

                return array[bitmask & (read + index)];
            }

            set
            {
                ValidateIndex(index);
                array[bitmask & (read + index)] = value;
            }
        }

        /// <summary>
        /// The max number of objects the <see cref="RingBuffer{T}"/> can
        /// hold.
        /// </summary>
        public int Capacity => array.Length;

        /// <summary>
        /// The number of objects contained in the
        /// <see cref="RingBuffer{T}"/>.
        /// </summary>
        public int Count => write - read;

        public bool IsEmpty => read == write;

        public bool IsFull => Count == Capacity;

        /// <summary>
        /// Adds an object to the end of the <see cref="RingBuffer{T}"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="RingBuffer{T}"/> is full.
        /// </exception>
        public void Enqueue(T value)
        {
            if (IsFull)
            {
                throw new InvalidOperationException($"Buffer is full. Cannot {nameof(Enqueue)} a new item.");
            }

            array[bitmask & write++] = value;
        }

        /// <summary>
        /// Removes and returns the object at the beginning of the
        /// <see cref="RingBuffer{T}"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="RingBuffer{T}"/> is empty.
        /// </exception>
        public T Dequeue()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException($"Buffer is empty. Cannot {nameof(Dequeue)} an item.");
            }

            return array[bitmask & read++];
        }

        /// <summary>
        /// Tries to remove and return the object at the beginning of the
        /// <see cref="RingBuffer{T}"/>.
        /// </summary>
        public bool TryDequeue(out T value)
        {
            if (!IsEmpty)
            {
                value = array[bitmask & read++];

                return true;
            }

            value = default;

            return false;
        }

        /// <summary>
        /// Returns the object at the beginning of the
        /// <see cref="RingBuffer{T}"/> without removing it.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="RingBuffer{T}"/> is empty.
        /// </exception>
        public T Peek()
        {
            if (IsEmpty)
            {
                throw new InvalidOperationException("Buffer is empty. Cannot Peek an item.");
            }

            return array[bitmask & read];
        }

        /// <summary>
        /// Tries to returns the object at the beginning of the
        /// <see cref="RingBuffer{T}"/> without removing it.
        /// </summary>
        public bool TryPeek(out T value)
        {
            if (!IsEmpty)
            {
                value = array[bitmask & read];

                return true;
            }

            value = default;

            return false;
        }

        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index),
                    "Index was out of range. Must be non-negative and less than the size of the collection");
            }
        }
    }
}
