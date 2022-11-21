using UnityEditor;
using UnityEngine;

namespace Exanite.Core.Editor.Utilities
{
    /// <summary>
    ///     Helper methods for drawing <see cref="Handles"/> in the
    ///     <see cref="SceneView"/>.
    /// </summary>
    public static class HandlesUtility
    {
        private static readonly Vector3[] Square =
        {
            new(-0.5f, 0, -0.5f), new(-0.5f, 0, 0.5f),
            new(-0.5f, 0, 0.5f), new(0.5f, 0, 0.5f),
            new(0.5f, 0, 0.5f), new(0.5f, 0, -0.5f),
            new(0.5f, 0, -0.5f), new(-0.5f, 0, -0.5f),
        };

        /// <summary>
        ///     Draws multiple lines.
        /// </summary>
        public static void DrawLines(Vector3[] lineSegments, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            for (var i = 0; i < lineSegments.Length / 2; i++)
            {
                var posA = rotation * Vector3.Scale(lineSegments[i * 2], scale) + position;
                var posB = rotation * Vector3.Scale(lineSegments[i * 2 + 1], scale) + position;

                Handles.DrawLine(posA, posB);
            }
        }

        /// <summary>
        ///     Draws a rectangle.
        /// </summary>
        public static void DrawRectangle(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            DrawLines(Square, position, rotation, scale);
        }
    }
}