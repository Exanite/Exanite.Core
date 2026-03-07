using System.Numerics;
using System.Runtime.InteropServices;
using Exanite.Core.Utilities;

namespace Exanite.Core.Numerics;

/// <summary>
/// Storage struct useful for explicitly storing an angle in linear format.
/// Primary recommended use is for interop and other scenarios requiring the byte-level format to be in linear.
/// <br/>
/// See <see cref="Numerics.Color"/> for a general color representation struct and corresponding APIs.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public record struct LinearColor3
{
    public Vector3 Value;
    public Color Color => Color.FromLinear(Value);

    public LinearColor3(Vector3 value)
    {
        Value = value;
    }

    public static implicit operator Color(LinearColor3 color)
    {
        return Color.From(color.Value.Xyz1(), ColorType.Linear);
    }

    public static implicit operator LinearColor3(Color color)
    {
        return new LinearColor3(color.To(ColorType.Linear).Xyz());
    }

    public readonly override string ToString()
    {
        return $"{Value} (Linear)";
    }
}
