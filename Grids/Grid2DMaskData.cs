using System;
using System.Collections.Generic;
using UnityEngine;

namespace Exanite.Grids
{
	/// <summary>
	/// Holds data for <see cref="Grid2D{T}"/> collision tests
	/// </summary>
	[Serializable]
	public class Grid2DMaskData
	{
		public readonly List<Vector2Int> coordinates;
		public readonly Grid2DSpaceAvailability availability;

		/// <summary>
		/// Creates a new <see cref="Grid2DMaskData"/>
		/// </summary>
		/// <param name="coordinates">List of coordinates</param>
		/// <param name="isOutOfRange">Was a coordinate out of range?</param>
		public Grid2DMaskData(List<Vector2Int> coordinates, Grid2DSpaceAvailability availability)
		{
			this.coordinates = coordinates;
			this.availability = availability;
		}
	}
}