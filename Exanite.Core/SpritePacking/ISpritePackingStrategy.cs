using Exanite.Core.Numerics;

namespace Exanite.Core.SpritePacking;

public interface ISpritePackingStrategy
{
    public bool TryAdd(Vector2Int size, out Rect2Int rect);
}
