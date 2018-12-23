using System;
using UnityEngine;

namespace ExaniteCore.Grids
{
	public class Grid2D<T>
	{
		#region Fields and Properties

		protected T[,] grid;
		/// <summary>
		/// The size of the grid
		/// </summary>
		public readonly Vector2Int Dimensions;
		/// <summary>
		/// Does this grid allow wrapping?
		/// </summary>
		public readonly bool AllowWrap;

		public T[,] Grid
		{
			get
			{
				return grid;
			}

			protected set
			{
				grid = value;
			}
		}
		/// <summary>
		/// X length of the grid
		/// </summary>
		public int XLength
		{
			get
			{
				return Dimensions.x;
			}
		}
		/// <summary>
		/// Y Length of the grid
		/// </summary>
		public int YLength
		{
			get
			{
				return Dimensions.y;
			}
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Creates a new Grid2D
		/// </summary>
		/// <param name="xLength">Length of the grid along the X-Axis</param>
		/// <param name="yLength">Length of the grid along the Y-Axis</param>
		/// <param name="allowWrap">Does the grid allow wrapping when the passed coordinates are out of range?</param>
		public Grid2D(int xLength, int yLength, bool allowWrap)
		{
			Dimensions = new Vector2Int(xLength, yLength);
			AllowWrap = allowWrap;

			grid = new T[xLength, yLength];
		}

		#endregion

		#region Setting Values

		/// <summary>
		/// Sets (x, y) in the grid to be the passed value
		/// </summary>
		/// <param name="value">Value to set</param>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		public virtual void SetValueAt(T value, int x, int y)
		{
			SetValueAt(value, new Vector2Int(x, y));
		}

		/// <summary>
		/// Sets (x, y) in the grid to be the passed value
		/// </summary>
		/// <param name="value">Value to set</param>
		/// <param name="coords">Vector2Int representation of (x, y)</param>
		public virtual void SetValueAt(T value, Vector2Int coords)
		{
			coords = Wrap(coords);
			grid[coords.x, coords.y] = value;
		}

		#endregion

		#region Getting Values

		/// <summary>
		/// Gets the value at (x, y) in the grid
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		/// <returns>Value at (x, y)</returns>
		public virtual T GetValueAt(int x, int y)
		{
			return GetValueAt(new Vector2Int(x, y));
		}

		/// <summary>
		/// Gets the value at (x, y) in the grid
		/// </summary>
		/// <param name="coords">Vector2Int representation of (x, y)</param>
		/// <returns>Value at (x, y)</returns>
		public virtual T GetValueAt(Vector2Int coords)
		{
			coords = Wrap(coords);
			return grid[coords.x, coords.y];
		}

		#endregion

		#region Internal

		protected virtual Vector2Int Wrap(int x, int y)
		{
			return Wrap(new Vector2Int(x, y));
		}

		protected virtual Vector2Int Wrap(Vector2Int coords)
		{
			#region Wrap X

			if (coords.x >= XLength)
			{
				if(AllowWrap)
				{
					coords.x -= XLength;
				}
				else
				{
					throw new ArgumentOutOfRangeException(nameof(coords.x));
				}
			}
			else if(coords.x < 0)
			{
				if (AllowWrap)
				{
					coords.x += XLength;
				}
				else
				{
					throw new ArgumentOutOfRangeException(nameof(coords.x));
				}
			}

			#endregion

			#region Wrap Y

			if (coords.y >= YLength)
			{
				if (AllowWrap)
				{
					coords.y -= YLength;
				}
				else
				{
					throw new ArgumentOutOfRangeException(nameof(coords.y));
				}
			}
			else if (coords.y < 0)
			{
				if (AllowWrap)
				{
					coords.y += YLength;
				}
				else
				{
					throw new ArgumentOutOfRangeException(nameof(coords.y));
				}
			}

			#endregion

			return coords;
		}

		#endregion
	}
}