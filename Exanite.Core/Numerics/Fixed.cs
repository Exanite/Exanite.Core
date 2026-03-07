namespace Exanite.Core.Numerics;

/// <summary>
/// A fixed point Q48.16 value (48-bits for integer, 16-bits for fraction).
/// </summary>
public readonly struct Fixed
{
    private readonly long value;

    private Fixed(long value)
    {
        this.value = value;
    }
}
