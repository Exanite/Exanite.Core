using System;
using System.Collections.Generic;
using UnityEngine;

namespace Exanite.DataStructures.Grids
{
    /// <summary>
    /// Holds data for Grid2DMask match tests
    /// </summary>
    [Serializable]
    public class Grid2DMaskData
    {
        /// <summary>
        /// List of matched coordinates
        /// </summary>
        public readonly List<Vector2Int> coordinates;
        /// <summary>
        /// 
        /// </summary>
        public readonly Grid2DSpaceAvailability availability;

        /// <summary>
        /// Creates a new <see cref="Grid2DMaskData"/>
        /// </summary>
        /// <param name="coordinates">List of coordinates</param>
        /// <param name="availability">Targeted coordinates' availability</param>
        public Grid2DMaskData(List<Vector2Int> coordinates, Grid2DSpaceAvailability availability)
        {
            this.coordinates = coordinates;
            this.availability = availability;
        }
    }
}