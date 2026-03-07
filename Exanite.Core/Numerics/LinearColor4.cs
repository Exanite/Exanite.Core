using System.Numerics;
using System.Runtime.InteropServices;

namespace Exanite.Core.Numerics;

/// <summary>
/// Storage struct useful for explicitly storing a color in linear format.
/// Primary recommended use is for interop and other scenarios requiring the underlying numeric format to be in linear.
/// <br/>
/// See <see cref="Color"/> for a general color representation struct and corresponding APIs.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public record struct LinearColor4
{
    public Vector4 Value;
    public readonly Color Color => this;

    public LinearColor4(Vector4 value)
    {
        Value = value;
    }

    public static implicit operator Color(LinearColor4 color)
    {
        return Color.FromLinear(color.Value);
    }

    public static implicit operator LinearColor4(Color color)
    {
        return new LinearColor4(color.Linear.Value);
    }

    public static implicit operator LinearColor4(Vector4 color)
    {
        return new LinearColor4(color);
    }

    public static implicit operator Vector4(LinearColor4 color)
    {
        return color.Value;
    }

    public readonly override string ToString()
    {
        return Color.ToString();
    }
}
