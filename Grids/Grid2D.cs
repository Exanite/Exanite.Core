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

		public Grid2D(int xLength, int yLength, bool allowWrap)
		{
			Dimensions = new Vector2Int(xLength, yLength);
			this.AllowWrap = allowWrap;

			grid = new T[xLength, yLength];
		}

		#endregion

		#region Setting Values

		public virtual void SetValueAt(T value, int x, int y)
		{
			SetValueAt(value, new Vector2Int(x, y));
		}

		public virtual void SetValueAt(T value, Vector2Int coords)
		{
			coords = Wrap(coords);
			grid[coords.x, coords.y] = value;
		}

		#endregion

		#region Getting Values

		public virtual T GetValueAt(int x, int y)
		{
			return GetValueAt(new Vector2Int(x, y));
		}

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