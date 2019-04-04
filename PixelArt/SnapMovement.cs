using Exanite.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Exanite.PixelArt
{
    public class SnapMovement : MonoBehaviour
    {
        [Required(MessageType = InfoMessageType.Error)]
        public CameraRenderEventsHelper cameraRenderEventsHelper;

        public int ppu = 16;

        private Vector3 position;

        private void Awake()
        {
            cameraRenderEventsHelper.PreRender += OnPreRender;
            cameraRenderEventsHelper.PostRender += OnPostRender;
        }

        private void OnPreRender()
        {
            position = transform.position;

            transform.position = new Vector3(Mathf.Round(position.x * ppu) / ppu, Mathf.Round(position.y * ppu) / ppu, Mathf.Round(position.z * ppu) / ppu);
        }

        private void OnPostRender()
        {
            transform.position = position;
        }

        private void OnDestroy()
        {
            cameraRenderEventsHelper.PreRender -= OnPreRender;
            cameraRenderEventsHelper.PostRender -= OnPostRender;
        }
    } 
}
