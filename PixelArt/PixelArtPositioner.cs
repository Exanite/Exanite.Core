using Sirenix.OdinInspector;
using UnityEngine;

namespace Exanite.PixelArt
{
    public class PixelArtPositioner : MonoBehaviour
    {
        public int ppu = 16;

        [ShowInInspector]
        public Vector2 PixelOffset
        {
            get
            {
                return transform.localPosition * ppu;
            }

            set
            {
                transform.localPosition = value / ppu;
            }
        }
    } 
}