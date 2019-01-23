using System;

namespace Exanite.Grids
{
    /// <summary>
    /// Represents if the targeted coordinate(s) are available
    /// </summary>
    [Serializable]
    public enum Grid2DSpaceAvailability
    {
        /// <summary>
        /// All of the targeted coordinate(s) hold <see langword="default"/> values (<see langword="null"/>/0/etc)
        /// </summary>
        Empty = 0,
        /// <summary>
        /// Some of the targeted coordinates(s) hold non-<see langword="default"/> values (<see langword="null"/>/0/etc)
        /// </summary>
        Occupied = 1,
        /// <summary>
        /// Some of the targeted coordinate(s) are out of range
        /// </summary>
        OutOfRange = 2,
    }
}
