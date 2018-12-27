using System;
using UnityEngine;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Exanite.Grids
{
	/// <summary>
	/// 2D grid that can store any type of value
	/// </summary>
	/// <typeparam name="T">Type of value to store</typeparam>
	[Serializable]
	public class Grid2D<T>
	{
		#region Fields and Properties

#if ODIN_INSPECTOR
		[PropertyOrder(-2)]
		[SerializeField]
#pragma warning disable 0414
		private bool showGrid = false;
#pragma warning restore 0414
#endif

#if ODIN_INSPECTOR
		[ShowIf("showGrid")]
		[PropertyOrder(-1)]
#endif
		[SerializeField]
		public T[,] Grid { get; protected set; }
		public bool AllowWrap;

		/// <summary>
		/// X length of the grid
		/// </summary>
		public int XLength
		{
			get
			{
				return Grid.GetLength(0);
			}
		}
		/// <summary>
		/// Y Length of the grid
		/// </summary>
		public int YLength
		{
			get
			{
				return Grid.GetLength(1);
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
			Grid = new T[xLength, yLength];
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
			if(AllowWrap) coords = Wrap(coords);
			Grid[coords.x, coords.y] = value;
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
			if(AllowWrap) coords = Wrap(coords);
			return Grid[coords.x, coords.y];
		}

		#endregion

		#region Transformations

		#region Odin Inspector

#if ODIN_INSPECTOR
		[HorizontalGroup("Buttons/A")]
		[Button(Name = "Rotate Clockwise")]
		private void RotateClockwiseOdin()
		{
			RotateClockwise();
		}

		[HorizontalGroup("Buttons/B")]
		[Button(Name = "Rotate Counter-Clockwise")]
		private void RotateCounterClockwiseOdin()
		{
			RotateCounterClockwise();
		}
#endif

		#endregion

		/// <summary>
		/// Rotates the grid clockwise the specified amount of times
		/// </summary>
		/// <param name="timesToRotate">Times to rotate the grid</param>
		public virtual void RotateClockwise(int timesToRotate = 1)
		{
			for (int i = 0; i < timesToRotate; i++)
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
		}

		/// <summary>
		/// Rotates the grid counter-clockwise the specified amount of times
		/// </summary>
		/// <param name="timesToRotate">Times to rotate the grid</param>
		public virtual void RotateCounterClockwise(int timesToRotate = 1)
		{
			for (int i = 0; i < timesToRotate; i++)
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
		}

		/// <summary>
		/// Mirrors the grid over the Y-Axis
		/// </summary>
#if ODIN_INSPECTOR
		[Button]
		[HorizontalGroup("Buttons/A")]
#endif
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
#if ODIN_INSPECTOR
		[Button]
		[HorizontalGroup("Buttons/B")]
#endif
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

		#endregion

		#region Utility

		#region Wrap

		/// <summary>
		/// Wraps the provided (x, y) coordinate to be within range of this Grid2D
		/// </summary>
		/// <param name="x">X-Coordinate</param>
		/// <param name="y">Y-Coordinate</param>
		/// <returns>Wrapped (x, y) coordinate</returns>
		public virtual Vector2Int Wrap(int x, int y)
		{
			return Wrap(new Vector2Int(x, y));
		}

		/// <summary>
		/// Wraps the provided (x, y) coordinate to be within range of this Grid2D
		/// </summary>
		/// <param name="coords">(x, y) coordinates</param>
		/// <returns>Wrapped (x, y) coordinate</returns>
		public virtual Vector2Int Wrap(Vector2Int coords)
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

			return coords;
		}

		#endregion

		#region IsInRange

		/// <summary>
		/// Checks if the provided (x, y) coordinate is in range of the grid
		/// </summary>
		/// <param name="x">X-Coordinate</param>
		/// <param name="y">Y-Coordinate</param>
		/// <returns>Is the provided (x, y) coordinate in range?</returns>
		public virtual bool IsInRange(int x, int y)
		{
			return IsInRange(new Vector2Int(x, y));
		}

		/// <summary>
		/// Checks if the provided (x, y) coordinate is in range of the grid
		/// </summary>
		/// <param name="coords">(x, y) coordinates</param>
		/// <returns>Is the provided (x, y) coordinate in range?</returns>
		public virtual bool IsInRange(Vector2Int coords)
		{
			bool result = !(coords.x < 0 || coords.x >= XLength || coords.y < 0 || coords.y >= YLength);
			Debug.Log(result);
			return result;
		}

		#endregion

		#endregion
	}
}