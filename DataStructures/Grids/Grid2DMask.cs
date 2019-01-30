using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Exanite.DataStructures.Grids
{
    /// <summary>
    /// <see cref="Grid2D{T}"/> used to apply a value to predefined locations
    /// </summary>
    [Serializable]
    public class Grid2DMask : Grid2D<bool>
    {
        #region Properties

        #region OdinInspector

#if ODIN_INSPECTOR && UNITY_EDITOR

        private readonly static Color OnColor = new Color(0.15f, 0.9f, 0.15f);
        private readonly static Color OffColor = new Color(0f, 0f, 0f, 0f);

        [ShowIf(nameof(ShowGrid))]
        [PropertyOrder(-1)]
        [ShowInInspector]
        [TableMatrix(DrawElementMethod = nameof(DrawBoolElement))]
        public override bool[,] Grid
        {
            get
            {
                return base.Grid;
            }

            protected set
            {
                base.Grid = value;
            }
        }

        // Code is from https://sirenix.net/odininspector > Create feature-rich editors > Custom tables
        private static bool DrawBoolElement(Rect rect, bool value)
        {
            if(Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                value = !value;
                GUI.changed = true;
                Event.current.Use();
            }

            EditorGUI.DrawRect(rect.Padding(1), value ? OnColor : OffColor);

            return value;
        }

#endif

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="Grid2DMask"/>
        /// </summary>
        /// <param name="xLength">Length of the grid along the X-Axis</param>
        /// <param name="yLength">Length of the grid along the Y-Axis</param>
        public Grid2DMask(int xLength, int yLength) : base(xLength, yLength) { }

        #endregion

        #region GetMaskDataFromGrid
        
        /// <summary>
        /// Returns a <see cref="Grid2DMaskData"/> with coordinates matching the <see langword="true"/> values of this <see cref="Grid2DMask"/> and the availability of the targeted area
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> of <see cref="Grid2D{T}"/></typeparam>
        /// <param name="grid"><see cref="Grid2D{T}"/> to check</param>
        /// <param name="x">X-Coordinate representing where to check</param>
        /// <param name="y">Y-Coordinate representing where to check</param>
        /// <param name="wrap">Should the coordinates be wrapped?</param>
        /// <returns><see cref="Grid2DMask"/> with coordinates matching the <see langword="true"/> values of this <see cref="Grid2DMask"/> and the availability of the targeted area</returns>
        public virtual Grid2DMaskData GetMaskDataFromGrid<T>(Grid2D<T> grid, int x, int y, bool wrap = false)
        {
            return GetMaskDataFromGrid<T>(grid, new Vector2Int(x, y), wrap);
        }

        /// <summary>
        /// Returns a <see cref="Grid2DMaskData"/> with coordinates matching the <see langword="true"/> values of this <see cref="Grid2DMask"/> and the availability of the targeted area
        /// </summary>
        /// <typeparam name="T"><see cref="Type"/> of <see cref="Grid2D{T}"/></typeparam>
        /// <param name="grid"><see cref="Grid2D{T}"/> to check</param>
        /// <param name="coords">(x, y) coordinates representing where to check</param>
        /// <param name="wrap">Should the coordinates be wrapped?</param>
        /// <returns><see cref="Grid2DMask"/> with coordinates matching the <see langword="true"/> values of this <see cref="Grid2DMask"/> and the availability of the targeted area</returns>
        public virtual Grid2DMaskData GetMaskDataFromGrid<T>(Grid2D<T> grid, Vector2Int coords, bool wrap = false)
        {
            List<Vector2Int> colliders = new List<Vector2Int>();
            Grid2DSpaceAvailability availability = Grid2DSpaceAvailability.Empty;

            for (int x = 0; x < XLength; x++)
            {
                for (int y = 0; y < YLength; y++)
                {
                    if(Grid[x, y])
                    {
                        Vector2Int coords2 = new Vector2Int(coords.x + x, coords.y + y);

                        if(wrap)
                        {
                            grid.Wrap(coords2);
                        }

                        if(grid.IsInRange(coords2))
                        {
                            if (!(EqualityComparer<T>.Default.Equals(grid[coords.x + x, coords.y + y], default(T))))
                            {
                                colliders.Add(new Vector2Int(coords.x + x, coords.y + y));

                                if(availability == Grid2DSpaceAvailability.Empty) // This is so that it will not overwrite Grid2DSpaceAvailability.OutOfRange
                                {
                                    availability = Grid2DSpaceAvailability.Occupied;
                                }
                            }
                        }
                        else
                        {
                            availability = Grid2DSpaceAvailability.OutOfRange;
                        }
                    }
                }
            }

            return new Grid2DMaskData(colliders, availability);
        }

        #endregion

        #region ApplyMaskToGrid

        /// <summary>
        /// Applies a value according to the true values of this <see cref="Grid2DMask"/> at the provided (x, y) coordinates
        /// </summary>
        /// <typeparam name="T">Type of <see cref="Grid2D{T}"/> to apply to</typeparam>
        /// <param name="grid"><see cref="Grid2D{T}"/> to apply to</param>
        /// <param name="value">Value to apply</param>
        /// <param name="x">X-Coordinate representing where to apply this mask</param>
        /// <param name="y">Y-Coordinate representing where to apply this mask</param>
        /// <param name="requiredAvailability">Highest availability allowed, if higher this method aborts and returns false</param>
        /// <param name="wrap">Should the coordinates be wrapped?</param>
        /// <returns>Did the method succeed?</returns>
        public virtual bool ApplyMaskToGrid<T>(Grid2D<T> grid, T value, int x, int y, Grid2DSpaceAvailability requiredAvailability = Grid2DSpaceAvailability.Empty, bool wrap = false)
        {
            return ApplyMaskToGrid(grid, value, new Vector2Int(x, y), requiredAvailability, wrap);
        }

        /// <summary>
        /// Applies a value according to the true values of this <see cref="Grid2DMask"/> at the provided (x, y) coordinates
        /// </summary>
        /// <typeparam name="T">Type of <see cref="Grid2D{T}"/> to apply to</typeparam>
        /// <param name="grid"><see cref="Grid2D{T}"/> to apply to</param>
        /// <param name="value">Value to apply</param>
        /// <param name="coords">(x, y) coordinates representing where to apply this mask</param>
        /// <param name="requiredAvailability">Highest availability allowed, if higher this method aborts and returns false</param>
        /// <param name="wrap">Should the coordinates be wrapped?</param>
        /// <returns>Did the method succeed?</returns>
        public virtual bool ApplyMaskToGrid<T>(Grid2D<T> grid, T value, Vector2Int coords, Grid2DSpaceAvailability requiredAvailability = Grid2DSpaceAvailability.Empty, bool wrap = false)
        {
            Grid2DMaskData data = GetMaskDataFromGrid(grid, coords, wrap);

            if(data.availability <= requiredAvailability)
            {
                for (int i = 0; i < data.coordinates.Count; i++)
                {
                    grid[data.coordinates[i], true] = value;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}