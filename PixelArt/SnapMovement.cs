﻿using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Exanite.Core.PixelArt
{
    public class SnapMovement : SerializedMonoBehaviour
    {
        [OdinSerialize] public int PixelsPerUnit { get; set; } = 16;

        [OdinSerialize] public bool RevertPosition { get; set; } = true;

        private Vector3 position;

        private void Awake()
        {
            Camera.onPreRender += PreRender;
            Camera.onPostRender += PostRender;
        }

        private void PreRender(Camera camera)
        {
            position = transform.position;

            transform.position = new Vector3(
                Mathf.Round(position.x * PixelsPerUnit) / PixelsPerUnit,
                Mathf.Round(position.y * PixelsPerUnit) / PixelsPerUnit,
                Mathf.Round(position.z * PixelsPerUnit) / PixelsPerUnit);
        }

        private void PostRender(Camera camera)
        {
            if (RevertPosition)
            {
                transform.position = position;
            }
        }

        private void OnDestroy()
        {
            Camera.onPreRender -= PreRender;
            Camera.onPostRender -= PostRender;
        }
    }
}
