using Sirenix.OdinInspector;
using UnityEngine;

namespace Exanite.Utility.PixelArt
{
    public class PixelArtPositioner : MonoBehaviour
    {
        public int ppu = 16;
        [SerializeField, HideInInspector]
        private Vector2 pixelOffset;

        [ShowInInspector]
        public Vector2 PixelOffset
        {
            get
            {
                return pixelOffset;
            }

            set
            {
                pixelOffset = value;
                transform.localPosition = pixelOffset / ppu;
            }
        }
    } 
}