using System;
using Exanite.Core.Utilities;

namespace Exanite.Core.Collections
{
    /// <summary>
    /// Represents a fixed-sized circular queue.
    /// </summary>
    public class RingBuffer<T>
    {
        private readonly T[] data;
        private readonly int capacityMask;

        private int readOffset;
        private int writeOffset;

        /// <summary>
        /// Creates a new <see cref="RingBuffer{T}"/>.
        /// </summary>
        /// <param name="capacity">
        /// Power of two capacity of the buffer.
        /// </param>
        public RingBuffer(int capacity)
        {
            capacity = MathUtility.GetNextPowerOfTwo(capacity);
            capacityMask = capacity - 1;

            data = new T[capacity];
        }

        /// <summary>
        /// Gets or sets the object at the specified index.
        /// </summary>
        public ref T this[int index] => ref data[capacityMask & (readOffset + index)];

        /// <summary>
        /// The max number of objects the <see cref="RingBuffer{T}"/> can
        /// hold.
        /// </summary>
        public int Capacity => data.Length;

        /// <summary>
        /// The number of objects contained in the
        /// <see cref="RingBuffer{T}"/>.
        /// </summary>
        public int Count => writeOffset - readOffset;

        public bool IsEmpty => Count == 0;
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

            data[capacityMask & writeOffset++] = value;
        }

        /// <summary>
        /// Tries to adds an object to the end of the <see cref="RingBuffer{T}"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// The <see cref="RingBuffer{T}"/> is full.
        /// </exception>
        public bool TryEnqueue(T value)
        {
            if (IsFull)
            {
                return false;
            }

            data[capacityMask & writeOffset++] = value;
            return true;
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

            return data[capacityMask & readOffset++];
        }

        /// <summary>
        /// Tries to remove and return the object at the beginning of the
        /// <see cref="RingBuffer{T}"/>.
        /// </summary>
        public bool TryDequeue(out T value)
        {
            if (IsEmpty)
            {
                value = default;
                return false;
            }

            value = data[capacityMask & readOffset++];
            return true;
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

            return data[capacityMask & readOffset];
        }

        /// <summary>
        /// Tries to returns the object at the beginning of the
        /// <see cref="RingBuffer{T}"/> without removing it.
        /// </summary>
        public bool TryPeek(out T value)
        {
            if (IsEmpty)
            {
                value = default;
                return false;
            }

            value = data[capacityMask & readOffset];
            return true;
        }
    }
}
