#if UNITY_2021_3_OR_NEWER
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exanite.Core.Collections
{
    /// <summary>
    /// 2D grid that can store any type of value.
    /// </summary>
    public class Grid2D<T> : IEnumerable<T>
    {
        public int XLength => Grid?.GetLength(0) ?? 0;

        public int YLength => Grid?.GetLength(1) ?? 0;

        /// <summary>
        /// Gets or sets the value at (x, y).
        /// </summary>
        public virtual T this[int x, int y]
        {
            get => Grid[x, y];

            set => Grid[x, y] = value;
        }

        /// <summary>
        /// Internal 2D array used for storing values.
        /// </summary>
        protected T[,] Grid { get; set; }

        /// <summary>
        /// Creates a new Grid2D with a size of (1, 1).
        /// </summary>
        public Grid2D() : this(1, 1) {}

        public Grid2D(int xLength, int yLength)
        {
            if (xLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(xLength));
            }

            if (yLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(yLength));
            }

            Grid = new T[xLength, yLength];
        }

        public virtual void RotateClockwise()
        {
            var newArray = new T[YLength, XLength];

            for (var x = 0; x < XLength; x++)
            {
                for (var y = 0; y < YLength; y++)
                {
                    newArray[YLength - 1 - y, x] = Grid[x, y];
                }
            }

            Grid = newArray;
        }

        public virtual void RotateCounterClockwise()
        {
            var newArray = new T[YLength, XLength];

            for (var x = 0; x < XLength; x++)
            {
                for (var y = 0; y < YLength; y++)
                {
                    newArray[y, XLength - 1 - x] = Grid[x, y];
                }
            }

            Grid = newArray;
        }

        /// <summary>
        /// Mirrors the grid over the y-axis.
        /// </summary>
        public virtual void MirrorOverY()
        {
            var newArray = new T[XLength, YLength];

            for (var x = 0; x < XLength; x++)
            {
                for (var y = 0; y < YLength; y++)
                {
                    newArray[x, y] = Grid[XLength - 1 - x, y];
                }
            }

            Grid = newArray;
        }

        /// <summary>
        /// Mirrors the grid over the x-axis.
        /// </summary>
        public virtual void MirrorOverX()
        {
            var newArray = new T[XLength, YLength];

            for (var x = 0; x < XLength; x++)
            {
                for (var y = 0; y < YLength; y++)
                {
                    newArray[x, y] = Grid[x, YLength - 1 - y];
                }
            }

            Grid = newArray;
        }

        /// <summary>
        /// Wraps the provided coordinate to be within range of this Grid2D.
        /// </summary>
        public virtual Vector2Int Wrap(int x, int y)
        {
            var coords = new Vector2Int(x, y);

            coords.x = coords.x % XLength;
            coords.y = coords.y % YLength;

            if (coords.x < 0)
            {
                coords.x = XLength + coords.x;
            }

            if (coords.y < 0)
            {
                coords.y = YLength + coords.y;
            }

            return coords;
        }

        /// <summary>
        /// Checks if the provided coordinate is in range of the grid.
        /// </summary>
        public virtual bool IsInRange(int x, int y)
        {
            var result = !(x < 0 || x >= XLength || y < 0 || y >= YLength);

            return result;
        }

        /// <summary>
        /// Resizes the grid by adding/subtracting the number of indexes
        /// specified from each side.
        /// <para/>
        /// Note: Positive values will always expand the grid, negative will
        /// always shrink the grid.
        /// </summary>
        public virtual void Resize(int posX, int negX, int posY, int negY)
        {
            var newSize = new Vector2Int(XLength + posX + negX, YLength + posY + negY);

            if (newSize.x < 0 || newSize.y < 0)
            {
                throw new ArgumentException($"Cannot create a grid of size {newSize}");
            }

            var newArray = new T[newSize.x, newSize.y];

            for (var x = 0; x < XLength; x++)
            {
                for (var y = 0; y < YLength; y++)
                {
                    // pos just check if in range
                    // neg shifts entire array
                    var newIndexes = new Vector2Int(x + negX, y + negY);

                    if (newIndexes.x < 0 || newIndexes.y < 0 || newIndexes.x >= newSize.x || newIndexes.y >= newSize.y) {}
                    else // if in range
                    {
                        newArray[newIndexes.x, newIndexes.y] = Grid[x, y];
                    }
                }
            }

            Grid = newArray;
        }

        /// <summary>
        /// Copies the grid to another <see cref="Grid2D{T}"/>.
        /// </summary>
        /// <returns>
        /// Other grid with copied values.
        /// </returns>
        public virtual Grid2D<T> CopyTo(Grid2D<T> other, int x = 0, int y = 0)
        {
            CopyTo(other.Grid);

            return other;
        }

        /// <summary>
        /// Copies the grid's internal array to a 2D array.
        /// </summary>
        /// <returns>
        /// 2D array with copied values.
        /// </returns>
        public virtual T[,] CopyTo(T[,] other, int x = 0, int y = 0)
        {
            var maxX = Math.Max(XLength, other.GetLength(0));
            var maxY = Math.Max(YLength, other.GetLength(1));

            for (; x < maxX; x++)
            {
                for (; y < maxY; y++)
                {
                    other[x, y] = Grid[x, y];
                }
            }

            return other;
        }

        /// <summary>
        /// Clears the grid.
        /// </summary>
        public virtual void Clear()
        {
            for (var x = 0; x < XLength; x++)
            {
                for (var y = 0; y < YLength; y++)
                {
                    this[x, y] = default;
                }
            }
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            for (var x = 0; x < XLength; x++)
            {
                for (var y = 0; y < YLength; y++)
                {
                    yield return this[x, y];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
#endif
