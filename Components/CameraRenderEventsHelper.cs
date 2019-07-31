using System;
using UnityEngine;

namespace Exanite.Core.Components
{
    [RequireComponent(typeof(Camera))]
    public class CameraRenderEventsHelper : MonoBehaviour
    {
        public event Action PreRender;
        public event Action PostRender;

        private void OnPreRender()
        {
            PreRender?.Invoke();
        }

        private void OnPostRender()
        {
            PostRender?.Invoke();
        }
    }
}