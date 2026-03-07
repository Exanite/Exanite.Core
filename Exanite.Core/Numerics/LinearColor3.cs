using System.Numerics;
using System.Runtime.InteropServices;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

/// <summary>
/// Storage struct useful for explicitly storing a color in linear format.
/// Primary recommended use is for interop and other scenarios requiring the underlying numeric format to be in linear.
/// <br/>
/// See <see cref="Numerics.Color"/> for a general color representation struct and corresponding APIs.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public record struct LinearColor3
{
    public Vector3 Value;
    public readonly Color Color => this;

    public LinearColor3(Vector3 value)
    {
        Value = value;
    }

    public static implicit operator Color(LinearColor3 color)
    {
        return Color.FromLinear(color.Value);
    }

    public static implicit operator LinearColor3(Color color)
    {
        return new LinearColor3(color.Linear.Value.Xyz());
    }

    public static implicit operator LinearColor3(Vector3 color)
    {
        return new LinearColor3(color);
    }

    public static implicit operator Vector3(LinearColor3 color)
    {
        return color.Value;
    }

    public readonly override string ToString()
    {
        return Color.ToString();
    }
}
