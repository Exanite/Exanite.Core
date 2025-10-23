using System;
using System.Numerics;
using Exanite.Core.Numerics;

namespace Exanite.Core.Utilities;

public static partial class M
{
    #region Vectors

    /// <summary>
    /// Swaps the component values of a <see cref="Vector3"/> from XYZ to
    /// the given format.
    /// </summary>
    public static Vector3 Swizzle(this Vector3 vector, Vector3Swizzle swizzle)
    {
        return swizzle switch
        {
            Vector3Swizzle.Xyz => vector,
            Vector3Swizzle.Xzy => new Vector3(vector.X, vector.Z, vector.Y),
            Vector3Swizzle.Yxz => new Vector3(vector.Y, vector.X, vector.Z),
            Vector3Swizzle.Yzx => new Vector3(vector.Y, vector.Z, vector.X),
            Vector3Swizzle.Zxy => new Vector3(vector.Z, vector.X, vector.Y),
            Vector3Swizzle.Zyx => new Vector3(vector.Z, vector.Y, vector.X),
            _ => throw ExceptionUtility.NotSupportedEnumValue(swizzle),
        };
    }

    /// <summary>
    /// Opposite of Swizzle. Swaps the component values of a
    /// <see cref="Vector3"/> in the given format back to XYZ.
    /// </summary>
    public static Vector3 InverseSwizzle(this Vector3 vector, Vector3Swizzle swizzle)
    {
        return swizzle switch
        {
            Vector3Swizzle.Xyz => vector,
            Vector3Swizzle.Xzy => new Vector3(vector.X, vector.Z, vector.Y),
            Vector3Swizzle.Yxz => new Vector3(vector.Y, vector.X, vector.Z),
            Vector3Swizzle.Yzx => new Vector3(vector.Z, vector.X, vector.Y),
            Vector3Swizzle.Zxy => new Vector3(vector.Y, vector.Z, vector.X),
            Vector3Swizzle.Zyx => new Vector3(vector.Z, vector.Y, vector.X),
            _ => throw ExceptionUtility.NotSupportedEnumValue(swizzle),
        };
    }

    #endregion

    #region Vector Conversion

    // Vector component count conversion
    // The goal is to keep these conversions minimal
    // so only the most common cases are handled

    // Vector2 <- Vector3

    /// <summary>
    /// Converts a Vector3 into a Vector2 by dropping the Z component.
    /// </summary>
    public static Vector2 Xy(this Vector3 value)
    {
        return new Vector2(value.X, value.Y);
    }

    // Vector2 -> Vector3

    /// <summary>
    /// Converts a Vector2 into a Vector3 using 0 for the Z component.
    /// </summary>
    public static Vector3 Xy0(this Vector2 value)
    {
        return new Vector3(value.X, value.Y, 0);
    }

    /// <summary>
    /// Converts a Vector2 into a Vector3 using 1 for the Z component.
    /// </summary>
    public static Vector3 Xy1(this Vector2 value)
    {
        return new Vector3(value.X, value.Y, 1);
    }

    // Vector3 <- Vector4

    /// <summary>
    /// Converts a Vector4 into a Vector3 by dropping the W component.
    /// </summary>
    public static Vector3 Xyz(this Vector4 value)
    {
        return new Vector3(value.X, value.Y, value.Z);
    }

    // Vector3 -> Vector4

    /// <summary>
    /// Converts a Vector3 into a Vector4 using 0 for the Z component.
    /// </summary>
    public static Vector4 Xyz0(this Vector3 value)
    {
        return new Vector4(value.X, value.Y, value.Z, 0);
    }

    /// <summary>
    /// Converts a Vector3 into a Vector4 using 1 for the Z component.
    /// </summary>
    public static Vector4 Xyz1(this Vector3 value)
    {
        return new Vector4(value.X, value.Y, value.Z, 1);
    }

    // Vector2Int <- Vector3Int

    /// <summary>
    /// Converts a Vector3Int into a Vector2Int by dropping the Z component.
    /// </summary>
    public static Vector2Int Xy(this Vector3Int value)
    {
        return new Vector2Int(value.X, value.Y);
    }

    // Vector2Int -> Vector3Int

    /// <summary>
    /// Converts a Vector2Int into a Vector3Int using 0 for the Z component.
    /// </summary>
    public static Vector3Int Xy0(this Vector2Int value)
    {
        return new Vector3Int(value.X, value.Y, 0);
    }

    /// <summary>
    /// Converts a Vector2Int into a Vector3Int using 1 for the Z component.
    /// </summary>
    public static Vector3Int Xy1(this Vector2Int value)
    {
        return new Vector3Int(value.X, value.Y, 1);
    }

    #endregion

    #region Matrices

    /// <summary>
    /// Same as <see cref="Matrix4x4.CreateOrthographic"/>, but allows for reversed near and far planes.
    /// </summary>
    public static Matrix4x4 CreateOrthographic(float width, float height, float nearPlane, float farPlane)
    {
        var range = 1 / (nearPlane - farPlane);

        return new Matrix4x4(
            2 / width, 0, 0, 0,
            0, 2 / height, 0, 0,
            0, 0, range, 0,
            0, 0, range * nearPlane, 1
        );
    }

    /// <summary>
    /// Same as <see cref="Matrix4x4.CreatePerspectiveFieldOfView"/>, but allows for reversed near and far planes.
    /// </summary>
    public static Matrix4x4 CreatePerspective(float fieldOfView, float aspectRatio, float nearPlane, float farPlane)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(fieldOfView, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(fieldOfView, float.Pi);

        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(nearPlane, 0);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(farPlane, 0);

        var height = 1 / float.Tan(fieldOfView * 0.5f);
        var width = height / aspectRatio;
        var range = float.IsPositiveInfinity(farPlane) ? -1 : farPlane / (nearPlane - farPlane);

        return new Matrix4x4(
            width, 0, 0, 0,
            0, height, 0, 0,
            0, 0, range, -1,
            0, 0, range * nearPlane, 0
        );
    }

    #endregion

    #region Planes

    /// <summary>
    /// Creates a plane from a position and a normal.
    /// </summary>
    /// <remarks>
    /// .NET doesn't provide this method of construction for some reason.
    /// </remarks>
    public static Plane CreatePlane(Vector3 normal, Vector3 position)
    {
        normal = normal.AsNormalizedSafe();
        var distance = -Vector3.Dot(normal, position);

        return new Plane(normal, distance);
    }

    /// <summary>
    /// Casts a ray against the specified plane.
    /// </summary>
    public static bool Raycast(this Plane plane, Ray ray, out float distance)
    {
        var vdot = Vector3.Dot(ray.Direction, plane.Normal);
        var ndot = -Vector3.Dot(ray.Origin, plane.Normal) - plane.D;

        if (ApproximatelyEquals(vdot, 0f))
        {
            distance = 0f;

            return false;
        }

        distance = ndot / vdot;

        return distance >= 0 && distance <= ray.Length;
    }

    #endregion
}
