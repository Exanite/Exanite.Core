﻿using UnityEditor;
using UnityEngine;

namespace Exanite.Core.Editor.Helpers
{
    /// <summary>
    /// Helper methods for drawing <see cref="Handles"/> in the <see cref="SceneView"/>
    /// </summary>
    public static class HandlesHelpers
    {
        private static readonly Vector3[] Square = new Vector3[]
        {
            new Vector3(-0.5f, 0, -0.5f), new Vector3(-0.5f, 0, 0.5f),
            new Vector3(-0.5f, 0, 0.5f), new Vector3(0.5f, 0, 0.5f),
            new Vector3(0.5f, 0, 0.5f), new Vector3(0.5f, 0, -0.5f),
            new Vector3(0.5f, 0, -0.5f), new Vector3(-0.5f, 0, -0.5f),
        };

        /// <summary>
        /// Draws multiple lines
        /// </summary>
        public static void DrawLines(Vector3[] lineSegments, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            for (int i = 0; i < lineSegments.Length / 2; i++)
            {
                Vector3 posA = (rotation * Vector3.Scale(lineSegments[i * 2], scale)) + position;
                Vector3 posB = (rotation * Vector3.Scale(lineSegments[(i * 2) + 1], scale)) + position;

                Handles.DrawLine(posA, posB);
            }
        }

        /// <summary>
        /// Draws a rectangle
        /// </summary>
        public static void DrawRectangle(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            DrawLines(Square, position, rotation, scale);
        }
    }
}
