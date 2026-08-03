using Exanite.Core.Numerics;

namespace Exanite.Core.SpritePacking;

public interface IRectPackingStrategy
{
    public bool TryAdd(Vector2Int size, out Rect2Int rect);
}
