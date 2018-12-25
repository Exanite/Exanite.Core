using System;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Exanite.Grids
{
	[Serializable]
	public class Grid2D<T>
	{
		#region Fields and Properties

		[SerializeField]
		[HideInInspector]
		protected T[,] grid;
		/// <summary>
		/// The size of the grid
		/// </summary>
#if ODIN_INSPECTOR
		[ReadOnly]
#endif
		public readonly Vector2Int Dimensions;
		/// <summary>
		/// Does this grid allow wrapping?
		/// </summary>
#if ODIN_INSPECTOR
		[ReadOnly]
#endif
		public readonly bool AllowWrap;

#if ODIN_INSPECTOR
		[PropertyOrder(-1)]
		[ShowInInspector]
#endif
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
			if (xLength <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(xLength));
			}
			if (yLength <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(yLength));
			}

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
#if ODIN_INSPECTOR
		[Button(ButtonHeight = 25)]
#endif
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
			if(AllowWrap)
			{
				coords.x = coords.x % XLength;
				coords.y = coords.y % YLength;

				if(coords.x < 0)
				{
					coords.x = XLength + coords.x;
				}
				if(coords.y < 0)
				{
					coords.y = YLength + coords.y;
				}
			}

			return coords;
		}

		#endregion
	}
}