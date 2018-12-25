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

#if ODIN_INSPECTOR
		[PropertyOrder(-2)]
		[ShowInInspector]
		private bool showGrid = false;
#endif

		[SerializeField]
		[HideInInspector]
		protected T[,] grid;

		public bool AllowWrap;

#if ODIN_INSPECTOR
		[ShowIf("showGrid")]
		[PropertyOrder(-1)]
		[ShowInInspector]
#endif
		public T[,] Grid
		{
			get
			{
				return grid;
			}

			set
			{
				if(value == null)
				{
					throw new ArgumentNullException($"Passed setter value for Property 'Grid' is null");
				}
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
				return grid.GetLength(0);
			}
		}
		/// <summary>
		/// Y Length of the grid
		/// </summary>
		public int YLength
		{
			get
			{
				return grid.GetLength(1);
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
		[FoldoutGroup("Buttons")]
		[Button(ButtonHeight = 25, Expanded = true)]
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

		#region Transformations

#if ODIN_INSPECTOR
		[HorizontalGroup("Buttons/A")]
		[Button(Name = "Rotate Clockwise")]
		private void RotateClockwiseOdin()
		{
			RotateClockwise();
		}
#endif

#if ODIN_INSPECTOR
		[HorizontalGroup("Buttons/B")]
		[Button(Name = "Rotate Counter-Clockwise")]
		private void RotateCounterClockwiseOdin()
		{
			RotateCounterClockwise();
		}
#endif

		/// <summary>
		/// Rotates the grid clockwise the specified amount of times
		/// </summary>
		/// <param name="timesToRotate">Times to rotate the grid</param>
		public void RotateClockwise(int timesToRotate = 1)
		{
			for (int i = 0; i < timesToRotate; i++)
			{
				T[,] newArray = new T[YLength, XLength];

				for (int x = 0; x < XLength; x++)
				{
					for (int y = 0; y < YLength; y++)
					{
						newArray[(YLength - 1) - y, x] = grid[x, y];
					}
				}

				grid = newArray;
			}
		}

		/// <summary>
		/// Rotates the grid counter-clockwise the specified amount of times
		/// </summary>
		/// <param name="timesToRotate">Times to rotate the grid</param>
		public void RotateCounterClockwise(int timesToRotate = 1)
		{
			for (int i = 0; i < timesToRotate; i++)
			{
				T[,] newArray = new T[YLength, XLength];

				for (int x = 0; x < XLength; x++)
				{
					for (int y = 0; y < YLength; y++)
					{
						newArray[y, (XLength - 1) - x] = grid[x, y];
					}
				}

				grid = newArray;
			}
		}

		/// <summary>
		/// Mirrors the grid over the Y-Axis
		/// </summary>
#if ODIN_INSPECTOR
		[Button]
		[HorizontalGroup("Buttons/A")]
#endif
		public void MirrorOverY()
		{
			T[,] newArray = new T[XLength, YLength];

			for (int x = 0; x < XLength; x++)
			{
				for (int y = 0; y < YLength; y++)
				{
					newArray[x, y] = grid[(XLength - 1) - x, y];
				}
			}

			grid = newArray;
		}

		/// <summary>
		/// Mirrors the grid over the X-Axis
		/// </summary>
#if ODIN_INSPECTOR
		[Button]
		[HorizontalGroup("Buttons/B")]
#endif
		public void MirrorOverX()
		{
			T[,] newArray = new T[XLength, YLength];

			for (int x = 0; x < XLength; x++)
			{
				for (int y = 0; y < YLength; y++)
				{
					newArray[x, y] = grid[x, (YLength - 1) - y];
				}
			}

			grid = newArray;
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