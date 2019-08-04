using System;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;

namespace Exanite.Core.DataStructures
{
    /// <summary>
    /// 2D grid that can store any type of value
    /// </summary>
    [ShowOdinSerializedPropertiesInInspector]
    public class Grid2D<T> : IEnumerable<T>
    {
        /// <summary>
        /// Internal 2D array used for storing values
        /// </summary>
        [OdinSerialize, TableMatrix(ResizableColumns = false)] protected T[,] Grid { get; set; }

        /// <summary>
        /// X length of the grid
        /// </summary>
        public int XLength => Grid?.GetLength(0) ?? 0;

        /// <summary>
        /// Y length of the grid
        /// </summary>
        public int YLength => Grid?.GetLength(1) ?? 0;

        /// <summary>
        /// Returns value at (x, y)
        /// </summary>
        public virtual T this[int x, int y]
        {
            get
            {
                return Grid[x, y];
            }

            set
            {
                Grid[x, y] = value;
            }
        }

        /// <summary>
        /// Creates a new Grid2D with a size of (1, 1)
        /// </summary>
        public Grid2D() : this(1, 1) { }

        /// <summary>
        /// Creates a new Grid2D
        /// </summary>
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

        /// <summary>
        /// Rotates the grid clockwise the specified amount of times, negative values are ignored
        /// </summary>
        [Button]
        [FoldoutGroup("Methods")]
        [HorizontalGroup("Methods/A")]
        public virtual void RotateClockwise()
        {
            T[,] newArray = new T[YLength, XLength];

            for (int x = 0; x < XLength; x++)
            {
                for (int y = 0; y < YLength; y++)
                {
                    newArray[(YLength - 1) - y, x] = Grid[x, y];
                }
            }

            Grid = newArray;
        }

        /// <summary>
        /// Rotates the grid counter-clockwise the specified amount of times, negative values are ignored
        /// </summary>
        [Button]
        [HorizontalGroup("Methods/B")]
        public virtual void RotateCounterClockwise()
        {
            T[,] newArray = new T[YLength, XLength];

            for (int x = 0; x < XLength; x++)
            {
                for (int y = 0; y < YLength; y++)
                {
                    newArray[y, (XLength - 1) - x] = Grid[x, y];
                }
            }

            Grid = newArray;
        }

        /// <summary>
        /// Mirrors the grid over the Y-Axis
        /// </summary>
        [Button]
        [HorizontalGroup("Methods/A")]
        public virtual void MirrorOverY()
        {
            T[,] newArray = new T[XLength, YLength];

            for (int x = 0; x < XLength; x++)
            {
                for (int y = 0; y < YLength; y++)
                {
                    newArray[x, y] = Grid[(XLength - 1) - x, y];
                }
            }

            Grid = newArray;
        }

        /// <summary>
        /// Mirrors the grid over the X-Axis
        /// </summary>
        [Button]
        [HorizontalGroup("Methods/B")]
        public virtual void MirrorOverX()
        {
            T[,] newArray = new T[XLength, YLength];

            for (int x = 0; x < XLength; x++)
            {
                for (int y = 0; y < YLength; y++)
                {
                    newArray[x, y] = Grid[x, (YLength - 1) - y];
                }
            }

            Grid = newArray;
        }

        /// <summary>
        /// Wraps the provided (x, y) coordinate to be within range of this Grid2D
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
        /// Checks if the provided (x, y) coordinate is in range of the grid
        /// </summary>
        public virtual bool IsInRange(int x, int y)
        {
            bool result = !(x < 0 || x >= XLength || y < 0 || y >= YLength);

            return result;
        }

        [Button]
        [HorizontalGroup("Methods/C")]
        /// <summary>
        /// Resizes the grid by adding/subtracting the number of indexes specified from each side <para/>
        /// Note: Positive values will always expand the grid, negative will always shrink the grid
        /// </summary>
        public virtual void Resize(int posX, int negX, int posY, int negY)
        {
            var newSize = new Vector2Int(XLength + posX + negX, YLength + posY + negY);

            var newArray = new T[newSize.x, newSize.y];

            for (int x = 0; x < XLength; x++)
            {
                for (int y = 0; y < YLength; y++)
                {
                    // pos just check if in range
                    // neg shifts entire array
                    var newIndexes = new Vector2Int(x + negX, y + negY);

                    if (newIndexes.x < 0 || newIndexes.y < 0 || newIndexes.x >= newSize.x || newIndexes.y >= newSize.y)
                    {
                        continue;
                    }
                    else // if in range
                    {
                        newArray[newIndexes.x, newIndexes.y] = Grid[x, y];
                    }
                }
            }

            Grid = newArray;
        }

        /// <summary>
        /// Copies the grid to another <see cref="Grid2D{T}"/>
        /// </summary>
        /// <returns>Other grid with copied values</returns>
        public virtual Grid2D<T> CopyTo(Grid2D<T> other, int x = 0, int y = 0)
        {
            CopyTo(other.Grid);

            return other;
        }

        /// <summary>
        /// Copies the grid's internal array to a 2D array
        /// </summary>
        /// <returns>2D array with copied values</returns>
        public virtual T[,] CopyTo(T[,] other, int x = 0, int y = 0)
        {
            int maxX = Math.Max(XLength, other.GetLength(0));
            int maxY = Math.Max(YLength, other.GetLength(1));

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
        /// Clears the grid
        /// </summary>
        public virtual void Clear()
        {
            for (int x = 0; x < XLength; x++)
            {
                for (int y = 0; y < YLength; y++)
                {
                    this[x, y] = default;
                }
            }
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            for (int x = 0; x < XLength; x++)
            {
                for (int y = 0; y < YLength; y++)
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