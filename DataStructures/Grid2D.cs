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
    public class Grid2D<T> : IEnumerable<T>
    {
        /// <summary>
        /// Internal 2D array used for storing values
        /// </summary>
        [ShowInInspector, OdinSerialize] protected virtual T[,] Grid { get; set; }

        /// <summary>
        /// X length of the grid
        /// </summary>
        public int XLength => Grid.GetLength(0);

        /// <summary>
        /// Y length of the grid
        /// </summary>
        public int YLength => Grid.GetLength(1);

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
        /// Creates a new Grid2D
        /// </summary>
        public Grid2D(int xLength = 10, int yLength = 10)
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
        [VerticalGroup("Buttons")]
        [HorizontalGroup("Buttons/A")]
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
        [HorizontalGroup("Buttons/B")]
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
        [HorizontalGroup("Buttons/A")]
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
        [HorizontalGroup("Buttons/B")]
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

        /// <summary>
        /// Resizes the grid to the new size
        /// </summary>
        public virtual void Resize(int newX, int newY)
        {
            var newArray = new T[newX, newY];

            for (int x = 0; x < XLength || x < newX; x++)
            {
                for (int y = 0; y < YLength || y < newY; y++)
                {
                    newArray[x, y] = Grid[x, y];
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