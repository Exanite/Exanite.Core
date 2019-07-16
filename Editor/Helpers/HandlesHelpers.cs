using UnityEditor;
using UnityEngine;

namespace Exanite.Core.Editor.Helpers
{
    public static class HandlesHelpers
    {
        public static void DrawLines(Vector3[] lineSegments, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            for (int i = 0; i < lineSegments.Length / 2; i++)
            {
                Vector3 posA = rotation * Vector3.Scale(lineSegments[i * 2], scale) + position;
                Vector3 posB = rotation * Vector3.Scale(lineSegments[i * 2 + 1], scale) + position;

                Handles.DrawLine(posA, posB);
            }
        }
    } 
}
